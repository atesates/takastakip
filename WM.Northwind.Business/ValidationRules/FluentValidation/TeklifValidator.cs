using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Northwind.Entities.Concrete.IlacTakip;

namespace WM.Northwind.Business.ValidationRules.FluentValidation
{
    public class TeklifValidator : AbstractValidator<Teklif>
    {
        public TeklifValidator()
        {
            RuleFor(p => p.AlimMiktari).GreaterThan(0);
            RuleFor(p => p.DepoFiyat).GreaterThan(0);
            RuleFor(p => p.EtiketFiyati).GreaterThan(0);
            RuleFor(p => p.HedeflenenAlim).GreaterThan(0);
            RuleFor(p => p.Maksimum).GreaterThan(0);
            RuleFor(p => p.NetFiyat).GreaterThan(0);
            RuleFor(p => p.BitisTarihi).GreaterThan(p => p.BaslangicTarihi);

            RuleFor(p => p.BaslangicTarihi).NotEmpty();
        }
    }
}