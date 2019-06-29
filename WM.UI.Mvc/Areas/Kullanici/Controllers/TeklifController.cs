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
using System.Text.RegularExpressions;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;

namespace WM.UI.Mvc.Areas.Kullanici.Controllers
{
    [Authorize(Roles = "Admin,Eczane,Grup Yöneticisi")]
    public class TeklifController : Controller
    {
        // GET: Kullanici/teklif
        #region ctor
        private ITeklifService _TeklifService;
        private IEczaneService _eczaneService;
        private ITeklifService _teklifService;
        private IAlimService _alimService;
        private IAlimDurumService _alimDurumService;
        private IEczaneGrupService _eczaneGrupService;
        private IEczaneUserService _eczaneUserService;
        private IUserService _userService;
        private ITeklifTurService _teklifTurService;
        private IGrupService _grupService;
        private ITeklifDurumService _teklifDurumService;
        private IYayinlamaTurService _yayinlamaTurService;
        private IIlacService _ilacService;

        public TeklifController(IEczaneGrupService eczaneGrupService,
                                IEczaneUserService eczaneUserService,
                                ITeklifService TeklifService,
                                IEczaneService eczaneService,
                                ITeklifService teklifService,
                                IUserService userService,
                                IAlimService alimService,
                                IAlimDurumService alimDurumService,
                                ITeklifTurService teklifTurService,
                                IGrupService grupService,
                                IYayinlamaTurService yayinlamaTurService,
                                ITeklifDurumService teklifDurumService,
                                IIlacService ilacService)
        {
            _TeklifService = TeklifService;
            _teklifService = teklifService;
            _eczaneUserService = eczaneUserService;
            _eczaneGrupService = eczaneGrupService;
            _userService = userService;
            _alimService = alimService;
            _alimDurumService = alimDurumService;
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
            int thispage = 0;
            if (page != 0)
                thispage = Convert.ToInt32(page);

            var teklifTurler = _teklifTurService.GetList();
            ViewBag.TeklifTurId = new SelectList(teklifTurler, "Id", "Adi");
            // var temp = TempData["EczaneGrupId"];

            var user = _userService.GetByUserName(User.Identity.Name);
            var eczaneler = _eczaneService.GetListByUser(user);

            var gruplar = _grupService.GetListByUser(user);
            var eczaneGruplar = _eczaneGrupService.GetListByUser(user);
            var eczanegrupIdler = eczaneGruplar.Select(s => s.Id).ToList();
            var grupIdler = gruplar.Select(s => s.Id).ToList();

            var teklifler = _teklifService.GetListByEczaneGruplar(eczanegrupIdler, grupIdler).OrderByDescending(o => o.BaslangicTarihi).ToList();
            //özel eczaneye yapılan teklifler gözükmesin
            teklifler = teklifler.Where(w => w.OzelEczaneGrupId == null ||
            eczanegrupIdler.Contains(Convert.ToInt32(w.OzelEczaneGrupId))).ToList();
            var ilaclar = _ilacService.GetList().Where(w => teklifler.Select(s => s.IlacId).Contains(w.Id)).ToList();
            //bu eczanenin bulunduğu tüm eczanegrup lardaki teklifleri gösterir

            var pager = new Pager(teklifler.Count(), thispage);


            var model = new TeklifDetayViewModel()
            {
                TeklifDetaylar = teklifler,//.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList(),
                Eczaneler = eczaneler,
                Ilaclar = ilaclar,
                EczaneGruplar = eczaneGruplar,
                Pager= pager
            };

            return View(model);
            //}      
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
        [HttpGet]
        //[ValidateAntiForgeryToken]
        public ActionResult GetTeklifler(string page, string teklifDurumId, string alimDurumId)
        {
            List<TeklifDetayAlimDetaylarViewModel> teklifDetayAlimDetaylarViewModel = new List<TeklifDetayAlimDetaylarViewModel>();
            teklifDetayAlimDetaylarViewModel = getTeklifler(teklifDetayAlimDetaylarViewModel, page, teklifDurumId, alimDurumId);
            return PartialView("TeklifPartialView", teklifDetayAlimDetaylarViewModel);
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
            var alimDetaylar = _alimService.GetDetaylar();
            if (alimDurumId != null && alimDurumId != "")
            {
                alimDetaylar = alimDetaylar.Where(w => w.AlimDurumId == Convert.ToInt32(alimDurumId)).ToList();
            }
            if (teklifDurumId != null && teklifDurumId != "")
            {            
                teklifDetaylar = _teklifService.GetListByEczaneGrupId(eczanegrupIdler.FirstOrDefault());
                teklifDetaylar = teklifDetaylar
                    .Where(w => !eczanegrupIdler.Contains(w.TeklifiVerenEczaneGrupId)
                    && w.TeklifDurumId == Convert.ToInt32(teklifDurumId)
                && (w.BitisTarihi > System.DateTime.Now || w.BitisTarihi == null)
                && w.BaslangicTarihi < System.DateTime.Now)
                .OrderByDescending(o => o.BaslangicTarihi).ToList();
            }
            else
            {
                teklifDetaylar = _teklifService.GetListByEczaneGrupId(eczanegrupIdler.FirstOrDefault());
                teklifDetaylar = teklifDetaylar
                    .Where(w => !eczanegrupIdler.Contains(w.TeklifiVerenEczaneGrupId) //&& w.TeklifDurumId == 1
                && (w.BitisTarihi > System.DateTime.Now || w.BitisTarihi == null)
                && w.BaslangicTarihi < System.DateTime.Now)
                .OrderByDescending(o => o.BaslangicTarihi).ToList();
            }
            if (page=="" || page == null)
            {
                page = "1";
            }
            var pager = new Pager(teklifDetaylar.Count(), Convert.ToInt32(page));
            //teklifDetaylar = teklifDetaylar.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList();
            var AlimDurumlar = _alimDurumService.GetList();
            ViewBag.AlimDurumIdFilter = new SelectList(AlimDurumlar, "Id", "Adi", alimDurumId);
            var TeklifDurumlar = _teklifDurumService.GetList();
            ViewBag.TeklifDurumIdFilter = new SelectList(TeklifDurumlar, "Id", "Adi", teklifDurumId);
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

        // GET: TeklifNobet/Teklif/Create
        [Authorize(Roles = "Admin,Grup Yöneticisi, Eczane")]
        public ActionResult Create()
        {
            Teklif Teklif = new Teklif() { BaslangicTarihi = DateTime.Now };
            var user = _userService.GetByUserName(User.Identity.Name);
            var teklifTurler = _teklifTurService.GetList();
            var teklifDurumlar = _teklifDurumService.GetList();
            var yayinlamaTurler = _yayinlamaTurService.GetList();
            var ilaclar = _ilacService.GetList();

            ViewBag.TeklifTurId = new SelectList(teklifTurler, "Id", "Adi");
            ViewBag.YayinlamaTurId = new SelectList(yayinlamaTurler, "Id", "Adi");
            ViewBag.TeklifDurumId = new SelectList(teklifDurumlar, "Id", "Adi");
            ViewBag.IlacId = new SelectList(ilaclar, "Id", "Adi");

            return View(Teklif);
        }

        // POST: TeklifNobet/Teklif/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IlacId,HedeflenenAlim,IlacMiad,MalFazlasi,NetFiyat,EtiketFiyati,DepoFiyat,AlimMiktari,BaslangicTarihi,BitisTarihi,Maksimum,Minimum,TeklifTurId,YayinlamaTurId,TeklifDurumId")] Teklif Teklif)
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var eczaneId = _eczaneUserService.GetListByUserId(user.Id).Select(s => s.EczaneId).FirstOrDefault();
            Teklif.TeklifiVerenEczaneGrupId = eczaneId;
            // Teklif.NetFiyat = (Math.Round(Teklif.NetFiyat, 4));

            //Teklif.KayitTarihi = Convert.ToDateTime(DateTime.Now.ToShortDateString());

            if (ModelState.IsValid)
            {
                try
                {
                    _TeklifService.Insert(Teklif);
                    return RedirectToAction("Index", eczaneId);
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

            ViewBag.TeklifTurId = new SelectList(teklifTurler, "Id", "Adi", Teklif.TeklifTurId);
            ViewBag.YayinlamaTurId = new SelectList(yayinlamaTurler, "Id", "Adi", Teklif.YayinlamaTurId);
            ViewBag.TeklifDurumId = new SelectList(teklifDurumlar, "Id", "Adi", Teklif.TeklifDurumId);
            ViewBag.IlacId = new SelectList(ilaclar, "Id", "Adi", Teklif.IlacId);
            return View(Teklif);
        }
        public ActionResult Search(string Keywords)
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
        // GET: TeklifNobet/Teklif/Edit/5
        [Authorize(Roles = "Admin,Grup Yöneticisi,Eczane")]
        public ActionResult Edit(int Id)
        {
            //if (id < 1)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //Teklif Teklif = _TeklifService.GetById(id);
            //if (Teklif == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(Teklif);

            return RedirectToAction("Create", "Alim", new { area = "Kullanici", id = Id });
        }

        // POST: TeklifNobet/Teklif/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IlacId,MalFazlasi,NetFiyat,DepoFiyat,AlimMiktari,BaslangicTarihi,BitisTarihi,Maksimum,Minimum,TeklifTurId,YayinlamaTurId,TeklifDurumId")] Teklif Teklif)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _TeklifService.Update(Teklif);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Message"] = "ERROR:" + ex.InnerException.InnerException.Message.ToString();
                }
            }
            return View(Teklif);
        }

        // GET: TeklifNobet/Teklif/Delete/5
        [Authorize(Roles = "Admin,Grup Yöneticisi,Eczane")]
        public ActionResult Delete(int id)
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

        // POST: TeklifNobet/Teklif/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Teklif Teklif = _TeklifService.GetById(id);
            try
            {
                _TeklifService.Delete(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "ERROR:" + ex.InnerException.InnerException.Message.ToString();
            }
            return View(Teklif);

        }
    }
}