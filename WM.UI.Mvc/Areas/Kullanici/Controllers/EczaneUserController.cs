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
    public class EczaneUserController : Controller
    {
        #region ctor
        private IUserService _UserService;
        private IEczaneService _eczaneService;
        private IEczaneUserService _eczaneUserService;
        private IUserService _userService;
        private IGridMvcHelper gridMvcHelper;
        public EczaneUserDetayViewModel MainLayoutViewModel { get; set; }

        public EczaneUserController(IEczaneUserService eczaneUserService,
                                    IEczaneService eczaneService,
                                    IUserService UserService,
                                    IUserService userService)
        {
            _eczaneUserService = eczaneUserService;
            _eczaneService = eczaneService;
            _UserService = UserService;
            _userService = userService;
            //this.gridMvcHelper = new GridMvcHelper();
            //this.MainLayoutViewModel = new EczaneUserDetayViewModel();//has property PageTitle
            //this.MainLayoutViewModel.EczaneUserID = 1;

            //this.ViewData["MainLayoutViewModel"] = this.MainLayoutViewModel;
        }
        #endregion
        // GET: EczaneNobet/EczaneUser
        public ActionResult Index(int? id)
        {
            var model = new EczaneUserDetayViewModel();
            
            if (id == 0)
            {// eğer bir userId ile devam etmiyorsa eczaneUserda olmayan tüm userlar gelir
                int Id = Convert.ToInt32(id);
                var user = _userService.GetByUserName(User.Identity.Name);
                var tumEczaneUserIdler = _eczaneUserService.GetList().Select(s => s.UserId);
                var userlar = _userService.GetList().Where(w => !tumEczaneUserIdler.Contains(w.Id)).ToList();
                var eczaneler = _eczaneService.GetListByUser(user);
                var eczaneUserDetaylar = _eczaneUserService.GetDetayListByUser(user);
                var rolIdler = _userService.GetUserRoles(user).OrderBy(s => s.RoleId).Select(u => u.RoleId).ToArray();
                var rolId = rolIdler.FirstOrDefault();
                ViewBag.rolId = rolId;
                //if (eczaneUserlar.Count == 1)
                //{//eczane eğer sadece tek bir Userta ise bu sayfayı görmesine gerek yok direk o grubu seçmiş gibi devam eder
                //    var eczaneUserId = eczaneUserlar.Select(s=>s.Id).FirstOrDefault().ToString();               
                //    Session["EczaneUserId"]= eczaneUserId.ToString();
                //    return RedirectToAction("Index", "Alim", new { area = "Kullanici", id = Convert.ToInt32(eczaneUserId)});
                //}
                model = new EczaneUserDetayViewModel()
                {
                    Eczaneler = eczaneler,
                    Userlar = userlar,
                    EczaneUserDetaylar = eczaneUserDetaylar,
                };

            }
            else
            {//seçili userId gelir.
                int Id = Convert.ToInt32(id);
                var user = _userService.GetByUserName(User.Identity.Name);
                var userlar = _userService.GetList().Where(w => w.Id == Id).ToList();//.Where(w => tumEczaneUserIdler.Contains(w.Id)).ToList();
                var eczaneler = _eczaneService.GetListByUser(user);
                var eczaneUserDetaylar = _eczaneUserService.GetDetayListByUser(user);
                var rolIdler = _userService.GetUserRoles(user).OrderBy(s => s.RoleId).Select(u => u.RoleId).ToArray();
                var rolId = rolIdler.FirstOrDefault();
                ViewBag.rolId = rolId;
                //if (eczaneUserlar.Count == 1)
                //{//eczane eğer sadece tek bir Userta ise bu sayfayı görmesine gerek yok direk o grubu seçmiş gibi devam eder
                //    var eczaneUserId = eczaneUserlar.Select(s=>s.Id).FirstOrDefault().ToString();               
                //    Session["EczaneUserId"]= eczaneUserId.ToString();
                //    return RedirectToAction("Index", "Alim", new { area = "Kullanici", id = Convert.ToInt32(eczaneUserId)});
                //}
                model = new EczaneUserDetayViewModel()
                {
                    Eczaneler = eczaneler,
                    Userlar = userlar,
                    EczaneUserDetaylar = eczaneUserDetaylar,
                };

            }
                
           
            return View(model);
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
       // public ActionResult UserSec(int? Id)
       // {//birden fazla Userta bulunan eczane buradan işlem yapacağı grubu seçer. eczaneUserId alim e gönderilir
            //model.EczaneUserID = id;
            //Session["EczaneUserId"] = Id;
            //TempData["EczaneUserId"] = Id;
          //  return RedirectToAction("Index", "EczaneRapor", new { area = "Kullanici", id = Id });
        //}
        [ChildActionOnly]
        public ActionResult EczaneUserPartial()
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var eczaneler = _eczaneService.GetListByUser(user).Select(s => s.Id);

            var items = this._eczaneUserService.GetDetaylar()
                .Where(s => eczaneler.Contains(s.EczaneId))
                .AsQueryable().OrderBy(f => f.Id);

            var grid = this.gridMvcHelper.GetAjaxGrid(items);
            return PartialView("EczaneUserPartial", grid);
        }

        [HttpGet]
        public ActionResult EczaneUserPartialPager(int? page)
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var eczaneler = _eczaneService.GetListByUser(user).Select(s => s.Id);

            var items = this._eczaneUserService.GetDetaylar()
                .Where(s => eczaneler.Contains(s.EczaneId))
                .AsQueryable().OrderBy(f => f.Id);

            var grid = this.gridMvcHelper.GetAjaxGrid(items, page);
            var strPath = "~/Areas/Kullanici/Views/User/EczaneUserPartial.cshtml";
            object jsonData = this.gridMvcHelper.GetGridJsonData(grid, strPath, this);
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        // GET: EczaneNobet/EczaneUser/Details/5
        public ActionResult Details(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EczaneUserDetay GetEczaneUserDetaylar = _eczaneUserService.GetDetayById(id);
            if (GetEczaneUserDetaylar == null)
            {
                return HttpNotFound();
            }
            return View(GetEczaneUserDetaylar);
        }

        // GET: EczaneNobet/EczaneUser/Create
        public ActionResult Create()
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var yetkiliOlduguUserlar = _eczaneUserService.GetDetayListByUser(user).Select(s => s.UserId).ToList();
            var Userlar = _eczaneUserService.GetList()
                .Where(w=> yetkiliOlduguUserlar.Contains(w.UserId))
                .Select(s=>s.UserId).ToList();
            var UserlDDL = _UserService.GetList().Where(w => Userlar.Contains(w.Id));
            var eczaneler = _eczaneUserService.GetListByUser(user).Where(w => !Userlar.Contains(w.UserId))
                .Select(s => s.EczaneId).ToList();
            var eczaneDDL = _eczaneService.GetList().Where(w => eczaneler.Contains(w.Id));
           // ViewBag.EczaneId = new SelectList(items.Select(s => new { s.Id, EczaneUserAdi = $"{s.EczaneAdi} {s.UserAdi}" }), "Id", "EczaneUserAdi");
            ViewBag.UserId = new SelectList(UserlDDL, "Id", "Adi");
            ViewBag.EczaneId = new SelectList(eczaneDDL, "Id", "Adi");
            return View();
        }

        // POST: EczaneNobet/EczaneUser/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Adi,BaslangicTarihi")] EczaneUser eczaneUser)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _eczaneUserService.Insert(eczaneUser);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Message"] = "ERROR:" + ex.Message.ToString();
                }
            }

            ViewBag.EczaneId = new SelectList(_eczaneService.GetList(), "Id", "Adi", eczaneUser.EczaneId);
            ViewBag.EczaneUserTanimId = new SelectList(_eczaneUserService.GetList(), "Id", "Adi", eczaneUser.Id);
            return View(eczaneUser);
        }

        // GET: EczaneNobet/EczaneUser/Edit/5
        public ActionResult Edit(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EczaneUser eczaneUser = _eczaneUserService.GetById(id);
            if (eczaneUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.EczaneId = new SelectList(_eczaneService.GetList(), "Id", "Adi", eczaneUser.EczaneId);
            ViewBag.EczaneUserTanimId = new SelectList(_eczaneUserService.GetList(), "Id", "Adi", eczaneUser.Id);
            return View(eczaneUser);
        }

        // POST: EczaneNobet/EczaneUser/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,EczaneId")] EczaneUser eczaneUser)
        {
            if (ModelState.IsValid)
            {try
                {
                    _eczaneUserService.Update(eczaneUser);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Message"] = "ERROR:" + ex.Message.ToString();
                }
            }
            ViewBag.EczaneId = new SelectList(_eczaneService.GetList(), "Id", "Adi", eczaneUser.EczaneId);
            ViewBag.EczaneUserTanimId = new SelectList(_eczaneUserService.GetList(), "Id", "Adi", eczaneUser.Id);
            return View(eczaneUser);
        }
    
        // GET: EczaneNobet/EczaneUser/Delete/5
        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EczaneUserDetay GetEczaneUserDetaylar = _eczaneUserService.GetDetayById(id);
            if (GetEczaneUserDetaylar == null)
            {
                return HttpNotFound();
            }
            return View(GetEczaneUserDetaylar);
        }

        // POST: EczaneNobet/EczaneUser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EczaneUser eczaneUser = _eczaneUserService.GetById(id);
            try
            {
                _eczaneUserService.Delete(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "ERROR:" + ex.Message.ToString();
            }
            return View(eczaneUser);
        }

    }
}