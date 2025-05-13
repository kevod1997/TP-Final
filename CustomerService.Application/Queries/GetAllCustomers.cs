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
    public class GetAllCustomersQuery : IRequest<Result<IEnumerable<CustomerDto>>> { }

    public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, Result<IEnumerable<CustomerDto>>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public GetAllCustomersQueryHandler(
            ICustomerRepository customerRepository,
            IMapper mapper,
            ILoggerService logger)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<CustomerDto>>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los clientes");

                var customers = await _customerRepository.GetAllAsync();
                var customersDto = _mapper.Map<IEnumerable<CustomerDto>>(customers);

                _logger.LogInformation("Obtenidos {0} clientes",
                    customers is null ? 0 : ((IEnumerable<CustomerDto>)customersDto).Count());

                return Result<IEnumerable<CustomerDto>>.Success(customersDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los clientes");
                return Result<IEnumerable<CustomerDto>>.Failure(
                    $"Error al obtener los clientes: {ex.Message}");
            }
        }
    }
}