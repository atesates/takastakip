using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Core.Entities;

namespace WM.Northwind.Entities.Concrete.IlacTakip
{
    public class QRKod : IEntity
    {
        public int Id { get; set; }
        public string QRKodu { get; set; }
        public int AlimId { get; set; }

        public virtual Alim Alim { get; set; }

    }
}