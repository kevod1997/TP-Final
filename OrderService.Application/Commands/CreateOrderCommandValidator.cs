using FluentValidation;
using OrderService.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabajoFinal.Common.Shared.Constants;

namespace OrderService.Application.Commands
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        private readonly ICustomerService _customerService;

        public CreateOrderCommandValidator(ICustomerService customerService)
        {
            _customerService = customerService;

            RuleFor(p => p.OrderDto.CustomerId)
                .NotEmpty().WithMessage(ErrorMessages.Order.CustomerRequired)
                .GreaterThan(0).WithMessage(ErrorMessages.Order.CustomerRequired)
                .MustAsync(async (customerId, cancellation) =>
                {
                    return await _customerService.CustomerExistsAsync(customerId);
                }).WithMessage(ErrorMessages.Order.CustomerNotFound);

            RuleFor(p => p.OrderDto.CustomerName)
                .NotEmpty().WithMessage("El nombre del cliente es requerido.")
                .MaximumLength(100).WithMessage("El nombre del cliente no debe exceder los 100 caracteres.");

            RuleFor(p => p.OrderDto.Items)
                .NotEmpty().WithMessage(ErrorMessages.Order.EmptyOrder)
                .Must(items => items.Count > 0).WithMessage(ErrorMessages.Order.EmptyOrder);

            RuleForEach(p => p.OrderDto.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId)
                    .GreaterThan(0).WithMessage("El ID del producto debe ser mayor que 0.");

                item.RuleFor(i => i.ProductName)
                    .NotEmpty().WithMessage("El nombre del producto es requerido.")
                    .MaximumLength(100).WithMessage("El nombre del producto no debe exceder los 100 caracteres.");

                item.RuleFor(i => i.UnitPrice)
                    .GreaterThanOrEqualTo(0).WithMessage("El precio unitario debe ser mayor o igual a 0.");

                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage(ErrorMessages.Order.InvalidQuantity);
            });
        }
    }
}