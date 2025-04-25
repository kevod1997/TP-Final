using AutoMapper;
using MediatR;
using ProductService.Application.DTOs;
using ProductService.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public UpdateProductCommandHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.ProductDto.Id);
            if (product == null)
                return null;

            product.UpdateDetails(request.ProductDto.Name, request.ProductDto.Description, request.ProductDto.Price);
            await _productRepository.UpdateAsync(product);

            return _mapper.Map<ProductDto>(product);
        }
    }
}