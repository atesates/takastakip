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
    public class GonderdiklerimEczanelereGoreController : Controller
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
        private ITeklifDurumService _teklifDurumService;
        private ITeklifService _teklifService;
        private ITeklifTurService _teklifTurService;
        private IITStransferDurumService _iTStransferDurumService;

        public GonderdiklerimEczanelereGoreController(IEczaneGrupService eczaneGrupService, 
                                IAlimService alimService,
                                IEczaneUserService eczaneUserService,
                                IIlacService ilacService,
                                ITeklifTurService teklifTurService,
                                IEczaneService eczaneService,
                                IGrupService grupService,
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
            _teklifTurService = teklifTurService;
            _eczaneUserService = eczaneUserService;
            _teklifService = teklifService;
            _teklifDurumService = teklifDurumService;
            _iTStransferDurumService = iTStransferDurumService;
            _alimDurumService = alimDurumService;
            _eczaneGrupService = eczaneGrupService;
        }
        #endregion

        public ActionResult Index(int? page)
        {
            int thispage = 0;
            if (page != null)
                thispage = Convert.ToInt32(page);
            else thispage = 1;
            var alimDurumlar = _alimDurumService.GetList();
            ViewBag.alimTurId = new SelectList(alimDurumlar, "Id", "Adi");

            var ITStransferDurumlar = _iTStransferDurumService.GetList();
            ViewBag.ITStransferDurumId = new SelectList(ITStransferDurumlar, "Id", "Adi");
            var AlimDurumlar = _alimDurumService.GetList().Where(w => w.AliciTarafiMi != true);
            ViewBag.AlimDurumId = new SelectList(AlimDurumlar, "Id", "Adi");

            // var temp = TempData["EczaneGrupId"];        
            var model = new EczaneGrupDetayViewModel();
            model = getAEczaneGrupDetayViewModel(thispage);
            return View(model);
        }

        private EczaneGrupDetayViewModel getAEczaneGrupDetayViewModel(int thispage)
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var gruplar = _grupService.GetListByUser(user).ToList();
            var eczaneGrupIdler = _eczaneGrupService.GetDetayListByUser(user).Select(s=>s.Id).ToList();
            var eczaneIdler = _eczaneGrupService.GetDetayListByUser(user).Select(s => s.EczaneId).ToList();
            var ayniGruptakiEczaneIdler = _eczaneGrupService.GetDetaylar(w=> eczaneGrupIdler.Contains(w.Id)).Select(s=>s.EczaneId).ToList();
            //kendi yaptığım alımlar:
            var eczaneGruplar = _eczaneGrupService.GetMyDetayListByUser(user);
            //var eczaneler = _eczaneService.GetListByUser(user);
            var eczaneler = _eczaneService.GetList().Where(w => ayniGruptakiEczaneIdler.Contains(w.Id)).ToList();
            var teklifler = _teklifService.GetListByUser(user);//tekliflerim 
            var teklifIdler = _teklifService.GetListByUser(user).Select(s => s.Id).ToList();//teklifIdlerim 

            var tekliflerimdenYapilanAlimlar = _alimService.GetDetayListByTeklifler(teklifIdler).GroupBy(g=>g.EczaneGrupId);
           
            var ilacIdler = teklifler.Where(w => teklifIdler.Contains(w.Id)).Select(s => s.IlacId);
            var ilaclar = _ilacService.GetList().Where(w => ilacIdler.Contains(w.Id)).ToList();

            var pager = new Pager(tekliflerimdenYapilanAlimlar.Count(), thispage);

            var model = new EczaneGrupDetayViewModel()
            {
                Eczaneler = eczaneler,
                EczaneGrupDetaylar = eczaneGruplar,
                Pager = pager
            };
            return model;
        }
        // GET: AlimNobet/Alim/Details/5
        public ActionResult Details(int? id)
        {
            var alimDurumlar = _alimDurumService.GetList();
            ViewBag.alimTurId = new SelectList(alimDurumlar, "Id", "Adi");
            // var temp = TempData["EczaneGrupId"];
            int Id = 0;
            if (id != null)
                Id = Convert.ToInt32(id);

            var user = _userService.GetByUserName(User.Identity.Name);
            var eczaneler = _eczaneService.GetListByUser(user);

            var gruplar = _grupService.GetListByUser(user);
            var eczaneGruplar = _eczaneGrupService.GetMyDetayListByUser(user);

            var eczaneGrupIdler = eczaneGruplar.Select(s => s.Id);
            var teklifIdler = _teklifService.GetList()
                .Where(w => eczaneGrupIdler.Contains(w.TeklifiVerenEczaneGrupId))
                .Select(s => s.Id).ToList();
            var alimlar = _alimService.GetDetayListByTeklifler(teklifIdler);
            if (id != null)
                alimlar = alimlar.Where(w => w.TeklifId == Id).ToList();

            var teklifler = _teklifService.GetMyDetayListByEczaneGruplar(eczaneGruplar);
            if (id != null)
                teklifler = teklifler.Where(w => w.Id == Id).ToList();
            var ilacIdler = teklifler.Where(w => teklifIdler.Contains(w.Id)).Select(s => s.IlacId);
            var ilaclar = _ilacService.GetList().Where(w => ilacIdler.Contains(w.Id)).ToList();

            var model = new AlimDetayViewModel()
            {

                AlimDetaylar = alimlar,
               // Eczaneler = eczaneler,
                Ilaclar = ilaclar,
                EczaneGrupDetaylar = eczaneGruplar,
                TeklifDetaylar = teklifler,
            };

            var eczaneninYaptigiTeklifIdler = _teklifService.GetList()
                  .Where(w => eczaneGrupIdler.Contains(w.TeklifiVerenEczaneGrupId))
                  .Select(s => s.Id).ToList();
            var eczaneninYaptigiTekliflerdenAlimYapilanTeklifIdler =
                _alimService.GetListByTeklifler(eczaneninYaptigiTeklifIdler)
                .Select(s => s.TeklifId).ToList();

            ViewBag.AlimYapilmisTeklifIdler = new SelectList(eczaneninYaptigiTekliflerdenAlimYapilanTeklifIdler, "Id");

            return View(model);
        }
        public ActionResult SearchIndex(string Keywords)
        {
            var alimDurumlar = _alimDurumService.GetList();
            ViewBag.alimTurId = new SelectList(alimDurumlar, "Id", "Adi");
            // var temp = TempData["EczaneGrupId"];
            
            var user = _userService.GetByUserName(User.Identity.Name);
            var eczaneler = _eczaneService.GetListByUser(user);

            var gruplar = _grupService.GetListByUser(user);
            var eczaneGruplar = _eczaneGrupService.GetMyDetayListByUser(user);

            var eczaneGrupIdler = eczaneGruplar.Select(s => s.Id);
            var teklifIdler = _teklifService.GetList()
                .Where(w => eczaneGrupIdler.Contains(w.TeklifiVerenEczaneGrupId))
                .Select(s => s.Id).ToList();
            var alimlar = _alimService.GetDetayListByTeklifler(teklifIdler).Where(p => Regex.Split(Keywords, @"\s")
            .Any(x => p.EczaneAdi.ToLower().Contains(x.ToLower()) || p.EczaneAdi.ToLower().Contains(x.ToLower()))).ToList();
            ; 
          

            var teklifler = _teklifService.GetMyDetayListByEczaneGruplar(eczaneGruplar);
           
            var ilacIdler = teklifler.Where(w => teklifIdler.Contains(w.Id)).Select(s => s.IlacId);
            var ilaclar = _ilacService.GetList().Where(w => ilacIdler.Contains(w.Id)).ToList();

            var model = new AlimDetayViewModel()
            {

                AlimDetaylar = alimlar,
                //Eczaneler = eczaneler,
                Ilaclar = ilaclar,
                EczaneGrupDetaylar = eczaneGruplar,
                TeklifDetaylar = teklifler,
            };

            var eczaneninYaptigiTeklifIdler = _teklifService.GetList()
                  .Where(w => eczaneGrupIdler.Contains(w.TeklifiVerenEczaneGrupId))
                  .Select(s => s.Id).ToList();
            var eczaneninYaptigiTekliflerdenAlimYapilanTeklifIdler =
                _alimService.GetListByTeklifler(eczaneninYaptigiTeklifIdler)
                .Select(s => s.TeklifId).ToList();

            ViewBag.AlimYapilmisTeklifIdler = new SelectList(eczaneninYaptigiTekliflerdenAlimYapilanTeklifIdler, "Id");

            return View("Index", model);//result:model



        }
        // GET: AlimNobet/Alim/Create
        [Authorize(Roles = "Admin,Grup Yöneticisi, Eczane")]
        public ActionResult Create(int? id)
        {
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

            Alim.TeklifId = teklifId;
            
            try
            {
                Alim.EczaneGrupId = Convert.ToInt32(Session["EczanegrupId"]);
            }
            catch
            {
                return RedirectToAction("Index", "EczaneGrup");
            }

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
        public ActionResult GetDetay(int? detayiGosterilecekAlimId)
        {
            int? id = detayiGosterilecekAlimId;
            var alimDurumlar = _alimDurumService.GetList();
            ViewBag.alimTurId = new SelectList(alimDurumlar, "Id", "Adi");
            // var temp = TempData["EczaneGrupId"];
            int Id = 0;
            if (id != null)
                Id = Convert.ToInt32(id);

            var user = _userService.GetByUserName(User.Identity.Name);
            var eczaneler = _eczaneService.GetListByUser(user);

            var gruplar = _grupService.GetListByUser(user);
            var eczaneGruplar = _eczaneGrupService.GetMyDetayListByUser(user);

            var eczaneGrupIdler = eczaneGruplar.Select(s => s.Id);

            var teklifIdler = _teklifService.GetList()
                .Where(w => eczaneGrupIdler.Contains(w.TeklifiVerenEczaneGrupId))
                .Select(s => s.Id).ToList();

            var gelenTeklifId = _alimService.GetDetayById(Id).TeklifId;
            var gelenEczaneGrupId = _alimService.GetDetayById(Id).EczaneGrupId;


            var alimlar = _alimService.GetDetayListByTeklifler(teklifIdler);
            if (id != null)
                alimlar = alimlar.Where(w => w.TeklifId == gelenTeklifId
                && w.EczaneGrupId == gelenEczaneGrupId).ToList();

            var teklifler = _teklifService.GetMyDetayListByEczaneGruplar(eczaneGruplar);
            if (id != null)
                teklifler = teklifler.Where(w => w.Id == gelenTeklifId).ToList();
            var ilacIdler = teklifler.Where(w => teklifIdler.Contains(w.Id)).Select(s => s.IlacId);
            var ilaclar = _ilacService.GetList().Where(w => ilacIdler.Contains(w.Id)).ToList();

            var model = new AlimDetayViewModel()
            {

                AlimDetaylar = alimlar,
                //Eczaneler = eczaneler,
                Ilaclar = ilaclar,
                EczaneGrupDetaylar = eczaneGruplar,
                TeklifDetaylar = teklifler,
            };

            var eczaneninYaptigiTeklifIdler = _teklifService.GetList()
                  .Where(w => eczaneGrupIdler.Contains(w.TeklifiVerenEczaneGrupId))
                  .Select(s => s.Id).ToList();
            var eczaneninYaptigiTekliflerdenAlimYapilanTeklifIdler =
                _alimService.GetListByTeklifler(eczaneninYaptigiTeklifIdler)
                .Select(s => s.TeklifId).ToList();

            ViewBag.AlimYapilmisTeklifIdler = new SelectList(eczaneninYaptigiTekliflerdenAlimYapilanTeklifIdler, "Id");

            return PartialView("GonderdiklerimPartialView", model);

        }
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
                    TempData["MessageSuccess"] = "Gönderim başarıyla oluşturulmuştur";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Message"] = "ERROR:" + ex.Message.ToString();
                }
            }
            var alimDurumlar = _alimDurumService.GetList();
            var iTStransferDurumlar = _iTStransferDurumService.GetList();
            ViewBag.AlimDurumId = new SelectList(alimDurumlar, "Id", "Adi");
            ViewBag.ITStransferDurumId = new SelectList(iTStransferDurumlar, "Id", "Adi");

            return View(Alim);
        }
        // GET: AlimNobet/Alim/Edit/5
        [Authorize(Roles = "Admin,Grup Yöneticisi,Eczane")]
        public ActionResult Edit(int? id)
        {
            int Id = 0;
            if (id == null)
                return RedirectToAction("Index", "Teklif");

            Id = Convert.ToInt32(id);
            if (id < 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Alim Alim = _alimService.GetById(Id);
            var ITStransferDurumlar = _iTStransferDurumService.GetList();
            ViewBag.ITStransferDurumId = new SelectList(ITStransferDurumlar, "Id", "Adi", Alim.ITStransferDurumId);
            var teklifId = Alim.TeklifId;
            ViewBag.TeklifId = teklifId;
            ViewBag.AlimId = Alim.Id;
            var alimDurumlar = _alimDurumService.GetList().Where(w => w.Id == 2 || w.Id == 3); 
            ViewBag.AlimDurumId = new SelectList(alimDurumlar, "Id", "Adi", Alim.AlimDurumId);

            var user = _userService.GetByUserName(User.Identity.Name);

            var teklifVerenEczaneGrupId = _teklifService.GetDetayById(teklifId).TeklifiVerenEczaneGrupId;
            var grupId = _eczaneGrupService.GetDetayById(teklifVerenEczaneGrupId).GrupId;
            var eczaneGruplar = _eczaneGrupService.GetListByUser(user);

            Alim.EczaneGrupId = Alim.EczaneGrupId;


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
                if(Alim.AlimDurumId == 2 || Alim.AlimDurumId == 3)
                    Alim.GonderimTarihi = DateTime.Now;
                _alimService.Update(Alim);
                int? Id = Alim.TeklifId;
                TempData["MessageSuccess"] = "Gönderim başarıyla düzenlenmiştir";
                return RedirectToAction("Details", new { id = Id });
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
        [HttpGet]
        //[ValidateAntiForgeryToken]
        public ActionResult getGonderimlerim(string page, string teklifDurumId, string alimDurumId)
        {
            List<EczaneGrupDetayAlimDetaylarViewModel> eczaneGrupDetayAlimDetaylarViewModel = new List<EczaneGrupDetayAlimDetaylarViewModel>();
            eczaneGrupDetayAlimDetaylarViewModel = getGonderimlerim_EczaneGrupdetay(eczaneGrupDetayAlimDetaylarViewModel, page,  teklifDurumId,  alimDurumId);
            return PartialView("GonderdiklerimEczanelereGorePartialView", eczaneGrupDetayAlimDetaylarViewModel);
        }
        // POST: AlimNobet/Alim/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Alim Alim = _alimService.GetById(id);
            _alimService.Delete(id);
            TempData["MessageSuccess"] = "Gönderim başarıyla silinmiştir";
            return RedirectToAction("Index");
        }
        private List<EczaneGrupDetayAlimDetaylarViewModel> getGonderimlerim_EczaneGrupdetay(List<EczaneGrupDetayAlimDetaylarViewModel>
            eczaneGrupDetayAlimDetaylarViewModel, string page, string teklifDurumId, string alimDurumId)
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var userId = _userService.GetByUserName(User.Identity.Name).Id;
            var eczaneGrupIdlerim = _eczaneGrupService.GetListByUser(user).Select(s => s.EczaneId).ToList();
            var grupIdler = _grupService.GetListByUser(user).Select(s=>s.Id);
            var bulundugumGruplardakiEczaneGrupIdler = _eczaneGrupService.GetList().Where(w => grupIdler.Contains(w.GrupId))
                .Select(s => s.Id).ToList();

            var verdigimTeklifIdler = new List<int>();
            if (teklifDurumId != null && teklifDurumId != "")
            {
                verdigimTeklifIdler = _teklifService.GetDetaylar(w => eczaneGrupIdlerim.Contains(w.TeklifiVerenEczaneGrupId))
                    .Where(w=>w.TeklifDurumId == Convert.ToInt32(teklifDurumId))
                    .Select(s => s.Id).ToList();
            }
            else
            {
                verdigimTeklifIdler = _teklifService.GetDetaylar(w => eczaneGrupIdlerim.Contains(w.TeklifiVerenEczaneGrupId))
                     .Select(s => s.Id).ToList();
            }

            var alimDetaylar = _alimService.GetDetaylar();
            if (alimDurumId != null && alimDurumId != "")
            {
                 alimDetaylar = _alimService.GetDetaylar(w => verdigimTeklifIdler.Contains(w.TeklifId) && w.AlimDurumId != 1)
                   .Where(w=>w.AlimDurumId == Convert.ToInt32(alimDurumId))
                    .OrderByDescending(o => o.BaslangicTarihi).ToList();
            }
            else
            {
                alimDetaylar = _alimService.GetDetaylar(w => verdigimTeklifIdler.Contains(w.TeklifId) && w.AlimDurumId != 1)
                .OrderByDescending(o => o.BaslangicTarihi).ToList();
            }

            var bendenAlimYapanEczaneGrupIdler = alimDetaylar.Select(s => s.EczaneGrupId).ToList();
            var eczaneGrupDetaylar = _eczaneGrupService.GetDetaylar(w=> bulundugumGruplardakiEczaneGrupIdler.Contains(w.Id))
                .Distinct().ToList();
            var bendenAlimYapanEczaneGruplar = eczaneGrupDetaylar.Where(w => bendenAlimYapanEczaneGrupIdler.Contains(w.Id))
                .OrderBy(o => o.Adi).ToList();

            if (page == "" || page == null)
            {
                page = "1";
            }
            var pager = new Pager(bendenAlimYapanEczaneGruplar.Count(), Convert.ToInt32(page));
            //eczaneGrupDetaylar = bendenAlimYapanEczaneGruplar.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList();
            foreach (var item in bendenAlimYapanEczaneGruplar)
            {
                var alimDetaylars = alimDetaylar.Where(w => w.EczaneGrupId == item.Id).ToList();
                foreach (var item2 in alimDetaylars)
                {
                    if (item.Id == item2.EczaneGrupId)
                    {
                        
                        if (!eczaneGrupDetayAlimDetaylarViewModel.Any(s => s.EczaneGrupDetay.Id == item2.EczaneGrupId))
                        {//distinct olması için
                            eczaneGrupDetayAlimDetaylarViewModel.Add(new EczaneGrupDetayAlimDetaylarViewModel
                            {
                                AlimDetaylar = alimDetaylars,
                                EczaneGrupDetay = item,
                                Pager = pager
                            });
                        }
                    }
                }
              
            }
            var AlimDurumlar = _alimDurumService.GetList();//.Where(w => w.AliciTarafiMi != true);
            ViewBag.AlimDurumIdFilter = new SelectList(AlimDurumlar, "Id", "Adi", alimDurumId);
            var TeklifDurumlar = _teklifDurumService.GetList();
            ViewBag.TeklifDurumIdFilter = new SelectList(TeklifDurumlar, "Id", "Adi", teklifDurumId);
            return eczaneGrupDetayAlimDetaylarViewModel;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecilenleriPasifYap(string pasifYapilacakTeklifler, string ExpandedForPasif, string pageForPasif, string teklifDurumIdForPasif, string alimDurumIdForPasif)
        {
            List<EczaneGrupDetayAlimDetaylarViewModel> eczaneGrupDetayAlimDetaylarViewModel = new List<EczaneGrupDetayAlimDetaylarViewModel>();
            List<int> teklifIdler = new List<int>();
            var uyariMesaji = "Seçim Yapmadınız!";

            if (pasifYapilacakTeklifler == null || pasifYapilacakTeklifler == ";" || pasifYapilacakTeklifler == "")
            {
                TempData["MessageSuccess"] = uyariMesaji;
                eczaneGrupDetayAlimDetaylarViewModel = getGonderimlerim_EczaneGrupdetay(eczaneGrupDetayAlimDetaylarViewModel, pageForPasif, teklifDurumIdForPasif, alimDurumIdForPasif);

                return PartialView("GonderdiklerimEczanelereGorePartialView", eczaneGrupDetayAlimDetaylarViewModel);
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

                    ////////henüz eczaneGrupDetayAlimDetaylarViewModel içinde değer yok//////

                    //foreach (var eczaneGrupdetays in eczaneGrupDetayAlimDetaylarViewModel)
                    //{
                    //    if (eczaneGrupdetays.EczaneGrupDetay.Id == Convert.ToInt32(item))
                    //    {
                    //        eczaneGrupdetays.EczaneGrupDetay.Checked = true;
                    //    }
                    //}
                }
            }
            eczaneGrupDetayAlimDetaylarViewModel = getGonderimlerim_EczaneGrupdetay(eczaneGrupDetayAlimDetaylarViewModel, pageForPasif, teklifDurumIdForPasif, alimDurumIdForPasif);

            TempData["MessageSuccess"] = "Secilen tekliflerin durumları pasif yapılmıştır.";

            /////////////////////////////////

            //var teklifDetaylar = _teklifService.GetDetaylar(w => teklifIdler.Contains(w.Id));          
            //var alimDetaylar = _alimService.GetDetaylar();

            //foreach (var item in teklifDetaylar)
            //{
            //    item.Checked = true;
            //    item.TeklifDurumId = 2;
            //    var alimDetays = alimDetaylar.Where(w => w.TeklifId == item.Id).ToList();
            //    eczaneGrupDetayAlimDetaylarViewModel.Add(new EczaneGrupDetayAlimDetaylarViewModel
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
                    foreach (var eczaneGrupdetays in eczaneGrupDetayAlimDetaylarViewModel)
                    {
                        if (eczaneGrupdetays.EczaneGrupDetay.Id == Convert.ToInt32(item))
                        {
                            eczaneGrupdetays.EczaneGrupDetay.Expanded = true;
                        }
                    }
                }
            }



            return PartialView("GonderdiklerimEczanelereGorePartialView", eczaneGrupDetayAlimDetaylarViewModel);
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
                //return PartialView("GonderdiklerimEczanelereGorePartialView", eczaneGrupDetayAlimDetaylarViewModel);
            }
            catch (Exception ex)
            {
                TempData["MessageDanger"] = "ERROR: ITS transfer durum değiştirilemedi " + ex.InnerException.InnerException.Message.ToString();

            }
            List<EczaneGrupDetayAlimDetaylarViewModel> eczaneGrupDetayAlimDetaylarViewModel = new List<EczaneGrupDetayAlimDetaylarViewModel>();
            eczaneGrupDetayAlimDetaylarViewModel = getGonderimlerim_EczaneGrupdetay(eczaneGrupDetayAlimDetaylarViewModel, page,  teklifDurumIdForITS,  alimDurumIdForITS);
            var liste3 = ExpandedForITS.Split(',');
            if (liste3[0].Length > 0)
            {
                foreach (string item in liste3)
                {
                    foreach (var eczaneGrupdetays in eczaneGrupDetayAlimDetaylarViewModel)
                    {
                        if (eczaneGrupdetays.EczaneGrupDetay.Id == Convert.ToInt32(item))
                        {
                            eczaneGrupdetays.EczaneGrupDetay.Expanded = true;
                        }
                    }
                }
            }
            return PartialView("GonderdiklerimEczanelereGorePartialView", eczaneGrupDetayAlimDetaylarViewModel);
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
                // return PartialView("GonderdiklerimEczanelereGorePartialView", eczaneGrupDetayAlimDetaylarViewModel);
            }
            catch (Exception ex)
            {
                TempData["MessageDanger"] = "ERROR: Alım Durum değiştirilemedi. " + ex.InnerException.InnerException.Message.ToString();

            }
            List<EczaneGrupDetayAlimDetaylarViewModel> eczaneGrupDetayAlimDetaylarViewModel = new List<EczaneGrupDetayAlimDetaylarViewModel>();
            eczaneGrupDetayAlimDetaylarViewModel = getGonderimlerim_EczaneGrupdetay(eczaneGrupDetayAlimDetaylarViewModel, page,  teklifDurumIdForALD,  alimDurumIdForALD);

            var liste3 = ExpandedForALD.Split(',');
            if (liste3[0].Length > 0)
            {
                foreach (string item in liste3)
                {
                    foreach (var eczaneGrupdetays in eczaneGrupDetayAlimDetaylarViewModel)
                    {
                        if (eczaneGrupdetays.EczaneGrupDetay.Id == Convert.ToInt32(item))
                        {
                            eczaneGrupdetays.EczaneGrupDetay.Expanded = true;
                        }
                    }
                }
            }
            return PartialView("GonderdiklerimEczanelereGorePartialView", eczaneGrupDetayAlimDetaylarViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecileniSil(int Id, string pageForSil, string teklifDurumIdForSil, string alimDurumIdForSil)
        {
            int id = Convert.ToInt32(Id);
            // int id = AlimId;
            List<EczaneGrupDetayAlimDetaylarViewModel> eczaneGrupDetayAlimDetaylarViewModel = new List<EczaneGrupDetayAlimDetaylarViewModel>();

            try
            {
                _teklifService.Delete(id);
                TempData["MessageSuccess"] = "Teklif başarıyla silinmiştir";
                eczaneGrupDetayAlimDetaylarViewModel = getGonderimlerim_EczaneGrupdetay(eczaneGrupDetayAlimDetaylarViewModel, pageForSil,  teklifDurumIdForSil,  alimDurumIdForSil);

                return PartialView("GonderdiklerimEczanelereGorePartialView", eczaneGrupDetayAlimDetaylarViewModel);
            }
            catch (Exception ex)
            {
                TempData["MessageDanger"] = "ERROR: Teklif silinemedi. " + ex.InnerException.InnerException.Message.ToString();

            }
            eczaneGrupDetayAlimDetaylarViewModel = getGonderimlerim_EczaneGrupdetay(eczaneGrupDetayAlimDetaylarViewModel, pageForSil, teklifDurumIdForSil, alimDurumIdForSil);

            return PartialView("GonderdiklerimEczanelereGorePartialView", eczaneGrupDetayAlimDetaylarViewModel);


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecilenleriAktifYap(string aktifYapilacakTeklifler, string ExpandedForAktif, string pageForAktif, string teklifDurumIdForAktif, string alimDurumIdForAktif)
        {
            List<EczaneGrupDetayAlimDetaylarViewModel> eczaneGrupDetayAlimDetaylarViewModel = new List<EczaneGrupDetayAlimDetaylarViewModel>();
            List<int> teklifIdler = new List<int>();
            var uyariMesaji = "Seçim Yapmadınız!";

            if (aktifYapilacakTeklifler == null || aktifYapilacakTeklifler == ";" || aktifYapilacakTeklifler == "")
            {
                //return Json(uyariMesaji, JsonRequestBehavior.AllowGet);
                TempData["MessageSuccess"] = uyariMesaji;
                eczaneGrupDetayAlimDetaylarViewModel = getGonderimlerim_EczaneGrupdetay(eczaneGrupDetayAlimDetaylarViewModel, pageForAktif, teklifDurumIdForAktif, alimDurumIdForAktif);

                return PartialView("GonderdiklerimEczanelereGorePartialView", eczaneGrupDetayAlimDetaylarViewModel);

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

                    //foreach (var eczaneGrupdetays in eczaneGrupDetayAlimDetaylarViewModel)
                    //{
                    //    if (eczaneGrupdetays.EczaneGrupDetay.Id == Convert.ToInt32(item))
                    //    {
                    //        eczaneGrupdetays.EczaneGrupDetay.Checked = true;

                    //    }
                    //}
                }
            }
            eczaneGrupDetayAlimDetaylarViewModel = getGonderimlerim_EczaneGrupdetay(eczaneGrupDetayAlimDetaylarViewModel, pageForAktif, teklifDurumIdForAktif, alimDurumIdForAktif);

            TempData["MessageSuccess"] = "Secilen tekliflerin durumları aktif yapılmıştır.";
            /////////////////////////////////
            //var teklifDetaylar = _teklifService.GetDetaylar(w => teklifIdler.Contains(w.Id));
            //var alimDetaylar = _alimService.GetDetaylar();

            //foreach (var item in teklifDetaylar)
            //{
            //    item.Checked = true;
            //    item.TeklifDurumId = 1;
            //    var alimDetays = alimDetaylar.Where(w => w.TeklifId == item.Id).ToList();
            //    eczaneGrupDetayAlimDetaylarViewModel.Add(new EczaneGrupDetayAlimDetaylarViewModel
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
                    foreach (var eczaneGrupdetays in eczaneGrupDetayAlimDetaylarViewModel)
                    {
                        if (eczaneGrupdetays.EczaneGrupDetay.Id == Convert.ToInt32(item))
                        {
                            eczaneGrupdetays.EczaneGrupDetay.Expanded = true;
                        }
                    }
                }
            }

            return PartialView("GonderdiklerimEczanelereGorePartialView", eczaneGrupDetayAlimDetaylarViewModel);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecilenleriSil(string silinecekTeklifler, string ExpandedForSil, string pageForCokluSil, string teklifDurumIdForCokluSil, string alimDurumIdForCokluSil)
        {
            List<EczaneGrupDetayAlimDetaylarViewModel> eczaneGrupDetayAlimDetaylarViewModel = new List<EczaneGrupDetayAlimDetaylarViewModel>();


            List<int> teklifIdler = new List<int>();
            var uyariMesaji = "Seçim Yapmadınız!";

            if (silinecekTeklifler == null || silinecekTeklifler == ";" || silinecekTeklifler == "")
            {
                TempData["MessageSuccess"] = uyariMesaji;
                eczaneGrupDetayAlimDetaylarViewModel = getGonderimlerim_EczaneGrupdetay(eczaneGrupDetayAlimDetaylarViewModel, pageForCokluSil,  teklifDurumIdForCokluSil,  alimDurumIdForCokluSil);

                return PartialView("GonderdiklerimEczanelereGorePartialView", eczaneGrupDetayAlimDetaylarViewModel);
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
                        eczaneGrupDetayAlimDetaylarViewModel = getGonderimlerim_EczaneGrupdetay(eczaneGrupDetayAlimDetaylarViewModel, pageForCokluSil, teklifDurumIdForCokluSil, alimDurumIdForCokluSil);
                    }
                    catch (Exception ex)
                    {
                        string hataMesaji = ex.InnerException.InnerException.Message;
                        TempData["MessageDanger"] = "Secilen tekliflerden bazıları silinemedi. " + hataMesaji;
                    }
                    teklifIdler.Add(Convert.ToInt32(item));
                }
            }
            //TempData["MessageSuccess"] = "Secilen teklifler silinmiştir.";
            eczaneGrupDetayAlimDetaylarViewModel = getGonderimlerim_EczaneGrupdetay(eczaneGrupDetayAlimDetaylarViewModel, pageForCokluSil, teklifDurumIdForCokluSil, alimDurumIdForCokluSil);
            return PartialView("GonderdiklerimEczanelereGorePartialView", eczaneGrupDetayAlimDetaylarViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecilenleriKapat(string kapatilacakTeklifler, string ExpandedForKapat, string pageForKapat, string teklifDurumIdForKapat, string alimDurumIdForKapat)
        {
            List<EczaneGrupDetayAlimDetaylarViewModel> eczaneGrupDetayAlimDetaylarViewModel = new List<EczaneGrupDetayAlimDetaylarViewModel>();
            List<int> teklifIdler = new List<int>();
            var uyariMesaji = "Seçim Yapmadınız!";

            if (kapatilacakTeklifler == null || kapatilacakTeklifler == ";" || kapatilacakTeklifler == "")
            {
                TempData["MessageSuccess"] = uyariMesaji;
                eczaneGrupDetayAlimDetaylarViewModel = getGonderimlerim_EczaneGrupdetay(eczaneGrupDetayAlimDetaylarViewModel, pageForKapat, teklifDurumIdForKapat, alimDurumIdForKapat);

                return PartialView("GonderdiklerimEczanelereGorePartialView", eczaneGrupDetayAlimDetaylarViewModel);
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



                    //foreach (var eczaneGrupdetays in eczaneGrupDetayAlimDetaylarViewModel)
                    //{
                    //    if (eczaneGrupdetays.EczaneGrupDetay.Id == Convert.ToInt32(item))
                    //    {
                    //        eczaneGrupdetays.EczaneGrupDetay.Checked = true;

                    //    }
                    //}
                }
            }
            eczaneGrupDetayAlimDetaylarViewModel = getGonderimlerim_EczaneGrupdetay(eczaneGrupDetayAlimDetaylarViewModel, pageForKapat, teklifDurumIdForKapat, alimDurumIdForKapat);
            TempData["MessageSuccess"] = "Secilen teklifler kapatılmıştır.";
            /////////////////////////////////
            //var teklifDetaylar = _teklifService.GetDetaylar(w => teklifIdler.Contains(w.Id));
            //var alimDetaylar = _alimService.GetDetaylar();

            //foreach (var item in teklifDetaylar)
            //{
            //    item.Checked = true;
            //    item.TeklifDurumId = 1;
            //    var alimDetays = alimDetaylar.Where(w => w.TeklifId == item.Id).ToList();
            //    eczaneGrupDetayAlimDetaylarViewModel.Add(new EczaneGrupDetayAlimDetaylarViewModel
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
                    foreach (var eczaneGrupdetays in eczaneGrupDetayAlimDetaylarViewModel)
                    {
                        if (eczaneGrupdetays.EczaneGrupDetay.Id == Convert.ToInt32(item))
                        {
                            eczaneGrupdetays.EczaneGrupDetay.Expanded = true;
                        }
                    }
                }
            }

            return PartialView("GonderdiklerimEczanelereGorePartialView", eczaneGrupDetayAlimDetaylarViewModel);

        }

    }
}