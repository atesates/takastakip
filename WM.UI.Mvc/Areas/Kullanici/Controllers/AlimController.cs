using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WM.Northwind.Business.Abstract.Authorization;
using WM.Northwind.Business.Abstract.IlacTakip;
using WM.UI.Mvc.Areas.Kullanici.Models;
using WM.Northwind.Entities.Concrete.IlacTakip;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using System.Text.RegularExpressions;

namespace WM.UI.Mvc.Areas.Kullanici.Controllers
{
    [Authorize(Roles = "Admin,Eczane,Grup Yöneticisi")]
    public class AlimController : Controller
    {
        // GET: Kullanici/Alim
        #region ctor
        private IIlacService _ilacService;
        private IEczaneService _eczaneService;
        private IAlimService _alimService;
        private IEczaneGrupService _eczaneGrupService;
        private IGrupService _grupService;
        private IUserService _userService;
        private IAlimDurumService _alimDurumService;
        private ITeklifService _teklifService;
        private ITeklifDurumService _teklifDurumService;
        private IITStransferDurumService _iTStransferDurumService;

        public AlimController(IEczaneGrupService eczaneGrupService,
                                IAlimService AlimService,
                                IIlacService ilacService,
                                IEczaneService eczaneService,
                                IGrupService grupService,
                                IAlimService alimService,
                                ITeklifService teklifService,
                                ITeklifDurumService teklifDurumService,
                                IAlimDurumService alimDurumService,
                                IITStransferDurumService iTStransferDurumService,
                                IUserService userService)
        {
            _ilacService = ilacService;
            _eczaneService = eczaneService;
            _grupService = grupService;
            _alimService = alimService;
            _userService = userService;
            _teklifService = teklifService;
            _teklifDurumService = teklifDurumService;
            _iTStransferDurumService = iTStransferDurumService;
            _alimDurumService = alimDurumService;
            _eczaneGrupService = eczaneGrupService;
        }
        #endregion
        public ActionResult Index(int? page, string teklifDurumId, string alimDurumId)
        {//id eczaneGrupId EczaneGrupcontroller dan geliyor
         //if(Id == null)
         //    return RedirectToAction("Index", "EczaneGrup");

            //int id = Convert.ToInt32(Id);
            int thispage = 0;
            if (page != null)
                thispage = Convert.ToInt32(page);
            else thispage = 1;

            var alimDurumlar = _alimDurumService.GetList();
            ViewBag.alimTurId = new SelectList(alimDurumlar, "Id", "Adi");

            var ITStransferDurumlar = _iTStransferDurumService.GetList();
            ViewBag.ITStransferDurumId = new SelectList(ITStransferDurumlar, "Id", "Adi");
            var AlimDurumlar = _alimDurumService.GetList().Where(w => w.AliciTarafiMi == false);
            ViewBag.AlimDurumId = new SelectList(AlimDurumlar, "Id", "Adi");

            var model = new AlimDetayViewModel();
            model = getAlimDetayViewModel(thispage);
            return View(model);
        }
        private AlimDetayViewModel getAlimDetayViewModel(int thispage)
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var gruplar = _grupService.GetListByUser(user).ToList();
            var eczaneGruplar = _eczaneGrupService.GetDetayListByUser(user).ToList();
            var eczaneIdler = _eczaneGrupService.GetDetayListByUser(user).Select(s => s.EczaneId).ToList();
            var eczaneler = _eczaneService.GetList().Where(w => eczaneIdler.Contains(w.Id)).ToList();
            //kendi yaptığım alımlar:
            eczaneGruplar = _eczaneGrupService.GetMyDetayListByUser(user);
            //var eczaneler = _eczaneService.GetListByUser(user);

            //var teklifler = _teklifService.GetListByUser(user);       

            var teklifler = _teklifService.GetDetaylar().ToList();
            var alimlar = _alimService.GetMyListByEczaneGruplar(eczaneGruplar).ToList().OrderByDescending(o => o.AlimTarihi).ToList();
            //o eczanenin alım yaptığı teklifler:
            var alimYaptigiTeklifler = teklifler.Where(w => alimlar.Select(s => s.TeklifId).Contains(w.Id)).ToList();

            var teklifIdler = alimlar.Select(s => s.TeklifId);
            var ilacIdler = teklifler.Where(w => teklifIdler.Contains(w.Id)).Select(s => s.IlacId);
            var ilaclar = _ilacService.GetList().Where(w => ilacIdler.Contains(w.Id)).ToList();

            var pager = new Pager(alimlar.Count(), thispage);

            var model = new AlimDetayViewModel()
            {
                AlimDetaylar = alimlar,//.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList(),
                //Eczaneler = eczaneler,
                Ilaclar = ilaclar,
                EczaneGrupDetaylar = eczaneGruplar,
                TeklifDetaylar = alimYaptigiTeklifler,
                Pager = pager

            };
            return model;
        }
        [HttpGet]
        //[ValidateAntiForgeryToken] get de olmuyor
        public ActionResult GetAlimlar(string page, string teklifDurumId, string alimDurumId)
        {
            List<AlimGroupByTeklifIdAlimDetaylarViewModel> alimGroupByTeklifIdAlimDetaylarViewModel = new List<AlimGroupByTeklifIdAlimDetaylarViewModel>();
            alimGroupByTeklifIdAlimDetaylarViewModel = GetAlimDetayViewModel_GroupByTeklifId(alimGroupByTeklifIdAlimDetaylarViewModel, page, teklifDurumId, alimDurumId);
            return PartialView("AlimPartialView", alimGroupByTeklifIdAlimDetaylarViewModel);
        }
        private List<AlimGroupByTeklifIdAlimDetaylarViewModel> GetAlimDetayViewModel_GroupByTeklifId(List<AlimGroupByTeklifIdAlimDetaylarViewModel> alimGroupByTeklifIdAlimDetaylarViewModel,
            string page, string teklifDurumId, string alimDurumId)
        {
            
            var alimDurumlar = _alimDurumService.GetList();
            ViewBag.alimTurId = new SelectList(alimDurumlar, "Id", "Adi");
            // var temp = TempData["EczaneGrupId"];        

            var user = _userService.GetByUserName(User.Identity.Name);
            var gruplar = _grupService.GetListByUser(user).ToList();
            var eczaneGruplar = _eczaneGrupService.GetDetayListByUser(user).ToList();
            var eczaneIdler = _eczaneGrupService.GetDetayListByUser(user).Select(s => s.EczaneId).ToList();
            var eczaneGrupIdlerim = _eczaneGrupService.GetListByUser(user).Select(s=>s.EczaneId);
            var eczaneler = _eczaneService.GetList().Where(w => eczaneIdler.Contains(w.Id)).ToList();
            //kendi yaptığım alımlar:
            // eczaneGruplar = _eczaneGrupService.GetMyDetayListByUser(user);
            //var eczaneler = _eczaneService.GetListByUser(user);
            var eczaneGruplarim = _eczaneGrupService.GetListByUser(user).Select(s => s.Id);
            //var teklifler = _teklifService.GetListByUser(user);       

            var teklifler = _teklifService.GetDetaylar().ToList();

            var alimlar = new List<AlimDetay>();
            if (teklifDurumId != null && teklifDurumId != "")
            {
                if (alimDurumId != null && alimDurumId != "")
                {
                    alimlar = _alimService.GetMyListByEczaneGruplar(eczaneGruplar)
                        .Where(w=>w.TeklifDurumId == Convert.ToInt32(teklifDurumId) 
                        && w.AlimDurumId == Convert.ToInt32(alimDurumId))
                        .ToList();
                }
                else
                {
                    alimlar = _alimService.GetMyListByEczaneGruplar(eczaneGruplar)
                        .Where(w => w.TeklifDurumId == Convert.ToInt32(teklifDurumId))
                        .ToList();
                }
            }
            else
            {
                if (alimDurumId != null && alimDurumId != "")
                {
                    alimlar = _alimService.GetMyListByEczaneGruplar(eczaneGruplar)
                        .Where(w => w.AlimDurumId == Convert.ToInt32(alimDurumId))
                        .ToList();
                }
                else
                {
                    alimlar = _alimService.GetMyListByEczaneGruplar(eczaneGruplar)
                        .ToList();
                }
            }

            var alimlarGroupByTeklifId = alimlar.GroupBy(g => g.TeklifId) //.Select(s => s.Sum(d => d.Miktar));
                    .Select(g => new AlimGroupByTeklifId
                    {
                        //EczaneGln = _eczaneService.GetById(alimlar.Where(w => w.TeklifId == g.Key).Select(s => s.EczaneId).FirstOrDefault()).EczaneGln,
                        AlimDurumAdi = alimlar.Where(w => w.TeklifId == g.Key).Select(s => s.AlimDurumAdi).FirstOrDefault(),
                        AlimDurumId = alimlar.Where(w => w.TeklifId == g.Key).Select(s => s.AlimDurumId).FirstOrDefault(),
                        TeklifId = g.Key,
                        // AlimTarihi = alimlar.Where(w => w.TeklifId == g.Key).Select(s => s.AlimTarihi).FirstOrDefault(),
                        ToplamAlimMiktari = g.Sum(x => x.Miktar),
                        AliciEczaneAdi = alimlar.Where(w => w.TeklifId == g.Key).Select(s => s.EczaneAdi).FirstOrDefault(),
                        DepoFiyat = teklifler.Where(w => w.Id == g.Key).Select(s => s.DepoFiyat).FirstOrDefault(),
                        NetFiyat = teklifler.Where(w => w.Id == g.Key).Select(s => s.NetFiyat).FirstOrDefault(),
                        IlacAdi = teklifler.Where(w => w.Id == g.Key).Select(s => s.IlacAdi).FirstOrDefault(),
                        MalFazlasi = teklifler.Where(w => w.Id == g.Key).Select(s => s.MalFazlasi).FirstOrDefault(),
                        GrupAdi = teklifler.Where(w => w.Id == g.Key).Select(s => s.TeklifiVerenEczaneGrupAdi).FirstOrDefault(),
                        Minimum = teklifler.Where(w => w.Id == g.Key).Select(s => s.Minimum).FirstOrDefault(),
                        Maksimum = teklifler.Where(w => w.Id == g.Key).Select(s => s.Maksimum).FirstOrDefault(),
                        BitisTarihi = teklifler.Where(w => w.Id == g.Key).Select(s => s.BitisTarihi).FirstOrDefault(),
                        IlacMiad = teklifler.Where(w => w.Id == g.Key).Select(s => s.IlacMiad).FirstOrDefault(),
                        YayinlamaTurAdi = teklifler.Where(w => w.Id == g.Key).Select(s => s.YayinlamaTurAdi).FirstOrDefault(),
                        TeklifiVerenEczaneAdi = teklifler.Where(w => w.Id == g.Key).Select(s => s.TeklifiVerenEczaneAdi).FirstOrDefault(),
                        HedeflenenAlim = teklifler.Where(w => w.Id == g.Key).Select(s => s.HedeflenenAlim).FirstOrDefault(),
                        Kalan = teklifler.Where(w => w.Id == g.Key).Select(s => s.HedeflenenAlim).FirstOrDefault() - g.Sum(x => x.Miktar),
                        ToplamAlimlar = alimlar.Where(w => w.TeklifId == g.Key && w.EczaneGrupId == eczaneGruplarim.FirstOrDefault()).Sum(s => s.Miktar),
                        ToplamFiyat = g.Sum(x => x.Miktar) * teklifler.Where(w => w.Id == g.Key).Select(s => s.NetFiyat).FirstOrDefault(),
                        TeklifDurumAdi = teklifler.Where(w => w.Id == g.Key).Select(s => s.TeklifDurumAdi).FirstOrDefault(),
                        Id = teklifler.Where(w => w.Id == g.Key).Select(s => s.Id).FirstOrDefault(),
                    }).ToList();

            var alimlarGroupby = alimlar.GroupBy(g => g.TeklifId).ToList();
            //o eczanenin alım yaptığı teklifler:
            var alimYaptigiTeklifler = teklifler.Where(w => alimlar.Select(s => s.TeklifId).Contains(w.Id)).ToList();

            var teklifIdler = alimlar.Select(s => s.TeklifId);
            var ilacIdler = teklifler.Where(w => teklifIdler.Contains(w.Id)).Select(s => s.IlacId);
            var ilaclar = _ilacService.GetList().Where(w => ilacIdler.Contains(w.Id)).ToList();
            if (page == "" || page == null)
            {
                page = "1";
            }
            var pager = new Pager(alimlarGroupByTeklifId.Count(), Convert.ToInt32(page));
            var AlimDurumlar = _alimDurumService.GetList();//.Where(w => w.AliciTarafiMi != true);
            ViewBag.AlimDurumIdFilter = new SelectList(AlimDurumlar, "Id", "Adi", alimDurumId);
            var TeklifDurumlar = _teklifDurumService.GetList();
            ViewBag.TeklifDurumIdFilter = new SelectList(TeklifDurumlar, "Id", "Adi", teklifDurumId);
            
            foreach (var item in alimlarGroupByTeklifId)
            {
                var alimDetaylars = alimlar.Where(w => w.TeklifId == item.TeklifId).ToList();

                alimGroupByTeklifIdAlimDetaylarViewModel.Add(new AlimGroupByTeklifIdAlimDetaylarViewModel
                {
                    AlimGroupByTeklifId = item,
                    AlimDetaylar = alimDetaylars,
                    Pager = pager
                });
            }
            return alimGroupByTeklifIdAlimDetaylarViewModel;

        }
        
        // GET: AlimNobet/Alim/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return RedirectToAction("Index", "Alim");
            int Id = Convert.ToInt32(id);
            if (Id < 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AlimDetay Alim = _alimService.GetDetayById(Id);
            if (Alim == null)
            {
                return HttpNotFound();
            }
            return View(Alim);
        }

        // GET: AlimNobet/Alim/Create
        [Authorize(Roles = "Admin,Grup Yöneticisi, Eczane")]
        public ActionResult Create(int? id)
        {//bu metod teklif sayfasından çağırılır. teklife alım yapmak için
            Alim Alim = new Alim();
            var user = _userService.GetByUserName(User.Identity.Name);
            var alimDurumlar = _alimDurumService.GetList();
            var iTStransferDurumlar = _iTStransferDurumService.GetList();
           
            if (id == null)
                return RedirectToAction("Index", "Teklif");

            var teklifId = Convert.ToInt32(id);
            var ilacId = _teklifService.GetById(Convert.ToInt32(teklifId)).IlacId;
            var ilacAdi = _ilacService.GetById(ilacId).Adi;

            ViewBag.Ilac = ilacAdi;
            ViewBag.AlimDurumId = new SelectList(alimDurumlar, "Id", "Adi");
            ViewBag.ITStransferDurumId = new SelectList(iTStransferDurumlar, "Id", "Adi");
            ViewBag.teklifId = teklifId;
            Alim.TeklifId = teklifId;
            var teklifVerenEczaneGrupId = _teklifService.GetDetayById(teklifId).TeklifiVerenEczaneGrupId;
            var grupId = _eczaneGrupService.GetDetayById(teklifVerenEczaneGrupId).GrupId;
            var eczaneGruplar = _eczaneGrupService.GetListByUser(user);

            Alim.EczaneGrupId = eczaneGruplar.Where(w => w.GrupId == grupId).Select(s => s.Id).FirstOrDefault();

            //try
            //{
            //    Alim.EczaneGrupId = Convert.ToInt32(Session["EczanegrupId"]);
            //    ViewBag.eczaneGrupId = Convert.ToInt32(Session["EczanegrupId"]);
            //}
            //catch
            //{
            //    return RedirectToAction("Index", "EczaneGrup");
            //}

            return View(Alim);
        }

        // POST: AlimNobet/Alim/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,AlimDurum,ITStransferDurum,Miktar")] Alim Alim)
        //{
        //    var user = _userService.GetByUserName(User.Identity.Name);
        //    var eczaneGrupId = _eczaneGrupService.GetListByUser(user).Select(s => s.EczaneId).FirstOrDefault();
        //    Alim.EczaneGrupId = eczaneGrupId;
        //    Alim.TeklifId = Convert.ToInt32(Session["AlimYapilacakTeklifId"].ToString());
        //    Alim.AlimTarihi = DateTime.Now;

        //    if (ModelState.IsValid)
        //    {
        //        _alimService.Insert(Alim);
        //        return RedirectToAction("Index");
        //    }
        //    var alimDurumlar = _alimDurumService.GetList();
        //    var iTStransferDurumlar = _iTStransferDurumService.GetList();
        //    ViewBag.AlimDurumId = new SelectList(alimDurumlar, "Id", "Adi");
        //    ViewBag.ITStransferDurumId = new SelectList(iTStransferDurumlar, "Id", "Adi");

        //    return View(Alim);
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Alim Alim)
        {

            Alim.AlimTarihi = DateTime.Now;
            Alim.Miktar = Alim.Miktar;
            Alim.AlimDurumId = 1;//bekleniyor
            Alim.ITStransferDurumId = 1;//bekleniyor


            if (ModelState.IsValid)
            {
                try
                {
                    _alimService.Insert(Alim);
                    return RedirectToAction("Index");

                }
                catch (Exception ex)
                {
                    TempData["Message"] = "ERROR:" + ex.Message.ToString();
                }
            }
            var alimDurumlar = _alimDurumService.GetList();
            var iTStransferDurumlar = _iTStransferDurumService.GetList();
            ViewBag.AlimDurumId = new SelectList(alimDurumlar, "Id", "Adi", Alim.AlimDurumId);
            ViewBag.ITStransferDurumId = new SelectList(iTStransferDurumlar, "Id", "Adi", Alim.ITStransferDurumId);

            return View(Alim);
        }
        // GET: AlimNobet/Alim/Edit/5
        [Authorize(Roles = "Admin,Grup Yöneticisi,Eczane")]
        public ActionResult Edit(int? id)
        {
            int Id = 0;
            try
            {
                Id = Convert.ToInt32(id);
            }
            catch
            {
                return RedirectToAction("Index", "Teklif");

            }
            if (id < 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Alim Alim = _alimService.GetById(Id);
            var ITStransferDurumlar = _iTStransferDurumService.GetList();
            ViewBag.ITStransferDurumId = new SelectList(ITStransferDurumlar, "Id", "Adi", Alim.ITStransferDurumId);
            var teklifId = Alim.TeklifId;
            ViewBag.TeklifId = teklifId;
            var alimDurumlar = _alimDurumService.GetList().Where(w => w.Id == 4 || w.Id == 1 || w.Id == 5);
            ViewBag.AlimDurumId = new SelectList(alimDurumlar, "Id", "Adi", Alim.AlimDurumId);

            var user = _userService.GetByUserName(User.Identity.Name);

            var teklifVerenEczaneGrupId = _teklifService.GetDetayById(teklifId).TeklifiVerenEczaneGrupId;
            var grupId = _eczaneGrupService.GetDetayById(teklifVerenEczaneGrupId).GrupId;
            var eczaneGruplar = _eczaneGrupService.GetListByUser(user);

            Alim.EczaneGrupId = Alim.EczaneGrupId;// eczaneGruplar.Where(w => w.GrupId == grupId).Select(s => s.Id).FirstOrDefault();


            if (Alim == null)
            {
                return HttpNotFound();
            }
            return View(Alim);
        }

        // POST: AlimNobet/Alim/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AlimDurumId,AlimTarihi,TeklifId,EczaneGrupId,ITStransferDurumId,Miktar")] Alim Alim)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _alimService.Update(Alim);
                    int? Id = Alim.EczaneGrupId;
                    return RedirectToAction("Index", new { id = Id });
                }
                catch (Exception ex)
                {
                    TempData["Message"] = "ERROR:" + ex.Message.ToString();
                }
            }
            return View(Alim);
        }

        // GET: AlimNobet/Alim/Delete/5
        [Authorize(Roles = "Admin,Grup Yöneticisi,Eczane")]
        public ActionResult Delete(int id)
        {
            if (id < 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Alim Alim = _alimService.GetById(id);
            var teklif = _teklifService.GetById(Alim.TeklifId);
            var ilacId = teklif.IlacId;
            ViewBag.IlacAdi = _ilacService.GetById(ilacId).Adi;

            if (Alim == null)
            {
                return HttpNotFound();
            }
            return View(Alim);
        }

        // POST: AlimNobet/Alim/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Alim Alim = _alimService.GetById(id);
            try
            {
                _alimService.Delete(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "ERROR:" + ex.Message.ToString();
            }
            return View(Alim);

        }
    }
}