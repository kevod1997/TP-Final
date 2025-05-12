using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Application.Commands
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(p => p.ProductDto.Name)
       .NotEmpty().WithMessage("{PropertyName} es requerido.")
       .MaximumLength(100).WithMessage("{PropertyName} no debe exceder 100 caracteres.");

            RuleFor(p => p.ProductDto.Description)
                .MaximumLength(500).WithMessage("{PropertyName} no debe exceder 500 caracteres.");

            RuleFor(p => p.ProductDto.Price)
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} debe ser mayor o igual a 0.");

            RuleFor(p => p.ProductDto.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} debe ser mayor o igual a 0.");
        }
    }
}