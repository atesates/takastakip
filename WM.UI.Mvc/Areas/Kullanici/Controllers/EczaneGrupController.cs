using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WM.Northwind.Business.Abstract.Authorization;
using WM.Northwind.Business.Abstract.IlacTakip;
using WM.UI.Mvc.HtmlHelpers;
using WM.UI.Mvc.Models;
using System.Net;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using WM.Northwind.Entities.Concrete.IlacTakip;
using WM.UI.Mvc.Areas.Kullanici.Models;

namespace WM.UI.Mvc.Areas.Kullanici.Controllers
{
    [Authorize]
    public class EczaneGrupController : Controller
    {
        #region ctor
        private IGrupService _grupService;
        private IEczaneService _eczaneService;
        private IEczaneGrupService _eczaneGrupService;
        private IUserService _userService;
        private IGridMvcHelper gridMvcHelper;
        public EczaneGrupDetayViewModel MainLayoutViewModel { get; set; }

        public EczaneGrupController(IEczaneGrupService eczaneGrupService,
                                    IEczaneService eczaneService,
                                    IGrupService grupService,
                                    IUserService userService)
        {
            _eczaneGrupService = eczaneGrupService;
            _eczaneService = eczaneService;
            _grupService = grupService;
            _userService = userService;
            //this.gridMvcHelper = new GridMvcHelper();
            //this.MainLayoutViewModel = new EczaneGrupDetayViewModel();//has property PageTitle
            //this.MainLayoutViewModel.EczaneGrupID = 1;

            //this.ViewData["MainLayoutViewModel"] = this.MainLayoutViewModel;
        }
        #endregion
        // GET: EczaneNobet/EczaneGrup
        public ActionResult Index(int? id)
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var eczaneler = _eczaneService.GetListByUser(user);
            var gruplar = _grupService.GetListByUser(user);
            var eczaneGrupDetaylar = _eczaneGrupService.GetDetayListByUser(user).Distinct().ToList();
            var rolIdler = _userService.GetUserRoles(user).OrderBy(s => s.RoleId).Select(u => u.RoleId).ToArray();
            var rolId = rolIdler.FirstOrDefault();
            ViewBag.rolId = rolId;
            //if (eczaneGruplar.Count == 1)
            //{//eczane eğer sadece tek bir grupta ise bu sayfayı görmesine gerek yok direk o grubu seçmiş gibi devam eder
            //    var eczaneGrupId = eczaneGruplar.Select(s=>s.Id).FirstOrDefault().ToString();               
            //    Session["EczanegrupId"]= eczaneGrupId.ToString();
            //    return RedirectToAction("Index", "Alim", new { area = "Kullanici", id = Convert.ToInt32(eczaneGrupId)});
            //}
            var model = new EczaneGrupDetayViewModel()
            {
                Eczaneler = eczaneler,
                //Gruplar = gruplar,
                EczaneGrupDetaylar = eczaneGrupDetaylar,
            };
            return View(model);
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
       // public ActionResult GrupSec(int? Id)
       // {//birden fazla grupta bulunan eczane buradan işlem yapacağı grubu seçer. eczanegrupId alim e gönderilir
            //model.EczaneGrupID = id;
            //Session["EczanegrupId"] = Id;
            //TempData["EczaneGrupId"] = Id;
          //  return RedirectToAction("Index", "EczaneRapor", new { area = "Kullanici", id = Id });
        //}
        [ChildActionOnly]
        public ActionResult EczaneGrupPartial()
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var eczaneler = _eczaneService.GetListByUser(user).Select(s => s.Id);

            var items = this._eczaneGrupService.GetDetaylar()
                .Where(s => eczaneler.Contains(s.EczaneId))
                .AsQueryable().OrderBy(f => f.Id);

            var grid = this.gridMvcHelper.GetAjaxGrid(items);
            return PartialView("EczaneGrupPartial", grid);
        }

        [HttpGet]
        public ActionResult EczaneGrupPartialPager(int? page)
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var eczaneler = _eczaneService.GetListByUser(user).Select(s => s.Id);

            var items = this._eczaneGrupService.GetDetaylar()
                .Where(s => eczaneler.Contains(s.EczaneId))
                .AsQueryable().OrderBy(f => f.Id);

            var grid = this.gridMvcHelper.GetAjaxGrid(items, page);
            var strPath = "~/Areas/Kullanici/Views/Grup/EczaneGrupPartial.cshtml";
            object jsonData = this.gridMvcHelper.GetGridJsonData(grid, strPath, this);
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        // GET: EczaneNobet/EczaneGrup/Details/5
        public ActionResult Details(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EczaneGrupDetay GetEczaneGrupDetaylar = _eczaneGrupService.GetDetayById(id);
            if (GetEczaneGrupDetaylar == null)
            {
                return HttpNotFound();
            }
            return View(GetEczaneGrupDetaylar);
        }

        // GET: EczaneNobet/EczaneGrup/Create
        public ActionResult Create()
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var yetkiliOlduguGruplar = _eczaneGrupService.GetDetayListByUser(user).Select(s => s.GrupId).ToList();
            var gruplar = _eczaneGrupService.GetList()
                .Where(w=> yetkiliOlduguGruplar.Contains(w.GrupId))
                .Select(s=>s.GrupId).ToList();
            var gruplDDL = _grupService.GetList().Where(w => gruplar.Contains(w.Id));
            var eczaneler = _eczaneGrupService.GetListByUser(user).Where(w => !gruplar.Contains(w.GrupId))
                .Select(s => s.EczaneId).ToList();
            var eczaneDDL = _eczaneService.GetList().Where(w => eczaneler.Contains(w.Id));
           // ViewBag.EczaneId = new SelectList(items.Select(s => new { s.Id, EczaneGrupAdi = $"{s.EczaneAdi} {s.GrupAdi}" }), "Id", "EczaneGrupAdi");
            ViewBag.GrupId = new SelectList(gruplDDL, "Id", "Adi");
            ViewBag.EczaneId = new SelectList(eczaneDDL, "Id", "Adi");
            return View();
        }

        // POST: EczaneNobet/EczaneGrup/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Adi,BaslangicTarihi")] EczaneGrup eczaneGrup)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _eczaneGrupService.Insert(eczaneGrup);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Message"] = "ERROR:" + ex.Message.ToString();
                }
            }

            ViewBag.EczaneId = new SelectList(_eczaneService.GetList(), "Id", "Adi", eczaneGrup.EczaneId);
            ViewBag.EczaneGrupTanimId = new SelectList(_eczaneGrupService.GetList(), "Id", "Adi", eczaneGrup.Id);
            return View(eczaneGrup);
        }

        // GET: EczaneNobet/EczaneGrup/Edit/5
        public ActionResult Edit(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EczaneGrup eczaneGrup = _eczaneGrupService.GetById(id);
            if (eczaneGrup == null)
            {
                return HttpNotFound();
            }
            ViewBag.EczaneId = new SelectList(_eczaneService.GetList(), "Id", "Adi", eczaneGrup.EczaneId);
            ViewBag.EczaneGrupTanimId = new SelectList(_eczaneGrupService.GetList(), "Id", "Adi", eczaneGrup.Id);
            return View(eczaneGrup);
        }

        // POST: EczaneNobet/EczaneGrup/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,GrupId,EczaneId")] EczaneGrup eczaneGrup)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _eczaneGrupService.Update(eczaneGrup);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Message"] = "ERROR:" + ex.Message.ToString();
                }
            }
            ViewBag.EczaneId = new SelectList(_eczaneService.GetList(), "Id", "Adi", eczaneGrup.EczaneId);
            ViewBag.EczaneGrupTanimId = new SelectList(_eczaneGrupService.GetList(), "Id", "Adi", eczaneGrup.Id);
            return View(eczaneGrup);
        }
    
        // GET: EczaneNobet/EczaneGrup/Delete/5
        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EczaneGrupDetay GetEczaneGrupDetaylar = _eczaneGrupService.GetDetayById(id);
            if (GetEczaneGrupDetaylar == null)
            {
                return HttpNotFound();
            }
            return View(GetEczaneGrupDetaylar);
        }

        // POST: EczaneNobet/EczaneGrup/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EczaneGrup eczaneGrup = _eczaneGrupService.GetById(id);
            try
            {
                _eczaneGrupService.Delete(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "ERROR:" + ex.Message.ToString();
            }
            return View(eczaneGrup);

        }

    }
}