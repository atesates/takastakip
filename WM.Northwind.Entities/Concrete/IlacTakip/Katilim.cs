using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Core.Entities;

namespace WM.Northwind.Entities.Concrete.IlacTakip
{
    public class Katilim : IEntity
    {
        public int Id { get; set; }
        public int EczaneGrupId { get; set; }
        public int Miktar { get; set; }
        public int TalepId { get; set; }
        public DateTime KatilimTarihi { get; set; }
        public virtual EczaneGrup EczaneGrup { get; set; }
        public virtual Talep Talep { get; set; }

    }
}