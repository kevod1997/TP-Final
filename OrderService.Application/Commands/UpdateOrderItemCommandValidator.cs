using FluentValidation;
using OrderService.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabajoFinal.Common.Shared.Constants;

namespace OrderService.Application.Commands
{
    public class UpdateOrderItemCommandValidator : AbstractValidator<UpdateOrderItemCommand>
    {
        private readonly IOrderRepository _orderRepository;

        public UpdateOrderItemCommandValidator(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;

            RuleFor(p => p.OrderId)
                .NotEmpty().WithMessage("El ID de la orden es requerido.")
                .GreaterThan(0).WithMessage("El ID de la orden debe ser mayor que 0.")
                .MustAsync(ExisteOrdenAsync).WithMessage(ErrorMessages.Order.NotFound);

            RuleFor(p => p.OrderItemDto.ProductId)
                .NotEmpty().WithMessage("El ID del producto es requerido.")
                .GreaterThan(0).WithMessage("El ID del producto debe ser mayor que 0.");

            RuleFor(p => p.OrderItemDto.Quantity)
                .GreaterThan(0).WithMessage(ErrorMessages.Order.InvalidQuantity);
        }

        private async Task<bool> ExisteOrdenAsync(int id, CancellationToken cancellationToken)
        {
            return await _orderRepository.ExistsAsync(id);
        }
    }
}