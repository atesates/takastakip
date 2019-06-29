using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Core.Entities;

namespace WM.Northwind.Entities.Concrete.IlacTakip
{
    public class Grup : IEntity
    {
        public int Id { get; set; }
        [Display(Name = "Grup Adi")]
        public string Adi { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        
        public virtual List<EczaneGrup> EczaneGruplar { get; set; }
    }
}