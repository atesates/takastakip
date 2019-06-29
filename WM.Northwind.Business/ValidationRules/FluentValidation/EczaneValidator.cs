using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Northwind.Entities.Concrete.IlacTakip;

namespace WM.Northwind.Business.ValidationRules.FluentValidation
{
    public class EczaneValidator : AbstractValidator<Eczane>
    {
        public EczaneValidator()
        {
            RuleFor(p => p.Adi).NotEmpty().Length(0, 20);
            RuleFor(p => p.Adres).MaximumLength(150);
            //RuleFor(p => p.KapanisTarihi);
            RuleFor(p => p.Email).MaximumLength(40).EmailAddress();
            RuleFor(p => p.Telefon).MaximumLength(10);
            RuleFor(p => p.Telefon2).MaximumLength(30);
            RuleFor(p => p.EczaneGln).MinimumLength(13);
            RuleFor(p => p.EczaneGln).MaximumLength(13);


        }
    }
}