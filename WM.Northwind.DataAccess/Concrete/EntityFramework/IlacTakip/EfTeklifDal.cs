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
    public class EfTeklifDal : EfEntityRepositoryBase<Teklif, IlacTakipContext>, ITeklifDal
    {
        public TeklifDetay GetDetay(Expression<Func<TeklifDetay, bool>> filter)
        {
            using (var ctx = new IlacTakipContext())
            {
                return ctx.Teklifler
                    .Select(s => new TeklifDetay
                    {
                        AlimMiktari = s.AlimMiktari,
                        Barkod = s.Ilac.Barkod,
                        BaslangicTarihi = s.BaslangicTarihi,
                        BitisTarihi = s.BitisTarihi,
                        DepoFiyat = s.DepoFiyat,
                        //GrupAdi = s.EczaneGrup.Grup.Adi,
                        EtiketFiyati = s.EtiketFiyati,
                        HedeflenenAlim = s.HedeflenenAlim,
                        //GonderimTarihi = s.Alimlar.FirstOrDefault().GonderimTarihi,//.Alim.GonderimTarihi,
                        Id = s.Id,
                        IlacId = s.IlacId,
                        IlacMiad = s.IlacMiad,
                        IlacAdi = s.Ilac.Adi,
                        KayitTarihi = s.KayitTarihi,
                        Maksimum = s.Maksimum,
                        Minimum = s.Minimum,
                        MalFazlasi = s.MalFazlasi,
                        NetFiyat = s.NetFiyat,
                        TeklifDurumId = s.TeklifDurumId,
                        TeklifiVerenEczaneGln = s.EczaneGrup.Eczane.EczaneGln,
                        TeklifiVerenEczaneGrupId = s.TeklifiVerenEczaneGrupId,
                        TeklifiVerenEczaneGrupAdi = s.EczaneGrup.Grup.Adi,
                        TeklifiVerenEczaneAdi = s.EczaneGrup.Eczane.Adi,
                        TeklifDurumAdi = s.TeklifDurum.Adi,
                        YayinlamaTurId = s.YayinlamaTurId,
                        TeklifTurAdi = s.TeklifTur.Adi,
                        TeklifTurId = s.TeklifTurId,
                        YayinlamaTurAdi = s.YayinlamaTur.Adi,
                        OzelEczaneGrupId = s.OzelEczaneGrupId,
                        BuTekliftenYapilanAlimSayisi = s.Alimlar.Count(),
                        ToplamTeklifMiktari = s.AlimMiktari + s.MalFazlasi,



                    }).SingleOrDefault(filter);
            }
        }
        public List<TeklifDetay> GetDetayList(Expression<Func<TeklifDetay, bool>> filter = null)
        {
            using (var ctx = new IlacTakipContext())
            {
                var liste = ctx.Teklifler
                    .Select(s => new TeklifDetay
                    {
                        AlimMiktari = s.AlimMiktari,
                        Barkod = s.Ilac.Barkod,
                        BaslangicTarihi = s.BaslangicTarihi,
                        BitisTarihi = s.BitisTarihi,
                        DepoFiyat = s.DepoFiyat,
                        EtiketFiyati = s.EtiketFiyati,
                        //GrupAdi = s.EczaneGrup.Grup.Adi,
                        HedeflenenAlim = s.HedeflenenAlim,
                        //GonderimTarihi = s.GonderimTarihi,//.Alim.GonderimTarihi,
                        Id = s.Id,
                        IlacId = s.IlacId,
                        IlacAdi = s.Ilac.Adi,
                        IlacMiad = s.IlacMiad,
                        KayitTarihi = s.KayitTarihi,
                        Maksimum = s.Maksimum,
                        Minimum = s.Minimum,
                        MalFazlasi = s.MalFazlasi,
                        NetFiyat = s.NetFiyat,
                        TeklifDurumId = s.TeklifDurumId,
                        TeklifiVerenEczaneGln = s.EczaneGrup.Eczane.EczaneGln,
                        TeklifiVerenEczaneGrupId = s.TeklifiVerenEczaneGrupId,
                        TeklifiVerenEczaneGrupAdi = s.EczaneGrup.Grup.Adi,
                        TeklifiVerenEczaneAdi = s.EczaneGrup.Eczane.Adi,
                        TeklifDurumAdi = s.TeklifDurum.Adi,
                        YayinlamaTurId = s.YayinlamaTurId,
                        TeklifTurAdi = s.TeklifTur.Adi,
                        TeklifTurId = s.TeklifTurId,
                        YayinlamaTurAdi = s.YayinlamaTur.Adi,
                        OzelEczaneGrupId = s.OzelEczaneGrupId,
                        BuTekliftenYapilanAlimSayisi = s.Alimlar.Count(),
                        ToplamTeklifMiktari = s.AlimMiktari + s.MalFazlasi,


                    });

                return filter == null
                    ? liste.ToList()
                    : liste.Where(filter).ToList();
            }
        }
    }
}