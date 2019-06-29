using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Core.Entities;

namespace WM.Northwind.Entities.Concrete.IlacTakip
{
    public class Sehir : IEntity
    {
        public int Id { get; set; }
        public string Adi { get; set; }

        public virtual List<Eczane> Eczaneler { get; set; }
        

    }
}