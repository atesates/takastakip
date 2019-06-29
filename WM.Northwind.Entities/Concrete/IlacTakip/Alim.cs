using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Core.Entities;

namespace WM.Northwind.Entities.Concrete.IlacTakip
{
    public class Alim : IEntity
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz..!")]
        public int EczaneGrupId { get; set; }

        [Required(ErrorMessage = "{0} alanı boş bırakılamaz..!")]
        [RegularExpression(@"^[1-9][0-9]*$", ErrorMessage = "Lütfen 0 dan büyük bir sayı giriniz..!")]
        public int Miktar { get; set; }
        [Display(Name = "Gönderim Tarihi")]

        public DateTime? GonderimTarihi { get; set; }
        [Display(Name = "Alım Tarihi")]

        public DateTime AlimTarihi { get; set; }
        [Display(Name = "Teslim Alım Tarihi")]

        public DateTime? TeslimAlimTarihi { get; set; }
        [Display(Name = "Alım Durumu")]
        public int AlimDurumId { get; set; }
        [Display(Name = "ITS Transfer Durumu")]
        public int ITStransferDurumId { get; set; }

        public int TeklifId { get; set; }

        public virtual AlimDurum AlimDurum { get; set; }
        public virtual EczaneGrup EczaneGrup { get; set; }
        public virtual ITStransferDurum ITStransferDurum { get; set; }
        public virtual Teklif Teklif { get; set; }
        
        //public virtual List<Teklif> Teklifler { get; set; }



    }
}