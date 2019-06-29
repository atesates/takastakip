using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WM.Northwind.Business.Abstract.IlacTakip;
using WM.Northwind.Entities.Concrete.IlacTakip;
using WM.UI.Mvc.Models;

namespace WM.UI.Mvc.Areas.Kullanici.Controllers
{
    [Authorize]
    public class MenuAltController : Controller
    {
        private IMenuAltService _menuAltService;
        private IMenuService _menuService;

        public MenuAltController(IMenuAltService menuAltService,
                                 IMenuService menuService)
        {
            _menuAltService = menuAltService;
            _menuService = menuService;
        }

        // GET: EczaneNobet/MenuAlt
        [Authorize]
        public ActionResult Index()
        {
            var model = _menuAltService.GetList();
            
            return View(model);
        }

        // GET: EczaneNobet/MenuAlt/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuAlt menuAlt = _menuAltService.GetById(id);
            if (menuAlt == null)
            {
                return HttpNotFound();
            }
            return View(menuAlt);
        }

        // GET: EczaneNobet/MenuAlt/Create
        [Authorize]
        public ActionResult Create()
        {
            var menuler = _menuService.GetList();
            ViewBag.MenuId = new SelectList(menuler, "Id", "LinkText");
            return View();
        }

        // POST: EczaneNobet/MenuAlt/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "Id,MenuId,LinkText,ActionName,ControllerName,AreaName,SpanCssClass,PasifMi")] MenuAlt menuAlt)
        {
            if (ModelState.IsValid)
            {
                _menuAltService.Insert(menuAlt);
                return RedirectToAction("Index");
            }

            var menuler = _menuService.GetList();
            ViewBag.MenuId = new SelectList(menuler, "Id", "LinkText", menuAlt.MenuId);
            return View(menuAlt);
        }

        // GET: EczaneNobet/MenuAlt/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuAlt menuAlt = _menuAltService.GetById(id);
            if (menuAlt == null)
            {
                return HttpNotFound();
            }

            var menuler = _menuService.GetList();
            ViewBag.MenuId = new SelectList(menuler, "Id", "LinkText", menuAlt.MenuId);
            return View(menuAlt);
        }

        // POST: EczaneNobet/MenuAlt/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,MenuId,LinkText,ActionName,ControllerName,AreaName,SpanCssClass,PasifMi")] MenuAlt menuAlt)
        {
            if (ModelState.IsValid)
            {
                _menuAltService.Update(menuAlt);
                return RedirectToAction("Index");
            }
            var menuler = _menuService.GetList();
            ViewBag.MenuId = new SelectList(menuler, "Id", "LinkText", menuAlt.MenuId);
            return View(menuAlt);
        }

        // GET: EczaneNobet/MenuAlt/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            if (id < 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuAlt menuAlt = _menuAltService.GetById(id);
            if (menuAlt == null)
            {
                return HttpNotFound();
            }
            return View(menuAlt);
        }

        // POST: EczaneNobet/MenuAlt/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            MenuAlt menuAlt = _menuAltService.GetById(id);
            _menuAltService.Delete(id);
            return RedirectToAction("Index");
        }
        
    }
}
