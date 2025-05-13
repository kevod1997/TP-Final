using AutoMapper;
using MediatR;
using ProductService.Application.DTOs;
using ProductService.Domain.Entities;
using ProductService.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabajoFinal.Common.Shared.Logging;
using TrabajoFinal.Common.Shared.Results;

namespace ProductService.Application.Commands
{
    public class CreateProductCommand : IRequest<Result<ProductDto>>
    {
        public CreateProductDto ProductDto { get; set; }
    }

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public CreateProductCommandHandler(
            IProductRepository productRepository,
            IMapper mapper,
            ILoggerService logger)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creando nuevo producto: {0}", request.ProductDto.Name);

                // Mapear DTO a entidad de dominio
                var product = _mapper.Map<Product>(request.ProductDto);

                // Guardar en la base de datos
                await _productRepository.AddAsync(product);

                // Mapear el resultado de vuelta a DTO
                var productDto = _mapper.Map<ProductDto>(product);

                _logger.LogInformation("Producto creado exitosamente con ID: {0}", product.Id);

                return Result<ProductDto>.Success(productDto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Error de validacion al crear el producto: {ex}");
                return Result<ProductDto>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                return Result<ProductDto>.Failure($"Error al crear el producto: {ex.Message}");
            }
        }
    }
}