using AutoMapper;
using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Exceptions;
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
    public class UpdateOrderItemCommand : IRequest<Result<OrderDto>>
    {
        public int OrderId { get; set; }
        public UpdateOrderItemDto OrderItemDto { get; set; }
    }

    public class UpdateOrderItemCommandHandler : IRequestHandler<UpdateOrderItemCommand, Result<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public UpdateOrderItemCommandHandler(
            IOrderRepository orderRepository,
            IMapper mapper,
            ILoggerService logger)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<OrderDto>> Handle(UpdateOrderItemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Actualizando ítem con ProductID {request.OrderItemDto.ProductId} de la orden ID: {request.OrderId}");

                var order = await _orderRepository.GetByIdAsync(request.OrderId);
                if (order == null)
                {
                    _logger.LogWarning($"Orden con ID {request.OrderId} no encontrada");
                    return Result<OrderDto>.NotFound(ErrorMessages.Order.NotFound);
                }

                try
                {
                    // Actualizar la cantidad del ítem
                    order.UpdateItemQuantity(request.OrderItemDto.ProductId, request.OrderItemDto.Quantity);

                    // Guardar los cambios
                    await _orderRepository.UpdateAsync(order);

                    // Mapear el resultado de vuelta a DTO
                    var orderDto = _mapper.Map<OrderDto>(order);

                    _logger.LogInformation($"Ítem actualizado exitosamente en la orden ID: {request.OrderId}");

                    return Result<OrderDto>.Success(orderDto);
                }
                catch (OrderDomainException ex)
                {
                    _logger.LogWarning($"Error de dominio al actualizar ítem: {ex.Message}");
                    return Result<OrderDto>.Failure(ex.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar ítem en la orden ID: {request.OrderId}");
                return Result<OrderDto>.Failure($"Error al actualizar el ítem: {ex.Message}");
            }
        }
    }
}