using AutoMapper;
using MediatR;
using ProductService.Application.DTOs;
using ProductService.Domain.Exceptions;
using ProductService.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabajoFinal.Common.Shared.Constants;
using TrabajoFinal.Common.Shared.Logging;
using TrabajoFinal.Common.Shared.Results;

namespace ProductService.Application.Commands
{
    public class UpdateProductCommand : IRequest<Result<ProductDto>>
    {
        public UpdateProductDto ProductDto { get; set; }
    }

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public UpdateProductCommandHandler(
            IProductRepository productRepository,
            IMapper mapper,
            ILoggerService logger)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Procesando actualización de producto ID: {request.ProductDto.Id}");

                var product = await _productRepository.GetByIdAsync(request.ProductDto.Id);
                if (product == null)
                {
                    _logger.LogWarning($"Producto con ID {request.ProductDto.Id} no encontrado");
                    return Result<ProductDto>.NotFound(ErrorMessages.Product.NotFound);
                }

                product.UpdateDetails(request.ProductDto.Name, request.ProductDto.Description, request.ProductDto.Price);
                await _productRepository.UpdateAsync(product);

                _logger.LogInformation($"Producto con ID {request.ProductDto.Id} actualizado exitosamente");

                return Result<ProductDto>.Success(_mapper.Map<ProductDto>(product));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Error de validación al actualizar el producto: {ex.Message}");
                return Result<ProductDto>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el producto con ID: {request.ProductDto.Id}");
                return Result<ProductDto>.Failure($"Error al actualizar el producto: {ex.Message}");
            }
        }
    }
}