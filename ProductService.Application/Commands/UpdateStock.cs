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
    public class UpdateStockCommand : IRequest<ProductDto>
    {
        public UpdateStockDto StockDto { get; set; }
    }

    public class UpdateStockCommandHandler : IRequestHandler<UpdateStockCommand, ProductDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public UpdateStockCommandHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ProductDto> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.StockDto.Id);
            if (product == null)
                return null;

            product.UpdateStock(request.StockDto.Quantity);
            await _productRepository.UpdateAsync(product);

            return _mapper.Map<ProductDto>(product);
        }
    }
}