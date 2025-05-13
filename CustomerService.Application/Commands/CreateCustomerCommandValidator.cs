using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabajoFinal.Common.Shared.Constants;

namespace CustomerService.Application.Commands
{
    public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerCommandValidator()
        {
            RuleFor(p => p.CustomerDto.Name)
                .NotEmpty().WithMessage(ErrorMessages.Customer.NameRequired)
                .MaximumLength(100).WithMessage("El nombre no debe exceder 100 caracteres.");

            RuleFor(p => p.CustomerDto.Email)
                .NotEmpty().WithMessage(ErrorMessages.Customer.EmailRequired)
                .MaximumLength(150).WithMessage("El email no debe exceder 150 caracteres.")
                .EmailAddress().WithMessage(ErrorMessages.Customer.EmailInvalid);

            RuleFor(p => p.CustomerDto.Address)
                .NotEmpty().WithMessage(ErrorMessages.Customer.AddressRequired)
                .MaximumLength(200).WithMessage("La dirección no debe exceder 200 caracteres.");
        }
    }
}