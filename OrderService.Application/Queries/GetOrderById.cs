using AutoMapper;
using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabajoFinal.Common.Shared.Constants;
using TrabajoFinal.Common.Shared.Logging;
using TrabajoFinal.Common.Shared.Results;

namespace OrderService.Application.Queries
{
    public class GetOrderByIdQuery : IRequest<Result<OrderDto>>
    {
        public int Id { get; set; }
    }

    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public GetOrderByIdQueryHandler(
            IOrderRepository orderRepository,
            IMapper mapper,
            ILoggerService logger)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Obteniendo orden con ID: {request.Id}");

                var order = await _orderRepository.GetByIdAsync(request.Id);

                if (order == null)
                {
                    _logger.LogWarning($"Orden con ID: {request.Id} no encontrada");
                    return Result<OrderDto>.NotFound(ErrorMessages.Order.NotFound);
                }

                var orderDto = _mapper.Map<OrderDto>(order);

                _logger.LogInformation($"Orden con ID: {request.Id} obtenida exitosamente");

                return Result<OrderDto>.Success(orderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la orden con ID: {request.Id}");
                return Result<OrderDto>.Failure($"Error al obtener la orden: {ex.Message}");
            }
        }
    }
}