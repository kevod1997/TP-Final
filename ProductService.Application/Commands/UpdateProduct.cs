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
using TrabajoFinal.Common.Shared.Logging;

namespace ProductService.Application.Commands
{
    public class UpdateProductCommand : IRequest<ProductDto>
    {
        public UpdateProductDto ProductDto { get; set; }
    }

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
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

        public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Manejando comando UpdateProduct para producto ID: {request.ProductDto.Id}");

            var product = await _productRepository.GetByIdAsync(request.ProductDto.Id);
            if (product == null)
            {
                _logger.LogWarning($"Producto con ID {request.ProductDto.Id} no encontrado");
                return null;
            }

            product.UpdateDetails(request.ProductDto.Name, request.ProductDto.Description, request.ProductDto.Price);
            await _productRepository.UpdateAsync(product);

            _logger.LogInformation($"Producto con ID {request.ProductDto.Id} actualizado exitosamente");

            return _mapper.Map<ProductDto>(product);
        }
    }
}