using AutoMapper;
using CustomerService.Application.DTOs;
using CustomerService.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabajoFinal.Common.Shared.Logging;
using TrabajoFinal.Common.Shared.Results;

namespace CustomerService.Application.Queries
{
    public class GetCustomerByEmailQuery : IRequest<Result<CustomerDto>>
    {
        public string Email { get; set; }
    }

    public class GetCustomerByEmailQueryHandler : IRequestHandler<GetCustomerByEmailQuery, Result<CustomerDto>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public GetCustomerByEmailQueryHandler(
            ICustomerRepository customerRepository,
            IMapper mapper,
            ILoggerService logger)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<CustomerDto>> Handle(GetCustomerByEmailQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Obteniendo cliente con email: {request.Email}");

                var customer = await _customerRepository.GetByEmailAsync(request.Email);

                if (customer == null)
                {
                    _logger.LogWarning($"Cliente con email: {request.Email} no encontrado");
                    return Result<CustomerDto>.NotFound("No se encontró un cliente con este email");
                }

                var customerDto = _mapper.Map<CustomerDto>(customer);

                _logger.LogInformation($"Cliente con email: {request.Email} obtenido exitosamente");

                return Result<CustomerDto>.Success(customerDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el cliente con email: {request.Email}");
                return Result<CustomerDto>.Failure($"Error al obtener el cliente: {ex.Message}");
            }
        }
    }
}