using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Core.Entities;

namespace WM.Northwind.Entities.Concrete.IlacTakip
{
    public class AlimDurum : IEntity
    {
        public int Id { get; set; }
        public bool? AliciTarafiMi { get; set; }
        public string Adi { get; set; }
        //public virtual FK_AlimDur FK_AlimDur { get; set; }
        public virtual List<Alim> Alimlar { get; set; }
    }
}