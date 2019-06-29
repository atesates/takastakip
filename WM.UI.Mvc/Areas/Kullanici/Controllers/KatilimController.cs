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
    public class KatilimController : Controller
    {
        // GET: Kullanici/Katilim
        #region ctor
        private IIlacService _ilacService;
        private IEczaneService _eczaneService;
        private IEczaneUserService _eczaneUserService;
        private IKatilimService _katilimService;
        private IEczaneGrupService _eczaneGrupService;
        private IGrupService _grupService;
        private IUserService _userService;
        private ITalepService _talepService;

        public KatilimController(IEczaneGrupService eczaneGrupService,
                                IKatilimService KatilimService,
                                IIlacService ilacService,
                                IEczaneUserService eczaneUserService,
                                IEczaneService eczaneService,
                                IGrupService grupService,
                                IKatilimService alimService,
                                ITalepService talepService,
                                IUserService userService)
        {
            _ilacService = ilacService;
            _eczaneService = eczaneService;
            _grupService = grupService;
            _eczaneUserService = eczaneUserService;
            _katilimService = alimService;
            _userService = userService;
            _talepService = talepService;
            _eczaneGrupService = eczaneGrupService;
        }
        #endregion
        public ActionResult Index(int? page)
        {//id eczaneGrupId EczaneGrupcontroller dan geliyor
         //if(Id == null)
         //    return RedirectToAction("Index", "EczaneGrup");

            //int id = Convert.ToInt32(Id);
            int thispage = 0;
            if (page != null)
                thispage = Convert.ToInt32(page);
            else thispage = 1;

            var model = new KatilimDetayViewModel();
            model = getKatilimDetayViewModel(thispage);
            return View(model);
        }

        [HttpGet]
        //[ValidateAntiForgeryToken] get de olmuyor
        public ActionResult GetKatilimlar(string page)
        {
            List<KatilimGroupByTalepIdKatilimDetaylarViewModel> katilimGroupByTalepIdKatilimDetaylarViewModel = new List<KatilimGroupByTalepIdKatilimDetaylarViewModel>();
            katilimGroupByTalepIdKatilimDetaylarViewModel = GetAlimDetayViewModel_GroupByTeklifId(katilimGroupByTalepIdKatilimDetaylarViewModel, page);
            return PartialView("KatilimPartialView", katilimGroupByTalepIdKatilimDetaylarViewModel);
        }
        private List<KatilimGroupByTalepIdKatilimDetaylarViewModel> GetAlimDetayViewModel_GroupByTeklifId(List<KatilimGroupByTalepIdKatilimDetaylarViewModel> katilimGroupByTalepIdKatilimDetaylarViewModel
            , string page)
        {      
            var user = _userService.GetByUserName(User.Identity.Name);
            var gruplar = _grupService.GetListByUser(user).ToList();
            var eczaneGruplar = _eczaneGrupService.GetDetayListByUser(user).ToList();
            var eczaneIdler = _eczaneGrupService.GetDetayListByUser(user).Select(s => s.EczaneId).ToList();
            var eczaneler = _eczaneService.GetList().Where(w => eczaneIdler.Contains(w.Id)).ToList();
            //kendi yaptığım alımlar:
            // eczaneGruplar = _eczaneGrupService.GetMyDetayListByUser(user);
            //var eczaneler = _eczaneService.GetListByUser(user);
            var eczaneGruplarim = _eczaneGrupService.GetListByUser(user).Select(s => s.Id);
            //var talepler = _talepService.GetListByUser(user);  
            var eczaneGrupIdlerim = _eczaneGrupService.GetListByUser(user).Select(s => s.EczaneId);
            var katilimDetaylar = _katilimService.GetDetaylar();

            var talepler = _talepService.GetDetaylar().ToList();
            var katilimlar = _katilimService.GetMyListByEczaneGruplar(eczaneGruplar).ToList();

            var katilimlarGroupByTalepId = katilimlar.GroupBy(g => g.TalepId) //.Select(s => s.Sum(d => d.Miktar));
                .Select(g => new KatilimGroupByTalepId
                {
                    //EczaneGln = _eczaneService.GetById(katilimlar.Where(w => w.TalepId == g.Key).Select(s => s.EczaneId).FirstOrDefault()).EczaneGln,
                    TalepMiktari= talepler.Where(w => w.Id == g.Key).Select(s => s.TalepMiktari).FirstOrDefault(),
                    TalepId = g.Key,
                    // KatilimTarihi = katilimlar.Where(w => w.TalepId == g.Key).Select(s => s.KatilimTarihi).FirstOrDefault(),
                    ToplamKatilimMiktari = g.Sum(x => x.Miktar),
                    KatilimYapanEczaneAdi = katilimlar.Where(w => w.TalepId == g.Key).Select(s => s.EczaneGrupAdi).FirstOrDefault(),
                    DepoFiyat = talepler.Where(w => w.Id == g.Key).Select(s => s.DepoFiyati).FirstOrDefault(),
                    IlacAdi = talepler.Where(w => w.Id == g.Key).Select(s => s.IlacAdi).FirstOrDefault(),
                    GrupAdi = talepler.Where(w => w.Id == g.Key).Select(s => s.TalepVerenEczaneGrupAdi).FirstOrDefault(),
                    Minimum = talepler.Where(w => w.Id == g.Key).Select(s => s.Minimum).FirstOrDefault(),
                    Maksimum = talepler.Where(w => w.Id == g.Key).Select(s => s.Maximum).FirstOrDefault(),
                    BitisTarihi = talepler.Where(w => w.Id == g.Key).Select(s => s.BitisTarihi).FirstOrDefault(),
                    Kalan = talepler.Where(w => w.Id == g.Key).Select(s => s.TalepMiktari).FirstOrDefault() - g.Sum(x => x.Miktar),
                    Katilimim = katilimlar.Where(w => w.TalepId == g.Key && w.EczaneGrupId == eczaneGruplarim.FirstOrDefault()).Sum(s => s.Miktar),
                    TalepVerenEczaneAdi = talepler.Where(w => w.Id == g.Key).Select(s => s.TalepVerenEczaneAdi).FirstOrDefault(),

                    EczaneGln = katilimlar.Where(w => w.Id == g.Key).Select(s => s.EczaneGln).FirstOrDefault(),
                    IlacId = katilimlar.Where(w => w.Id == g.Key).Select(s => s.IlacId).FirstOrDefault(),
                    KatilimTarihi = katilimlar.Where(w => w.Id == g.Key).Select(s => s.KatilimTarihi).FirstOrDefault(),
                    Miktar = katilimlar.Where(w => w.Id == g.Key).Select(s => s.Miktar).FirstOrDefault(),
                    KayitTarihi = katilimlar.Where(w => w.Id == g.Key).Select(s => s.KayitTarihi).FirstOrDefault(),
                    TalepDurumAdi = talepler.Where(w => w.Id == g.Key).Select(s => s.TalepDurumAdi).FirstOrDefault(),
                    TalepVerenEczaneGrupAdi = talepler.Where(w => w.Id == g.Key).Select(s => s.TalepVerenEczaneGrupAdi).FirstOrDefault(),
                    TalepVerenEczaneGrupId = talepler.Where(w => w.Id == g.Key).Select(s => s.TalepVerenEczaneGrupId).FirstOrDefault(),
                    Id = katilimlar.Where(w => w.Id == g.Key).Select(s => s.Id).FirstOrDefault(),
                    TalepDurumId = talepler.Where(w => w.Id == g.Key).Select(s => s.TalepDurumId).FirstOrDefault(),
                }).ToList();
            #region baskaAlimlarGroupByTeklifIdler
            //var baskaAlimlarGroupByTeklifIdler = alimDetaylar.GroupBy(g => g.TalepId) //.Select(s => s.Sum(d => d.Miktar));
            //   .Select(g => new KatilimGroupByTalepId
            //   {
            //       //EczaneGln = _eczaneService.GetById(katilimlar.Where(w => w.TalepId == g.Key).Select(s => s.EczaneId).FirstOrDefault()).EczaneGln,
            //       TalepMiktari = talepler.Where(w => w.Id == g.Key).Select(s => s.TalepMiktari).FirstOrDefault(),
            //       TalepId = g.Key,
            //       // KatilimTarihi = katilimlar.Where(w => w.TalepId == g.Key).Select(s => s.KatilimTarihi).FirstOrDefault(),
            //       ToplamKatilimMiktari = g.Sum(x => x.Miktar),
            //       KatilimYapanEczaneAdi = katilimlar.Where(w => w.TalepId == g.Key).Select(s => s.EczaneGrupAdi).FirstOrDefault(),
            //       DepoFiyat = talepler.Where(w => w.Id == g.Key).Select(s => s.DepoFiyati).FirstOrDefault(),
            //       IlacAdi = talepler.Where(w => w.Id == g.Key).Select(s => s.IlacAdi).FirstOrDefault(),
            //       GrupAdi = talepler.Where(w => w.Id == g.Key).Select(s => s.TalepVerenEczaneGrupAdi).FirstOrDefault(),
            //       Minimum = talepler.Where(w => w.Id == g.Key).Select(s => s.Minimum).FirstOrDefault(),
            //       Maksimum = talepler.Where(w => w.Id == g.Key).Select(s => s.Maximum).FirstOrDefault(),
            //       BitisTarihi = talepler.Where(w => w.Id == g.Key).Select(s => s.BitisTarihi).FirstOrDefault(),
            //       Kalan = talepler.Where(w => w.Id == g.Key).Select(s => s.TalepMiktari).FirstOrDefault() - g.Sum(x => x.Miktar),
            //       Katilimim = katilimlar.Where(w => w.TalepId == g.Key && w.EczaneGrupId == eczaneGruplarim.FirstOrDefault()).Sum(s => s.Miktar),
            //       TalepVerenEczaneAdi = talepler.Where(w => w.Id == g.Key).Select(s => s.TalepVerenEczaneAdi).FirstOrDefault(),

            //       EczaneGln = katilimlar.Where(w => w.Id == g.Key).Select(s => s.EczaneGln).FirstOrDefault(),
            //       IlacId = katilimlar.Where(w => w.Id == g.Key).Select(s => s.IlacId).FirstOrDefault(),
            //       KatilimTarihi = katilimlar.Where(w => w.Id == g.Key).Select(s => s.KatilimTarihi).FirstOrDefault(),
            //       Miktar = katilimlar.Where(w => w.Id == g.Key).Select(s => s.Miktar).FirstOrDefault(),
            //       KayitTarihi = katilimlar.Where(w => w.Id == g.Key).Select(s => s.KayitTarihi).FirstOrDefault(),
            //       TalepDurumAdi = talepler.Where(w => w.Id == g.Key).Select(s => s.TalepDurumAdi).FirstOrDefault(),
            //       TalepVerenEczaneGrupAdi = talepler.Where(w => w.Id == g.Key).Select(s => s.TalepVerenEczaneGrupAdi).FirstOrDefault(),
            //       TalepVerenEczaneGrupId = talepler.Where(w => w.Id == g.Key).Select(s => s.TalepVerenEczaneGrupId).FirstOrDefault(),
            //       Id = katilimlar.Where(w => w.Id == g.Key).Select(s => s.Id).FirstOrDefault()
            //   })
            //   .OrderByDescending(o => o.KatilimTarihi).ToList();
            #endregion


            var alimlarGroupby = katilimlar.GroupBy(g => g.TalepId).ToList();
            //o eczanenin alım yaptığı talepler:
            var alimYaptigiTeklifler = talepler.Where(w => katilimlar.Select(s => s.TalepId).Contains(w.Id)).ToList();

            var teklifIdler = katilimlar.Select(s => s.TalepId);
            var ilacIdler = talepler.Where(w => teklifIdler.Contains(w.Id)).Select(s => s.IlacId);
            var ilaclar = _ilacService.GetList().Where(w => ilacIdler.Contains(w.Id)).ToList();

            
            if (page == "" || page == null)
            {
                page = "1";
            }
            var pager = new Pager(katilimlarGroupByTalepId.Count(), Convert.ToInt32(page));
            //katilimDetaylar = katilimDetaylar.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList();
            foreach (var item in katilimlarGroupByTalepId)
            {
                var katilimDetaylars = katilimDetaylar.Where(w => w.TalepId == item.TalepId).ToList();

                katilimGroupByTalepIdKatilimDetaylarViewModel.Add(new KatilimGroupByTalepIdKatilimDetaylarViewModel
                {
                    KatilimGroupByTalepId = item,
                    KatilimDetaylar = katilimDetaylars,
                    Pager = pager
                });
            }
            return katilimGroupByTalepIdKatilimDetaylarViewModel;

        }
        private KatilimDetayViewModel getKatilimDetayViewModel(int thispage)
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var gruplar = _grupService.GetListByUser(user).ToList();
            var eczaneGruplar = _eczaneGrupService.GetDetayListByUser(user).ToList();

            var talepDetaylar = _talepService.GetDetaylar().ToList();
            var katilimlar = _katilimService.GetMyListByEczaneGruplar(eczaneGruplar).ToList().OrderByDescending(o => o.KatilimTarihi).ToList();

            var teklifIdler = katilimlar.Select(s => s.TalepId);
            var ilacIdler = talepDetaylar.Where(w => teklifIdler.Contains(w.Id)).Select(s => s.IlacId);
            var ilaclar = _ilacService.GetList().Where(w => ilacIdler.Contains(w.Id)).ToList();

            var pager = new Pager(katilimlar.Count(), thispage);

            var model = new KatilimDetayViewModel()
            {
                KatilimDetaylar = katilimlar.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList(),
                //Eczaneler = eczaneler,
                Ilaclar = ilaclar,
                EczaneGrupDetaylar = eczaneGruplar,
                TalepDetaylar = talepDetaylar,
                Pager = pager

            };
            return model;
        }
        
        private List<KatilimDetayKatilimDetaylarViewModel> getKatilimlar(List<KatilimDetayKatilimDetaylarViewModel> KatilimDetayKatilimDetaylarViewModel,
            string page)
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var userId = _userService.GetByUserName(User.Identity.Name).Id;
            var eczaneler = _eczaneService.GetListByUser(user);
            var eczaneIdlerim = _eczaneUserService.GetListByUserId(userId).Select(s => s.EczaneId).ToList();
            var eczaneGrupIdlerim = _eczaneGrupService.GetListByUser(user).Select(s => s.EczaneId);
            var gruplar = _grupService.GetListByUser(user);
            var eczaneGruplar = _eczaneGrupService.GetListByUser(user);
            var eczanegrupIdler = eczaneGruplar.Select(s => s.Id).ToList();
            var eczaneIdler = eczaneler.Select(s => s.Id).ToList();

            var alimDetaylar = _katilimService.GetDetaylar(w => !eczaneGrupIdlerim.Contains(w.EczaneGrupId));

            var talepler = _talepService.GetDetaylar().ToList();
            var katilimlar = _katilimService.GetMyListByEczaneGruplar(eczaneGruplar).ToList().OrderByDescending(o => o.KatilimTarihi).ToList();
            var eczaneGruplarim = _eczaneGrupService.GetListByUser(user).Select(s => s.Id);
            
            if (page == "" || page == null)
            {
                page = "1";
            }

            var pager = new Pager(katilimlar.Count(), Convert.ToInt32(page));
            var katilimlarGroupByTalepIdlers = new List<KatilimGroupByTalepId>();
            foreach (var item in katilimlar)
            {
                //katilimlarGroupByTalepIdlers = baskaAlimlarGroupByTeklifIdler.Where(w => w.TalepId == item.TalepId).ToList();
                var katilimDetaylars = katilimlar.Where(w => w.TalepId == item.TalepId).ToList();

                KatilimDetayKatilimDetaylarViewModel.Add(new KatilimDetayKatilimDetaylarViewModel
                {
                    // AlimDetay = item,
                    // AlimDetaylar = alimDetaylars,
                    KatilimDetay = item,
                    KatilimDetaylar = katilimDetaylars,
                    Pager = pager
                });
            }
            return KatilimDetayKatilimDetaylarViewModel;
        }
        // GET: AlimNobet/Katilim/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return RedirectToAction("Index", "Katilim");
            int Id = Convert.ToInt32(id);
            if (Id < 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KatilimDetay Katilim = _katilimService.GetDetayById(Id);
            if (Katilim == null)
            {
                return HttpNotFound();
            }
            return View(Katilim);
        }

        // GET: AlimNobet/Katilim/Create
        [Authorize(Roles = "Admin,Grup Yöneticisi, Eczane")]
        public ActionResult Create(int? id)
        {//bu metod teklif sayfasından çağırılır. teklife alım yapmak için
            Katilim Katilim = new Katilim();
            var user = _userService.GetByUserName(User.Identity.Name);

            if (id == null)
                return RedirectToAction("Index", "Talep");

            var talepId = Convert.ToInt32(id);
            var ilacId = _talepService.GetById(Convert.ToInt32(talepId)).IlacId;
            var ilacAdi = _ilacService.GetById(ilacId).Adi;

            ViewBag.Ilac = ilacAdi;
            ViewBag.talepId = talepId;
            Katilim.TalepId = talepId;
            var teklifVerenEczaneGrupId = _talepService.GetDetayById(talepId).TalepVerenEczaneGrupId;
            var grupId = _eczaneGrupService.GetDetayById(teklifVerenEczaneGrupId).GrupId;
            var eczaneGruplar = _eczaneGrupService.GetListByUser(user);

            Katilim.EczaneGrupId = eczaneGruplar.Where(w => w.GrupId == grupId).Select(s => s.Id).FirstOrDefault();
            
            return View(Katilim);
        }

        // POST: AlimNobet/Katilim/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,AlimDurum,ITStransferDurum,Miktar")] Katilim Katilim)
        //{
        //    var user = _userService.GetByUserName(User.Identity.Name);
        //    var eczaneGrupId = _eczaneGrupService.GetListByUser(user).Select(s => s.EczaneId).FirstOrDefault();
        //    Katilim.EczaneGrupId = eczaneGrupId;
        //    Katilim.TalepId = Convert.ToInt32(Session["AlimYapilacakTeklifId"].ToString());
        //    Katilim.KatilimTarihi = DateTime.Now;

        //    if (ModelState.IsValid)
        //    {
        //        _katilimService.Insert(Katilim);
        //        return RedirectToAction("Index");
        //    }
        //    var alimDurumlar = _alimDurumService.GetList();
        //    var iTStransferDurumlar = _iTStransferDurumService.GetList();
        //    ViewBag.AlimDurumId = new SelectList(alimDurumlar, "Id", "Adi");
        //    ViewBag.ITStransferDurumId = new SelectList(iTStransferDurumlar, "Id", "Adi");

        //    return View(Katilim);
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Katilim Katilim)
        {

            Katilim.KatilimTarihi = DateTime.Now;
            Katilim.Miktar = Katilim.Miktar;


            if (ModelState.IsValid)
            {
                try
                {
                    _katilimService.Insert(Katilim);
                    return RedirectToAction("Index");

                }
                catch (Exception ex)
                {
                    TempData["Message"] = "ERROR:" + ex.Message.ToString();
                }
            }
         

            return View(Katilim);
        }
        // GET: AlimNobet/Katilim/Edit/5
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
                return RedirectToAction("Index", "Talep");

            }
            if (id < 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Katilim Katilim = _katilimService.GetById(Id);
            var talepId = Katilim.TalepId;
            ViewBag.TalepId = talepId;

            var user = _userService.GetByUserName(User.Identity.Name);

            var teklifVerenEczaneGrupId = _talepService.GetDetayById(talepId).TalepVerenEczaneGrupId;
            var grupId = _eczaneGrupService.GetDetayById(teklifVerenEczaneGrupId).GrupId;
            var eczaneGruplar = _eczaneGrupService.GetListByUser(user);

            Katilim.EczaneGrupId = Katilim.EczaneGrupId;// eczaneGruplar.Where(w => w.GrupId == grupId).Select(s => s.Id).FirstOrDefault();


            if (Katilim == null)
            {
                return HttpNotFound();
            }
            return View(Katilim);
        }

        // POST: AlimNobet/Katilim/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,KatilimTarihi,TalepId,EczaneGrupId,Miktar")] Katilim Katilim)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _katilimService.Update(Katilim);
                    int? Id = Katilim.EczaneGrupId;
                    return RedirectToAction("Index", new { id = Id });
                }
                catch (Exception ex)
                {
                    TempData["Message"] = "ERROR:" + ex.Message.ToString();
                }
            }
            return View(Katilim);
        }

        // GET: AlimNobet/Katilim/Delete/5
        [Authorize(Roles = "Admin,Grup Yöneticisi,Eczane")]
        public ActionResult Delete(int id)
        {
            if (id < 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Katilim Katilim = _katilimService.GetById(id);
            var teklif = _talepService.GetById(Katilim.TalepId);
            var ilacId = teklif.IlacId;
            ViewBag.IlacAdi = _ilacService.GetById(ilacId).Adi;

            if (Katilim == null)
            {
                return HttpNotFound();
            }
            return View(Katilim);
        }

        // POST: AlimNobet/Katilim/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Katilim Katilim = _katilimService.GetById(id);
            try
            {
                _katilimService.Delete(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "ERROR:" + ex.Message.ToString();
            }
            return View(Katilim);

        }
    }
}