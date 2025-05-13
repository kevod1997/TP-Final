using CustomerService.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabajoFinal.Common.Shared.Constants;
using TrabajoFinal.Common.Shared.Logging;
using TrabajoFinal.Common.Shared.Results;

namespace CustomerService.Application.Commands
{
    public class DeleteCustomerCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }
    }

    public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Result<bool>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILoggerService _logger;

        public DeleteCustomerCommandHandler(
            ICustomerRepository customerRepository,
            ILoggerService logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Procesando la eliminación del cliente con ID: {request.Id}");

                if (!await _customerRepository.ExistsAsync(request.Id))
                {
                    _logger.LogWarning($"Cliente con ID {request.Id} no encontrado para eliminar");
                    return Result<bool>.NotFound(ErrorMessages.Customer.NotFound);
                }

                await _customerRepository.DeleteAsync(request.Id);

                _logger.LogInformation($"Cliente con ID {request.Id} eliminado exitosamente");

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el cliente con ID: {request.Id}");
                return Result<bool>.Failure($"Error al eliminar el cliente: {ex.Message}");
            }
        }
    }
}