using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WM.Northwind.Business.Abstract.Authorization;
using WM.Northwind.Business.Abstract.IlacTakip;
using WM.Northwind.Entities.Concrete.IlacTakip;

namespace WM.UI.Mvc.Areas.Kullanici.Controllers
{
    public class GrupController : Controller
    {
        private IGrupService _grupService;
        private IEczaneGrupService _eczaneGrupService;
        private IEczaneUserService _eczaneUserService;
        private IUserService _userService;
        private IUserRoleService _userRoleService;

        public GrupController(IGrupService grupService,
                                 IUserRoleService userRoleService,
                                 IEczaneGrupService eczaneGrupService,
                                 IEczaneUserService eczaneUserService,
                                 IUserService userService)
        {
            _grupService = grupService;
            _userService = userService;
            _eczaneUserService = eczaneUserService;
            _userRoleService = userRoleService;
            _eczaneGrupService = eczaneGrupService;
        }

      
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
            if (Convert.ToInt32(min) == 1)
            {
                var model = _grupService.GetList();
                return View(model);
            }
            else
            {
                var eczaneler = _eczaneUserService.GetListByUserId(user.Id).Select(s => s.EczaneId).ToList();
                var model = _eczaneGrupService.GetList().Where(w => eczaneler.Contains(w.EczaneId))
                    .Select(s => s.Grup).ToList();
                return View(model);           
            }
        }
     
        // GET: EczaneNobet/Eczane/Details/5
        public ActionResult Details(int id)
        {
            if (id < 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grup grup = _grupService.GetById(id);
            if (grup == null)
            {
                return HttpNotFound();
            }
            return View(grup);
        }

        // GET: EczaneNobet/Eczane/Create
        [Authorize(Roles = "Admin,Grup Yöneticisi, Eczane")]
        public ActionResult Create()
        {
            Grup grup = new Grup();

            return View(grup);
        }

        // POST: EczaneNobet/Eczane/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Adi,BaslangicTarihi, BitisTarihi")] Grup grup)
        {
            if (ModelState.IsValid)
            {
                _grupService.Insert(grup);
                return RedirectToAction("Index");
            }

            return View(grup);
        }

        // GET: EczaneNobet/Eczane/Edit/5
        [Authorize(Roles = "Admin,Grup Yöneticisi,Eczane")]
        public ActionResult Edit(int id)
        {
            if (id < 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grup grup = _grupService.GetById(id);
            if (grup == null)
            {
                return HttpNotFound();
            }
            return View(grup);
        }

        // POST: EczaneNobet/Eczane/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Adi,BaslangicTarihi, BitisTarihi")] Grup grup)
        {
            if (ModelState.IsValid)
            {
                _grupService.Update(grup);
                return RedirectToAction("Index");
            }
            return View(grup);
        }

        // GET: EczaneNobet/Eczane/Delete/5
        [Authorize(Roles = "Admin,Grup Yöneticisi,Eczane")]
        public ActionResult Delete(int id)
        {
            if (id < 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grup grup = _grupService.GetById(id);
            if (grup == null)
            {
                return HttpNotFound();
            }
            return View(grup);
        }

        // POST: EczaneNobet/Eczane/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Grup grup = _grupService.GetById(id);
            _grupService.Delete(id);
            return RedirectToAction("Index");
        }

    }
}