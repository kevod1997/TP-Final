using AutoMapper;
using MediatR;
using ProductService.Application.DTOs;
using ProductService.Domain.Exceptions;
using ProductService.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabajoFinal.Common.Shared.Constants;
using TrabajoFinal.Common.Shared.Logging;
using TrabajoFinal.Common.Shared.Results;

namespace ProductService.Application.Commands
{
    public class UpdateStockCommand : IRequest<Result<ProductDto>>
    {
        public UpdateStockDto StockDto { get; set; }
    }

    public class UpdateStockCommandHandler : IRequestHandler<UpdateStockCommand, Result<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public UpdateStockCommandHandler(
            IProductRepository productRepository,
            IMapper mapper,
            ILoggerService logger)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<ProductDto>> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Iniciando actualización de stock para producto ID: {request.StockDto.Id}, cantidad: {request.StockDto.Quantity}");

            var product = await _productRepository.GetByIdAsync(request.StockDto.Id);
            if (product == null)
            {
                _logger.LogWarning($"Producto con ID {request.StockDto.Id} no encontrado");
                return Result<ProductDto>.NotFound(ErrorMessages.Product.NotFound);
            }

            try
            {
                product.UpdateStock(request.StockDto.Quantity);
                await _productRepository.UpdateAsync(product);

                _logger.LogInformation($"Stock actualizado exitosamente para producto ID: {product.Id}, nuevo stock: {product.StockQuantity}");

                var productDto = _mapper.Map<ProductDto>(product);
                return Result<ProductDto>.Success(productDto);
            }
            catch (InsufficientStockException ex)
            {
                _logger.LogWarning($"Stock insuficiente para producto ID: {ex.ProductId}. Solicitado: {ex.RequestedQuantity}, Disponible: {ex.AvailableQuantity}");

                // Creamos un objeto con los detalles específicos para esta excepción
                var errorData = new
                {
                    productId = ex.ProductId,
                    requestedQuantity = ex.RequestedQuantity,
                    availableQuantity = ex.AvailableQuantity
                };

                return Result<ProductDto>.Failure(
                    new ErrorDetails(ErrorMessages.Product.InsufficientStock, System.Net.HttpStatusCode.BadRequest, errorData));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar stock para producto ID: {request.StockDto.Id}");
                return Result<ProductDto>.Failure($"Error al actualizar stock: {ex.Message}");
            }
        }
    }
}