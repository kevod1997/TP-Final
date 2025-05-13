using AutoMapper;
using CustomerService.Application.DTOs;
using CustomerService.Domain.Entities;
using CustomerService.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabajoFinal.Common.Shared.Logging;
using TrabajoFinal.Common.Shared.Results;

namespace CustomerService.Application.Commands
{
    public class CreateCustomerCommand : IRequest<Result<CustomerDto>>
    {
        public CreateCustomerDto CustomerDto { get; set; }
    }

    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public CreateCustomerCommandHandler(
            ICustomerRepository customerRepository,
            IMapper mapper,
            ILoggerService logger)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<CustomerDto>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creando nuevo cliente: {0}", request.CustomerDto.Name);

                // Verificar si ya existe un cliente con ese email
                if (await _customerRepository.ExistsByEmailAsync(request.CustomerDto.Email))
                {
                    _logger.LogWarning($"Ya existe un cliente con el email: {request.CustomerDto.Email}");
                    return Result<CustomerDto>.Failure("Ya existe un cliente con este email");
                }

                // Mapear DTO a entidad de dominio
                var customer = _mapper.Map<Customer>(request.CustomerDto);

                // Guardar en la base de datos
                await _customerRepository.AddAsync(customer);

                // Mapear el resultado de vuelta a DTO
                var customerDto = _mapper.Map<CustomerDto>(customer);

                _logger.LogInformation("Cliente creado exitosamente con ID: {0}", customer.Id);

                return Result<CustomerDto>.Success(customerDto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Error de validación al crear el cliente: {ex}");
                return Result<CustomerDto>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear cliente");
                return Result<CustomerDto>.Failure($"Error al crear el cliente: {ex.Message}");
            }
        }
    }
}