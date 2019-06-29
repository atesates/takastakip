using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using WM.Northwind.Entities.Concrete.IlacTakip;

namespace WM.Northwind.Business.ValidationRules.FluentValidation
{
    public class LoginItemValidator : AbstractValidator<LoginItem>
    {
        public LoginItemValidator()
        {
            RuleFor(p => p.Email)
                    .NotEmpty()
                    //.Length(3, 100)
                    .EmailAddress()
                    //.WithMessage("E-mail zorunludur-----")
                    ;

            RuleFor(p => p.Password)
                    .NotEmpty()
                    .MaximumLength(50);
        }
    }
}