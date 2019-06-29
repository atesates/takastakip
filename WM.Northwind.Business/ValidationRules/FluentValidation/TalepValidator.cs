using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Northwind.Entities.Concrete.IlacTakip;

namespace WM.Northwind.Business.ValidationRules.FluentValidation
{
    public class TalepValidator : AbstractValidator<Talep>
    {
        public TalepValidator()
        {
            RuleFor(p => p.TalepMiktari).GreaterThan(0);
            RuleFor(p => p.DepoFiyati).GreaterThan(0);
            RuleFor(p => p.Maximum).GreaterThan(0);
            RuleFor(p => p.BitisTarihi).GreaterThan(p => p.KayitTarihi);
            
        }
    }
}