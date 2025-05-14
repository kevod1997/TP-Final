using MediatR;
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
    public class DeleteOrderCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }
    }

    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, Result<bool>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILoggerService _logger;

        public DeleteOrderCommandHandler(
            IOrderRepository orderRepository,
            ILoggerService logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Procesando la eliminación de la orden con ID: {request.Id}");

                if (!await _orderRepository.ExistsAsync(request.Id))
                {
                    _logger.LogWarning($"Orden con ID {request.Id} no encontrada para eliminar");
                    return Result<bool>.NotFound(ErrorMessages.Order.NotFound);
                }

                await _orderRepository.DeleteAsync(request.Id);

                _logger.LogInformation($"Orden con ID {request.Id} eliminada exitosamente");

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar la orden con ID: {request.Id}");
                return Result<bool>.Failure($"Error al eliminar la orden: {ex.Message}");
            }
        }
    }
}