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
    public class GrupRaporController : Controller
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

        public GrupRaporController(IEczaneGrupService eczaneGrupService,
                                IEczaneUserService eczaneUserService,
                                ITeklifService TeklifService,
                                IEczaneService eczaneService,
                                ITeklifService teklifService,
                                IUserService userService,
                                ITeklifTurService teklifTurService,
                                IGrupService grupService,
                                IYayinlamaTurService yayinlamaTurService,
                                ITeklifDurumService teklifDurumService,
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
            _alimService = alimService;
        }
        #endregion

        public ActionResult Index(int? id, int? page)
        {
            int Id = Convert.ToInt32(id);
            int thispage = 0;
            if (page != 0)
                thispage = Convert.ToInt32(page);

            var user = _userService.GetByUserName(User.Identity.Name);
            var eczaneGruplar = _eczaneGrupService.GetDetayListByUser(user).ToList();
            var grupIdler = eczaneGruplar.Select(s => s.GrupId).Distinct().ToList();
            var gruplar = _grupService.GetList().Where(w => grupIdler.Contains(w.Id)).ToList();
            ViewBag.EczaneGrupId = new SelectList(gruplar, "Id", "Adi", Id);

            if (Id == 0)
            {//seçilmemiş ise ilk sıradaki grup gelsin
                Id = grupIdler.First();
            }
            var eczaneGruplarSeciliEczaneGrupIdIcin = _eczaneGrupService.GetDetaylar().Where(w => w.GrupId == Id).ToList();
            //var grupId = Id;
            var eczaneGrupIdler = eczaneGruplar.Where(w => w.GrupId == Id).Select(s => s.Id).Distinct().ToList();
            var teklifIdler = _teklifService.GetList()
                .Where(w => eczaneGrupIdler.Contains(w.TeklifiVerenEczaneGrupId))
                .Select(s => s.Id).ToList();
            var alimlar = _alimService.GetDetayListByTeklifler(teklifIdler).ToList();
            var teklifler = _teklifService.GetMyDetayListByEczaneGruplar(eczaneGruplarSeciliEczaneGrupIdIcin).ToList();
            var eczaneler = _eczaneService.GetListByUser(user);

            var alimlarGroupByEczaneGrupId = alimlar.GroupBy(g => g.EczaneGrupId) //.Select(s => s.Sum(d => d.Miktar));
           .Select(g => new AlimGroupByEczaneGrupId
           {
               EczaneGrupId = g.Key,
               EczaneAdi = alimlar.Where(w => w.EczaneGrupId == g.Key).Select(s => s.EczaneAdi).FirstOrDefault(),
               GrupAdi = eczaneGruplar.Where(w => w.Id == g.Key).Select(s => s.GrupAdi).FirstOrDefault(),
               GrupId = eczaneGruplar.Where(w => w.Id == g.Key).Select(s => s.GrupId).FirstOrDefault(),
               EczaneGln = alimlar.Where(w => w.EczaneGrupId == g.Key).Select(s => s.EczaneGln).FirstOrDefault(),
               Gelir = _alimService.GetDetaylar().Where(w =>
               (teklifler.Where(x => x.TeklifiVerenEczaneGrupId == g.Key).Select(s => s.Id)).Contains(w.TeklifId)
               && w.AlimDurumId == 4)
                 .Sum(s => s.NetFiyat * s.Miktar),
               Gider = alimlar.Where(w => w.AlimDurumId == 4 && w.EczaneGrupId == g.Key).Sum(s => s.NetFiyat * s.Miktar)
             ,
               Bakiye =
                 _alimService.GetDetaylar().Where(w =>
                (teklifler.Where(x => x.TeklifiVerenEczaneGrupId == g.Key).Select(s => s.Id)).Contains(w.TeklifId)
                && w.AlimDurumId == 4)
                 .Sum(s => s.NetFiyat * s.Miktar)
                - alimlar.Where(w => w.AlimDurumId == 4 && w.EczaneGrupId == g.Key).Sum(s => s.NetFiyat * s.Miktar)
                ,
               ToplamAlimMiktari = alimlar.Where(w => w.EczaneGrupId == g.Key).Sum(s => s.Miktar),
               BuAyHareketleri = alimlar.Where(w => w.EczaneGrupId == g.Key && w.GonderimTarihi > DateTime.Now.AddMonths(-1)).Sum(s => s.Id),
               GecenAyHareketleri = alimlar.Where(w => w.EczaneGrupId == g.Key && w.GonderimTarihi > DateTime.Now.AddMonths(-2) && w.GonderimTarihi < DateTime.Now.AddMonths(-1)).Sum(s => s.Id),
               DagittigiTeklifSayisi = teklifler.Where(w => w.TeklifiVerenEczaneGrupId == g.Key && alimlar.
                    Where(x => x.AlimDurumId == 4).Select(s => s.TeklifId).Contains(w.Id)).Count(),
               DagitimToplami = _alimService.GetDetaylar().Where(w =>
                    (teklifler.Where(x => x.TeklifiVerenEczaneGrupId == g.Key).Select(s => s.Id))
                    .Contains(w.TeklifId) && w.AlimDurumId == 4).Sum(s => s.Miktar),
               GrubaGirdigiTeklifSayisi = teklifler.Where(w => w.TeklifiVerenEczaneGrupId == g.Key).Count(),
               TekliflerdenKazandirdigiMiktar =
               _alimService.GetDetaylar().Where(w =>
                    (teklifler.Where(x => x.TeklifiVerenEczaneGrupId == g.Key).Select(s => s.Id)).Contains(w.TeklifId))
                    .Sum(s => s.DepoFiyati * s.Miktar) - _alimService.GetDetaylar().Where(w =>
                    (teklifler.Where(x => x.TeklifiVerenEczaneGrupId == g.Key).Select(s => s.Id)).Contains(w.TeklifId))
                    .Sum(s => s.NetFiyat * s.Miktar)

           }).Where(w => w.GrupId == Id)
           .OrderByDescending(o => o.Bakiye).ToList();

            //alım yapmamış ama teklif vermiş olanlar olabilir, bunlar için teklif tablosundan çekmek gerekir herşeyi:
            var tekliflerGroupByEczaneGrupId = teklifler
                .Where(w => !alimlarGroupByEczaneGrupId.Select(s => s.EczaneGrupId).Contains(w.TeklifiVerenEczaneGrupId))
                .GroupBy(g => g.TeklifiVerenEczaneGrupId)

        .Select(g => new AlimGroupByEczaneGrupId
        {
            EczaneGrupId = g.Key,
            EczaneAdi = teklifler.Where(w => w.TeklifiVerenEczaneGrupId == g.Key).Select(s => s.TeklifiVerenEczaneAdi).FirstOrDefault(),
            GrupAdi = eczaneGruplar.Where(w => w.Id == g.Key).Select(s => s.GrupAdi).FirstOrDefault(),
            GrupId = eczaneGruplar.Where(w => w.Id == g.Key).Select(s => s.GrupId).FirstOrDefault(),
            EczaneGln = teklifler.Where(w => w.TeklifiVerenEczaneGrupId == g.Key).Select(s => s.TeklifiVerenEczaneGln).FirstOrDefault(),
                //EczaneGln = eczaneler.Where(x => x.Id == eczaneGruplar.Where(w => w.Id == g.Key).Select(s => s.EczaneId).FirstOrDefault()).Select(s => s.EczaneGln).FirstOrDefault(),
                Gelir = _alimService.GetDetaylar().Where(w =>
                (teklifler.Where(x => x.TeklifiVerenEczaneGrupId == g.Key).Select(s => s.Id)).Contains(w.TeklifId)
                && w.AlimDurumId == 4)
                 .Sum(s => s.NetFiyat * s.Miktar),
            Gider = alimlar.Where(w => w.AlimDurumId == 4 && w.EczaneGrupId == g.Key).Sum(s => s.NetFiyat * s.Miktar)
             ,
            Bakiye = _alimService.GetDetaylar().Where(w =>
                (teklifler.Where(x => x.TeklifiVerenEczaneGrupId == g.Key).Select(s => s.Id)).Contains(w.TeklifId)
                && w.AlimDurumId == 4)
                 .Sum(s => s.NetFiyat * s.Miktar) - alimlar.Where(w => w.AlimDurumId == 4 && w.EczaneGrupId == g.Key).Sum(s => s.NetFiyat * s.Miktar),


            ToplamAlimMiktari = alimlar.Where(w => w.TeklifVerenEczaneGrupId == teklifler.Where(x => x.TeklifiVerenEczaneGrupId == g.Key).Select(s => s.Id).FirstOrDefault()).Sum(s => s.Miktar),

            BuAyHareketleri = teklifler.Where(w => w.TeklifiVerenEczaneGrupId == g.Key && w.GonderimTarihi > DateTime.Now.AddMonths(-1)).Sum(s => s.Id),
            GecenAyHareketleri = teklifler.Where(w => w.TeklifiVerenEczaneGrupId == g.Key && w.GonderimTarihi > DateTime.Now.AddMonths(-2) && w.GonderimTarihi < DateTime.Now.AddMonths(-1)).Sum(s => s.Id),
            DagittigiTeklifSayisi = teklifler.Where(w => w.TeklifiVerenEczaneGrupId == g.Key && alimlar.
                 Where(x => x.AlimDurumId == 4).Select(s => s.TeklifId).Contains(w.Id)).Count(),
            DagitimToplami = _alimService.GetDetaylar().Where(w =>
            (teklifler.Where(x => x.TeklifiVerenEczaneGrupId == g.Key).Select(s => s.Id))
            .Contains(w.TeklifId) && w.AlimDurumId == 4).Sum(s => s.Miktar),
            GrubaGirdigiTeklifSayisi = teklifler.Where(w => w.TeklifiVerenEczaneGrupId == g.Key).Count(),
            TekliflerdenKazandirdigiMiktar =
              _alimService.GetDetaylar().Where(w =>
                   (teklifler.Where(x => x.TeklifiVerenEczaneGrupId == g.Key).Select(s => s.Id)).Contains(w.TeklifId))
                   .Sum(s => s.DepoFiyati * s.Miktar) - _alimService.GetDetaylar().Where(w =>
                    (teklifler.Where(x => x.TeklifiVerenEczaneGrupId == g.Key).Select(s => s.Id)).Contains(w.TeklifId))
                   .Sum(s => s.NetFiyat * s.Miktar)

        })//.Where(w => w.GrupId == Id)
        .OrderByDescending(o => o.Bakiye).ToList();

            var unionAll = tekliflerGroupByEczaneGrupId.Union(alimlarGroupByEczaneGrupId)
               .OrderByDescending(o => o.Bakiye).ToList();
            var pager = new Pager(unionAll.Count(), thispage);
            var model = new GrupRaporViewModel()
            {
                AlimlarGroupByEczaneGrupId = unionAll.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList(),

                //AlimDetaylar = alimlar,
                //Eczaneler = eczaneler,
                EczaneGrupDetaylar = eczaneGruplarSeciliEczaneGrupIdIcin,
                //TeklifDetaylar = teklifler,
                Pager = pager,
            };
            return View(model);

        }
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