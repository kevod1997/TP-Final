using AutoMapper;
using CustomerService.Application.DTOs;
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
    public class UpdateCustomerCommand : IRequest<Result<CustomerDto>>
    {
        public UpdateCustomerDto CustomerDto { get; set; }
    }

    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result<CustomerDto>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public UpdateCustomerCommandHandler(
            ICustomerRepository customerRepository,
            IMapper mapper,
            ILoggerService logger)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<CustomerDto>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Procesando actualización de cliente ID: {request.CustomerDto.Id}");

                var customer = await _customerRepository.GetByIdAsync(request.CustomerDto.Id);
                if (customer == null)
                {
                    _logger.LogWarning($"Cliente con ID {request.CustomerDto.Id} no encontrado");
                    return Result<CustomerDto>.NotFound(ErrorMessages.Customer.NotFound);
                }

                // Verificar si ya existe otro cliente con el mismo email
                var existingCustomer = await _customerRepository.GetByEmailAsync(request.CustomerDto.Email);
                if (existingCustomer != null && existingCustomer.Id != request.CustomerDto.Id)
                {
                    _logger.LogWarning($"Ya existe otro cliente con el email: {request.CustomerDto.Email}");
                    return Result<CustomerDto>.Failure(ErrorMessages.Customer.EmailAlreadyExists);
                }

                customer.UpdateDetails(request.CustomerDto.Name, request.CustomerDto.Email, request.CustomerDto.Address);
                await _customerRepository.UpdateAsync(customer);

                _logger.LogInformation($"Cliente con ID {request.CustomerDto.Id} actualizado exitosamente");

                return Result<CustomerDto>.Success(_mapper.Map<CustomerDto>(customer));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Error de validación al actualizar el cliente: {ex.Message}");
                return Result<CustomerDto>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el cliente con ID: {request.CustomerDto.Id}");
                return Result<CustomerDto>.Failure($"Error al actualizar el cliente: {ex.Message}");
            }
        }
    }
}