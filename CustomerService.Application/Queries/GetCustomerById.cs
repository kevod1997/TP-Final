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

namespace CustomerService.Application.Queries
{
    public class GetCustomerByIdQuery : IRequest<Result<CustomerDto>>
    {
        public int Id { get; set; }
    }

    public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public GetCustomerByIdQueryHandler(
            ICustomerRepository customerRepository,
            IMapper mapper,
            ILoggerService logger)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<CustomerDto>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Obteniendo cliente con ID: {request.Id}");

                var customer = await _customerRepository.GetByIdAsync(request.Id);

                if (customer == null)
                {
                    _logger.LogWarning($"Cliente con ID: {request.Id} no encontrado");
                    return Result<CustomerDto>.NotFound(ErrorMessages.Customer.NotFound);
                }

                var customerDto = _mapper.Map<CustomerDto>(customer);

                _logger.LogInformation($"Cliente con ID: {request.Id} obtenido exitosamente");

                return Result<CustomerDto>.Success(customerDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el cliente con ID: {request.Id}");
                return Result<CustomerDto>.Failure($"Error al obtener el cliente: {ex.Message}");
            }
        }
    }
}
