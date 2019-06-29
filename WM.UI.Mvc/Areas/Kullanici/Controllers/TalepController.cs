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
    public class TalepController : Controller
    {
        // GET: Kullanici/talep
        #region ctor
        private ITalepService _talepService;
        private IEczaneService _eczaneService;
        private IKatilimService _katilimService;
        private IEczaneGrupService _eczaneGrupService;
        private IEczaneUserService _eczaneUserService;
        private IUserService _userService;
        private IGrupService _grupService;
        private ITalepDurumService _talepDurumService;
        private IYayinlamaTurService _yayinlamaTurService;
        private ITeklifDurumService _teklifDurumService;
        private IIlacService _ilacService;

        public TalepController(IEczaneGrupService eczaneGrupService,
                                IEczaneUserService eczaneUserService,
                                IEczaneService eczaneService,
                                ITalepService talepService,
                                IUserService userService,
                                ITalepDurumService talepDurumService,
                                IKatilimService katilimService,
                                IGrupService grupService,
                                IYayinlamaTurService yayinlamaTurService,
                                ITeklifDurumService teklifDurumService,
                                IIlacService ilacService)
        {
            
            _talepService = talepService;
            _eczaneUserService = eczaneUserService;
            _eczaneGrupService = eczaneGrupService;
            _userService = userService;
            _yayinlamaTurService = yayinlamaTurService;
            _katilimService = katilimService;
            _grupService = grupService;
            _teklifDurumService = teklifDurumService;
            _eczaneService = eczaneService;
            _talepDurumService = talepDurumService;
            _ilacService = ilacService;
        }
        #endregion
        public ActionResult Index(int? page)
        {
            int thispage = 0;
            if (page != 0)
                thispage = Convert.ToInt32(page);       

            var user = _userService.GetByUserName(User.Identity.Name);
            var eczaneler = _eczaneService.GetListByUser(user);

            var gruplar = _grupService.GetListByUser(user);
            var eczaneGruplar = _eczaneGrupService.GetListByUser(user);
            var eczanegrupIdler = eczaneGruplar.Select(s => s.Id).ToList();
            var grupIdler = gruplar.Select(s => s.Id).ToList();

            var talepler = _talepService.GetListByEczaneGruplar(eczanegrupIdler, grupIdler).OrderByDescending(o => o.KayitTarihi).ToList();
            var ilaclar = _ilacService.GetList().Where(w => talepler.Select(s => s.IlacId).Contains(w.Id)).ToList();
            //bu eczanenin bulunduğu tüm eczanegrup lardaki teklifleri gösterir

            var pager = new Pager(talepler.Count(), thispage);


            var model = new TalepDetayViewModel()
            {
                TalepDetaylar = talepler,//.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList(),
                Eczaneler = eczaneler,
                Ilaclar = ilaclar,
                EczaneGruplar = eczaneGruplar,
                Pager = pager
            };

            return View(model);
            //}      
        }
        // GET: TeklifNobet/Talep/Details/5
        public ActionResult Details(int id)
        {
            if (id < 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Talep Talep = _talepService.GetById(id);
            if (Talep == null)
            {
                return HttpNotFound();
            }
            return View(Talep);
        }
        [HttpGet]
        //[ValidateAntiForgeryToken]
        public ActionResult GetTalepler(string page, string talepDurumId)
        {
            List<TalepDetayKatilimDetaylarViewModel> talepDetayKatilimDetaylarViewModel = new List<TalepDetayKatilimDetaylarViewModel>();
            talepDetayKatilimDetaylarViewModel = getTalepler(talepDetayKatilimDetaylarViewModel, page, talepDurumId);
            return PartialView("TalepPartialView", talepDetayKatilimDetaylarViewModel);
        }
        private List<TalepDetayKatilimDetaylarViewModel> getTalepler(List<TalepDetayKatilimDetaylarViewModel>
            talepDetayKatilimDetaylarViewModel, string page, string talepDurumId)
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
                talepDetaylar = _talepService.GetListByEczaneGrupId(eczanegrupIdler.FirstOrDefault());
                talepDetaylar = talepDetaylar.Where(w => !eczanegrupIdler.Contains(w.TalepVerenEczaneGrupId) 
                && w.TalepDurumId == Convert.ToInt32(talepDurumId)
                && (w.BitisTarihi > System.DateTime.Now || w.BitisTarihi == null))
                .OrderByDescending(o => o.KayitTarihi).ToList();
            }
            else
            {
                talepDetaylar = _talepService.GetListByEczaneGrupId(eczanegrupIdler.FirstOrDefault());
                talepDetaylar = talepDetaylar.Where(w => !eczanegrupIdler.Contains(w.TalepVerenEczaneGrupId) //&& w.TalepDurumId == 1
                && (w.BitisTarihi > System.DateTime.Now || w.BitisTarihi == null))
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
                var alimDetaylars = alimDetaylar.Where(w => w.TalepId == item.Id).ToList();

                talepDetayKatilimDetaylarViewModel.Add(new TalepDetayKatilimDetaylarViewModel
                {
                    TalepDetay = item,
                    KatilimDetaylar = alimDetaylars,
                    Pager = pager
                });
            }
            return talepDetayKatilimDetaylarViewModel;
        }

        // GET: TeklifNobet/Talep/Create
        [Authorize(Roles = "Admin,Grup Yöneticisi, Eczane")]
        public ActionResult Create()
        {
            Talep Talep = new Talep() {  };
            var user = _userService.GetByUserName(User.Identity.Name);
            var talepDurumlar = _talepDurumService.GetList();
            var yayinlamaTurler = _yayinlamaTurService.GetList();
            var ilaclar = _ilacService.GetList();
            
            ViewBag.YayinlamaTurId = new SelectList(yayinlamaTurler, "Id", "Adi");
            ViewBag.TalepDurumId = new SelectList(talepDurumlar, "Id", "Adi");
            ViewBag.IlacId = new SelectList(ilaclar, "Id", "Adi");

            return View(Talep);
        }

        // POST: TeklifNobet/Talep/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IlacId,HedeflenenAlim,IlacMiad,MalFazlasi,NetFiyat,EtiketFiyati,DepoFiyat,AlimMiktari,BaslangicTarihi,BitisTarihi,Maksimum,Minimum,TeklifTurId,YayinlamaTurId,TalepDurumId")] Talep Talep)
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var eczaneId = _eczaneUserService.GetListByUserId(user.Id).Select(s => s.EczaneId).FirstOrDefault();
            Talep.TalepVerenEczaneGrupId = eczaneId;
            // Talep.NetFiyat = (Math.Round(Talep.NetFiyat, 4));

            //Talep.KayitTarihi = Convert.ToDateTime(DateTime.Now.ToShortDateString());

            if (ModelState.IsValid)
            {
                try
                {
                    _talepService.Insert(Talep);
                    return RedirectToAction("Index", eczaneId);
                }
                catch (Exception ex)
                {
                    TempData["Message"] = "ERROR:" + ex.InnerException.InnerException.Message.ToString();

                }
            }

            
            var talepDurumlar = _talepDurumService.GetList();
            var yayinlamaTurler = _yayinlamaTurService.GetList();
            var ilaclar = _ilacService.GetList();

          
            ViewBag.TalepDurumId = new SelectList(talepDurumlar, "Id", "Adi", Talep.TalepDurumId);
            ViewBag.IlacId = new SelectList(ilaclar, "Id", "Adi", Talep.IlacId);
            return View(Talep);
        }
        public ActionResult Search(string Keywords)
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
        // GET: TeklifNobet/Talep/Edit/5
        [Authorize(Roles = "Admin,Grup Yöneticisi,Eczane")]
        public ActionResult Edit(int Id)
        {
            //if (id < 1)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //Talep Talep = _talepService.GetById(id);
            //if (Talep == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(Talep);

            return RedirectToAction("Create", "Alim", new { area = "Kullanici", id = Id });
        }

        // POST: TeklifNobet/Talep/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IlacId,MalFazlasi,NetFiyat,DepoFiyat,AlimMiktari,BaslangicTarihi,BitisTarihi,Maksimum,Minimum,TeklifTurId,YayinlamaTurId,TalepDurumId")] Talep Talep)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _talepService.Update(Talep);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Message"] = "ERROR:" + ex.InnerException.InnerException.Message.ToString();
                }
            }
            return View(Talep);
        }

        // GET: TeklifNobet/Talep/Delete/5
        [Authorize(Roles = "Admin,Grup Yöneticisi,Eczane")]
        public ActionResult Delete(int id)
        {
            if (id < 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Talep Talep = _talepService.GetById(id);
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