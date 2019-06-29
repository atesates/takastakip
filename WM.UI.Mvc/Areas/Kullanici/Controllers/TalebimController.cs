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
    public class TalebimController : Controller
    {
        // GET: Kullanici/talep
        #region ctor
        private IKatilimService _katilimService;
        private IEczaneService _eczaneService;
        private ITalepService _talepService;
        private IEczaneGrupService _eczaneGrupService;
        private IEczaneUserService _eczaneUserService;
        private IUserService _userService;
        private IGrupService _grupService;
        private ITalepDurumService _talepDurumService;
        private IIlacService _ilacService;

        public TalebimController(IEczaneGrupService eczaneGrupService,
                                IKatilimService katilimService,
                                IEczaneUserService eczaneUserService,
                                IEczaneService eczaneService,
                                ITalepService talepService,
                                IUserService userService,
                                IGrupService grupService,
                                ITalepDurumService talepDurumService,
                                IIlacService ilacService)
        {

            _katilimService = katilimService;
            _talepService = talepService;
            _eczaneUserService = eczaneUserService;
            _eczaneGrupService = eczaneGrupService;
            _userService = userService;
            _grupService = grupService;
            _eczaneService = eczaneService;
            _talepDurumService = talepDurumService;
            _ilacService = ilacService;
        }
        #endregion

        public ActionResult Index(int? page)
        {
            // var temp = TempData["EczaneGrupId"];
            int thispage = 0;
            if (page != 0)
                thispage = Convert.ToInt32(page);

            var user = _userService.GetByUserName(User.Identity.Name);
            var eczaneler = _eczaneService.GetListByUser(user);
            var gruplar = _grupService.GetListByUser(user);
            var eczaneGruplar = _eczaneGrupService.GetListByUser(user);
            var eczanegrupIdler = eczaneGruplar.Select(s => s.Id).ToList();
            var eczaneIdler = eczaneler.Select(s => s.Id).ToList();
            
            var talepDetaylar = _talepService.GetMyDetayListByEczaneGruplar(eczaneGruplar);
            var talepIdler = _talepService.GetMyListByEczaneGruplar(eczaneGruplar).Select(s => s.Id).ToList();
            var katilimlar = _katilimService.GetListByTeklifler(talepIdler);
            var pager = new Pager(talepDetaylar.Count(), thispage);

            var TalepDurumlar = _talepDurumService.GetList();
            ViewBag.TalepDurumId = new SelectList(TalepDurumlar, "Id", "Adi");

            var model = new TalepDetayViewModel()
            {
                Katilimlar = katilimlar,
                TalepDetaylar = talepDetaylar,//.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList(),
                Eczaneler = eczaneler,
                Pager = pager,
            };

            return View(model);
            //}      
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecilenleriPasifYap(string pasifYapilacakTalepler, string ExpandedForPasif, string pageForPasif, string talepDurumIdForPasif)
        {
            List<TalepDetayKatilimDetaylarViewModel> talepdetayKatilimDetaylarViewModel = new List<TalepDetayKatilimDetaylarViewModel>();
            List<int> talepIdler = new List<int>();
            var uyariMesaji = "Seçim Yapmadınız!";

            if (pasifYapilacakTalepler == null || pasifYapilacakTalepler == ";" || pasifYapilacakTalepler == "")
            {
                TempData["MessageSuccess"] = uyariMesaji;
                talepdetayKatilimDetaylarViewModel = getTalepler(talepdetayKatilimDetaylarViewModel, pageForPasif, talepDurumIdForPasif);

                return PartialView("TalebimPartialView", talepdetayKatilimDetaylarViewModel);
                // return Json(uyariMesaji, JsonRequestBehavior.AllowGet);
            }

            Int32 basamak = pasifYapilacakTalepler.IndexOf(';');
            Int32 toplam = pasifYapilacakTalepler.Length;

            var talepler = pasifYapilacakTalepler.Substring(0, basamak);

            var liste = talepler.Split(',');

            //talepler update 
            if (liste[0].Length > 0)
            {
                foreach (string item in liste)
                {
                    var talep = new Talep();
                    talep = _talepService.GetById(Convert.ToInt32(item));
                    talep.TalepDurumId = 2;//pasif
                    _talepService.Update(talep);
                    talepIdler.Add(Convert.ToInt32(item));

                    ////////henüz talepdetayKatilimDetaylarViewModel içinde değer yok//////

                    //foreach (var talepDetays in talepdetayKatilimDetaylarViewModel)
                    //{
                    //    if (talepDetays.TalepDetay.Id == Convert.ToInt32(item))
                    //    {
                    //        talepDetays.TalepDetay.Checked = true;
                    //        talepDetays.TalepDetay.TalepDurumId = 2;
                    //        talepDetays.TalepDetay.TalepDurumAdi = "Pasif";
                    //    }
                    //}
                }
            }
            talepdetayKatilimDetaylarViewModel = getTalepler(talepdetayKatilimDetaylarViewModel, pageForPasif, talepDurumIdForPasif);

            TempData["MessageSuccess"] = "Secilen tekliflerin durumları pasif yapılmıştır.";

            /////////////////////////////////

            //var talepDetaylar = _talepService.GetDetaylar(w => talepIdler.Contains(w.Id));          
            //var alimDetaylar = _katilimService.GetDetaylar();

            //foreach (var item in talepDetaylar)
            //{
            //    item.Checked = true;
            //    item.TalepDurumId = 2;
            //    var alimDetays = alimDetaylar.Where(w => w.TalepId == item.Id).ToList();
            //    talepdetayKatilimDetaylarViewModel.Add(new TalepDetayKatilimDetaylarViewModel
            //    {
            //        TalepDetay = item,

            //        KatilimDetaylar = alimDetays,
            //    });
            //}


            var liste3 = ExpandedForPasif.Split(',');
            if (liste3[0].Length > 0)
            {
                foreach (string item in liste3)
                {
                    foreach (var talepDetays in talepdetayKatilimDetaylarViewModel)
                    {
                        if (talepDetays.TalepDetay.Id == Convert.ToInt32(item))
                        {
                            talepDetays.TalepDetay.Expanded = true;
                        }
                    }
                }
            }

            var TalepDurumlar = _talepDurumService.GetList();
            ViewBag.TalepDurumIdFilter = new SelectList(TalepDurumlar, "Id", "Adi", talepDurumIdForPasif);

            return PartialView("TalebimPartialView", talepdetayKatilimDetaylarViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetTalepDurum(string TalepId, string TalepDurumId, string ExpandedForTalepDurum, string page, string talepDurumIdForTalepDurum)
        {
            int id = Convert.ToInt32(TalepId);
            // int id = AlimId;


            Talep talep = new Talep();
            talep = _talepService.GetById(id);
            try
            {
                talep.TalepDurumId = Convert.ToInt32(TalepDurumId);
                _talepService.Update(talep);
                TempData["MessageSuccess"] = "Talep durum başarıyla değiştirildi";
                // return PartialView("TalebimPartialView", talepdetayKatilimDetaylarViewModel);
            }
            catch (Exception ex)
            {
                TempData["MessageDanger"] = "ERROR: Talep Durum değiştirilemedi. " + ex.InnerException.InnerException.Message.ToString();

            }
            List<TalepDetayKatilimDetaylarViewModel> talepdetayKatilimDetaylarViewModel = new List<TalepDetayKatilimDetaylarViewModel>();
            talepdetayKatilimDetaylarViewModel = getTalepler(talepdetayKatilimDetaylarViewModel, page, talepDurumIdForTalepDurum);

            var liste3 = ExpandedForTalepDurum.Split(',');
            if (liste3[0].Length > 0)
            {
                foreach (string item in liste3)
                {
                    foreach (var talepDetays in talepdetayKatilimDetaylarViewModel)
                    {
                        if (talepDetays.TalepDetay.Id == Convert.ToInt32(item))
                        {
                            talepDetays.TalepDetay.Expanded = true;
                        }
                    }
                }
            }
            var TalepDurumlar = _talepDurumService.GetList();
            ViewBag.TalepDurumIdFilter = new SelectList(TalepDurumlar, "Id", "Adi", talepDurumIdForTalepDurum);
            return PartialView("TalebimPartialView", talepdetayKatilimDetaylarViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecileniSil(int Id, string pageForSil, string talepDurumIdForSil)
        {
            int id = Convert.ToInt32(Id);
            // int id = AlimId;
            List<TalepDetayKatilimDetaylarViewModel> talepdetayKatilimDetaylarViewModel = new List<TalepDetayKatilimDetaylarViewModel>();

            try
            {
                _talepService.Delete(id);
                TempData["MessageSuccess"] = "Talep başarıyla silinmiştir";
                talepdetayKatilimDetaylarViewModel = getTalepler(talepdetayKatilimDetaylarViewModel, pageForSil, talepDurumIdForSil);

                return PartialView("TalebimPartialView", talepdetayKatilimDetaylarViewModel);
            }
            catch (Exception ex)
            {
                TempData["MessageDanger"] = "ERROR: Talep silinemedi. " + ex.InnerException.InnerException.Message.ToString();

            }
            talepdetayKatilimDetaylarViewModel = getTalepler(talepdetayKatilimDetaylarViewModel, pageForSil, talepDurumIdForSil);
            var TalepDurumlar = _talepDurumService.GetList();
            ViewBag.TalepDurumIdFilter = new SelectList(TalepDurumlar, "Id", "Adi", talepDurumIdForSil);
            return PartialView("TalebimPartialView", talepdetayKatilimDetaylarViewModel);


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecilenleriAktifYap(string aktifYapilacakTalepler, string ExpandedForAktif, string pageForAktif,string talepDurumIdForAktif)
        {
            List<TalepDetayKatilimDetaylarViewModel> talepdetayKatilimDetaylarViewModel = new List<TalepDetayKatilimDetaylarViewModel>();
            List<int> talepIdler = new List<int>();
            var uyariMesaji = "Seçim Yapmadınız!";

            if (aktifYapilacakTalepler == null || aktifYapilacakTalepler == ";" || aktifYapilacakTalepler == "")
            {
                //return Json(uyariMesaji, JsonRequestBehavior.AllowGet);
                TempData["MessageSuccess"] = uyariMesaji;
                talepdetayKatilimDetaylarViewModel = getTalepler(talepdetayKatilimDetaylarViewModel, pageForAktif, talepDurumIdForAktif);

                return PartialView("TalebimPartialView", talepdetayKatilimDetaylarViewModel);

            }

            Int32 basamak = aktifYapilacakTalepler.IndexOf(';');
            Int32 toplam = aktifYapilacakTalepler.Length;

            var talepler = aktifYapilacakTalepler.Substring(0, basamak);

            var liste = talepler.Split(',');

            //talepler update 
            if (liste[0].Length > 0)
            {
                foreach (string item in liste)
                {
                    var talep = new Talep();
                    talep = _talepService.GetById(Convert.ToInt32(item));
                    talep.TalepDurumId = 3;//aktif
                    _talepService.Update(talep);
                    talepIdler.Add(Convert.ToInt32(item));


                    //foreach (var talepDetays in talepdetayKatilimDetaylarViewModel)
                    //{
                    //    if (talepDetays.TalepDetay.Id == Convert.ToInt32(item))
                    //    {
                    //        talepDetays.TalepDetay.Checked = true;
                    //        talepDetays.TalepDetay.TalepDurumId = 3;
                    //        talepDetays.TalepDetay.TalepDurumAdi = "Aktif";

                    //    }
                    //}
                }
            }
            talepdetayKatilimDetaylarViewModel = getTalepler(talepdetayKatilimDetaylarViewModel, pageForAktif, talepDurumIdForAktif);

            TempData["MessageSuccess"] = "Secilen tekliflerin durumları aktif yapılmıştır.";
            /////////////////////////////////
            //var talepDetaylar = _talepService.GetDetaylar(w => talepIdler.Contains(w.Id));
            //var alimDetaylar = _katilimService.GetDetaylar();

            //foreach (var item in talepDetaylar)
            //{
            //    item.Checked = true;
            //    item.TalepDurumId = 1;
            //    var alimDetays = alimDetaylar.Where(w => w.TalepId == item.Id).ToList();
            //    talepdetayKatilimDetaylarViewModel.Add(new TalepDetayKatilimDetaylarViewModel
            //    {
            //        TalepDetay = item,
            //        KatilimDetaylar = alimDetays,
            //    });
            //}

            var liste3 = ExpandedForAktif.Split(',');
            if (liste3[0].Length > 0)
            {
                foreach (string item in liste3)
                {
                    foreach (var talepDetays in talepdetayKatilimDetaylarViewModel)
                    {
                        if (talepDetays.TalepDetay.Id == Convert.ToInt32(item))
                        {
                            talepDetays.TalepDetay.Expanded = true;
                        }
                    }
                }
            }
            var TalepDurumlar = _talepDurumService.GetList();
            ViewBag.TalepDurumIdFilter = new SelectList(TalepDurumlar, "Id", "Adi", talepDurumIdForAktif);
            return PartialView("TalebimPartialView", talepdetayKatilimDetaylarViewModel);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecilenleriSil(string silinecekTalepler, string ExpandedForSil, string pageForCokluSil, string talepDurumIdForCokluSil)
        {
            List<TalepDetayKatilimDetaylarViewModel> talepdetayKatilimDetaylarViewModel = new List<TalepDetayKatilimDetaylarViewModel>();


            List<int> talepIdler = new List<int>();
            var uyariMesaji = "Seçim Yapmadınız!";

            if (silinecekTalepler == null || silinecekTalepler == ";" || silinecekTalepler == "")
            {
                TempData["MessageSuccess"] = uyariMesaji;
                talepdetayKatilimDetaylarViewModel = getTalepler(talepdetayKatilimDetaylarViewModel, pageForCokluSil, talepDurumIdForCokluSil);

                return PartialView("TalebimPartialView", talepdetayKatilimDetaylarViewModel);
                // return Json(uyariMesaji, JsonRequestBehavior.AllowGet);
            }

            Int32 basamak = silinecekTalepler.IndexOf(';');
            Int32 toplam = silinecekTalepler.Length;

            var talepler = silinecekTalepler.Substring(0, basamak);

            var liste = talepler.Split(',');

            //talepler update 
            if (liste[0].Length > 0)
            {
                foreach (string item in liste)
                {
                    var talep = new Talep();
                    talep = _talepService.GetById(Convert.ToInt32(item));
                    try
                    {
                        _talepService.Delete(talep.Id);
                        TempData["MessageSuccess"] = "Secilen taleplerden bazıları silindi. ";
                        talepdetayKatilimDetaylarViewModel = getTalepler(talepdetayKatilimDetaylarViewModel, pageForCokluSil, talepDurumIdForCokluSil);
                    }
                    catch (Exception ex)
                    {
                        string hataMesaji = ex.InnerException.InnerException.Message;
                        TempData["MessageDanger"] = "Secilen taleplerden bazıları silinemedi. " + hataMesaji;
                    }
                    talepIdler.Add(Convert.ToInt32(item));
                }
            }
            var TalepDurumlar = _talepDurumService.GetList();
            ViewBag.TalepDurumIdFilter = new SelectList(TalepDurumlar, "Id", "Adi", talepDurumIdForCokluSil);
            //TempData["MessageSuccess"] = "Secilen talepler silinmiştir.";
            talepdetayKatilimDetaylarViewModel = getTalepler(talepdetayKatilimDetaylarViewModel, pageForCokluSil, talepDurumIdForCokluSil);
            return PartialView("TalebimPartialView", talepdetayKatilimDetaylarViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetTransferEt(string TalepId, string TalepDurumId, string ExpandedForTalepDurum, string page, string talepDurumId)
        {
            int id = Convert.ToInt32(TalepId);
            // int id = AlimId;
            Talep talep = new Talep();
            talep = _talepService.GetById(id);
            try
            {
                talep.TalepDurumId = Convert.ToInt32(TalepDurumId);
                _talepService.Update(talep);
                return RedirectToAction("Create", "Teklifim", new { id = talep.Id });
                
            }
            catch (Exception ex)
            {
                TempData["MessageDanger"] = "ERROR: Talep Durum değiştirilemedi. " + ex.InnerException.InnerException.Message.ToString();
            }
            List<TalepDetayKatilimDetaylarViewModel> talepdetayKatilimDetaylarViewModel = new List<TalepDetayKatilimDetaylarViewModel>();
            talepdetayKatilimDetaylarViewModel = getTalepler(talepdetayKatilimDetaylarViewModel, page, talepDurumId);

            var liste3 = ExpandedForTalepDurum.Split(',');
            if (liste3[0].Length > 0)
            {
                foreach (string item in liste3)
                {
                    foreach (var talepDetays in talepdetayKatilimDetaylarViewModel)
                    {
                        if (talepDetays.TalepDetay.Id == Convert.ToInt32(item))
                        {
                            talepDetays.TalepDetay.Expanded = true;
                        }
                    }
                }
            }
            var TalepDurumlar = _talepDurumService.GetList();
            ViewBag.TalepDurumIdFilter = new SelectList(TalepDurumlar, "Id", "Adi", talepDurumId);
            return PartialView("TalebimPartialView", talepdetayKatilimDetaylarViewModel);
        }

        private List<TalepDetayKatilimDetaylarViewModel> getTalepler(List<TalepDetayKatilimDetaylarViewModel>
            talepdetayKatilimDetaylarViewModel, string page, string talepDurumId)
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var eczaneler = _eczaneService.GetListByUser(user);
            var gruplar = _grupService.GetListByUser(user);
            var eczaneGruplar = _eczaneGrupService.GetListByUser(user);
            var eczanegrupIdler = eczaneGruplar.Select(s => s.Id).ToList();
            var eczaneIdler = eczaneler.Select(s => s.Id).ToList();

            var talepDetaylar = new List<TalepDetay>();
            if (talepDurumId != null && talepDurumId != "")
            {
                talepDetaylar = _talepService.GetMyListByEczaneGrupId(eczanegrupIdler.FirstOrDefault())
                    .Where(w=>w.TalepDurumId == Convert.ToInt32(talepDurumId))
                    .OrderByDescending(o => o.KayitTarihi).ToList();
            }
            else
            {
                talepDetaylar = _talepService.GetMyListByEczaneGrupId(eczanegrupIdler.FirstOrDefault())
               .OrderByDescending(o => o.KayitTarihi).ToList();
            }
            var alimDetaylar = _katilimService.GetDetaylar();

            if (page == "" || page == null)
            {
                page = "1";
            }

            
            var TalepDurumlar = _talepDurumService.GetList();
            ViewBag.TalepDurumIdFilter = new SelectList(TalepDurumlar, "Id", "Adi", talepDurumId);
            var pager = new Pager(talepDetaylar.Count(), Convert.ToInt32(page));
            //talepDetaylar = talepDetaylar.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList();
            foreach (var item in talepDetaylar)
            {
                var katilimDetaylars = alimDetaylar.Where(w => w.TalepId == item.Id).ToList();

                talepdetayKatilimDetaylarViewModel.Add(new TalepDetayKatilimDetaylarViewModel
                {
                    TalepDetay = item,
                    KatilimDetaylar = katilimDetaylars,
                    Pager = pager
                });
            }
            return talepdetayKatilimDetaylarViewModel;
        }

        [HttpGet]
        //[ValidateAntiForgeryToken]
        public ActionResult GetTaleplerim(string page, string talepDurumId)
        {
            List<TalepDetayKatilimDetaylarViewModel> talepdetayKatilimDetaylarViewModel = new List<TalepDetayKatilimDetaylarViewModel>();
            talepdetayKatilimDetaylarViewModel = getTalepler(talepdetayKatilimDetaylarViewModel, page, talepDurumId);
            return PartialView("TalebimPartialView", talepdetayKatilimDetaylarViewModel);
        }
        public ActionResult Search(string Keywords)
        {
            var result = _ilacService.GetList().Where(p => Regex.Split(Keywords, @"\s").Any(x => p.Adi.ToLower().Contains(x.ToLower()) || p.Adi.ToLower().Contains(x.ToLower()))).Select(s => s.Id).ToList();
            return View("Create", result);//result:model
        }
        // GET: TeklifNobet/Talep/Details/5
        public ActionResult Details(int id)
        {
            if (id < 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TalepDetay Talep = _talepService.GetDetayById(id);
            if (Talep == null)
            {
                return HttpNotFound();
            }
            return View(Talep);
        }
        [HttpGet]
        //[Authorize(Roles = "Admin,Grup Yöneticisi, Eczane")]
        public ActionResult CreateViaApi(string id)
        {
            Talep Talep = new Talep() { KayitTarihi = DateTime.Now };
            var user = _userService.GetByUserName(User.Identity.Name);
            var eczaneGruplar = _eczaneGrupService.GetListByUser(user);
            var gruplar = _grupService.GetListByUser(user);
            var eczaneler = _eczaneService.GetListByUser(user);

            var eczaneGrupEczaneler = gruplar
                 .Join(eczaneGruplar, g => g.Id, ge => ge.GrupId, (g, ge) => new { g, ge })
                 .Join(eczaneler, pge => pge.ge.EczaneId, e => e.Id, (pge, e) => new { pge, e })
                 .Select(m => new
                 {
                     GrupAdi = m.pge.g.Adi,
                     EczaneAdi = m.e.Adi,
                     EczaneGrupId = m.pge.ge.Id
                 });
            
            var talepDurumlar = _talepDurumService.GetList();
            var ilaclar = _ilacService.GetList();
            if (id != null)
                ilaclar = _ilacService.GetList()
                    .Where(p => Regex.Split(id, @"\s")
                    .Any(x => p.Adi.ToLower()
                    .Contains(x.ToLower()) || p.Adi.ToLower().Contains(x.ToLower()))).ToList();
            
            ViewBag.TalepDurumId = new SelectList(talepDurumlar, "Id", "Adi");
            ViewBag.IlacId = new SelectList(ilaclar, "Id", "Adi");
            ViewBag.TalepVerenEczaneGrupId = new SelectList(eczaneGrupEczaneler, "EczaneGrupId", "GrupAdi", Talep.TalepVerenEczaneGrupId);

            return View(Talep);
        }
        //GET: TeklifNobet/Talep/Create
        [HttpGet]
        [Authorize(Roles = "Admin,Grup Yöneticisi, Eczane")]
        public ActionResult Create(string id)
        {
            Talep Talep = new Talep() { KayitTarihi = DateTime.Now };
            var user = _userService.GetByUserName(User.Identity.Name);
            var eczaneGruplar = _eczaneGrupService.GetListByUser(user);
            var gruplar = _grupService.GetListByUser(user);
            var eczaneler = _eczaneService.GetListByUser(user);

            var eczaneGrupEczaneler = gruplar
                 .Join(eczaneGruplar, g => g.Id, ge => ge.GrupId, (g, ge) => new { g, ge })
                 .Join(eczaneler, pge => pge.ge.EczaneId, e => e.Id, (pge, e) => new { pge, e })
                 .Select(m => new
                 {
                     GrupAdi = m.pge.g.Adi,
                     EczaneAdi = m.e.Adi,
                     EczaneGrupId = m.pge.ge.Id
                 });
            
            var talepDurumlar = _talepDurumService.GetList();
            var ilaclar = _ilacService.GetList();
            if (id != null)
                ilaclar = _ilacService.GetList()
                    .Where(p => Regex.Split(id, @"\s")
                    .Any(x => p.Adi.ToLower()
                    .Contains(x.ToLower()) || p.Adi.ToLower().Contains(x.ToLower()))).ToList();

            
            ViewBag.TalepDurumId = new SelectList(talepDurumlar, "Id", "Adi");
            ViewBag.IlacId = new SelectList(ilaclar, "Id", "Adi");
            ViewBag.TalepVerenEczaneGrupId = new SelectList(eczaneGrupEczaneler, "EczaneGrupId", "GrupAdi", Talep.TalepVerenEczaneGrupId);

            return View(Talep);
        }
        public ActionResult SearchIndex(string Keywords)
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var eczaneler = _eczaneService.GetListByUser(user);
            var gruplar = _grupService.GetListByUser(user);
            var eczaneGruplar = _eczaneGrupService.GetListByUser(user);
            var eczanegrupIdler = eczaneGruplar.Select(s => s.Id).ToList();
            var grupIdler = gruplar.Select(s => s.Id).ToList();


            var talepler = _talepService.GetListByEczaneGruplar(eczanegrupIdler, grupIdler);
            var ilaclar = _ilacService.GetList().Where(w => talepler.Select(s => s.IlacId).Contains(w.Id)).ToList();

            var result = talepler.ToList()
                .Where(p => Regex.Split(Keywords, @"\s")
                .Any(x => p.IlacAdi.ToLower().Contains(x.ToLower()) || p.IlacAdi.ToLower().Contains(x.ToLower()))).ToList();
            //bu eczanenin bulunduğu tüm eczanegrup lardaki teklifleri gösterir
            var model = new TalepDetayViewModel()
            {
                TalepDetaylar = result,
                Eczaneler = eczaneler,
                Ilaclar = ilaclar,
                EczaneGruplar = eczaneGruplar
            };

            return View("Index", model);//result:model
        }
        // POST: TeklifNobet/Talep/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IlacId,TalepVerenEczaneGrupId,DepoFiyati,TalepMiktari,KayitTarihi,BitisTarihi,Maximum,Minimum,Aciklama,TalepDurumId")] Talep Talep)
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            //var eczaneId = _eczaneUserService.GetListByUserId(user.Id).Select(s=>s.EczaneId).FirstOrDefault();
            // Talep.NetFiyat =(Math.Round(Talep.NetFiyat, 4));
            Talep.KayitTarihi = DateTime.Now;

            if (ModelState.IsValid)
            {
                if (Talep.Aciklama == null)
                    Talep.Aciklama = " ";
                try
                {
                    _talepService.Insert(Talep);
                    TempData["MessageSuccess"] = "Talep başarıyla oluşturulmuştur";
                    return RedirectToAction("Index", "Talebim");
                }
                catch (Exception ex)
                {
                    TempData["Message"] = "ERROR:" + ex.InnerException.InnerException.Message.ToString();

                }
            }
            
            var talepDurumlar = _talepDurumService.GetList();
            var ilaclar = _ilacService.GetList();
            var eczaneGruplar = _eczaneGrupService.GetListByUser(user);
            var gruplar = _grupService.GetListByUser(user);
            var eczaneler = _eczaneService.GetListByUser(user);
            var eczaneGrupEczaneler = gruplar
                 .Join(eczaneGruplar, g => g.Id, ge => ge.GrupId, (g, ge) => new { g, ge })
                 .Join(eczaneler, pge => pge.ge.EczaneId, e => e.Id, (pge, e) => new { pge, e })
                 .Select(m => new
                 {
                     GrupAdi = m.pge.g.Adi,
                     EczaneAdi = m.e.Adi,
                     EczaneGrupId = m.pge.ge.Id
                 });


         
            ViewBag.TalepDurumId = new SelectList(talepDurumlar, "Id", "Adi", Talep.TalepDurumId);
            ViewBag.IlacId = new SelectList(ilaclar, "Id", "Adi", Talep.IlacId);
            ViewBag.TalepVerenEczaneGrupId = new SelectList(eczaneGrupEczaneler, "EczaneGrupId", "GrupAdi", Talep.TalepVerenEczaneGrupId);

            return View(Talep);
        }

        // GET: TeklifNobet/Talep/Edit/5
        [Authorize(Roles = "Admin,Grup Yöneticisi,Eczane")]
        public ActionResult Edit(int? id)
        {
            int Id = 0;
            if (id == null)
                return RedirectToAction("Index", "Talebim");

            Id = Convert.ToInt32(id);
            if (id < 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Talep Talep = _talepService.GetById(Id);
            var user = _userService.GetByUserName(User.Identity.Name);

            var talepDurumlar = _talepDurumService.GetList();
            var ilaclar = _ilacService.GetList();
            var eczaneGruplar = _eczaneGrupService.GetListByUser(user);
            var gruplar = _grupService.GetListByUser(user);
            var eczaneler = _eczaneService.GetListByUser(user);

            var eczaneGrupEczaneler = gruplar
                 .Join(eczaneGruplar, g => g.Id, ge => ge.GrupId, (g, ge) => new { g, ge })
                 .Join(eczaneler, pge => pge.ge.EczaneId, e => e.Id, (pge, e) => new { pge, e })
                 .Select(m => new
                 {
                     GrupAdi = m.pge.g.Adi,
                     EczaneAdi = m.e.Adi,
                     EczaneGrupId = m.pge.ge.Id
                 });


            ViewBag.TalepDurumId = new SelectList(talepDurumlar, "Id", "Adi", Talep.TalepDurumId);
            ViewBag.IlacId = new SelectList(ilaclar, "Id", "Adi", Talep.IlacId);
            ViewBag.TalepVerenEczaneGrupId = new SelectList(eczaneGrupEczaneler, "EczaneGrupId", "GrupAdi", Talep.TalepVerenEczaneGrupId);


            if (Talep == null)
            {
                return HttpNotFound();
            }
            return View(Talep);
        }

        // POST: TeklifNobet/Talep/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IlacId,DepoFiyati,TalepVerenEczaneGrupId,KayitTarihi,TalepMiktari,KayitTarihi,BitisTarihi,Maximum,Minimum,TalepDurumId,Aciklama")] Talep Talep)
        {
            if (ModelState.IsValid)
            {
                if (Talep.Aciklama == null)
                    Talep.Aciklama = " ";
                try
                {
                    _talepService.Update(Talep);
                    TempData["MessageSuccess"] = "Talep başarıyla düzenlenmiştir";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Message"] = "ERROR:" + ex.InnerException.InnerException.Message.ToString();
                }
            }

            var user = _userService.GetByUserName(User.Identity.Name);

            var talepDurumlar = _talepDurumService.GetList();
            var ilaclar = _ilacService.GetList();
            var eczaneGruplar = _eczaneGrupService.GetListByUser(user);
            var gruplar = _grupService.GetListByUser(user);
            var eczaneler = _eczaneService.GetListByUser(user);

            var eczaneGrupEczaneler = gruplar
                 .Join(eczaneGruplar, g => g.Id, ge => ge.GrupId, (g, ge) => new { g, ge })
                 .Join(eczaneler, pge => pge.ge.EczaneId, e => e.Id, (pge, e) => new { pge, e })
                 .Select(m => new
                 {
                     GrupAdi = m.pge.g.Adi,
                     EczaneAdi = m.e.Adi,
                     EczaneGrupId = m.pge.ge.Id
                 });

            ViewBag.TalepDurumId = new SelectList(talepDurumlar, "Id", "Adi", Talep.TalepDurumId);
            ViewBag.IlacId = new SelectList(ilaclar, "Id", "Adi", Talep.IlacId);
            ViewBag.TalepVerenEczaneGrupId = new SelectList(eczaneGrupEczaneler, "EczaneGrupId", "GrupAdi", Talep.TalepVerenEczaneGrupId);

            return View(Talep);
        }

        // GET: TeklifNobet/Talep/Delete/5
        [HttpGet]
        [Authorize(Roles = "Admin,Grup Yöneticisi,Eczane")]
        public ActionResult Delete(int? id)
        {
            //if (Id == null)
            //    return RedirectToAction("Index");
            int Id = Convert.ToInt32(id);

            if (Id < 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Talep Talep = _talepService.GetById(Id);
            ViewBag.IlacAdi = _ilacService.GetById(Talep.IlacId).Adi;
            if (Talep == null)
            {
                return HttpNotFound();
            }
            return View(Talep);
        }

        // POST: TeklifNobet/Talep/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Talep Talep = _talepService.GetById(id);
            try
            {
                TempData["MessageSuccess"] = "Talep başarıyla silinmiştir";
                _talepService.Delete(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "ERROR:" + ex.InnerException.InnerException.Message.ToString();
            }
            return View(Talep);

        }
    }
}