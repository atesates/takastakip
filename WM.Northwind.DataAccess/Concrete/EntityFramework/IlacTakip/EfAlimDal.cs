using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WM.Core.DAL.EntityFramework;
using WM.Northwind.DataAccess.Abstract.IlacTakip;
using WM.Northwind.DataAccess.Concrete.EntityFramework.Contexts;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using WM.Northwind.Entities.Concrete.IlacTakip;

namespace WM.Northwind.DataAccess.Concrete.EntityFramework.IlacTakip
{
    public class EfAlimDal : EfEntityRepositoryBase<Alim, IlacTakipContext>, IAlimDal
    {
        public AlimDetay GetDetay(Expression<Func<AlimDetay, bool>> filter)
        {
            using (var ctx = new IlacTakipContext())
            {
                return ctx.Alimlar
                    .Select(s => new AlimDetay
                    {
                        Adres = s.EczaneGrup.Eczane.Adres,

                        AlimDurumId = s.AlimDurumId,
                        AlimDurumAdi = s.AlimDurum.Adi,
                        AlimTarihi = s.AlimTarihi,
                        BaslangicTarihi = s.Teklif.BaslangicTarihi,
                        BitisTarihi = s.Teklif.BitisTarihi,
                        DepoFiyati = s.Teklif.DepoFiyat,
                        EczaneGrupId = s.EczaneGrupId,
                        EczaneAdi = s.EczaneGrup.Eczane.Adi,
                        EczaneGln = s.EczaneGrup.Eczane.EczaneGln,
                        EtiketFiyati = s.Teklif.EtiketFiyati,
                        Email = s.EczaneGrup.Eczane.Email,
                        FaturaAdSoyad = s.EczaneGrup.Eczane.FaturaAdSoyad,
                        GonderimTarihi = s.GonderimTarihi,
                        GrupId = s.EczaneGrup.GrupId,
                        GrupAdi = s.EczaneGrup.Grup.Adi,
                        HedeflenenAlim = s.Teklif.HedeflenenAlim,
                        IlacAdi = s.Teklif.Ilac.Adi,
                        IlacId = s.Teklif.IlacId,
                        IlacMiad = s.Teklif.IlacMiad,
                        ITStransferDurumId = s.ITStransferDurumId,
                        ITStransferDurumAdi = s.ITStransferDurum.Adi,
                        Id = s.Id,
                        KayitTarihi = s.Teklif.KayitTarihi,
                        MalFazlasi = s.Teklif.MalFazlasi,
                        Miktar = s.Miktar,
                        Minimum = s.Teklif.Minimum,
                        Maximum = s.Teklif.Maksimum,
                        NetFiyat = s.Teklif.NetFiyat,
                        Sehir = s.EczaneGrup.Eczane.Sehir.Adi,
                        SehirId = s.EczaneGrup.Eczane.SehirId,
                        TeklifId = s.TeklifId,
                        TeklifDurumAdi=s.Teklif.TeklifDurum.Adi,
                        TeklifTurAdi = s.Teklif.TeklifTur.Adi,
                        TeklifVerenEczaneGrupAdi = s.Teklif.EczaneGrup.Grup.Adi,
                        TeklifVerenEczaneGrupId = s.Teklif.TeklifiVerenEczaneGrupId,
                        TeklifVerenEczaneAdi = s.Teklif.EczaneGrup.Eczane.Adi,
                        TeslimAlimTarihi = s.TeslimAlimTarihi,
                        ToplamAlimMiktari = s.Teklif.Alimlar.Sum(m => m.Miktar),
                        Kalan = s.Teklif.AlimMiktari + s.Teklif.MalFazlasi - s.Teklif.Alimlar.Sum(m => m.Miktar),
                        ToplamTeklifMiktari = s.Teklif.AlimMiktari + s.Teklif.MalFazlasi,
                        TeklifDurumId = s.Teklif.TeklifDurumId


                    }).SingleOrDefault(filter);
            }
        }
        public List<AlimDetay> GetDetayList(Expression<Func<AlimDetay, bool>> filter = null)
        {
            using (var ctx = new IlacTakipContext())
            {
                var liste = ctx.Alimlar
                    .Select(s => new AlimDetay
                    {
                        Adres = s.EczaneGrup.Eczane.Adres,

                        AlimDurumId = s.AlimDurumId,
                        AlimDurumAdi = s.AlimDurum.Adi,
                        AlimTarihi = s.AlimTarihi,
                        BaslangicTarihi = s.Teklif.BaslangicTarihi,
                        BitisTarihi = s.Teklif.BitisTarihi,
                        DepoFiyati = s.Teklif.DepoFiyat,
                        EczaneGrupId = s.EczaneGrupId,
                        EczaneAdi = s.EczaneGrup.Eczane.Adi,
                        EczaneGln = s.EczaneGrup.Eczane.EczaneGln,
                        Email = s.EczaneGrup.Eczane.Email,
                        FaturaAdSoyad = s.EczaneGrup.Eczane.FaturaAdSoyad,
                        EtiketFiyati = s.Teklif.EtiketFiyati,
                        GonderimTarihi = s.GonderimTarihi,
                        GrupId = s.EczaneGrup.GrupId,
                        GrupAdi = s.EczaneGrup.Grup.Adi,
                        HedeflenenAlim = s.Teklif.HedeflenenAlim,
                        IlacAdi = s.Teklif.Ilac.Adi,
                        IlacId = s.Teklif.IlacId,                
                        IlacMiad = s.Teklif.IlacMiad,
                        ITStransferDurumId = s.ITStransferDurumId,
                        ITStransferDurumAdi = s.ITStransferDurum.Adi,
                        Id = s.Id,
                        KayitTarihi = s.Teklif.KayitTarihi,
                        MalFazlasi = s.Teklif.MalFazlasi,
                        Miktar = s.Miktar,
                        Minimum = s.Teklif.Minimum,
                        Maximum = s.Teklif.Maksimum,
                        NetFiyat = s.Teklif.NetFiyat,
                        Sehir = s.EczaneGrup.Eczane.Sehir.Adi,
                        SehirId = s.EczaneGrup.Eczane.SehirId,
                        TeklifId = s.TeklifId,
                        TeklifDurumAdi = s.Teklif.TeklifDurum.Adi,
                        TeklifTurAdi = s.Teklif.TeklifTur.Adi,
                        TeklifVerenEczaneGrupAdi = s.Teklif.EczaneGrup.Grup.Adi,
                        TeklifVerenEczaneGrupId = s.Teklif.TeklifiVerenEczaneGrupId,
                        TeklifVerenEczaneAdi = s.Teklif.EczaneGrup.Eczane.Adi,
                        TeslimAlimTarihi = s.TeslimAlimTarihi,
                        ToplamAlimMiktari = s.Teklif.Alimlar.Sum(m => m.Miktar),
                        Kalan = s.Teklif.AlimMiktari + s.Teklif.MalFazlasi - s.Teklif.Alimlar.Sum(m => m.Miktar),
                        ToplamTeklifMiktari = s.Teklif.AlimMiktari + s.Teklif.MalFazlasi,
                        TeklifDurumId = s.Teklif.TeklifDurumId

                    });

                return filter == null
                    ? liste.ToList()
                    : liste.Where(filter).ToList();
            }
        }
    }
}