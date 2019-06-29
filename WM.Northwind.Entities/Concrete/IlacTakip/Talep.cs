using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Core.Entities;

namespace WM.Northwind.Entities.Concrete.IlacTakip
{
    public class Talep : IEntity
    {
        public int Id { get; set; }
        public int TalepVerenEczaneGrupId { get; set; }
        public int TalepMiktari { get; set; }
        public float DepoFiyati { get; set; }
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        public string Aciklama { get; set; }
        public int TalepDurumId { get; set; }
        public DateTime KayitTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public int IlacId { get; set; }
        public virtual EczaneGrup EczaneGrup { get; set; }
        public virtual Ilac Ilac { get; set; }
        public virtual TalepDurum TalepDurum { get; set; }

        public virtual List<Katilim> Katilimlar { get; set; }
    }
}