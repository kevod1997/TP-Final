using FluentValidation;
using ProductService.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabajoFinal.Common.Shared.Constants;

namespace ProductService.Application.Commands
{
    public class UpdateStockCommandValidator : AbstractValidator<UpdateStockCommand>
    {
        private readonly IProductRepository _productRepository;

        public UpdateStockCommandValidator(IProductRepository productRepository)
        {
            _productRepository = productRepository;

            RuleFor(p => p.StockDto.Id)
                .NotEmpty().WithMessage(ErrorMessages.Product.IdRequired)
                .GreaterThan(0).WithMessage(ErrorMessages.Product.IdRequired);

            RuleFor(p => p.StockDto.Quantity)
                .NotEqual(0).WithMessage(ErrorMessages.Product.QuantityZero);

            RuleFor(p => p.StockDto.Id)
                .MustAsync(ExisteProductoAsync).WithMessage(ErrorMessages.Product.NotFound);
        }

        private async Task<bool> ExisteProductoAsync(int id, CancellationToken cancellationToken)
        {
            return await _productRepository.ExistsAsync(id);
        }
    }
}