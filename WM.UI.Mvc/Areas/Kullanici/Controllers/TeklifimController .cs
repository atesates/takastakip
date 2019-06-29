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
    public class TeklifimController : Controller
    {
        // GET: Kullanici/teklif
        #region ctor
        private IAlimService _alimService;
        private IAlimDurumService _alimDurumService;
        private IEczaneService _eczaneService;
        private ITeklifService _teklifService;
        private ITalepService _talepService;
        private IEczaneGrupService _eczaneGrupService;
        private IEczaneUserService _eczaneUserService;
        private IUserService _userService;
        private ITeklifTurService _teklifTurService;
        private IGrupService _grupService;
        private ITeklifDurumService _teklifDurumService;
        private IYayinlamaTurService _yayinlamaTurService;
        private IIlacService _ilacService;
        private IITStransferDurumService _ITStransferDurumService;

        public TeklifimController(IEczaneGrupService eczaneGrupService,
                                IAlimService alimService,
                                ITalepService talepService,
                                IAlimDurumService alimDurumService,
                                IEczaneUserService eczaneUserService,
                                IEczaneService eczaneService,
                                ITeklifService teklifService,
                                IUserService userService,
                                ITeklifTurService teklifTurService,
                                IGrupService grupService,
                                IYayinlamaTurService yayinlamaTurService,
                                ITeklifDurumService teklifDurumService,
                                IITStransferDurumService iTStransferDurumService,
                                IIlacService ilacService)
        {
            
            _alimService = alimService;
            _teklifService = teklifService;
            _alimDurumService = alimDurumService;
            _ITStransferDurumService = iTStransferDurumService;
            _eczaneUserService = eczaneUserService;
            _eczaneGrupService = eczaneGrupService;
            _userService = userService;
            _talepService = talepService;
            _grupService = grupService;
            _eczaneService = eczaneService;
            _teklifTurService = teklifTurService;
            _teklifDurumService = teklifDurumService;
            _yayinlamaTurService = yayinlamaTurService;
            _ilacService = ilacService;
        }
        #endregion

        public ActionResult Index(int? page)
        {
            var teklifTurler = _teklifTurService.GetList();
            ViewBag.TeklifTurId = new SelectList(teklifTurler, "Id", "Adi");
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

            var ITStransferDurumlar = _ITStransferDurumService.GetList();
            ViewBag.ITStransferDurumId = new SelectList(ITStransferDurumlar, "Id", "Adi");
            var AlimDurumlar = _alimDurumService.GetList().Where(w=>w.AliciTarafiMi != true);
            ViewBag.AlimDurumId = new SelectList(AlimDurumlar, "Id", "Adi");
            var TeklifDurumlar = _teklifDurumService.GetList();
            ViewBag.TeklifDurumId = new SelectList(TeklifDurumlar, "Id", "Adi");

            var teklifDetaylar = _teklifService.GetMyDetayListByEczaneGruplar(eczaneGruplar);
            var teklifIdler = _teklifService.GetMyListByEczaneGruplar(eczaneGruplar).Select(s=>s.Id).ToList();
            var alimlar = _alimService.GetListByTeklifler(teklifIdler);
            var pager = new Pager(teklifDetaylar.Count(), thispage);

            var model = new TeklifDetayViewModel()
            {
                Alimlar = alimlar,
                TeklifDetaylar = teklifDetaylar,//.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList(),
                Eczaneler = eczaneler,
                Pager=pager,
            };

            return View(model);
            //}      
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecilenleriPasifYap(string pasifYapilacakTeklifler, string ExpandedForPasif, string pageForPasif, string teklifDurumIdForPasif, string alimDurumIdForPasif)
        {
            List<TeklifDetayAlimDetaylarViewModel> teklifDetayAlimDetaylarViewModel = new List<TeklifDetayAlimDetaylarViewModel>();
           
            List<int> teklifIdler = new List<int>();
            var uyariMesaji = "Seçim Yapmadınız!";

            if (pasifYapilacakTeklifler == null || pasifYapilacakTeklifler == ";" || pasifYapilacakTeklifler == "")
            {
                TempData["MessageSuccess"] = uyariMesaji;
                teklifDetayAlimDetaylarViewModel = getTeklifler(teklifDetayAlimDetaylarViewModel, pageForPasif,  teklifDurumIdForPasif,  alimDurumIdForPasif);

                return PartialView("TeklifimPartialView", teklifDetayAlimDetaylarViewModel);
                // return Json(uyariMesaji, JsonRequestBehavior.AllowGet);
            }

            Int32 basamak = pasifYapilacakTeklifler.IndexOf(';');
            Int32 toplam = pasifYapilacakTeklifler.Length;

            var teklifler = pasifYapilacakTeklifler.Substring(0, basamak);

            var liste = teklifler.Split(',');

            //teklifler update 
            if (liste[0].Length > 0)
            {
                foreach (string item in liste)
                {
                    var teklif = new Teklif();
                    teklif = _teklifService.GetById(Convert.ToInt32(item));
                    teklif.TeklifDurumId = 2;//pasif
                    _teklifService.Update(teklif);
                    teklifIdler.Add(Convert.ToInt32(item));

                    ////////henüz teklifDetayAlimDetaylarViewModel içinde değer yok//////

                    //foreach (var teklifDetays in teklifDetayAlimDetaylarViewModel)
                    //{
                    //    if (teklifDetays.TeklifDetay.Id == Convert.ToInt32(item))
                    //    {
                    //        teklifDetays.TeklifDetay.Checked = true;
                    //        teklifDetays.TeklifDetay.TeklifDurumId = 2;
                    //        teklifDetays.TeklifDetay.TeklifDurumAdi = "Pasif";
                    //    }
                    //}
                }
            }
            teklifDetayAlimDetaylarViewModel = getTeklifler(teklifDetayAlimDetaylarViewModel, pageForPasif, teklifDurumIdForPasif, alimDurumIdForPasif);
            TempData["MessageSuccess"] = "Secilen tekliflerin durumları pasif yapılmıştır.";

            /////////////////////////////////

            //var teklifDetaylar = _teklifService.GetDetaylar(w => teklifIdler.Contains(w.Id));          
            //var alimDetaylar = _alimService.GetDetaylar();

            //foreach (var item in teklifDetaylar)
            //{
            //    item.Checked = true;
            //    item.TeklifDurumId = 2;
            //    var alimDetays = alimDetaylar.Where(w => w.TeklifId == item.Id).ToList();
            //    teklifDetayAlimDetaylarViewModel.Add(new TeklifDetayAlimDetaylarViewModel
            //    {
            //        TeklifDetay = item,
                    
            //        AlimDetaylar = alimDetays,
            //    });
            //}


            var liste3 = ExpandedForPasif.Split(',');
            if (liste3[0].Length > 0)
            {
                foreach (string item in liste3)
                {
                    foreach (var teklifDetays in teklifDetayAlimDetaylarViewModel)
                    {
                        if (teklifDetays.TeklifDetay.Id == Convert.ToInt32(item))
                        {
                            teklifDetays.TeklifDetay.Expanded = true;
                        }
                    }
                }
            }

            var AlimDurumlar = _alimDurumService.GetList();
            ViewBag.AlimDurumIdFilter = new SelectList(AlimDurumlar, "Id", "Adi", alimDurumIdForPasif);
            var TeklifDurumlar = _teklifDurumService.GetList();
            ViewBag.TeklifDurumIdFilter = new SelectList(TeklifDurumlar, "Id", "Adi", teklifDurumIdForPasif);

            return PartialView("TeklifimPartialView", teklifDetayAlimDetaylarViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetITS(string AlimIDForITS, string ITSTransferDurumId, string ExpandedForITS, string page, string teklifDurumIdForITS, string alimDurumIdForITS)
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
            List<TeklifDetayAlimDetaylarViewModel> teklifDetayAlimDetaylarViewModel = new List<TeklifDetayAlimDetaylarViewModel>();
            teklifDetayAlimDetaylarViewModel = getTeklifler(teklifDetayAlimDetaylarViewModel, page,   teklifDurumIdForITS, alimDurumIdForITS);
            var liste3 = ExpandedForITS.Split(',');
            if (liste3[0].Length > 0)
            {
                foreach (string item in liste3)
                {
                    foreach (var teklifDetays in teklifDetayAlimDetaylarViewModel)
                    {
                        if (teklifDetays.TeklifDetay.Id == Convert.ToInt32(item))
                        {
                            teklifDetays.TeklifDetay.Expanded = true;
                        }
                    }
                }
            }
            var AlimDurumlar = _alimDurumService.GetList();
            ViewBag.AlimDurumIdFilter = new SelectList(AlimDurumlar, "Id", "Adi", alimDurumIdForITS);
            var TeklifDurumlar = _teklifDurumService.GetList();
            ViewBag.TeklifDurumIdFilter = new SelectList(TeklifDurumlar, "Id", "Adi", teklifDurumIdForITS);
            return PartialView("TeklifimPartialView", teklifDetayAlimDetaylarViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetALD(string AlimIDForALD, string AlimDurumId, string ExpandedForALD, string page, string teklifDurumIdForALD, string alimDurumIdForALD)
        {
            int id = Convert.ToInt32(AlimIDForALD);
            // int id = AlimId;

            
            Alim alim = new Alim();
            alim = _alimService.GetById(id);
            alim.AlimDurumId = Convert.ToInt32(AlimDurumId);
            if (Convert.ToInt32(AlimDurumId) == 2 || Convert.ToInt32(AlimDurumId) == 3)
                alim.GonderimTarihi = DateTime.Now;
            else if (Convert.ToInt32(AlimDurumId) == 1)
                alim.GonderimTarihi = null;
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
            List<TeklifDetayAlimDetaylarViewModel> teklifDetayAlimDetaylarViewModel = new List<TeklifDetayAlimDetaylarViewModel>();
            teklifDetayAlimDetaylarViewModel = getTeklifler(teklifDetayAlimDetaylarViewModel, page, teklifDurumIdForALD, alimDurumIdForALD);

            var liste3 = ExpandedForALD.Split(',');
            if (liste3[0].Length > 0)
            {
                foreach (string item in liste3)
                {
                    foreach (var teklifDetays in teklifDetayAlimDetaylarViewModel)
                    {
                        if (teklifDetays.TeklifDetay.Id == Convert.ToInt32(item))
                        {
                            teklifDetays.TeklifDetay.Expanded = true;
                        }
                    }
                }
            }
            var AlimDurumlar = _alimDurumService.GetList();
            ViewBag.AlimDurumIdFilter = new SelectList(AlimDurumlar, "Id", "Adi", alimDurumIdForALD);
            var TeklifDurumlar = _teklifDurumService.GetList();
            ViewBag.TeklifDurumIdFilter = new SelectList(TeklifDurumlar, "Id", "Adi", teklifDurumIdForALD);
            return PartialView("TeklifimPartialView", teklifDetayAlimDetaylarViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecileniSil(int Id, string pageForSil, string teklifDurumIdForSil, string alimDurumIdForSil)
        {
            int id = Convert.ToInt32(Id);
            // int id = AlimId;
            List<TeklifDetayAlimDetaylarViewModel> teklifDetayAlimDetaylarViewModel = new List<TeklifDetayAlimDetaylarViewModel>();

            try
            {
                _teklifService.Delete(id);
                TempData["MessageSuccess"] = "Teklif başarıyla silinmiştir";
                teklifDetayAlimDetaylarViewModel = getTeklifler(teklifDetayAlimDetaylarViewModel, pageForSil,  teklifDurumIdForSil,  alimDurumIdForSil);

                return PartialView("TeklifimPartialView", teklifDetayAlimDetaylarViewModel);
            }
            catch (Exception ex)
            {         
                 TempData["MessageDanger"] = "ERROR: Teklif silinemedi. " + ex.InnerException.InnerException.Message.ToString();
        
            }
            teklifDetayAlimDetaylarViewModel = getTeklifler(teklifDetayAlimDetaylarViewModel, pageForSil,  teklifDurumIdForSil,  alimDurumIdForSil);
            var AlimDurumlar = _alimDurumService.GetList();
            ViewBag.AlimDurumIdFilter = new SelectList(AlimDurumlar, "Id", "Adi", alimDurumIdForSil);
            var TeklifDurumlar = _teklifDurumService.GetList();
            ViewBag.TeklifDurumIdFilter = new SelectList(TeklifDurumlar, "Id", "Adi", teklifDurumIdForSil);
            return PartialView("TeklifimPartialView", teklifDetayAlimDetaylarViewModel);


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecilenleriAktifYap(string aktifYapilacakTeklifler, string ExpandedForAktif, string pageForAktif, string teklifDurumIdForAktif, string alimDurumIdForAktif)
        {
            List<TeklifDetayAlimDetaylarViewModel> teklifDetayAlimDetaylarViewModel = new List<TeklifDetayAlimDetaylarViewModel>();
            List<int> teklifIdler = new List<int>();
            var uyariMesaji = "Seçim Yapmadınız!";

            if (aktifYapilacakTeklifler == null || aktifYapilacakTeklifler == ";" || aktifYapilacakTeklifler == "")
            {
                //return Json(uyariMesaji, JsonRequestBehavior.AllowGet);
                TempData["MessageSuccess"] =  uyariMesaji;
                teklifDetayAlimDetaylarViewModel = getTeklifler(teklifDetayAlimDetaylarViewModel, pageForAktif,  teklifDurumIdForAktif,  alimDurumIdForAktif);

                return PartialView("TeklifimPartialView", teklifDetayAlimDetaylarViewModel);

            }

            Int32 basamak = aktifYapilacakTeklifler.IndexOf(';');
            Int32 toplam = aktifYapilacakTeklifler.Length;

            var teklifler = aktifYapilacakTeklifler.Substring(0, basamak);

            var liste = teklifler.Split(',');

            //teklifler update 
            if (liste[0].Length > 0)
            {
                foreach (string item in liste)
                {
                    var teklif = new Teklif();
                    teklif = _teklifService.GetById(Convert.ToInt32(item));
                    teklif.TeklifDurumId = 1;//aktif
                    _teklifService.Update(teklif);
                    teklifIdler.Add(Convert.ToInt32(item));



                    //foreach (var teklifDetays in teklifDetayAlimDetaylarViewModel)
                    //{
                    //    if (teklifDetays.TeklifDetay.Id == Convert.ToInt32(item))
                    //    {
                    //        teklifDetays.TeklifDetay.Checked = true;
                    //        teklifDetays.TeklifDetay.TeklifDurumId = 1;
                    //        teklifDetays.TeklifDetay.TeklifDurumAdi = "Aktif";

                    //    }
                    //}
                }
            }
            teklifDetayAlimDetaylarViewModel = getTeklifler(teklifDetayAlimDetaylarViewModel, pageForAktif, teklifDurumIdForAktif, alimDurumIdForAktif);
            TempData["MessageSuccess"] = "Secilen tekliflerin durumları aktif yapılmıştır.";
            /////////////////////////////////
            //var teklifDetaylar = _teklifService.GetDetaylar(w => teklifIdler.Contains(w.Id));
            //var alimDetaylar = _alimService.GetDetaylar();

            //foreach (var item in teklifDetaylar)
            //{
            //    item.Checked = true;
            //    item.TeklifDurumId = 1;
            //    var alimDetays = alimDetaylar.Where(w => w.TeklifId == item.Id).ToList();
            //    teklifDetayAlimDetaylarViewModel.Add(new TeklifDetayAlimDetaylarViewModel
            //    {
            //        TeklifDetay = item,
            //        AlimDetaylar = alimDetays,
            //    });
            //}

            var liste3 = ExpandedForAktif.Split(',');
            if (liste3[0].Length > 0)
            {
                foreach (string item in liste3)
                {
                    foreach (var teklifDetays in teklifDetayAlimDetaylarViewModel)
                    {
                        if (teklifDetays.TeklifDetay.Id == Convert.ToInt32(item))
                        {
                            teklifDetays.TeklifDetay.Expanded = true;
                        }
                    }
                }
            }
            var AlimDurumlar = _alimDurumService.GetList();
            ViewBag.AlimDurumIdFilter = new SelectList(AlimDurumlar, "Id", "Adi", alimDurumIdForAktif);
            var TeklifDurumlar = _teklifDurumService.GetList();
            ViewBag.TeklifDurumIdFilter = new SelectList(TeklifDurumlar, "Id", "Adi", teklifDurumIdForAktif);
            return PartialView("TeklifimPartialView", teklifDetayAlimDetaylarViewModel);
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecilenleriSil(string silinecekTeklifler, string ExpandedForSil, string pageForCokluSil, string teklifDurumIdForCokluSil, string alimDurumIdForCokluSil)
        {
            List<TeklifDetayAlimDetaylarViewModel> teklifDetayAlimDetaylarViewModel = new List<TeklifDetayAlimDetaylarViewModel>();
            

            List<int> teklifIdler = new List<int>();
            var uyariMesaji = "Seçim Yapmadınız!";

            if (silinecekTeklifler == null || silinecekTeklifler == ";" || silinecekTeklifler == "")
            {
                TempData["MessageSuccess"] = uyariMesaji;
                teklifDetayAlimDetaylarViewModel = getTeklifler(teklifDetayAlimDetaylarViewModel, pageForCokluSil,  teklifDurumIdForCokluSil,  alimDurumIdForCokluSil);

                return PartialView("TeklifimPartialView", teklifDetayAlimDetaylarViewModel);
                // return Json(uyariMesaji, JsonRequestBehavior.AllowGet);
            }

            Int32 basamak = silinecekTeklifler.IndexOf(';');
            Int32 toplam = silinecekTeklifler.Length;

            var teklifler = silinecekTeklifler.Substring(0, basamak);

            var liste = teklifler.Split(',');

            //teklifler update 
            if (liste[0].Length > 0)
            {
                foreach (string item in liste)
                {
                    var teklif = new Teklif();
                    teklif = _teklifService.GetById(Convert.ToInt32(item));
                    try
                    {
                        _teklifService.Delete(teklif.Id);
                        TempData["MessageSuccess"] = "Secilen tekliflerden bazıları silindi. ";
                        teklifDetayAlimDetaylarViewModel = getTeklifler(teklifDetayAlimDetaylarViewModel, pageForCokluSil,  teklifDurumIdForCokluSil,  alimDurumIdForCokluSil);
                    }
                    catch(Exception ex)
                    {
                       string  hataMesaji = ex.InnerException.InnerException.Message;
                       TempData["MessageDanger"] = "Secilen tekliflerden bazıları silinemedi. " + hataMesaji;
                    }
                    teklifIdler.Add(Convert.ToInt32(item));                                 
                }
            }
            //TempData["MessageSuccess"] = "Secilen teklifler silinmiştir.";
            teklifDetayAlimDetaylarViewModel = getTeklifler(teklifDetayAlimDetaylarViewModel, pageForCokluSil,  teklifDurumIdForCokluSil,  alimDurumIdForCokluSil);
            var AlimDurumlar = _alimDurumService.GetList();
            ViewBag.AlimDurumIdFilter = new SelectList(AlimDurumlar, "Id", "Adi", alimDurumIdForCokluSil);
            var TeklifDurumlar = _teklifDurumService.GetList();
            ViewBag.TeklifDurumIdFilter = new SelectList(TeklifDurumlar, "Id", "Adi", teklifDurumIdForCokluSil);
            return PartialView("TeklifimPartialView", teklifDetayAlimDetaylarViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecilenleriKapat(string kapatilacakTeklifler, string ExpandedForKapat, string pageForKapat,string teklifDurumIdForKapat, string alimDurumIdForKapat)
        {
            List<TeklifDetayAlimDetaylarViewModel> teklifDetayAlimDetaylarViewModel = new List<TeklifDetayAlimDetaylarViewModel>();
            List<int> teklifIdler = new List<int>();
            var uyariMesaji = "Seçim Yapmadınız!";

            if (kapatilacakTeklifler == null || kapatilacakTeklifler == ";" || kapatilacakTeklifler == "")
            {
                TempData["MessageSuccess"] = uyariMesaji;
                teklifDetayAlimDetaylarViewModel = getTeklifler(teklifDetayAlimDetaylarViewModel, pageForKapat, teklifDurumIdForKapat, alimDurumIdForKapat);

                return PartialView("TeklifimPartialView", teklifDetayAlimDetaylarViewModel);
                // return Json(uyariMesaji, JsonRequestBehavior.AllowGet);
            }

            Int32 basamak = kapatilacakTeklifler.IndexOf(';');
            Int32 toplam = kapatilacakTeklifler.Length;

            var teklifler = kapatilacakTeklifler.Substring(0, basamak);

            var liste = teklifler.Split(',');

            //teklifler update 
            if (liste[0].Length > 0)
            {
                foreach (string item in liste)
                {
                    var teklif = new Teklif();
                    teklif = _teklifService.GetById(Convert.ToInt32(item));
                    teklif.TeklifDurumId = 4;//kapandı
                    _teklifService.Update(teklif);
                    teklifIdler.Add(Convert.ToInt32(item));

                    //foreach (var teklifDetays in teklifDetayAlimDetaylarViewModel)
                    //{
                    //    if (teklifDetays.TeklifDetay.Id == Convert.ToInt32(item))
                    //    {
                    //        teklifDetays.TeklifDetay.Checked = true;
                    //        teklifDetays.TeklifDetay.TeklifDurumId = 4;
                    //        teklifDetays.TeklifDetay.TeklifDurumAdi = "Kapandı";

                    //    }
                    //}
                }

            }
            teklifDetayAlimDetaylarViewModel = getTeklifler(teklifDetayAlimDetaylarViewModel, pageForKapat, teklifDurumIdForKapat, alimDurumIdForKapat);
            TempData["MessageSuccess"] = "Secilen teklifler kapatılmıştır.";
            /////////////////////////////////
            //var teklifDetaylar = _teklifService.GetDetaylar(w => teklifIdler.Contains(w.Id));
            //var alimDetaylar = _alimService.GetDetaylar();

            //foreach (var item in teklifDetaylar)
            //{
            //    item.Checked = true;
            //    item.TeklifDurumId = 1;
            //    var alimDetays = alimDetaylar.Where(w => w.TeklifId == item.Id).ToList();
            //    teklifDetayAlimDetaylarViewModel.Add(new TeklifDetayAlimDetaylarViewModel
            //    {
            //        TeklifDetay = item,
            //        AlimDetaylar = alimDetays,
            //    });
            //}

            var liste3 = ExpandedForKapat.Split(',');
            if (liste3[0].Length > 0)
            {
                foreach (string item in liste3)
                {
                    foreach (var teklifDetays in teklifDetayAlimDetaylarViewModel)
                    {
                        if (teklifDetays.TeklifDetay.Id == Convert.ToInt32(item))
                        {
                            teklifDetays.TeklifDetay.Expanded = true;
                        }
                    }
                }
            }

            var AlimDurumlar = _alimDurumService.GetList();
            ViewBag.AlimDurumIdFilter = new SelectList(AlimDurumlar, "Id", "Adi", alimDurumIdForKapat);
            var TeklifDurumlar = _teklifDurumService.GetList();
            ViewBag.TeklifDurumIdFilter = new SelectList(TeklifDurumlar, "Id", "Adi", teklifDurumIdForKapat);
          
            return PartialView("TeklifimPartialView", teklifDetayAlimDetaylarViewModel);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetTeklifDurum(string TeklifId, string TeklifDurumId, string ExpandedForTeklifDurum, string page, string teklifDurumIdForTKD , string alimDurumIdForTKD)
        {
            int id = Convert.ToInt32(TeklifId);
            // int id = AlimId;


            Teklif teklif = new Teklif();
            teklif = _teklifService.GetById(id);
            try
            {
                teklif.TeklifDurumId = Convert.ToInt32(TeklifDurumId);
                _teklifService.Update(teklif);
                TempData["MessageSuccess"] = "Teklif durum başarıyla değiştirildi";
                // return PartialView("TalebimPartialView", talepdetayKatilimDetaylarViewModel);
            }
            catch (Exception ex)
            {
                TempData["MessageDanger"] = "ERROR: Teklif Durum değiştirilemedi. " + ex.InnerException.InnerException.Message.ToString();

            }
            List<TeklifDetayAlimDetaylarViewModel> teklifDetayAlimDetaylarViewModel = new List<TeklifDetayAlimDetaylarViewModel>();
            teklifDetayAlimDetaylarViewModel = getTeklifler(teklifDetayAlimDetaylarViewModel, page, teklifDurumIdForTKD, alimDurumIdForTKD);

            var liste3 = ExpandedForTeklifDurum.Split(',');
            if (liste3[0].Length > 0)
            {
                foreach (string item in liste3)
                {
                    foreach (var teklifDetays in teklifDetayAlimDetaylarViewModel)
                    {
                        if (teklifDetays.TeklifDetay.Id == Convert.ToInt32(item))
                        {
                            teklifDetays.TeklifDetay.Expanded = true;
                        }
                    }
                }
            }
            var AlimDurumlar = _alimDurumService.GetList();
            ViewBag.AlimDurumIdFilter = new SelectList(AlimDurumlar, "Id", "Adi", alimDurumIdForTKD);
            var TeklifDurumlar = _teklifDurumService.GetList();
            ViewBag.TeklifDurumIdFilter = new SelectList(TeklifDurumlar, "Id", "Adi", teklifDurumIdForTKD);
            return PartialView("TeklifimPartialView", teklifDetayAlimDetaylarViewModel);
        }
       
        private List<TeklifDetayAlimDetaylarViewModel> getTeklifler(List<TeklifDetayAlimDetaylarViewModel> 
            teklifDetayAlimDetaylarViewModel, string page, string teklifDurumId, string alimDurumId)
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var eczaneler = _eczaneService.GetListByUser(user);
            var gruplar = _grupService.GetListByUser(user);
            var eczaneGruplar = _eczaneGrupService.GetListByUser(user);
            var eczanegrupIdler = eczaneGruplar.Select(s => s.Id).ToList();
            var eczaneIdler = eczaneler.Select(s => s.Id).ToList();

            var teklifDetaylar = new List<TeklifDetay>();
            if(teklifDurumId != null  && teklifDurumId !="")
            {             
                teklifDetaylar = _teklifService.GetMyListByEczaneGrupId(eczanegrupIdler.FirstOrDefault())
                    .Where(w => w.TeklifDurumId == Convert.ToInt32(teklifDurumId))
                .OrderByDescending(o => o.BaslangicTarihi).ToList();
            }
            else
            {
                teklifDetaylar = _teklifService.GetMyListByEczaneGrupId(eczanegrupIdler.FirstOrDefault())
                .OrderByDescending(o => o.BaslangicTarihi).ToList();
            }

            var alimDetaylar = _alimService.GetDetaylar();
            if (alimDurumId != null && alimDurumId != "")
            {
                alimDetaylar = alimDetaylar.Where(w => w.AlimDurumId == Convert.ToInt32(alimDurumId)).ToList();
            }

            if (page == "" || page == null)
            {
                page = "1";
            }
            var AlimDurumlar = _alimDurumService.GetList();//.Where(w => w.AliciTarafiMi != true);
            ViewBag.AlimDurumIdFilter = new SelectList(AlimDurumlar, "Id", "Adi", alimDurumId);
            var TeklifDurumlar = _teklifDurumService.GetList();
            ViewBag.TeklifDurumIdFilter = new SelectList(TeklifDurumlar, "Id", "Adi", teklifDurumId);
            var pager = new Pager(teklifDetaylar.Count(), Convert.ToInt32(page));
            //teklifDetaylar = teklifDetaylar.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList();
            foreach (var item in teklifDetaylar)
            {
                var alimDetaylars = alimDetaylar.Where(w => w.TeklifId == item.Id).ToList();
              
                teklifDetayAlimDetaylarViewModel.Add(new TeklifDetayAlimDetaylarViewModel
                {
                    TeklifDetay = item,
                    AlimDetaylar = alimDetaylars,
                    Pager = pager
                });
            }
            return teklifDetayAlimDetaylarViewModel;
        }
        
        [HttpGet]
        //[ValidateAntiForgeryToken]
        public ActionResult GetTekliflerim(string page, string teklifDurumId, string alimDurumId)
        {
            List<TeklifDetayAlimDetaylarViewModel> teklifDetayAlimDetaylarViewModel = new List<TeklifDetayAlimDetaylarViewModel>();
            teklifDetayAlimDetaylarViewModel = getTeklifler(teklifDetayAlimDetaylarViewModel,page, teklifDurumId, alimDurumId);
            return PartialView("TeklifimPartialView", teklifDetayAlimDetaylarViewModel);
        }
        public ActionResult Search(string Keywords)
        {
            var result = _ilacService.GetList().Where(p => Regex.Split(Keywords, @"\s").Any(x => p.Adi.ToLower().Contains(x.ToLower()) || p.Adi.ToLower().Contains(x.ToLower()))).Select(s => s.Id).ToList();
            return View("Create", result);//result:model
        }
        // GET: TeklifNobet/Teklif/Details/5
        public ActionResult Details(int id)
        {
            if (id < 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeklifDetay Teklif = _teklifService.GetDetayById(id);
            if (Teklif == null)
            {
                return HttpNotFound();
            }
            return View(Teklif);
        }
        [HttpGet]
        //[Authorize(Roles = "Admin,Grup Yöneticisi, Eczane")]
        public ActionResult CreateViaApi(string id)
        {
            Teklif Teklif = new Teklif() { BaslangicTarihi = DateTime.Now };
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

            var teklifTurler = _teklifTurService.GetList();
            var teklifDurumlar = _teklifDurumService.GetList();
            var yayinlamaTurler = _yayinlamaTurService.GetList();
            var ilaclar = _ilacService.GetList();
            if (id != null)
                ilaclar = _ilacService.GetList()
                    .Where(p => Regex.Split(id, @"\s")
                    .Any(x => p.Adi.ToLower()
                    .Contains(x.ToLower()) || p.Adi.ToLower().Contains(x.ToLower()))).ToList();

            ViewBag.TeklifTurId = new SelectList(teklifTurler, "Id", "Adi");
            ViewBag.YayinlamaTurId = new SelectList(yayinlamaTurler, "Id", "Adi");
            ViewBag.TeklifDurumId = new SelectList(teklifDurumlar, "Id", "Adi");
            ViewBag.IlacId = new SelectList(ilaclar, "Id", "Adi");
            ViewBag.TeklifiVerenEczaneGrupId = new SelectList(eczaneGrupEczaneler, "EczaneGrupId", "GrupAdi", Teklif.TeklifiVerenEczaneGrupId);

            return View(Teklif);
        }
        [HttpGet]
        [Authorize(Roles = "Admin,Grup Yöneticisi, Eczane")]
        public ActionResult CreateFromTalep(int id)
        {
            Teklif Teklif = new Teklif() { BaslangicTarihi = DateTime.Now };
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

            var teklifTurler = _teklifTurService.GetList();
            var teklifDurumlar = _teklifDurumService.GetList();
            var yayinlamaTurler = _yayinlamaTurService.GetList();
            var ilaclar = _ilacService.GetList();
          

            ViewBag.TeklifTurId = new SelectList(teklifTurler, "Id", "Adi");
            ViewBag.YayinlamaTurId = new SelectList(yayinlamaTurler, "Id", "Adi");
            ViewBag.TeklifDurumId = new SelectList(teklifDurumlar, "Id", "Adi");
            ViewBag.IlacId = new SelectList(ilaclar, "Id", "Adi");
            ViewBag.TeklifiVerenEczaneGrupId = new SelectList(eczaneGrupEczaneler, "EczaneGrupId", "GrupAdi", Teklif.TeklifiVerenEczaneGrupId);
            ///////talepten gelenler///////
            Talep talep = new Talep();
            try
            {
                talep = _talepService.GetById(id);
                Teklif.AlimMiktari = talep.TalepMiktari;
                Teklif.BitisTarihi = talep.BitisTarihi;
                Teklif.DepoFiyat = talep.DepoFiyati;
                Teklif.IlacId = talep.IlacId;
                Teklif.Maksimum = talep.Maximum;
                Teklif.Minimum = talep.Minimum;
            }
            catch
            {

            }
            return View("Create", Teklif);
        }
        //GET: TeklifNobet/Teklif/Create
        [HttpGet]
        [Authorize(Roles = "Admin,Grup Yöneticisi, Eczane")]
        public ActionResult Create(string id)
        {
            Teklif Teklif = new Teklif() { BaslangicTarihi = DateTime.Now }; 
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

            var teklifTurler = _teklifTurService.GetList();
            var teklifDurumlar = _teklifDurumService.GetList();
            var yayinlamaTurler = _yayinlamaTurService.GetList();
            var ilaclar = _ilacService.GetList();
            if (id!= null)
                ilaclar =_ilacService.GetList()
                    .Where(p => Regex.Split(id, @"\s")
                    .Any(x => p.Adi.ToLower()
                    .Contains(x.ToLower()) || p.Adi.ToLower().Contains(x.ToLower()))).ToList();
            
            ViewBag.TeklifTurId = new SelectList(teklifTurler, "Id", "Adi");
            ViewBag.YayinlamaTurId = new SelectList(yayinlamaTurler, "Id", "Adi");
            ViewBag.TeklifDurumId = new SelectList(teklifDurumlar, "Id", "Adi");
            ViewBag.IlacId = new SelectList(ilaclar, "Id", "Adi");
            ViewBag.TeklifiVerenEczaneGrupId = new SelectList(eczaneGrupEczaneler, "EczaneGrupId", "GrupAdi", Teklif.TeklifiVerenEczaneGrupId);
            var gruplarimdakiTumEczaneler = _eczaneGrupService.GetDetayListByUser(user).ToList();
            var gruplarimdakiTumEczaneleri =  gruplarimdakiTumEczaneler.Select(m => new
            {
                OzelEczaneGrupId = m.Id,
                EczaneAdi = m.EczaneAdi,
            });
            
            ViewBag.OzelEczaneGrupId = new SelectList(gruplarimdakiTumEczaneleri, "OzelEczaneGrupId", "EczaneAdi");
           
            Talep talep = new Talep();
            try
            {
                talep = _talepService.GetById(Convert.ToInt32(id));
                Teklif.AlimMiktari = talep.TalepMiktari;
                Teklif.BitisTarihi = talep.BitisTarihi;
                Teklif.DepoFiyat = talep.DepoFiyati;
                Teklif.IlacId = talep.IlacId;
                Teklif.Maksimum = talep.Maximum;
                Teklif.Minimum = talep.Minimum;
            }
            catch
            {

            }
            return View(Teklif);
        }
        [HttpGet]
        //[Authorize(Roles = "Admin,Grup Yöneticisi, Eczane")]
        public JsonResult getEtiketFiyati(string id)
        {
      
            int Id = Convert.ToInt32(id);
            var data = _ilacService.GetById(Id).EtiketFiyati;
            return Json(data, JsonRequestBehavior.AllowGet); 

        }
        public ActionResult SearchIndex(string Keywords)
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var eczaneler = _eczaneService.GetListByUser(user);
            var gruplar = _grupService.GetListByUser(user);
            var eczaneGruplar = _eczaneGrupService.GetListByUser(user);
            var eczanegrupIdler = eczaneGruplar.Select(s => s.Id).ToList();
            var grupIdler = gruplar.Select(s => s.Id).ToList();



            var teklifler = _teklifService.GetListByEczaneGruplar(eczanegrupIdler, grupIdler);
            var ilaclar = _ilacService.GetList().Where(w => teklifler.Select(s => s.IlacId).Contains(w.Id)).ToList();

            var result = teklifler.ToList()
                .Where(p => Regex.Split(Keywords, @"\s")
                .Any(x => p.IlacAdi.ToLower().Contains(x.ToLower()) || p.IlacAdi.ToLower().Contains(x.ToLower()))).ToList();
            //bu eczanenin bulunduğu tüm eczanegrup lardaki teklifleri gösterir
            var model = new TeklifDetayViewModel()
            {
                TeklifDetaylar = result,
                Eczaneler = eczaneler,
                Ilaclar = ilaclar,
                EczaneGruplar = eczaneGruplar
            };

            return View("Index", model);//result:model
        }
        // POST: TeklifNobet/Teklif/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IlacId,TeklifiVerenEczaneGrupId,HedeflenenAlim,IlacMiad,MalFazlasi,NetFiyat,EtiketFiyati,DepoFiyat,AlimMiktari,BaslangicTarihi,BitisTarihi,Maksimum,Minimum,TeklifTurId,YayinlamaTurId,TeklifDurumId,OzelEczaneGrupId")] Teklif Teklif)
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            //var eczaneId = _eczaneUserService.GetListByUserId(user.Id).Select(s=>s.EczaneId).FirstOrDefault();
           // Teklif.NetFiyat =(Math.Round(Teklif.NetFiyat, 4));
            Teklif.KayitTarihi = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    _teklifService.Insert(Teklif);
                    TempData["MessageSuccess"] = "Teklif başarıyla oluşturulmuştur";
                    return RedirectToAction("Index", "Teklifim");
                }
                catch (Exception ex)
                {                  
                    TempData["Message"] = "ERROR:" + ex.InnerException.InnerException.Message.ToString();
               
                }
            }

            var teklifTurler = _teklifTurService.GetList();
            var teklifDurumlar = _teklifDurumService.GetList();
            var yayinlamaTurler = _yayinlamaTurService.GetList();
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


            ViewBag.TeklifTurId = new SelectList(teklifTurler, "Id", "Adi", Teklif.TeklifTurId);
            ViewBag.YayinlamaTurId = new SelectList(yayinlamaTurler, "Id", "Adi", Teklif.YayinlamaTurId);
            ViewBag.TeklifDurumId = new SelectList(teklifDurumlar, "Id", "Adi", Teklif.TeklifDurumId);
            ViewBag.IlacId = new SelectList(ilaclar, "Id", "Adi", Teklif.IlacId);
            ViewBag.TeklifiVerenEczaneGrupId = new SelectList(eczaneGrupEczaneler, "EczaneGrupId", "GrupAdi", Teklif.TeklifiVerenEczaneGrupId);
            var gruplarimdakiTumEczaneler = _eczaneGrupService.GetDetayListByUser(user).ToList();
            var gruplarimdakiTumEczaneleri = gruplarimdakiTumEczaneler.Select(m => new
            {
                OzelEczaneGrupId = m.Id,
                EczaneAdi = m.EczaneAdi,
            });
            ViewBag.OzelEczaneGrupId = new SelectList(gruplarimdakiTumEczaneleri, "OzelEczaneGrupId", "EczaneAdi");

            return View(Teklif);
        }

        // GET: TeklifNobet/Teklif/Edit/5
        [Authorize(Roles = "Admin,Grup Yöneticisi,Eczane")]
        public ActionResult Edit(int? id)
        {
            int Id = 0;
            if(id == null)
                return RedirectToAction("Index", "Teklifim");
  
            Id = Convert.ToInt32(id);
            if (id < 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teklif Teklif = _teklifService.GetById(Id);
            var user = _userService.GetByUserName(User.Identity.Name);

            var teklifTurler = _teklifTurService.GetList();
            var teklifDurumlar = _teklifDurumService.GetList();
            var yayinlamaTurler = _yayinlamaTurService.GetList();
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


            ViewBag.TeklifTurId = new SelectList(teklifTurler, "Id", "Adi", Teklif.TeklifTurId);
            ViewBag.YayinlamaTurId = new SelectList(yayinlamaTurler, "Id", "Adi", Teklif.YayinlamaTurId);
            ViewBag.TeklifDurumId = new SelectList(teklifDurumlar, "Id", "Adi", Teklif.TeklifDurumId );
            ViewBag.IlacId = new SelectList(ilaclar, "Id", "Adi", Teklif.IlacId);
            ViewBag.TeklifiVerenEczaneGrupId = new SelectList(eczaneGrupEczaneler, "EczaneGrupId", "GrupAdi", Teklif.TeklifiVerenEczaneGrupId);
             

            if (Teklif == null)
            {
                return HttpNotFound();
            }
            return View(Teklif);
        }

        // POST: TeklifNobet/Teklif/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IlacId,HedeflenenAlim,IlacMiad,MalFazlasi,NetFiyat,EtiketFiyati,DepoFiyat,TeklifiVerenEczaneGrupId,KayitTarihi,AlimMiktari,BaslangicTarihi,BitisTarihi,Maksimum,Minimum,TeklifTurId,YayinlamaTurId,TeklifDurumId")] Teklif Teklif)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _teklifService.Update(Teklif);
                    TempData["MessageSuccess"] = "Teklif başarıyla düzenlenmiştir";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                     TempData["Message"] = "ERROR:" + ex.InnerException.InnerException.Message.ToString();     
                }
            }
            
            var user = _userService.GetByUserName(User.Identity.Name);

            var teklifTurler = _teklifTurService.GetList();
            var teklifDurumlar = _teklifDurumService.GetList();
            var yayinlamaTurler = _yayinlamaTurService.GetList();
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

            ViewBag.TeklifTurId = new SelectList(teklifTurler, "Id", "Adi", Teklif.TeklifTurId);
            ViewBag.YayinlamaTurId = new SelectList(yayinlamaTurler, "Id", "Adi", Teklif.YayinlamaTurId);
            ViewBag.TeklifDurumId = new SelectList(teklifDurumlar, "Id", "Adi", Teklif.TeklifDurumId);
            ViewBag.IlacId = new SelectList(ilaclar, "Id", "Adi", Teklif.IlacId);
            ViewBag.TeklifiVerenEczaneGrupId = new SelectList(eczaneGrupEczaneler, "EczaneGrupId", "GrupAdi", Teklif.TeklifiVerenEczaneGrupId);

            return View(Teklif);
        }

        // GET: TeklifNobet/Teklif/Delete/5
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
            Teklif Teklif = _teklifService.GetById(Id);
            ViewBag.IlacAdi = _ilacService.GetById(Teklif.IlacId).Adi;
            if (Teklif == null)
            {
                return HttpNotFound();
            }
            return View(Teklif);
        }

        // POST: TeklifNobet/Teklif/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Teklif Teklif = _teklifService.GetById(id);
            try
            {
               TempData["MessageSuccess"] = "Teklif başarıyla silinmiştir";
                _teklifService.Delete(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "ERROR:" + ex.InnerException.InnerException.Message.ToString();
            }
            return View(Teklif);

        }

        public JsonResult getBuTekliftenYapilanToplamAlimMiktari(int Id)
        {
            var teklif = _teklifService.GetById(Id).Id;
            var alim = _alimService.GetListByTeklifId(teklif);
            var buTekliftenYapilanToplamAlimMiktari = alim.Sum(s=>s.Miktar);
            var toplamTeklifMiktari = _teklifService.GetDetayById(Id).ToplamTeklifMiktari;
            var kalan = Convert.ToInt32(toplamTeklifMiktari) - Convert.ToInt32(buTekliftenYapilanToplamAlimMiktari);
            var alimDetaylar = _alimService.GetListByTeklifId(Id);
            
            var model = (new
            {
                BuTekliftenYapilanToplamAlimMiktari = buTekliftenYapilanToplamAlimMiktari,
                ToplamTeklifMiktari = toplamTeklifMiktari,
                Kalan = kalan,
                Alimlar = alimDetaylar
            });
            var jsonResult = Json(model, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }
     
    }
}