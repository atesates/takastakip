namespace WM.UI.Mvc.Areas.Kullanici.Models
{
    internal class TeklifAlimViewModel
    {
        public string Tip { get; set; }
        public string IlacAdi { get; set; }
        public string Eczane1Adi { get; set; }
        public string Eczane2Adi { get; set; }
        public int Miktari { get; set; }
        public int AlimId { get; set; }
        public int TeklifId { get; set; }
        public float NetFiyat { get; set; }

    }
}