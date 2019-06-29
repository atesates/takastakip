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
using System.Net.Http;

namespace WM.UI.Mvc.Areas.Kullanici.Controllers
{
    [Authorize(Roles = "Admin,Eczane,Grup Yöneticisi")]
    public class AlimimController : Controller
    {
        // GET: Kullanici/Alim
        #region ctor
        private IIlacService _ilacService;
        private IEczaneService _eczaneService;
        private IEczaneUserService _eczaneUserService;
        private IAlimService _alimService;
        private IEczaneGrupService _eczaneGrupService;
        private IGrupService _grupService;
        private IUserService _userService;
        private IAlimDurumService _alimDurumService;
        private ITeklifService _teklifService;
        private ITeklifDurumService _teklifDurumService;
        private IITStransferDurumService _iTStransferDurumService;

        public AlimimController(IEczaneGrupService eczaneGrupService,
                                IEczaneUserService eczaneUserService,
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
            _eczaneUserService = eczaneUserService;
            _teklifService = teklifService;
            _teklifDurumService = teklifDurumService;
            _iTStransferDurumService = iTStransferDurumService;
            _alimDurumService = alimDurumService;
            _eczaneGrupService = eczaneGrupService;
        }
        #endregion
        public ActionResult Index(int? page)
        {//id eczaneGrupId EczaneGrupcontroller dan geliyor
         //if(Id == null)
         //    return RedirectToAction("Index", "EczaneGrup");

            //int id = Convert.ToInt32(Id);
            int thispage = 0;
            if (page != 0)
                thispage = Convert.ToInt32(page);

            var alimDurumlar = _alimDurumService.GetList();
            ViewBag.alimTurId = new SelectList(alimDurumlar, "Id", "Adi");

            var ITStransferDurumlar = _iTStransferDurumService.GetList();          
            ViewBag.ITStransferDurumId = new SelectList(ITStransferDurumlar, "Id", "Adi");
            var AlimDurumlar = _alimDurumService.GetList().Where(w => w.AliciTarafiMi != false);
            ViewBag.AlimDurumId = new SelectList(AlimDurumlar, "Id", "Adi");

            // var temp = TempData["EczaneGrupId"];        
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetAlimMiktari(string AlimIDForMiktar, string alimmiktari, string ExpandedForAll, string pageForAlimMiktari, string teklifDurumIdForALD, string alimDurumIdForALD)
        {
            int id = Convert.ToInt32(AlimIDForMiktar);
            // int id = AlimId;

            Alim alim = new Alim();
            alim = _alimService.GetById(id);
            alim.Miktar = Convert.ToInt32(alimmiktari);
            try
            {
                _alimService.Update(alim);
                TempData["MessageSuccess"] = "Alim miktarı başarıyla değiştirlmiştir.";
                //return PartialView("TeklifimPartialView", teklifDetayAlimDetaylarViewModel);
            }
            catch (Exception ex)
            {
                TempData["MessageDanger"] = "ERROR: Alim miktarı değiştirilemedi " + ex.InnerException.InnerException.Message.ToString();

            }
            List<AlimDetayAlimDetaylarViewModel> alimDetayAlimDetaylarViewModel = new List<AlimDetayAlimDetaylarViewModel>();
            alimDetayAlimDetaylarViewModel = getAlimlarim(alimDetayAlimDetaylarViewModel, pageForAlimMiktari, teklifDurumIdForALD, alimDurumIdForALD);
            var liste3 = ExpandedForAll.Split(',');
            if (liste3[0].Length > 0)
            {
                foreach (string item in liste3)
                {
                    foreach (var alimDetays in alimDetayAlimDetaylarViewModel)
                    {
                        if (alimDetays.AlimDetay.Id == Convert.ToInt32(item))
                        {
                            alimDetays.AlimDetay.Expanded = true;
                        }
                    }
                }
            }
            return PartialView("AlimimPartialView", alimDetayAlimDetaylarViewModel);
        }
        public ActionResult SearchIndex(string Keywords)
        {
        
            var alimDurumlar = _alimDurumService.GetList();
            ViewBag.alimTurId = new SelectList(alimDurumlar, "Id", "Adi");
            // var temp = TempData["EczaneGrupId"];        

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
            var alimlar = _alimService.GetMyListByEczaneGruplar(eczaneGruplar).ToList()
                .Where(p => Regex.Split(Keywords, @"\s")
                .Any(x => p.IlacAdi.ToLower().Contains(x.ToLower()) || p.IlacAdi.ToLower().Contains(x.ToLower()))).ToList();

            ;
            //o eczanenin alım yaptığı teklifler:
            var alimYaptigiTeklifler = teklifler.Where(w => alimlar.Select(s => s.TeklifId).Contains(w.Id)).ToList();

            var teklifIdler = alimlar.Select(s => s.TeklifId);
            var ilacIdler = teklifler.Where(w => teklifIdler.Contains(w.Id)).Select(s => s.IlacId);
            var ilaclar = _ilacService.GetList().Where(w => ilacIdler.Contains(w.Id)).ToList();

            var model = new AlimDetayViewModel()
            {
                AlimDetaylar = alimlar,
                //Eczaneler = eczaneler,
                Ilaclar = ilaclar,
                EczaneGrupDetaylar = eczaneGruplar,
                TeklifDetaylar = alimYaptigiTeklifler,
            };
            return View("Index", model);//result:model



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
            ViewBag.Maksimum = _teklifService.GetDetayById(teklifId).Maksimum;
            ViewBag.Minimum = _teklifService.GetDetayById(teklifId).Minimum;
            //maksimum ve minimum u sağlaması yetmez, kalan miktardan da az olmalı
            var alimMiktari = _teklifService.GetById(teklifId).AlimMiktari;
            var buTekliftenAlinanMiktar = _alimService.GetListByTeklifId(teklifId)
                .Sum(x => x.Miktar);
            var kalan = alimMiktari - buTekliftenAlinanMiktar;
            if (kalan < ViewBag.Maksimum)
                ViewBag.Maksimum = kalan;
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
                    TempData["MessageSuccess"] = "Alim başarıyla gerçekleştirilmiştir";
                    return RedirectToAction("Index");

                }
                catch (Exception ex)
                {
                    TempData["Message"] = "ERROR:" + ex.InnerException.InnerException.Message.ToString();
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
           
            ViewBag.Maksimum = _teklifService.GetDetayById(teklifId).Maksimum;
            ViewBag.Minimum = _teklifService.GetDetayById(teklifId).Minimum;
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
                    TempData["MessageSuccess"] = "Alim başarıyla düzenlenmiştir";
                    int? Id = Alim.EczaneGrupId;
                    return RedirectToAction("Index", new { id = Id });
                }
                catch (Exception ex)
                {
                    TempData["MessageDanger"] = "ERROR:" + ex.InnerException.InnerException.Message.ToString();
                }
            }

            var ITStransferDurumlar = _iTStransferDurumService.GetList();
            ViewBag.ITStransferDurumId = new SelectList(ITStransferDurumlar, "Id", "Adi", Alim.ITStransferDurumId);
            var teklifId = Alim.TeklifId;
            ViewBag.TeklifId = teklifId;
            var alimDurumlar = _alimDurumService.GetList().Where(w => w.Id == 4 || w.Id == 1 || w.Id == 5);
            ViewBag.AlimDurumId = new SelectList(alimDurumlar, "Id", "Adi", Alim.AlimDurumId);
            ViewBag.Maksimum = _teklifService.GetDetayById(teklifId).Maksimum;
            ViewBag.Minimum = _teklifService.GetDetayById(teklifId).Minimum;
            return View(Alim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetITS(string AlimIDForITS, string ITSTransferDurumId, string ExpandedForITS, string pageForITS, string teklifDurumIdForITS, string alimDurumIdForITS)
        {
            int id = Convert.ToInt32(AlimIDForITS);
            // int id = AlimId;

            Alim alim = new Alim();
            alim = _alimService.GetById(id);
            alim.ITStransferDurumId = Convert.ToInt32(ITSTransferDurumId);
            try
            {
                _alimService.Update(alim);
                TempData["MessageSuccess"] = "ITS transfer durum başarıyla değiştirlmiştir.";
                //return PartialView("TeklifimPartialView", teklifDetayAlimDetaylarViewModel);
            }
            catch (Exception ex)
            {
                TempData["MessageDanger"] = "ERROR: ITS transfer durum değiştirilemedi " + ex.InnerException.InnerException.Message.ToString();

            }
            List<AlimDetayAlimDetaylarViewModel> alimDetayAlimDetaylarViewModel = new List<AlimDetayAlimDetaylarViewModel>();
            alimDetayAlimDetaylarViewModel = getAlimlarim(alimDetayAlimDetaylarViewModel, pageForITS,  teklifDurumIdForITS,  alimDurumIdForITS);
            var liste3 = ExpandedForITS.Split(',');
            if (liste3[0].Length > 0)
            {
                foreach (string item in liste3)
                {
                    foreach (var alimDetays in alimDetayAlimDetaylarViewModel)
                    {
                        if (alimDetays.AlimDetay.Id == Convert.ToInt32(item))
                        {
                            alimDetays.AlimDetay.Expanded = true;
                        }
                    }
                }
            }
            return PartialView("AlimimPartialView", alimDetayAlimDetaylarViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetALD(string AlimIDForALD, string AlimDurumId, string ExpandedForALD, string pageForALD, string teklifDurumIdForALD, string alimDurumIdForALD)
        {
            int id = Convert.ToInt32(AlimIDForALD);
            // int id = AlimId;


            Alim alim = new Alim();
            alim = _alimService.GetById(id);
            alim.AlimDurumId = Convert.ToInt32(AlimDurumId);
            if (Convert.ToInt32(AlimDurumId) == 4)
                alim.TeslimAlimTarihi = DateTime.Now;
            else 
                alim.TeslimAlimTarihi = null;
            try
            {
                _alimService.Update(alim);
                TempData["MessageSuccess"] = "Alım durum başarıyla Değiştirildi";
                // return PartialView("TeklifimPartialView", teklifDetayAlimDetaylarViewModel);
            }
            catch (Exception ex)
            {
                TempData["MessageDanger"] = "ERROR: Alım Durum değiştirilemedi. " + ex.InnerException.InnerException.Message.ToString();

            }
            List<AlimDetayAlimDetaylarViewModel> alimDetayAlimDetaylarViewModel = new List<AlimDetayAlimDetaylarViewModel>();
            alimDetayAlimDetaylarViewModel = getAlimlarim(alimDetayAlimDetaylarViewModel, pageForALD, teklifDurumIdForALD, alimDurumIdForALD);

            var liste3 = ExpandedForALD.Split(',');
            if (liste3[0].Length > 0)
            {
                foreach (string item in liste3)
                {
                    foreach (var alimDetays in alimDetayAlimDetaylarViewModel)
                    {
                        if (alimDetays.AlimDetay.Id == Convert.ToInt32(item))
                        {
                            alimDetays.AlimDetay.Expanded = true;
                        }
                    }
                }
            }
            return PartialView("AlimimPartialView", alimDetayAlimDetaylarViewModel);
        }
        // GET: AlimNobet/Alim/Delete/5
        [HttpGet]
        [Authorize(Roles = "Admin,Grup Yöneticisi,Eczane")]
        public ActionResult Delete(int id)
        {
           // int id = Convert.ToInt32(Id);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Grup Yöneticisi,Eczane")]
        public ActionResult GetAlim(int? Id)
        {
            int id = Convert.ToInt32(Id);
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
            var jsonResult = Json(Alim, JsonRequestBehavior.AllowGet);
            return jsonResult;

        }

        private List<AlimDetayAlimDetaylarViewModel> getAlimlarim(List<AlimDetayAlimDetaylarViewModel> AlimDetayAlimDetaylarViewModel, 
            string page, string teklifDurumId, string alimDurumId)
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var userId = _userService.GetByUserName(User.Identity.Name).Id;
            var eczaneler = _eczaneService.GetListByUser(user);
            var eczaneIdlerim = _eczaneUserService.GetListByUserId(userId).Select(s=>s.EczaneId).ToList();
            var eczaneGrupIdlerim = _eczaneGrupService.GetListByUser(user).Select(s=>s.EczaneId);
            var gruplar = _grupService.GetListByUser(user);
            var eczaneGruplar = _eczaneGrupService.GetListByUser(user);
            var eczanegrupIdler = eczaneGruplar.Select(s => s.Id).ToList();
            var eczaneIdler = eczaneler.Select(s => s.Id).ToList();


            var digerAlimDetaylar = _alimService.GetDetaylar(w=> !eczaneGrupIdlerim.Contains(w.EczaneGrupId));

            var alimDetaylar = new List<AlimDetay>();
            if (alimDurumId != null && alimDurumId != "")
            {
                if (teklifDurumId != null && teklifDurumId != "")
                {
                    alimDetaylar = _alimService.GetDetaylar(w => eczaneGrupIdlerim.Contains(w.EczaneGrupId))
                   .Where(w => w.AlimDurumId == Convert.ToInt32(alimDurumId)
                   && w.TeklifDurumId == Convert.ToInt32(teklifDurumId))
                   .OrderByDescending(o => o.AlimTarihi).ToList();
                }
                else
                {
                    alimDetaylar = _alimService.GetDetaylar(w => eczaneGrupIdlerim.Contains(w.EczaneGrupId))
                   .Where(w => w.AlimDurumId == Convert.ToInt32(alimDurumId))
                   .OrderByDescending(o => o.AlimTarihi).ToList();
                }
                            
            }
            else
            {
                if (teklifDurumId != null && teklifDurumId != "")
                {
                    alimDetaylar = _alimService.GetDetaylar(w => eczaneGrupIdlerim.Contains(w.EczaneGrupId))
                        .Where(w=>w.TeklifDurumId == Convert.ToInt32(teklifDurumId))
                        .OrderByDescending(o => o.AlimTarihi).ToList();
                }
                else
                {
                    alimDetaylar = _alimService.GetDetaylar(w => eczaneGrupIdlerim.Contains(w.EczaneGrupId))
                                                    .OrderByDescending(o => o.AlimTarihi).ToList();
                }
            }
      
            if (page == "" || page == null)
            {
                page = "1";
            }
            var AlimDurumlar = _alimDurumService.GetList();//.Where(w => w.AliciTarafiMi != true);
            ViewBag.AlimDurumIdFilter = new SelectList(AlimDurumlar, "Id", "Adi", alimDurumId);
            var TeklifDurumlar = _teklifDurumService.GetList();
            ViewBag.TeklifDurumIdFilter = new SelectList(TeklifDurumlar, "Id", "Adi", teklifDurumId);

            var pager = new Pager(alimDetaylar.Count(), Convert.ToInt32(page));
            //alimDetaylar = alimDetaylar.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList();
            foreach (var item in alimDetaylar)
            {
                var alimDetaylars = digerAlimDetaylar.Where(w => w.TeklifId == item.TeklifId).ToList();

                AlimDetayAlimDetaylarViewModel.Add(new AlimDetayAlimDetaylarViewModel
                {
                    AlimDetay = item,
                    AlimDetaylar = alimDetaylars,
                    Pager = pager
                });
            }
            return AlimDetayAlimDetaylarViewModel;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetAlimMiktar(string AlimIDForMiktar, string alimmiktari, string ExpandedForMiktar, string pageForAlimMiktar, string teklifDurumIdForALD, string alimDurumIdForALD)
        {
            int id = Convert.ToInt32(AlimIDForMiktar);
            // int id = AlimId;

            Alim alim = new Alim();
            alim = _alimService.GetById(id);
            alim.Miktar = Convert.ToInt32(alimmiktari);
            try
            {
                _alimService.Update(alim);
                TempData["MessageSuccess"] = "Alım miktarı başarıyla kaydedilmiştir.";
                //return PartialView("TeklifimPartialView", teklifDetayAlimDetaylarViewModel);
            }
            catch (Exception ex)
            {
                TempData["MessageDanger"] = "ERROR: Alım miktarı kaydedilemedi " + ex.InnerException.InnerException.Message.ToString();

            }
            List<AlimDetayAlimDetaylarViewModel> alimDetayAlimDetaylarViewModel = new List<AlimDetayAlimDetaylarViewModel>();
            alimDetayAlimDetaylarViewModel = getAlimlarim(alimDetayAlimDetaylarViewModel, pageForAlimMiktar, teklifDurumIdForALD, alimDurumIdForALD);
            var liste3 = ExpandedForMiktar.Split(',');
            if (liste3[0].Length > 0)
            {
                foreach (string item in liste3)
                {
                    foreach (var alimDetays in alimDetayAlimDetaylarViewModel)
                    {
                        if (alimDetays.AlimDetay.Id == Convert.ToInt32(item))
                        {
                            alimDetays.AlimDetay.Expanded = true;
                        }
                    }
                }
            }
            return PartialView("AlimimPartialView", alimDetayAlimDetaylarViewModel);
        }
     
        [HttpGet]
        //[ValidateAntiForgeryToken] get de olmuyor
        public ActionResult GetAlimlarim(string page, string teklifDurumId, string alimDurumId)
        {
            List<AlimDetayAlimDetaylarViewModel> AlimDetayAlimDetaylarViewModel = new List<AlimDetayAlimDetaylarViewModel>();
            AlimDetayAlimDetaylarViewModel = getAlimlarim(AlimDetayAlimDetaylarViewModel, page,  teklifDurumId,  alimDurumId);
            return PartialView("AlimimPartialView", AlimDetayAlimDetaylarViewModel);
        }

        // POST: AlimNobet/Alim/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? Id)
        {
            int id = Convert.ToInt32(Id);
            // int id = AlimId;
            Alim Alim = _alimService.GetById(id);
            try
            {
                TempData["MessageSuccess"] = "Teklif başarıyla silinmiştir";
                _alimService.Delete(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "ERROR:" + ex.InnerException.InnerException.Message.ToString();
            }
            return View(Alim);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecilenleriBekleniyorYap(string bekleniyorYapilacakAlimlar, string ExpandedForBekleniyor, string pageForBekleniyor, string alimDurumIdForBekleniyor, string teklifDurumIdForBekleniyor)
        {
            List<AlimDetayAlimDetaylarViewModel> AlimDetayAlimDetaylarViewModel = new List<AlimDetayAlimDetaylarViewModel>();
            List<int> alimIdler = new List<int>();
            var uyariMesaji = "Seçim Yapmadınız!";

            if (bekleniyorYapilacakAlimlar == null || bekleniyorYapilacakAlimlar=="")
            {
                return Json(uyariMesaji, JsonRequestBehavior.AllowGet);
            }

            Int32 basamak = bekleniyorYapilacakAlimlar.IndexOf(';');
            Int32 toplam = bekleniyorYapilacakAlimlar.Length;

            var alimlar = bekleniyorYapilacakAlimlar.Substring(0, basamak);

            var liste = alimlar.Split(',');

            //alimlar update 
            if (liste[0].Length > 0)
            {
                foreach (string item in liste)
                {
                    var alim = new Alim();
                    alim = _alimService.GetById(Convert.ToInt32(item));
                    alim.AlimDurumId = 1;//bekleniyor
                    _alimService.Update(alim);
                    alimIdler.Add(Convert.ToInt32(item));

                    ////////henüz AlimDetayAlimDetaylarViewModel içinde değer yok//////

                    //foreach (var alimDetays in AlimDetayAlimDetaylarViewModel)
                    //{
                    //    if (alimDetays.AlimDetay.Id == Convert.ToInt32(item))
                    //    {
                    //        alimDetays.AlimDetay.Checked = true;
                    //        alimDetays.AlimDetay.AlimDurumId = 1;
                    //        alimDetays.AlimDetay.AlimDurumAdi = _alimDurumService.GetById(1).Adi;
                    //    }
                    //}
                }
            }
            AlimDetayAlimDetaylarViewModel = getAlimlarim(AlimDetayAlimDetaylarViewModel, pageForBekleniyor, teklifDurumIdForBekleniyor, alimDurumIdForBekleniyor);

            TempData["MessageSuccess"] = "Seçilen alımlar beklemeye alınmıştır.";


            var liste3 = ExpandedForBekleniyor.Split(',');
            if (liste3[0].Length > 0)
            {
                foreach (string item in liste3)
                {
                    foreach (var alimDetays in AlimDetayAlimDetaylarViewModel)
                    {
                        if (alimDetays.AlimDetay.Id == Convert.ToInt32(item))
                        {
                            alimDetays.AlimDetay.Expanded = true;
                        }
                    }
                }
            }

            return PartialView("AlimimPartialView", AlimDetayAlimDetaylarViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecileniSil(int Id, string pageForSil,string teklifDurumIdForSil, string alimDurumIdForSil)
        {
            int id = Convert.ToInt32(Id);
            //AlimDetayViewModel model = new AlimDetayViewModel();
            //model = getAliDetayViewModel(1);
            List<AlimDetayAlimDetaylarViewModel> AlimDetayAlimDetaylarViewModel = new List<AlimDetayAlimDetaylarViewModel>();
            AlimDetayAlimDetaylarViewModel = getAlimlarim(AlimDetayAlimDetaylarViewModel, pageForSil, teklifDurumIdForSil, alimDurumIdForSil);

            try
            {
                _alimService.Delete(id);
                TempData["MessageSuccess"] = "Alım başarıyla silinmiştir";
                AlimDetayAlimDetaylarViewModel = getAlimlarim(AlimDetayAlimDetaylarViewModel, pageForSil, teklifDurumIdForSil, alimDurumIdForSil);

                return PartialView("AlimimPartialView", AlimDetayAlimDetaylarViewModel);

                //return View("Index", model);
            }
            catch (Exception ex)
            {
                TempData["MessageDanger"] = "ERROR:" + ex.InnerException.InnerException.Message.ToString();
            }

            //return View("Index", model);
            return PartialView("AlimimPartialView", AlimDetayAlimDetaylarViewModel);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecilenleriStokaAl(string stogaAlinacakAlimlar, string ExpandedForStok, string pageForStok, string teklifDurumIdForStok, string alimDurumIdForStok)
        {
            List<AlimDetayAlimDetaylarViewModel> AlimDetayAlimDetaylarViewModel = new List<AlimDetayAlimDetaylarViewModel>();
            List<int> alimIdler = new List<int>();
            var uyariMesaji = "Seçim Yapmadınız!";

            if (stogaAlinacakAlimlar == null || stogaAlinacakAlimlar == "")
            {
                return Json(uyariMesaji, JsonRequestBehavior.AllowGet);
            }

            Int32 basamak = stogaAlinacakAlimlar.IndexOf(';');
            Int32 toplam = stogaAlinacakAlimlar.Length;

            var alimlar = stogaAlinacakAlimlar.Substring(0, basamak);

            var liste = alimlar.Split(',');

            //alimlar update 
            if (liste[0].Length > 0)
            {
                foreach (string item in liste)
                {
                    var alim = new Alim();
                    alim = _alimService.GetById(Convert.ToInt32(item));
                    alim.AlimDurumId = 4;//stok
                    _alimService.Update(alim);
                    alimIdler.Add(Convert.ToInt32(item));

                   

                    //foreach (var alimDetays in AlimDetayAlimDetaylarViewModel)
                    //{
                    //    if (alimDetays.AlimDetay.Id == Convert.ToInt32(item))
                    //    {
                    //        alimDetays.AlimDetay.Checked = true;
                    //        alimDetays.AlimDetay.AlimDurumId = 4;
                    //        alimDetays.AlimDetay.AlimDurumAdi = _alimDurumService.GetById(4).Adi;

                    //    }
                    //}
                }
            }
            AlimDetayAlimDetaylarViewModel = getAlimlarim(AlimDetayAlimDetaylarViewModel, pageForStok, teklifDurumIdForStok, alimDurumIdForStok);

            TempData["MessageSuccess"] = "Seçilen alımlar stoğa alınmıştır.";

            var liste3 = ExpandedForStok.Split(',');
            if (liste3[0].Length > 0)
            {
                foreach (string item in liste3)
                {
                    foreach (var alimDetays in AlimDetayAlimDetaylarViewModel)
                    {
                        if (alimDetays.AlimDetay.Id == Convert.ToInt32(item))
                        {
                            alimDetays.AlimDetay.Expanded = true;
                        }
                    }
                }
            }

            return PartialView("AlimimPartialView", AlimDetayAlimDetaylarViewModel);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecilenleriSil(string silinecekAlimlar, string ExpandedForSil, string pageForCokluSil, string teklifDurumIdForCokluSil, string alimDurumIdForCokluSil)
        {
            List<AlimDetayAlimDetaylarViewModel> AlimDetayAlimDetaylarViewModel = new List<AlimDetayAlimDetaylarViewModel>();

            List<int> alimIdler = new List<int>();
            var uyariMesaji = "Seçim Yapmadınız!";

            if (silinecekAlimlar == null || silinecekAlimlar=="")
            {
                return Json(uyariMesaji, JsonRequestBehavior.AllowGet);
            }

            Int32 basamak = silinecekAlimlar.IndexOf(';');
            Int32 toplam = silinecekAlimlar.Length;

            var alimlar = silinecekAlimlar.Substring(0, basamak);

            var liste = alimlar.Split(',');

            //alimlar update 
            if (liste[0].Length > 0)
            {
                foreach (string item in liste)
                {
                    var alim = new Alim();
                    alim = _alimService.GetById(Convert.ToInt32(item));
                    try
                    {
                        _alimService.Delete(alim.Id);
                    }
                    catch (Exception ex)
                    {

                        string hataMesaji = ex.InnerException.InnerException.Message;
                        TempData["MessageDanger"] = "Seçilen alimlar silinemedi." + hataMesaji;

                        return Json(new HttpResponseMessage(HttpStatusCode.BadRequest), JsonRequestBehavior.AllowGet);

                    }
                    alimIdler.Add(Convert.ToInt32(item));
                }
            }
            AlimDetayAlimDetaylarViewModel = getAlimlarim(AlimDetayAlimDetaylarViewModel, pageForCokluSil, teklifDurumIdForCokluSil, alimDurumIdForCokluSil);

            TempData["MessageSuccess"] = "Seçilen alimlar silinmiştir.";

            return PartialView("AlimimPartialView", AlimDetayAlimDetaylarViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecilenleriYanlisGonderimYap(string yanlisGonderimYapilacakAlimlar, string ExpandedForYanlis, string pageForYanlis, string teklifDurumIdForYanlis, string alimDurumIdForYanlis)
        {
            List<AlimDetayAlimDetaylarViewModel> AlimDetayAlimDetaylarViewModel = new List<AlimDetayAlimDetaylarViewModel>();
            List<int> alimIdler = new List<int>();
            var uyariMesaji = "Seçim Yapmadınız!";

            if (yanlisGonderimYapilacakAlimlar == null || yanlisGonderimYapilacakAlimlar=="")
            {
                return Json(uyariMesaji, JsonRequestBehavior.AllowGet);
            }

            Int32 basamak = yanlisGonderimYapilacakAlimlar.IndexOf(';');
            Int32 toplam = yanlisGonderimYapilacakAlimlar.Length;

            var alimlar = yanlisGonderimYapilacakAlimlar.Substring(0, basamak);

            var liste = alimlar.Split(',');

            //alimlar update 
            if (liste[0].Length > 0)
            {
                foreach (string item in liste)
                {
                    var alim = new Alim();
                    alim = _alimService.GetById(Convert.ToInt32(item));
                    alim.AlimDurumId = 5;//yanlış ilaç gönderimi
                    _alimService.Update(alim);
                    alimIdler.Add(Convert.ToInt32(item));



                    //foreach (var alimDetays in AlimDetayAlimDetaylarViewModel)
                    //{
                    //    if (alimDetays.AlimDetay.Id == Convert.ToInt32(item))
                    //    {
                    //        alimDetays.AlimDetay.Checked = true;
                    //        alimDetays.AlimDetay.AlimDurumId = 5;
                    //        alimDetays.AlimDetay.AlimDurumAdi = _alimDurumService.GetById(5).Adi;

                    //    }
                    //}
                }
            }
            AlimDetayAlimDetaylarViewModel = getAlimlarim(AlimDetayAlimDetaylarViewModel, pageForYanlis, teklifDurumIdForYanlis, alimDurumIdForYanlis);

            TempData["MessageSuccess"] = "Seçilen alımların durumları yanlış gönderim yapılmıştır.";

            var liste3 = ExpandedForYanlis.Split(',');
            if (liste3[0].Length > 0)
            {
                foreach (string item in liste3)
                {
                    foreach (var alimDetays in AlimDetayAlimDetaylarViewModel)
                    {
                        if (alimDetays.AlimDetay.Id == Convert.ToInt32(item))
                        {
                            alimDetays.AlimDetay.Expanded = true;
                        }
                    }
                }
            }

            return PartialView("AlimimPartialView", AlimDetayAlimDetaylarViewModel);

        }
    }
}