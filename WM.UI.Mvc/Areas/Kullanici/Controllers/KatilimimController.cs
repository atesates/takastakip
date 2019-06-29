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
    public class KatilimimController : Controller
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

        public KatilimimController(IEczaneGrupService eczaneGrupService,
                                IEczaneUserService eczaneUserService,
                                IIlacService ilacService,
                                IEczaneService eczaneService,
                                IGrupService grupService,
                                IKatilimService katilimService,
                                ITalepService talepService,
                                IUserService userService)
        {
            _ilacService = ilacService;
            _eczaneService = eczaneService;
            _grupService = grupService;
            _katilimService = katilimService;
            _userService = userService;
            _eczaneUserService = eczaneUserService;
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
            if (page != 0)
                thispage = Convert.ToInt32(page);


            // var temp = TempData["EczaneGrupId"];        
            var model = new KatilimDetayViewModel();
            model = getKatilimDetayViewModel(thispage);
            return View(model);

        }
        private KatilimDetayViewModel getKatilimDetayViewModel(int thispage)
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var gruplar = _grupService.GetListByUser(user).ToList();
            var eczaneGruplar = _eczaneGrupService.GetDetayListByUser(user).ToList();
            var eczaneIdler = _eczaneGrupService.GetDetayListByUser(user).Select(s => s.EczaneId).ToList();
            var eczaneler = _eczaneService.GetList().Where(w => eczaneIdler.Contains(w.Id)).ToList();
            //kendi yaptığım alımlar:
            eczaneGruplar = _eczaneGrupService.GetMyDetayListByUser(user);
            //var eczaneler = _eczaneService.GetListByUser(user);

            //var teklifler = _talepService.GetListByUser(user);       

            var teklifler = _talepService.GetDetaylar().ToList();
            var alimlar = _katilimService.GetMyListByEczaneGruplar(eczaneGruplar).ToList().OrderByDescending(o => o.KatilimTarihi).ToList();
            //o eczanenin alım yaptığı teklifler:
            var alimYaptigiTeklifler = teklifler.Where(w => alimlar.Select(s => s.TalepId).Contains(w.Id)).ToList();

            var teklifIdler = alimlar.Select(s => s.TalepId);
            var ilacIdler = teklifler.Where(w => teklifIdler.Contains(w.Id)).Select(s => s.IlacId);
            var ilaclar = _ilacService.GetList().Where(w => ilacIdler.Contains(w.Id)).ToList();

            var pager = new Pager(alimlar.Count(), thispage);

            var model = new KatilimDetayViewModel()
            {
                KatilimDetaylar = alimlar,//.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList(),
                //Eczaneler = eczaneler,
                Ilaclar = ilaclar,
                EczaneGrupDetaylar = eczaneGruplar,
                TalepDetaylar = alimYaptigiTeklifler,
                Pager = pager

            };
            return model;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetAlimMiktari(string KatilimIDForMiktar, string katilimmiktari, string ExpandedForAll, string pageForAlimMiktari)
        {
            int id = Convert.ToInt32(KatilimIDForMiktar);
            // int id = AlimId;

            Katilim katilim = new Katilim();
            katilim = _katilimService.GetById(id);
            katilim.Miktar = Convert.ToInt32(katilimmiktari);
            try
            {
                _katilimService.Update(katilim);
                TempData["MessageSuccess"] = "Katilim miktarı başarıyla değiştirlmiştir.";
                //return PartialView("TeklifimPartialView", teklifDetayAlimDetaylarViewModel);
            }
            catch (Exception ex)
            {
                TempData["MessageDanger"] = "ERROR: Katilim miktarı değiştirilemedi " + ex.InnerException.InnerException.Message.ToString();

            }
            List<KatilimDetayKatilimDetaylarViewModel> katilimDetayKatilimDetaylarViewModel = new List<KatilimDetayKatilimDetaylarViewModel>();
            katilimDetayKatilimDetaylarViewModel = getKatilimlarim(katilimDetayKatilimDetaylarViewModel, pageForAlimMiktari);
            var liste3 = ExpandedForAll.Split(',');
            if (liste3[0].Length > 0)
            {
                foreach (string item in liste3)
                {
                    foreach (var katilimDetays in katilimDetayKatilimDetaylarViewModel)
                    {
                        if (katilimDetays.KatilimDetay.Id == Convert.ToInt32(item))
                        {
                            katilimDetays.KatilimDetay.Expanded = true;
                        }
                    }
                }
            }
            return PartialView("KatilimimPartialView", katilimDetayKatilimDetaylarViewModel);
        }
        public ActionResult SearchIndex(string Keywords)
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var gruplar = _grupService.GetListByUser(user).ToList();
            var eczaneGruplar = _eczaneGrupService.GetDetayListByUser(user).ToList();
            var eczaneIdler = _eczaneGrupService.GetDetayListByUser(user).Select(s => s.EczaneId).ToList();
            var eczaneler = _eczaneService.GetList().Where(w => eczaneIdler.Contains(w.Id)).ToList();
            //kendi yaptığım alımlar:
            eczaneGruplar = _eczaneGrupService.GetMyDetayListByUser(user);
            //var eczaneler = _eczaneService.GetListByUser(user);

            //var teklifler = _talepService.GetListByUser(user);       

            var teklifler = _talepService.GetDetaylar().ToList();
            var alimlar = _katilimService.GetMyListByEczaneGruplar(eczaneGruplar).ToList()
                .Where(p => Regex.Split(Keywords, @"\s")
                .Any(x => p.IlacAdi.ToLower().Contains(x.ToLower()) || p.IlacAdi.ToLower().Contains(x.ToLower()))).ToList();

            ;
            //o eczanenin alım yaptığı teklifler:
            var alimYaptigiTeklifler = teklifler.Where(w => alimlar.Select(s => s.TalepId).Contains(w.Id)).ToList();

            var teklifIdler = alimlar.Select(s => s.TalepId);
            var ilacIdler = teklifler.Where(w => teklifIdler.Contains(w.Id)).Select(s => s.IlacId);
            var ilaclar = _ilacService.GetList().Where(w => ilacIdler.Contains(w.Id)).ToList();

            var model = new KatilimDetayViewModel()
            {
                KatilimDetaylar = alimlar,
                //Eczaneler = eczaneler,
                Ilaclar = ilaclar,
                EczaneGrupDetaylar = eczaneGruplar,
                TalepDetaylar = alimYaptigiTeklifler,
            };
            return View("Index", model);//result:model



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
                return RedirectToAction("Index", "Teklif");

            var talepId = Convert.ToInt32(id);
            var ilacId = _talepService.GetById(Convert.ToInt32(talepId)).IlacId;
            var ilacAdi = _ilacService.GetById(ilacId).Adi;

            ViewBag.Ilac = ilacAdi;
            ViewBag.Maximum = _talepService.GetDetayById(talepId).Maximum;
            ViewBag.Minimum = _talepService.GetDetayById(talepId).Minimum;
            //maksimum ve minimum u sağlaması yetmez, kalan miktardan da az olmalı
            var talepMiktari = _talepService.GetById(talepId).TalepMiktari;
            var buTekliftenAlinanMiktar = _katilimService.GetListByTeklifId(talepId)
                .Sum(x => x.Miktar);
            var kalan = talepMiktari - buTekliftenAlinanMiktar;
            if (kalan < ViewBag.Maximum)
                ViewBag.Maximum = kalan;
            ViewBag.talepId = talepId;
            Katilim.TalepId = talepId;
            var teklifVerenEczaneGrupId = _talepService.GetDetayById(talepId).TalepVerenEczaneGrupId;
            var grupId = _eczaneGrupService.GetDetayById(teklifVerenEczaneGrupId).GrupId;
            var eczaneGruplar = _eczaneGrupService.GetListByUser(user);

            Katilim.EczaneGrupId = eczaneGruplar.Where(w => w.GrupId == grupId).Select(s => s.Id).FirstOrDefault();

            //try
            //{
            //    Katilim.EczaneGrupId = Convert.ToInt32(Session["EczanegrupId"]);
            //    ViewBag.eczaneGrupId = Convert.ToInt32(Session["EczanegrupId"]);
            //}
            //catch
            //{
            //    return RedirectToAction("Index", "EczaneGrup");
            //}

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
        //    ViewBag.KatilimDurumId = new SelectList(alimDurumlar, "Id", "Adi");
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
                    TempData["MessageSuccess"] = "Katilim başarıyla gerçekleştirilmiştir";
                    return RedirectToAction("Index");

                }
                catch (Exception ex)
                {
                    TempData["Message"] = "ERROR:" + ex.InnerException.InnerException.Message.ToString();
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
                return RedirectToAction("Index", "Teklif");

            }
            if (id < 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Katilim Katilim = _katilimService.GetById(Id);
            var talepId = Katilim.TalepId;

            ViewBag.Maximum = _talepService.GetDetayById(talepId).Maximum;
            ViewBag.Minimum = _talepService.GetDetayById(talepId).Minimum;
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
        public ActionResult Edit([Bind(Include = "Id,KatilimDurumId,KatilimTarihi,TalepId,EczaneGrupId,ITStransferDurumId,Miktar")] Katilim Katilim)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _katilimService.Update(Katilim);
                    TempData["MessageSuccess"] = "Katilim başarıyla düzenlenmiştir";
                    int? Id = Katilim.EczaneGrupId;
                    return RedirectToAction("Index", new { id = Id });
                }
                catch (Exception ex)
                {
                    TempData["MessageDanger"] = "ERROR:" + ex.InnerException.InnerException.Message.ToString();
                }
            }

           
            var talepId = Katilim.TalepId;
            ViewBag.TalepId = talepId;
            
            ViewBag.Maximum = _talepService.GetDetayById(talepId).Maximum;
            ViewBag.Minimum = _talepService.GetDetayById(talepId).Minimum;
            return View(Katilim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetITS(string KatilimIDForITS, string ITSTransferDurumId, string ExpandedForITS, string pageForITS)
        {
            int id = Convert.ToInt32(KatilimIDForITS);
            // int id = AlimId;

            Katilim katilim = new Katilim();
            katilim = _katilimService.GetById(id);
            try
            {
                _katilimService.Update(katilim);
                TempData["MessageSuccess"] = "ITS transfer durum başarıyla değiştirlmiştir.";
                //return PartialView("TeklifimPartialView", teklifDetayAlimDetaylarViewModel);
            }
            catch (Exception ex)
            {
                TempData["MessageDanger"] = "ERROR: ITS transfer durum değiştirilemedi " + ex.InnerException.InnerException.Message.ToString();

            }
            List<KatilimDetayKatilimDetaylarViewModel> katilimDetayKatilimDetaylarViewModel = new List<KatilimDetayKatilimDetaylarViewModel>();
            katilimDetayKatilimDetaylarViewModel = getKatilimlarim(katilimDetayKatilimDetaylarViewModel, pageForITS);
            var liste3 = ExpandedForITS.Split(',');
            if (liste3[0].Length > 0)
            {
                foreach (string item in liste3)
                {
                    foreach (var katilimDetays in katilimDetayKatilimDetaylarViewModel)
                    {
                        if (katilimDetays.KatilimDetay.Id == Convert.ToInt32(item))
                        {
                            katilimDetays.KatilimDetay.Expanded = true;
                        }
                    }
                }
            }
            return PartialView("KatilimimPartialView", katilimDetayKatilimDetaylarViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetALD(string KatilimIDForALD, string KatilimDurumId, string ExpandedForALD, string pageForALD)
        {
            int id = Convert.ToInt32(KatilimIDForALD);
            // int id = AlimId;


            Katilim katilim = new Katilim();
            katilim = _katilimService.GetById(id);
         
            try
            {
                _katilimService.Update(katilim);
                TempData["MessageSuccess"] = "Alım durum başarıyla Değiştirildi";
                // return PartialView("TeklifimPartialView", teklifDetayAlimDetaylarViewModel);
            }
            catch (Exception ex)
            {
                TempData["MessageDanger"] = "ERROR: Alım Durum değiştirilemedi. " + ex.InnerException.InnerException.Message.ToString();

            }
            List<KatilimDetayKatilimDetaylarViewModel> katilimDetayKatilimDetaylarViewModel = new List<KatilimDetayKatilimDetaylarViewModel>();
            katilimDetayKatilimDetaylarViewModel = getKatilimlarim(katilimDetayKatilimDetaylarViewModel, pageForALD);

            var liste3 = ExpandedForALD.Split(',');
            if (liste3[0].Length > 0)
            {
                foreach (string item in liste3)
                {
                    foreach (var katilimDetays in katilimDetayKatilimDetaylarViewModel)
                    {
                        if (katilimDetays.KatilimDetay.Id == Convert.ToInt32(item))
                        {
                            katilimDetays.KatilimDetay.Expanded = true;
                        }
                    }
                }
            }
            return PartialView("KatilimimPartialView", katilimDetayKatilimDetaylarViewModel);
        }
        // GET: AlimNobet/Katilim/Delete/5
        [HttpGet]
        [Authorize(Roles = "Admin,Grup Yöneticisi,Eczane")]
        public ActionResult Delete(int id)
        {
            // int id = Convert.ToInt32(Id);
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
            Katilim Katilim = _katilimService.GetById(id);
            var teklif = _talepService.GetById(Katilim.TalepId);
            var ilacId = teklif.IlacId;
            ViewBag.IlacAdi = _ilacService.GetById(ilacId).Adi;

            if (Katilim == null)
            {
                return HttpNotFound();
            }
            var jsonResult = Json(Katilim, JsonRequestBehavior.AllowGet);
            return jsonResult;

        }

        private List<KatilimDetayKatilimDetaylarViewModel> getKatilimlarim(List<KatilimDetayKatilimDetaylarViewModel> KatilimDetayKatilimDetaylarViewModel,
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


            var digerKatilimDetaylar = _katilimService.GetDetaylar(w => !eczaneGrupIdlerim.Contains(w.EczaneGrupId));
            var katilimDetaylar = _katilimService.GetDetaylar(w => !eczaneGrupIdlerim.Contains(w.EczaneGrupId))
                .OrderByDescending(o => o.KatilimTarihi).ToList(); 

            if (page == "" || page == null)
            {
                page = "1";
            }
            var pager = new Pager(katilimDetaylar.Count(), Convert.ToInt32(page));
            //katilimDetaylar = katilimDetaylar.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList();
            foreach (var item in katilimDetaylar)
            {
                var katilimDetaylars = digerKatilimDetaylar.Where(w => w.TalepId == item.TalepId).ToList();

                KatilimDetayKatilimDetaylarViewModel.Add(new KatilimDetayKatilimDetaylarViewModel
                {
                    KatilimDetay = item,
                    KatilimDetaylar = katilimDetaylars,
                    Pager = pager
                });
            }
            return KatilimDetayKatilimDetaylarViewModel;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetKatilimMiktar(string KatilimIDForMiktar, string katilimmiktari, string ExpandedForMiktar, string pageForKatilimMiktar)
        {
            int id = Convert.ToInt32(KatilimIDForMiktar);
            // int id = AlimId;

            Katilim katilim = new Katilim();
            katilim = _katilimService.GetById(id);
            katilim.Miktar = Convert.ToInt32(katilimmiktari);
            try
            {
                _katilimService.Update(katilim);
                TempData["MessageSuccess"] = "Alım miktarı başarıyla kaydedilmiştir.";
                //return PartialView("TeklifimPartialView", teklifDetayAlimDetaylarViewModel);
            }
            catch (Exception ex)
            {
                TempData["MessageDanger"] = "ERROR: Alım miktarı kaydedilemedi " + ex.InnerException.InnerException.Message.ToString();

            }
            List<KatilimDetayKatilimDetaylarViewModel> katilimDetayKatilimDetaylarViewModel = new List<KatilimDetayKatilimDetaylarViewModel>();
            katilimDetayKatilimDetaylarViewModel = getKatilimlarim(katilimDetayKatilimDetaylarViewModel, pageForKatilimMiktar);
            var liste3 = ExpandedForMiktar.Split(',');
            if (liste3[0].Length > 0)
            {
                foreach (string item in liste3)
                {
                    foreach (var katilimDetays in katilimDetayKatilimDetaylarViewModel)
                    {
                        if (katilimDetays.KatilimDetay.Id == Convert.ToInt32(item))
                        {
                            katilimDetays.KatilimDetay.Expanded = true;
                        }
                    }
                }
            }
            return PartialView("KatilimimPartialView", katilimDetayKatilimDetaylarViewModel);
        }

        [HttpGet]
        //[ValidateAntiForgeryToken] get de olmuyor
        public ActionResult GetKatilimlarim(string page)
        {
            List<KatilimDetayKatilimDetaylarViewModel> KatilimDetayKatilimDetaylarViewModel = new List<KatilimDetayKatilimDetaylarViewModel>();
            KatilimDetayKatilimDetaylarViewModel = getKatilimlarim(KatilimDetayKatilimDetaylarViewModel, page);
            return PartialView("KatilimimPartialView", KatilimDetayKatilimDetaylarViewModel);
        }

        // POST: AlimNobet/Katilim/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? Id)
        {
            int id = Convert.ToInt32(Id);
            // int id = AlimId;
            Katilim Katilim = _katilimService.GetById(id);
            try
            {
                TempData["MessageSuccess"] = "Teklif başarıyla silinmiştir";
                _katilimService.Delete(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "ERROR:" + ex.InnerException.InnerException.Message.ToString();
            }
            return View(Katilim);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecilenleriBekleniyorYap(string bekleniyorYapilacakKatilimlar, string ExpandedForBekleniyor, string pageForBekleniyor)
        {
            List<KatilimDetayKatilimDetaylarViewModel> KatilimDetayKatilimDetaylarViewModel = new List<KatilimDetayKatilimDetaylarViewModel>();
            KatilimDetayKatilimDetaylarViewModel = getKatilimlarim(KatilimDetayKatilimDetaylarViewModel, pageForBekleniyor);
            List<int> alimIdler = new List<int>();
            var uyariMesaji = "Seçim Yapmadınız!";

            if (bekleniyorYapilacakKatilimlar == null || bekleniyorYapilacakKatilimlar == "")
            {
                return Json(uyariMesaji, JsonRequestBehavior.AllowGet);
            }

            Int32 basamak = bekleniyorYapilacakKatilimlar.IndexOf(';');
            Int32 toplam = bekleniyorYapilacakKatilimlar.Length;

            var alimlar = bekleniyorYapilacakKatilimlar.Substring(0, basamak);

            var liste = alimlar.Split(',');

            //alimlar update 
            if (liste[0].Length > 0)
            {
                foreach (string item in liste)
                {
                    var katilim = new Katilim();
                    katilim = _katilimService.GetById(Convert.ToInt32(item));
                  
                    _katilimService.Update(katilim);
                    alimIdler.Add(Convert.ToInt32(item));

                    ////////henüz KatilimDetayKatilimDetaylarViewModel içinde değer yok//////

                    foreach (var katilimDetays in KatilimDetayKatilimDetaylarViewModel)
                    {
                        if (katilimDetays.KatilimDetay.Id == Convert.ToInt32(item))
                        {
                            katilimDetays.KatilimDetay.Checked = true;
                        }
                    }
                }
            }
            TempData["MessageSuccess"] = "Seçilen alımlar beklemeye alınmıştır.";


            var liste3 = ExpandedForBekleniyor.Split(',');
            if (liste3[0].Length > 0)
            {
                foreach (string item in liste3)
                {
                    foreach (var katilimDetays in KatilimDetayKatilimDetaylarViewModel)
                    {
                        if (katilimDetays.KatilimDetay.Id == Convert.ToInt32(item))
                        {
                            katilimDetays.KatilimDetay.Expanded = true;
                        }
                    }
                }
            }

            return PartialView("KatilimimPartialView", KatilimDetayKatilimDetaylarViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecileniSil(int Id, string pageForSil)
        {
            int id = Convert.ToInt32(Id);
            //KatilimDetayViewModel model = new KatilimDetayViewModel();
            //model = getAliDetayViewModel(1);
            List<KatilimDetayKatilimDetaylarViewModel> KatilimDetayKatilimDetaylarViewModel = new List<KatilimDetayKatilimDetaylarViewModel>();
            KatilimDetayKatilimDetaylarViewModel = getKatilimlarim(KatilimDetayKatilimDetaylarViewModel, pageForSil);
            try
            {
                _katilimService.Delete(id);
                TempData["MessageSuccess"] = "Alım başarıyla silinmiştir";
                return PartialView("KatilimimPartialView", KatilimDetayKatilimDetaylarViewModel);

                //return View("Index", model);
            }
            catch (Exception ex)
            {
                TempData["MessageDanger"] = "ERROR:" + ex.InnerException.InnerException.Message.ToString();
            }

            //return View("Index", model);
            return PartialView("KatilimimPartialView", KatilimDetayKatilimDetaylarViewModel);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecilenleriStokaAl(string stogaAlinacakKatilimlar, string ExpandedForStok, string pageForStok)
        {
            List<KatilimDetayKatilimDetaylarViewModel> KatilimDetayKatilimDetaylarViewModel = new List<KatilimDetayKatilimDetaylarViewModel>();
            KatilimDetayKatilimDetaylarViewModel = getKatilimlarim(KatilimDetayKatilimDetaylarViewModel, pageForStok);
            List<int> alimIdler = new List<int>();
            var uyariMesaji = "Seçim Yapmadınız!";

            if (stogaAlinacakKatilimlar == null || stogaAlinacakKatilimlar == "")
            {
                return Json(uyariMesaji, JsonRequestBehavior.AllowGet);
            }

            Int32 basamak = stogaAlinacakKatilimlar.IndexOf(';');
            Int32 toplam = stogaAlinacakKatilimlar.Length;

            var alimlar = stogaAlinacakKatilimlar.Substring(0, basamak);

            var liste = alimlar.Split(',');

            //alimlar update 
            if (liste[0].Length > 0)
            {
                foreach (string item in liste)
                {
                    var katilim = new Katilim();
                    katilim = _katilimService.GetById(Convert.ToInt32(item));
                    _katilimService.Update(katilim);
                    alimIdler.Add(Convert.ToInt32(item));



                    foreach (var katilimDetays in KatilimDetayKatilimDetaylarViewModel)
                    {
                        if (katilimDetays.KatilimDetay.Id == Convert.ToInt32(item))
                        {
                            katilimDetays.KatilimDetay.Checked = true;

                        }
                    }
                }
            }
            TempData["MessageSuccess"] = "Seçilen alımlar stoğa alınmıştır.";

            var liste3 = ExpandedForStok.Split(',');
            if (liste3[0].Length > 0)
            {
                foreach (string item in liste3)
                {
                    foreach (var katilimDetays in KatilimDetayKatilimDetaylarViewModel)
                    {
                        if (katilimDetays.KatilimDetay.Id == Convert.ToInt32(item))
                        {
                            katilimDetays.KatilimDetay.Expanded = true;
                        }
                    }
                }
            }

            return PartialView("KatilimimPartialView", KatilimDetayKatilimDetaylarViewModel);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecilenleriSil(string silinecekKatilimlar, string ExpandedForSil, string pageForCokluSil)
        {
            List<KatilimDetayKatilimDetaylarViewModel> KatilimDetayKatilimDetaylarViewModel = new List<KatilimDetayKatilimDetaylarViewModel>();
            KatilimDetayKatilimDetaylarViewModel = getKatilimlarim(KatilimDetayKatilimDetaylarViewModel, pageForCokluSil);

            List<int> alimIdler = new List<int>();
            var uyariMesaji = "Seçim Yapmadınız!";

            if (silinecekKatilimlar == null || silinecekKatilimlar=="")
            {
                return Json(uyariMesaji, JsonRequestBehavior.AllowGet);
            }

            Int32 basamak = silinecekKatilimlar.IndexOf(';');
            Int32 toplam = silinecekKatilimlar.Length;

            var alimlar = silinecekKatilimlar.Substring(0, basamak);

            var liste = alimlar.Split(',');

            //alimlar update 
            if (liste[0].Length > 0)
            {
                foreach (string item in liste)
                {
                    var katilim = new Katilim();
                    katilim = _katilimService.GetById(Convert.ToInt32(item));
                    try
                    {
                        _katilimService.Delete(katilim.Id);
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
            TempData["MessageSuccess"] = "Seçilen alimlar silinmiştir.";

            return PartialView("KatilimimPartialView", KatilimDetayKatilimDetaylarViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecilenleriYanlisGonderimYap(string yanlisGonderimYapilacakKatilimlar, string ExpandedForYanlis, string pageForYanlis)
        {
            List<KatilimDetayKatilimDetaylarViewModel> KatilimDetayKatilimDetaylarViewModel = new List<KatilimDetayKatilimDetaylarViewModel>();
            KatilimDetayKatilimDetaylarViewModel = getKatilimlarim(KatilimDetayKatilimDetaylarViewModel, pageForYanlis);
            List<int> alimIdler = new List<int>();
            var uyariMesaji = "Seçim Yapmadınız!";

            if (yanlisGonderimYapilacakKatilimlar == null || yanlisGonderimYapilacakKatilimlar == "")
            {
                return Json(uyariMesaji, JsonRequestBehavior.AllowGet);
            }

            Int32 basamak = yanlisGonderimYapilacakKatilimlar.IndexOf(';');
            Int32 toplam = yanlisGonderimYapilacakKatilimlar.Length;

            var alimlar = yanlisGonderimYapilacakKatilimlar.Substring(0, basamak);

            var liste = alimlar.Split(',');

            //alimlar update 
            if (liste[0].Length > 0)
            {
                foreach (string item in liste)
                {
                    var katilim = new Katilim();
                    katilim = _katilimService.GetById(Convert.ToInt32(item));
                    _katilimService.Update(katilim);
                    alimIdler.Add(Convert.ToInt32(item));



                    foreach (var katilimDetays in KatilimDetayKatilimDetaylarViewModel)
                    {
                        if (katilimDetays.KatilimDetay.Id == Convert.ToInt32(item))
                        {
                            katilimDetays.KatilimDetay.Checked = true;

                        }
                    }
                }
            }
            TempData["MessageSuccess"] = "Seçilen alımların durumları yanlış gönderim yapılmıştır.";

            var liste3 = ExpandedForYanlis.Split(',');
            if (liste3[0].Length > 0)
            {
                foreach (string item in liste3)
                {
                    foreach (var katilimDetays in KatilimDetayKatilimDetaylarViewModel)
                    {
                        if (katilimDetays.KatilimDetay.Id == Convert.ToInt32(item))
                        {
                            katilimDetays.KatilimDetay.Expanded = true;
                        }
                    }
                }
            }

            return PartialView("KatilimimPartialView", KatilimDetayKatilimDetaylarViewModel);

        }
    }
}