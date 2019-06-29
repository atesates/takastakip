using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Core.Entities;

namespace WM.Northwind.Entities.Concrete.IlacTakip
{
    public class Teklif : IEntity
    {
        public int Id { get; set; }
        public int IlacId { get; set; }
        public float EtiketFiyati { get; set; }
        [Display(Name = "Hedeflenen Alım")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz..!")]
        [RegularExpression(@"^[1-9][0-9]*$", ErrorMessage = "Lütfen 0 dan büyük bir sayı giriniz..!")]
        public int HedeflenenAlim { get; set; }
        [Display(Name = "Net Fiyat")]
        //[Required(ErrorMessage = "{0} alanı boş bırakılamaz..!")]
        //[RegularExpression(@"^[1-9][0-9]*$", ErrorMessage = "Lütfen 0 dan büyük bir sayı giriniz..!")]
        public float NetFiyat { get; set; }
        [Display(Name = "Depo Fiyatı")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz..!")]
        [RegularExpression(@"(^\d*\,?\d*[1-9]+\d*$)|(^[1-9]+\,?\d*$)", ErrorMessage = "Lütfen 0 dan büyük bir sayı giriniz..!")]
        // [RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Lütfen 0 dan büyük bir değer giriniz..!")]
        public float DepoFiyat { get; set; }
        public int TeklifiVerenEczaneGrupId { get; set; }
        public DateTime KayitTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz..!")]
        public DateTime? IlacMiad { get; set; }

        [Display(Name = "Mal Fazlası")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz..!")]
        [RegularExpression(@"^[1-9][0-9]*$", ErrorMessage = "Lütfen 0 dan büyük bir sayı giriniz..!")]
        public int MalFazlasi { get; set; }
        [Display(Name = "Maximum")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz..!")]
        [RegularExpression(@"^[1-9][0-9]*$", ErrorMessage = "Lütfen 0 dan büyük bir sayı giriniz..!")]
        public int Maksimum { get; set; }
        [Display(Name = "Minimum")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz..!")]
        [RegularExpression(@"^[1-9][0-9]*$", ErrorMessage = "Lütfen 0 dan büyük bir sayı giriniz..!")]
        public int Minimum { get; set; }
        [Display(Name = "Baslangıç Tarihi")]
        public DateTime BaslangicTarihi { get; set; }
        public int YayinlamaTurId { get; set; }
        public int TeklifTurId { get; set; }
        [Display(Name = "Alım Miktarı")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz..!")]
        [RegularExpression(@"^[1-9][0-9]*$", ErrorMessage = "Lütfen 0 dan büyük bir sayı giriniz..!")]
        public int AlimMiktari { get; set; }
        public int TeklifDurumId { get; set; }
        public int? OzelEczaneGrupId { get; set; }

        public virtual EczaneGrup EczaneGrup { get; set; }
        public virtual Ilac Ilac { get; set; }
        public virtual TeklifDurum TeklifDurum { get; set; }
        public virtual TeklifTur TeklifTur { get; set; }
        public virtual YayinlamaTur YayinlamaTur { get; set; }
        public virtual List<Alim> Alimlar { get; set; }
       // public virtual Alim Alim { get; set; }



    }
}