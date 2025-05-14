using AutoMapper;
using MediatR;
using OrderService.Application.DTOs;
using OrderService.Application.Services;
using OrderService.Domain.Entities;
using OrderService.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabajoFinal.Common.Shared.Constants;
using TrabajoFinal.Common.Shared.Logging;
using TrabajoFinal.Common.Shared.Results;

namespace OrderService.Application.Commands
{
    public class CreateOrderCommand : IRequest<Result<OrderDto>>
    {
        public CreateOrderDto OrderDto { get; set; }
    }

    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public CreateOrderCommandHandler(
            IOrderRepository orderRepository,
            IProductService productService,
            ICustomerService customerService,
            IMapper mapper,
            ILoggerService logger)
        {
            _orderRepository = orderRepository;
            _productService = productService;
            _customerService = customerService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creando nueva orden para el cliente ID: {0}, Nombre: {1}",
                    request.OrderDto.CustomerId, request.OrderDto.CustomerName);

                // Verificar que el cliente existe
                var customerExists = await _customerService.CustomerExistsAsync(request.OrderDto.CustomerId);
                if (!customerExists)
                {
                    _logger.LogWarning($"El cliente con ID {request.OrderDto.CustomerId} no existe");
                    return Result<OrderDto>.Failure(ErrorMessages.Order.CustomerNotFound);
                }

                // Verificar stock de productos y obtener información actualizada
                var insufficientStockProducts = new List<(int ProductId, int Requested, int Available)>();
                var productsToAddIds = request.OrderDto.Items.Select(i => i.ProductId).Distinct().ToList();
                var productsInfo = await _productService.GetProductsByIdsAsync(productsToAddIds);

                // Crear un diccionario para acceso rápido a la información del producto
                var productDict = productsInfo.ToDictionary(p => p.Id, p => p);

                // Verificar que todos los productos existen
                if (productsInfo.Count() != productsToAddIds.Count)
                {
                    var missingProducts = productsToAddIds.Where(id => !productDict.ContainsKey(id));
                    _logger.LogWarning($"Algunos productos no existen: {string.Join(", ", missingProducts)}");
                    return Result<OrderDto>.Failure(ErrorMessages.Order.ProductNotFound);
                }

                // Verificar stock de cada producto
                foreach (var item in request.OrderDto.Items)
                {
                    var product = productDict[item.ProductId];
                    if (product.StockQuantity < item.Quantity)
                    {
                        insufficientStockProducts.Add((item.ProductId, item.Quantity, product.StockQuantity));
                    }
                }

                if (insufficientStockProducts.Any())
                {
                    _logger.LogWarning("Algunos productos no tienen suficiente stock");
                    var details = insufficientStockProducts.Select(p =>
                        new {
                            ProductId = p.ProductId,
                            ProductName = productDict[p.ProductId].Name,
                            Requested = p.Requested,
                            Available = p.Available
                        }).ToList();

                    return Result<OrderDto>.Failure(
                        new ErrorDetails(
                            ErrorMessages.Order.InsufficientStock,
                            System.Net.HttpStatusCode.BadRequest,
                            details
                        )
                    );
                }

                // Crear la orden
                var order = _mapper.Map<Order>(request.OrderDto);

                // Agregar los items a la orden con información actualizada de productos
                foreach (var item in request.OrderDto.Items)
                {
                    var product = productDict[item.ProductId];
                    order.AddItem(product.Id, product.Name, product.Price, item.Quantity);
                }

                // Guardar la orden en la base de datos
                await _orderRepository.AddAsync(order);

                // Actualizar stock de productos
                foreach (var item in order.Items)
                {
                    await _productService.UpdateProductStockAsync(item.ProductId, -item.Quantity);
                }

                // Mapear el resultado de vuelta a DTO
                var orderDto = _mapper.Map<OrderDto>(order);

                _logger.LogInformation("Orden creada exitosamente con ID: {0}", order.Id);

                return Result<OrderDto>.Success(orderDto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Error de validación al crear la orden: {ex}");
                return Result<OrderDto>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la orden");
                return Result<OrderDto>.Failure($"Error al crear la orden: {ex.Message}");
            }
        }
    }
}