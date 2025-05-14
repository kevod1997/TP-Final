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
    public class GetAllOrdersQuery : IRequest<Result<IEnumerable<OrderDto>>> { }

    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, Result<IEnumerable<OrderDto>>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public GetAllOrdersQueryHandler(
            IOrderRepository orderRepository,
            IMapper mapper,
            ILoggerService logger)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<OrderDto>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Obteniendo todas las órdenes");

                var orders = await _orderRepository.GetAllAsync();
                var ordersDto = _mapper.Map<IEnumerable<OrderDto>>(orders);

                _logger.LogInformation("Obtenidas {0} órdenes",
                    orders is null ? 0 : ((IEnumerable<OrderDto>)ordersDto).Count());

                return Result<IEnumerable<OrderDto>>.Success(ordersDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las órdenes");
                return Result<IEnumerable<OrderDto>>.Failure(
                    $"Error al obtener las órdenes: {ex.Message}");
            }
        }
    }
}