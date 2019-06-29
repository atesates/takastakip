using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WM.Northwind.Business.Abstract.Authorization;
using WM.Northwind.Business.Abstract.IlacTakip;
using WM.Northwind.Entities.Concrete.IlacTakip;
using WM.UI.Mvc.Areas.Kullanici.Models;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;

namespace WM.UI.Mvc.Areas.Kullanici.Controllers
{
    public class EczaneController : Controller
    {
        #region ctor
        private IEczaneService _eczaneService;
        private ISehirService _sehirService;
        private IEczaneGrupService _eczaneGrupService;
        private IEczaneUserService _eczaneUserService;
        private IUserService _userService;
        private IUserRoleService _userRoleService;

        public EczaneController(IEczaneService eczaneService,
                                 IUserRoleService userRoleService,
                                 ISehirService sehirService,
                                 IEczaneGrupService eczaneGrupService,
                                 IEczaneUserService eczaneUserService,
                                 IUserService userService)
        {
            _eczaneService = eczaneService;
            _userService = userService;
            _sehirService = sehirService;
            _eczaneUserService = eczaneUserService;
            _userRoleService = userRoleService;
            _eczaneGrupService = eczaneGrupService;
        }
        #endregion
        // GET: EczaneNobet/Eczane
        [Authorize]
        public ActionResult Index(int? id)
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var role_id = _userRoleService.GetListByUserId(user.Id);
            int min = 10;
            foreach (var item in role_id)
            {
                if (item.RoleId < min)
                {
                    min = item.RoleId;
                }
            }
            if (Convert.ToInt32(min) == 2)
            {
                var eczaneIdler = _eczaneUserService.GetListByUserId(user.Id).Select(s => s.EczaneId);
                var model = _eczaneService.GetList().Where(w => eczaneIdler.Contains(w.Id));
                return View(model);
            }
            else if (Convert.ToInt32(min) == 3)
            {
                var eczaneIdler = _eczaneUserService.GetListByUserId(user.Id).Select(s => s.EczaneId);
                var model = _eczaneService.GetList().Where(w => eczaneIdler.Contains(w.Id));
                return View(model);
            }
            else
            {
                var model = _eczaneService.GetList();
                return View(model);
            }

        }
        public ActionResult GetDagiticiDetaylar(int EczaneGrupId)
        {
            EczaneDetay EczaneDetay = new EczaneDetay();
            int EczaneId = _eczaneGrupService.GetById(EczaneGrupId).Id;
            EczaneDetay = _eczaneService.GetDetayById(EczaneId);
            return PartialView("EczanePartialView", EczaneDetay);

        }
        // GET: EczaneNobet/Eczane/Details/5
        public ActionResult Details(int id)
        {
            if (id < 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Eczane eczane = _eczaneService.GetById(id);
            if (eczane == null)
            {
                return HttpNotFound();
            }
            return View(eczane);
        }

        // GET: EczaneNobet/Eczane/Create
        [Authorize(Roles = "Admin,Grup Yöneticisi, Eczane")]
        public ActionResult Create()
        {
            Eczane eczane = new Eczane();

            return View(eczane);
        }

        // POST: EczaneNobet/Eczane/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Adi,Telefon,Adres,Telefon2,EczaneGln,FaturaAdSoyad,VergiNumarasi,VergiDairesi,SehirId")] Eczane eczane)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _eczaneService.Insert(eczane);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Message"] = "ERROR:" + ex.Message.ToString();
                }
            }

            return View(eczane);
        }

        // GET: EczaneNobet/Eczane/Edit/5
        [Authorize(Roles = "Admin,Grup Yöneticisi,Eczane")]
        public ActionResult Edit(int? id)
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var eczaneId = _eczaneUserService.GetListByUserId(user.Id).Select(s=>s.Id).FirstOrDefault();
            int Id = 0;
            if (id == null)
                try
                {
                    Id = Convert.ToInt32(eczaneId);
                }
                catch(Exception ex)
                {
                    return RedirectToAction("Index", "Eczane");

                }

            //Id = Convert.ToInt32(id);

            if (id < 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Eczane eczane = _eczaneService.GetById(Id);

            var sehirler = _sehirService.GetList();
            ViewBag.SehirId = new SelectList(sehirler, "Id", "Adi", eczane.SehirId);

            if (eczane == null)
            {
                return HttpNotFound();
            }
            return View(eczane);
        }

        // POST: EczaneNobet/Eczane/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Adi,Telefon,Adres,Email,Telefon2,EczaneGln,FaturaAdSoyad,VergiNumarasi,VergiDairesi,SehirId")] Eczane eczane)
        {
            var sehirler = _sehirService.GetList();

            if (ModelState.IsValid)
            {
                try
                {
                    _eczaneService.Update(eczane);
                    TempData["MessageSuccess"] = "Bilgiler başarıyla düzenlenmiştir";
                    ViewBag.SehirId = new SelectList(sehirler, "Id", "Adi", eczane.SehirId);
                    return View(eczane);

                    // return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Message"] = "ERROR:" + ex.Message.ToString();
                }
            }
            ViewBag.SehirId = new SelectList(sehirler, "Id", "Adi", eczane.SehirId);

            return View(eczane);
        }

        // GET: EczaneNobet/Eczane/Delete/5
        [Authorize(Roles = "Admin,Grup Yöneticisi,Eczane")]
        public ActionResult Delete(int id)
        {
            if (id < 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Eczane eczane = _eczaneService.GetById(id);
            if (eczane == null)
            {
                return HttpNotFound();
            }
            return View(eczane);
        }

        // POST: EczaneNobet/Eczane/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Eczane eczane = _eczaneService.GetById(id);
            try
            {
                _eczaneService.Delete(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "ERROR:" + ex.Message.ToString();
            }

            return View(eczane);
        }

    }
}