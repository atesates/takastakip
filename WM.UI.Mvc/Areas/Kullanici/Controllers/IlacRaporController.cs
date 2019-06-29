using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WM.Northwind.Business.Abstract.Authorization;
using WM.Northwind.Entities.Concrete.IlacTakip;
using WM.Northwind.Business.Abstract.IlacTakip;
using WM.UI.Mvc.Areas.Kullanici.Models;
using System.Text.RegularExpressions;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;

namespace WM.UI.Mvc.Areas.Kullanici.Controllers
{
    [Authorize(Roles = "Admin,Eczane,Grup Yöneticisi")]
    public class IlacRaporController : Controller
    {
        // GET: Kullanici/teklif
        #region ctor
        private ITeklifService _TeklifService;
        private IEczaneService _eczaneService;
        private ITeklifService _teklifService;
        private IEczaneGrupService _eczaneGrupService;
        private IEczaneUserService _eczaneUserService;
        private IUserService _userService;
        private ITeklifTurService _teklifTurService;
        private IGrupService _grupService;
        private ITeklifDurumService _teklifDurumService;
        private IYayinlamaTurService _yayinlamaTurService;
        private IAlimService _alimService;
        private IIlacService _ilacService;

        public IlacRaporController(IEczaneGrupService eczaneGrupService,
                                IEczaneUserService eczaneUserService,
                                ITeklifService TeklifService,
                                IEczaneService eczaneService,
                                ITeklifService teklifService,
                                IUserService userService,
                                ITeklifTurService teklifTurService,
                                IGrupService grupService,
                                IYayinlamaTurService yayinlamaTurService,
                                ITeklifDurumService teklifDurumService,
                                IIlacService ilacService,
                                IAlimService alimService)
        {
            _TeklifService = TeklifService;
            _teklifService = teklifService;
            _eczaneUserService = eczaneUserService;
            _eczaneGrupService = eczaneGrupService;
            _userService = userService;
            _grupService = grupService;
            _eczaneService = eczaneService;
            _teklifTurService = teklifTurService;
            _teklifDurumService = teklifDurumService;
            _yayinlamaTurService = yayinlamaTurService;
            _ilacService = ilacService;
            _alimService = alimService;
        }
        #endregion

        public ActionResult Index(int? id, int? page)
        {
            int Id = Convert.ToInt32(id);
            int thispage = 0;
            if (page != 0)
                thispage = Convert.ToInt32(page);
            if (Id == 0)
            {//tüm gruplar için
                var user = _userService.GetByUserName(User.Identity.Name);
                var eczaneGruplar = _eczaneGrupService.GetDetayListByUser(user);
                var alimlar = _alimService.GetListByEczaneGruplar(eczaneGruplar);
                var teklifler = _teklifService.GetMyDetayListByEczaneGruplar(eczaneGruplar);
                var grupIdler = eczaneGruplar.Select(s => s.GrupId).Distinct().ToList();
                var gruplar = _grupService.GetList().Where(w => grupIdler.Contains(w.Id));
                var ilaclar = _ilacService.GetList();

                var alimlarGroupByIlacId = teklifler.GroupBy(g => g.IlacId) //.Select(s => s.Sum(d => d.Miktar));
               .Select(g => new AlimGroupByIlacId
               {
                   IlacId = g.Key,
                   //AlimDurumAdi = alimlar.Where(w => w.TeklifId == g.Key).Select(s => s.AlimDurumAdi).FirstOrDefault(),
                   //AlimDurumId = alimlar.Where(w => w.TeklifId == g.Key).Select(s => s.AlimDurumId).FirstOrDefault(),
                   //ToplamAlimMiktari = g.Sum(x => x.Miktar),
                   //AlimTarihi = alimlar.Where(w => w.TeklifId == g.Key).Select(s => s.AlimTarihi).FirstOrDefault(),
                   //NetFiyat = teklifler.Where(w => w.Id == g.Key).Select(s => s.NetFiyat).FirstOrDefault(),
                   //YayinlamaTurAdi = teklifler.Where(w => w.Id == g.Key).Select(s => s.YayinlamaTurAdi).FirstOrDefault(),
                   //TeklifiVerenEczaneAdi = teklifler.Where(w => w.Id == g.Key).Select(s => s.TeklifiVerenEczaneAdi).FirstOrDefault(),
                   //Kalan = teklifler.Where(w => w.Id == g.Key).Select(s => s.HedeflenenAlim).FirstOrDefault() - g.Sum(x => x.Miktar),
                   //ToplamFiyat = g.Sum(x => x.Miktar) * teklifler.Where(w => w.Id == g.Key).Select(s => s.NetFiyat).FirstOrDefault(),

                   IlacAdi = teklifler.Where(w => w.IlacId == g.Key).Select(s => s.IlacAdi).FirstOrDefault(),


                   BuAyHareketleri = _alimService.GetDetaylar().Where(w =>
                      (teklifler.Where(x => x.IlacId == g.Key).Select(s => s.Id)).Contains(w.TeklifId)
                      && w.GonderimTarihi > DateTime.Now.AddMonths(-1)).Count(),
                   GecenAyHareketleri = _alimService.GetDetaylar().Where(w =>
                     (teklifler.Where(x => x.IlacId == g.Key).Select(s => s.Id)).Contains(w.TeklifId)
                     && w.GonderimTarihi > DateTime.Now.AddMonths(-2) && w.GonderimTarihi < DateTime.Now.AddMonths(-1)).Count(),
                   DagitilanTeklifSayisi = teklifler.Where(w => w.IlacId == g.Key && alimlar.
                        Where(x => x.AlimDurumId == 4).Select(s => s.TeklifId).Contains(w.Id)).Count(),
                   DagitimToplami = _alimService.GetDetaylar().Where(w =>
                      (teklifler.Where(x => x.IlacId == g.Key).Select(s => s.Id)).Contains(w.TeklifId)).Sum(s => s.Miktar),
                   GrubaGirdigiTeklifSayisi = teklifler.Where(w => w.IlacId == g.Key).Count(),
                   TekliflerdenKazandirdigiMiktar = _alimService.GetDetaylar().Where(w =>
                  (teklifler.Where(x => x.IlacId == g.Key).Select(s => s.Id)).Contains(w.TeklifId))
                     .Sum(s => s.EtiketFiyati * s.Miktar) - _alimService.GetDetaylar().Where(w =>
                     (teklifler.Where(x => x.IlacId == g.Key).Select(s => s.Id)).Contains(w.TeklifId))
                     .Sum(s => s.NetFiyat * s.Miktar)

               }).OrderByDescending(o => o.TekliflerdenKazandirdigiMiktar).ToList();

                ViewBag.EczaneGrupId = new SelectList(gruplar, "Id", "Adi", Id);
                var pager = new Pager(alimlarGroupByIlacId.Count(), thispage);

                var model = new IlacRaporViewModel()
                {
                    AlimlarGroupByIlacIdler = alimlarGroupByIlacId.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList(),

                    //AlimDetaylar = alimlar,
                    //Eczaneler = eczaneler,
                    Ilaclar = ilaclar,
                    //TeklifDetaylar = teklifler,
                    Pager = pager
                };
                return View(model);

            }
            else
            {//seçili grup için
                var user = _userService.GetByUserName(User.Identity.Name);
                var eczaneGruplar = _eczaneGrupService.GetDetayListByUser(user).ToList();
                var eczaneGruplarAz = _eczaneGrupService.GetDetaylar().Where(w => w.GrupId == Id).ToList();
                //var grupId = Id;
                var eczaneGrupIdler = eczaneGruplar.Where(w => w.GrupId == Id).Select(s => s.Id).Distinct().ToList();
                var grupIdler = eczaneGruplar.Select(s => s.GrupId).Distinct().ToList();
                var gruplar = _grupService.GetList().Where(w => grupIdler.Contains(w.Id));
                var ilaclar = _ilacService.GetList();


                var teklifIdler = _teklifService.GetList()
                    .Where(w => eczaneGrupIdler.Contains(w.TeklifiVerenEczaneGrupId))
                    .Select(s => s.Id).ToList();
                var alimlar = _alimService.GetDetayListByTeklifler(teklifIdler).ToList();

                var teklifler = _teklifService.GetMyListByEczaneGruplar(eczaneGruplar);


                var alimlarGroupByIlacId = alimlar.GroupBy(g => g.IlacId) //.Select(s => s.Sum(d => d.Miktar));
               .Select(g => new AlimGroupByIlacId
               {
                   IlacId = g.Key,
                   //AlimDurumAdi = alimlar.Where(w => w.TeklifId == g.Key).Select(s => s.AlimDurumAdi).FirstOrDefault(),
                   //AlimDurumId = alimlar.Where(w => w.TeklifId == g.Key).Select(s => s.AlimDurumId).FirstOrDefault(),
                   //ToplamAlimMiktari = g.Sum(x => x.Miktar),
                   //AlimTarihi = alimlar.Where(w => w.TeklifId == g.Key).Select(s => s.AlimTarihi).FirstOrDefault(),
                   //NetFiyat = teklifler.Where(w => w.Id == g.Key).Select(s => s.NetFiyat).FirstOrDefault(),
                   //YayinlamaTurAdi = teklifler.Where(w => w.Id == g.Key).Select(s => s.YayinlamaTurAdi).FirstOrDefault(),
                   //TeklifiVerenEczaneAdi = teklifler.Where(w => w.Id == g.Key).Select(s => s.TeklifiVerenEczaneAdi).FirstOrDefault(),
                   //Kalan = teklifler.Where(w => w.Id == g.Key).Select(s => s.HedeflenenAlim).FirstOrDefault() - g.Sum(x => x.Miktar),
                   //ToplamFiyat = g.Sum(x => x.Miktar) * teklifler.Where(w => w.Id == g.Key).Select(s => s.NetFiyat).FirstOrDefault(),

                   IlacAdi = alimlar.Where(w => w.IlacId == g.Key).Select(s => s.IlacAdi).FirstOrDefault(),


                   BuAyHareketleri = _alimService.GetDetaylar().Where(w =>
    (teklifler.Where(x => x.IlacId == g.Key).Select(s => s.Id)).Contains(w.TeklifId)
    && w.GonderimTarihi > DateTime.Now.AddMonths(-1)).Count(),
                   GecenAyHareketleri = _alimService.GetDetaylar().Where(w =>
                     (teklifler.Where(x => x.IlacId == g.Key).Select(s => s.Id)).Contains(w.TeklifId)
                     && w.GonderimTarihi > DateTime.Now.AddMonths(-2) && w.GonderimTarihi < DateTime.Now.AddMonths(-1)).Count(),
                   DagitilanTeklifSayisi = teklifler.Where(w => w.IlacId == g.Key && alimlar.
                        Where(x => x.AlimDurumId == 4).Select(s => s.TeklifId).Contains(w.Id)).Count(),
                   DagitimToplami = _alimService.GetDetaylar().Where(w =>
                     (teklifler.Where(x => x.IlacId == g.Key).Select(s => s.Id)).Contains(w.TeklifId)).Sum(s => s.Miktar),
                   GrubaGirdigiTeklifSayisi = teklifler.Where(w => w.IlacId == g.Key).Count(),
                   TekliflerdenKazandirdigiMiktar = _alimService.GetDetaylar().Where(w =>
                  (teklifler.Where(x => x.IlacId == g.Key).Select(s => s.Id)).Contains(w.TeklifId))
                     .Sum(s => s.EtiketFiyati * s.Miktar) - _alimService.GetDetaylar().Where(w =>
                     (teklifler.Where(x => x.IlacId == g.Key).Select(s => s.Id)).Contains(w.TeklifId))
                     .Sum(s => s.NetFiyat * s.Miktar)

               }).OrderByDescending(o => o.TekliflerdenKazandirdigiMiktar).ToList();

                ViewBag.EczaneGrupId = new SelectList(gruplar, "Id", "Adi", Id);
                var pager = new Pager(alimlarGroupByIlacId.Count(), thispage);


                var model = new IlacRaporViewModel()
                {
                    AlimlarGroupByIlacIdler = alimlarGroupByIlacId.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList(),

                    //AlimDetaylar = alimlar,
                    //Eczaneler = eczaneler,
                    Ilaclar = ilaclar,
                    //TeklifDetaylar = teklifler,
                    Pager = pager
                };
                return View(model);

            }

        }

        public ActionResult Search(string Keywords)
        {
            int Id = 0;


            if (Id == 0)
            {//tüm gruplar için
                var user = _userService.GetByUserName(User.Identity.Name);
                var eczaneGruplar = _eczaneGrupService.GetDetayListByUser(user);
                var alimlar = _alimService.GetListByEczaneGruplar(eczaneGruplar);
                var teklifler = _teklifService.GetMyDetayListByEczaneGruplar(eczaneGruplar);
                var grupIdler = eczaneGruplar.Select(s => s.GrupId).Distinct().ToList();
                var gruplar = _grupService.GetList().Where(w => grupIdler.Contains(w.Id));
                var ilaclar = _ilacService.GetList();

                var alimlarGroupByIlacId = teklifler.GroupBy(g => g.IlacId) //.Select(s => s.Sum(d => d.Miktar));
               .Select(g => new AlimGroupByIlacId
               {
                   IlacId = g.Key,
                   //AlimDurumAdi = alimlar.Where(w => w.TeklifId == g.Key).Select(s => s.AlimDurumAdi).FirstOrDefault(),
                   //AlimDurumId = alimlar.Where(w => w.TeklifId == g.Key).Select(s => s.AlimDurumId).FirstOrDefault(),
                   //ToplamAlimMiktari = g.Sum(x => x.Miktar),
                   //AlimTarihi = alimlar.Where(w => w.TeklifId == g.Key).Select(s => s.AlimTarihi).FirstOrDefault(),
                   //NetFiyat = teklifler.Where(w => w.Id == g.Key).Select(s => s.NetFiyat).FirstOrDefault(),
                   //YayinlamaTurAdi = teklifler.Where(w => w.Id == g.Key).Select(s => s.YayinlamaTurAdi).FirstOrDefault(),
                   //TeklifiVerenEczaneAdi = teklifler.Where(w => w.Id == g.Key).Select(s => s.TeklifiVerenEczaneAdi).FirstOrDefault(),
                   //Kalan = teklifler.Where(w => w.Id == g.Key).Select(s => s.HedeflenenAlim).FirstOrDefault() - g.Sum(x => x.Miktar),
                   //ToplamFiyat = g.Sum(x => x.Miktar) * teklifler.Where(w => w.Id == g.Key).Select(s => s.NetFiyat).FirstOrDefault(),

                   IlacAdi = teklifler.Where(w => w.IlacId == g.Key).Select(s => s.IlacAdi).FirstOrDefault(),
                   GrupAdi = teklifler.Where(w => w.IlacId == g.Key).Select(s => s.TeklifiVerenEczaneGrupAdi).FirstOrDefault(),


                   BuAyHareketleri = _alimService.GetDetaylar().Where(w =>
                      (teklifler.Where(x => x.IlacId == g.Key).Select(s => s.Id)).Contains(w.TeklifId)
                      && w.GonderimTarihi > DateTime.Now.AddMonths(-1)).Count(),
                   GecenAyHareketleri = _alimService.GetDetaylar().Where(w =>
                     (teklifler.Where(x => x.IlacId == g.Key).Select(s => s.Id)).Contains(w.TeklifId)
                     && w.GonderimTarihi > DateTime.Now.AddMonths(-2) && w.GonderimTarihi < DateTime.Now.AddMonths(-1)).Count(),
                   DagitilanTeklifSayisi = teklifler.Where(w => w.IlacId == g.Key && alimlar.
                        Where(x => x.AlimDurumId == 4).Select(s => s.TeklifId).Contains(w.Id)).Count(),
                   DagitimToplami = _alimService.GetDetaylar().Where(w =>
                      (teklifler.Where(x => x.IlacId == g.Key).Select(s => s.Id)).Contains(w.TeklifId)).Sum(s => s.Miktar),
                   GrubaGirdigiTeklifSayisi = teklifler.Where(w => w.IlacId == g.Key).Count(),
                   TekliflerdenKazandirdigiMiktar = _alimService.GetDetaylar().Where(w =>
                  (teklifler.Where(x => x.IlacId == g.Key).Select(s => s.Id)).Contains(w.TeklifId))
                     .Sum(s => s.DepoFiyati * s.Miktar) - _alimService.GetDetaylar().Where(w =>
                     (teklifler.Where(x => x.IlacId == g.Key).Select(s => s.Id)).Contains(w.TeklifId))
                     .Sum(s => s.NetFiyat * s.Miktar)

               }).Where(p => Regex.Split(Keywords, @"\s")
                .Any(x => p.IlacAdi.ToLower().Contains(x.ToLower()) || p.IlacAdi.ToLower().Contains(x.ToLower())))
                .OrderByDescending(o => o.GrubaGirdigiTeklifSayisi)
                .OrderByDescending(o => o.GrubaGirdigiTeklifSayisi).ToList();

                ViewBag.EczaneGrupId = new SelectList(gruplar, "Id", "Adi", Id);

                var model = new IlacRaporViewModel()
                {
                    AlimlarGroupByIlacIdler = alimlarGroupByIlacId,

                    //AlimDetaylar = alimlar,
                    //Eczaneler = eczaneler,
                    Ilaclar = ilaclar,
                    //TeklifDetaylar = teklifler,
                };
                return View("Index", model);

            }
            else
            {//seçili grup için
                var user = _userService.GetByUserName(User.Identity.Name);
                var eczaneGruplar = _eczaneGrupService.GetDetayListByUser(user);
                //var grupId = Id;
                var eczaneGrupId = eczaneGruplar.Where(w => w.GrupId == Id).Select(s => s.Id).Distinct().ToList();
                var grupIdler = eczaneGruplar.Select(s => s.GrupId).Distinct().ToList();
                var gruplar = _grupService.GetList().Where(w => grupIdler.Contains(w.Id));
                var ilaclar = _ilacService.GetList();


                var teklifIdler = _teklifService.GetList()
                    .Where(w => eczaneGrupId.Contains(w.TeklifiVerenEczaneGrupId))
                    .Select(s => s.Id).ToList();
                var alimlar = _alimService.GetDetayListByTeklifler(teklifIdler).ToList();

                var teklifler = _teklifService.GetMyDetayListByEczaneGruplar(eczaneGruplar);


                var alimlarGroupByIlacId = alimlar.GroupBy(g => g.IlacId) //.Select(s => s.Sum(d => d.Miktar));
               .Select(g => new AlimGroupByIlacId
               {
                   IlacId = g.Key,
                   //AlimDurumAdi = alimlar.Where(w => w.TeklifId == g.Key).Select(s => s.AlimDurumAdi).FirstOrDefault(),
                   //AlimDurumId = alimlar.Where(w => w.TeklifId == g.Key).Select(s => s.AlimDurumId).FirstOrDefault(),
                   //ToplamAlimMiktari = g.Sum(x => x.Miktar),
                   //AlimTarihi = alimlar.Where(w => w.TeklifId == g.Key).Select(s => s.AlimTarihi).FirstOrDefault(),
                   //NetFiyat = teklifler.Where(w => w.Id == g.Key).Select(s => s.NetFiyat).FirstOrDefault(),
                   //YayinlamaTurAdi = teklifler.Where(w => w.Id == g.Key).Select(s => s.YayinlamaTurAdi).FirstOrDefault(),
                   //TeklifiVerenEczaneAdi = teklifler.Where(w => w.Id == g.Key).Select(s => s.TeklifiVerenEczaneAdi).FirstOrDefault(),
                   //Kalan = teklifler.Where(w => w.Id == g.Key).Select(s => s.HedeflenenAlim).FirstOrDefault() - g.Sum(x => x.Miktar),
                   //ToplamFiyat = g.Sum(x => x.Miktar) * teklifler.Where(w => w.Id == g.Key).Select(s => s.NetFiyat).FirstOrDefault(),

                   IlacAdi = alimlar.Where(w => w.IlacId == g.Key).Select(s => s.IlacAdi).FirstOrDefault(),
                   GrupAdi = teklifler.Where(w => w.IlacId == g.Key).Select(s => s.TeklifiVerenEczaneGrupAdi).FirstOrDefault(),


                   BuAyHareketleri = _alimService.GetDetaylar().Where(w =>
    (teklifler.Where(x => x.IlacId == g.Key).Select(s => s.Id)).Contains(w.TeklifId)
    && w.GonderimTarihi > DateTime.Now.AddMonths(-1)).Count(),
                   GecenAyHareketleri = _alimService.GetDetaylar().Where(w =>
                     (teklifler.Where(x => x.IlacId == g.Key).Select(s => s.Id)).Contains(w.TeklifId)
                     && w.GonderimTarihi > DateTime.Now.AddMonths(-2) && w.GonderimTarihi < DateTime.Now.AddMonths(-1)).Count(),
                   DagitilanTeklifSayisi = teklifler.Where(w => w.IlacId == g.Key && alimlar.
                        Where(x => x.AlimDurumId == 4).Select(s => s.TeklifId).Contains(w.Id)).Count(),
                   DagitimToplami = _alimService.GetDetaylar().Where(w =>
                     (teklifler.Where(x => x.IlacId == g.Key).Select(s => s.Id)).Contains(w.TeklifId)).Sum(s => s.Miktar),
                   GrubaGirdigiTeklifSayisi = teklifler.Where(w => w.IlacId == g.Key).Count(),
                   TekliflerdenKazandirdigiMiktar = _alimService.GetDetaylar().Where(w =>
                  (teklifler.Where(x => x.IlacId == g.Key).Select(s => s.Id)).Contains(w.TeklifId))
                     .Sum(s => s.DepoFiyati * s.Miktar) - _alimService.GetDetaylar().Where(w =>
                     (teklifler.Where(x => x.IlacId == g.Key).Select(s => s.Id)).Contains(w.TeklifId))
                     .Sum(s => s.NetFiyat * s.Miktar)

               }).Where(p => Regex.Split(Keywords, @"\s")
                .Any(x => p.IlacAdi.ToLower().Contains(x.ToLower()) || p.IlacAdi.ToLower().Contains(x.ToLower())))
                .OrderByDescending(o => o.GrubaGirdigiTeklifSayisi)
                .OrderByDescending(o => o.GrubaGirdigiTeklifSayisi).ToList();

                ViewBag.EczaneGrupId = new SelectList(gruplar, "Id", "Adi", Id);


                var model = new IlacRaporViewModel()
                {
                    AlimlarGroupByIlacIdler = alimlarGroupByIlacId,

                    //AlimDetaylar = alimlar,
                    //Eczaneler = eczaneler,
                    Ilaclar = ilaclar,
                    //TeklifDetaylar = teklifler,
                };
                return View("Index", model);

            }
          ;//result:model

        }
        // GET: TeklifNobet/Teklif/Details/5
        public ActionResult Details(int id)
        {
            if (id < 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teklif Teklif = _TeklifService.GetById(id);
            if (Teklif == null)
            {
                return HttpNotFound();
            }
            return View(Teklif);
        }
    }
}