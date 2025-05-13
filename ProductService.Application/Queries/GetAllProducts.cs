using AutoMapper;
using MediatR;
using ProductService.Application.DTOs;
using ProductService.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabajoFinal.Common.Shared.Logging;
using TrabajoFinal.Common.Shared.Results;

namespace ProductService.Application.Queries
{
    public class GetAllProductsQuery : IRequest<Result<IEnumerable<ProductDto>>> { }

    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Result<IEnumerable<ProductDto>>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public GetAllProductsQueryHandler(
            IProductRepository productRepository,
            IMapper mapper,
            ILoggerService logger)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<ProductDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los productos");

                var products = await _productRepository.GetAllAsync();
                var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);

                _logger.LogInformation("Obtenidos {0} productos",
                    products is null ? 0 : ((IEnumerable<ProductDto>)productsDto).Count());

                return Result<IEnumerable<ProductDto>>.Success(productsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los productos");
                return Result<IEnumerable<ProductDto>>.Failure(
                    $"Error al obtener los productos: {ex.Message}");
            }
        }
    }
}