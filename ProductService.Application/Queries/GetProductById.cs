using AutoMapper;
using MediatR;
using ProductService.Application.DTOs;
using ProductService.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabajoFinal.Common.Shared.Constants;
using TrabajoFinal.Common.Shared.Logging;
using TrabajoFinal.Common.Shared.Results;

namespace ProductService.Application.Queries
{
    public class GetProductByIdQuery : IRequest<Result<ProductDto>>
    {
        public int Id { get; set; }
    }

    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public GetProductByIdQueryHandler(
            IProductRepository productRepository,
            IMapper mapper,
            ILoggerService logger)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Obteniendo producto con ID: {request.Id}");

                var product = await _productRepository.GetByIdAsync(request.Id);

                if (product == null)
                {
                    _logger.LogWarning($"Producto con ID: {request.Id} no encontrado");
                    return Result<ProductDto>.NotFound(ErrorMessages.Product.NotFound);
                }

                var productDto = _mapper.Map<ProductDto>(product);

                _logger.LogInformation($"Producto con ID: {request.Id} obtenido exitosamente");

                return Result<ProductDto>.Success(productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el producto con ID: {request.Id}");
                return Result<ProductDto>.Failure($"Error al obtener el producto: {ex.Message}");
            }
        }
    }
}