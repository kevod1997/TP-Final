using AutoMapper;
using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabajoFinal.Common.Shared.Logging;
using TrabajoFinal.Common.Shared.Results;

namespace OrderService.Application.Queries
{
    public class GetOrdersByCustomerIdQuery : IRequest<Result<IEnumerable<OrderDto>>>
    {
        public int CustomerId { get; set; }
    }

    public class GetOrdersByCustomerIdQueryHandler : IRequestHandler<GetOrdersByCustomerIdQuery, Result<IEnumerable<OrderDto>>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public GetOrdersByCustomerIdQueryHandler(
            IOrderRepository orderRepository,
            IMapper mapper,
            ILoggerService logger)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<OrderDto>>> Handle(GetOrdersByCustomerIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Obteniendo órdenes del cliente con ID: {request.CustomerId}");

                var orders = await _orderRepository.GetByCustomerIdAsync(request.CustomerId);
                var ordersDto = _mapper.Map<IEnumerable<OrderDto>>(orders);

                _logger.LogInformation($"Obtenidas {ordersDto.Count()} órdenes para el cliente con ID: {request.CustomerId}");

                return Result<IEnumerable<OrderDto>>.Success(ordersDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener las órdenes del cliente con ID: {request.CustomerId}");
                return Result<IEnumerable<OrderDto>>.Failure(
                    $"Error al obtener las órdenes del cliente: {ex.Message}");
            }
        }
    }
}