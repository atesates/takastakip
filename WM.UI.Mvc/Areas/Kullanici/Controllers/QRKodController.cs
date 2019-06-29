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
using System.Text;

namespace WM.UI.Mvc.Areas.Kullanici.Controllers
{
    public class QRKodController : Controller
    {
        // GET: Kullanici/QRKod
        #region ctor
        private IIlacService _ilacService;
        private IEczaneService _eczaneService;
        private IAlimService _alimService;
        private IEczaneGrupService _eczaneGrupService;
        private IGrupService _grupService;
        private IUserService _userService;
        private IAlimDurumService _alimDurumService;
        private ITeklifService _teklifService;
        private IITStransferDurumService _iTStransferDurumService;
        private IQRKodService _qRKodService;

        public QRKodController(IEczaneGrupService eczaneGrupService,
                                IAlimService AlimService,
                                IIlacService ilacService,
                                IEczaneService eczaneService,
                                IGrupService grupService,
                                IAlimService alimService,
                                ITeklifService teklifService,
                                IAlimDurumService alimDurumService,
                                IITStransferDurumService iTStransferDurumService,
                                IQRKodService qRKodService,
                                IUserService userService)
        {
            _ilacService = ilacService;
            _eczaneService = eczaneService;
            _grupService = grupService;
            _alimService = alimService;
            _userService = userService;
            _teklifService = teklifService;
            _iTStransferDurumService = iTStransferDurumService;
            _alimDurumService = alimDurumService;
            _eczaneGrupService = eczaneGrupService;
            _qRKodService = qRKodService;
        }
        #endregion
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create(int? id)
        {
            QRKod QRKod = new QRKod();
            var user = _userService.GetByUserName(User.Identity.Name);
            if (id == null)
                return RedirectToAction("Index", "Teklif");

            var alimId = Convert.ToInt32(id);
            var teklifId = _alimService.GetById(Convert.ToInt32(alimId)).TeklifId;
            var ilacId = _teklifService.GetById(Convert.ToInt32(teklifId)).IlacId;
            var ilacAdi = _ilacService.GetById(ilacId).Adi;

            ViewBag.Ilac = ilacAdi;
            ViewBag.AlimId = alimId;
            QRKod.AlimId = alimId;
            var qRKodlar = _qRKodService.GetListByAlimId(alimId).Select(s=>s.QRKodu);
            ViewBag.QRKodu = qRKodlar;

            StringBuilder sb = new StringBuilder();
            foreach (var item in qRKodlar)
            {
                sb.Append(item + "\n");
            }

            var model = new QRKodDetayViewModel()
            {
                QRKodlar = sb.ToString(),
                AlimId = alimId
            };
           

            // QRKod.QRKodu = qRKodlar;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(QRKodDetayViewModel qRKodDetayViewModel)
        {
            QRKod qRKod =  new QRKod();
            qRKod.AlimId = qRKodDetayViewModel.AlimId;
            var qRKodlar = qRKodDetayViewModel.QRKodlar.ToString();
            string[] tokens = qRKodlar.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None);
            foreach (var item in tokens)
            {
                if(item.Length !=0)
                    qRKod.QRKodu = item.ToString();

                if (ModelState.IsValid)
                {
                    try
                    {
                        _qRKodService.Insert(qRKod);

                    }
                    catch (Exception ex)
                    {
                        TempData["Message"] = "ERROR:" + ex.Message.ToString();
                    }
                }
            }
            return RedirectToAction("Index", "GonderdiklerimeczanelereGore");
           // return View(QRKod);


        }

        public ActionResult Edit(int? id)
        {
            int Id = 0;
            try
            {
                Id = Convert.ToInt32(id);
            }
            catch
            {
                return RedirectToAction("Index", "GonderdiklerimeczanelereGore");

            }
            if (id < 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QRKod QRKod = new QRKod();
            var alimId = Convert.ToInt32(id);
            var teklifId = _alimService.GetById(Convert.ToInt32(alimId)).TeklifId;
            var ilacId = _teklifService.GetById(Convert.ToInt32(teklifId)).IlacId;
            var ilacAdi = _ilacService.GetById(ilacId).Adi;


            var qRKodlar = _qRKodService.GetListByAlimId(alimId).Select(s => s.QRKodu);
            ViewBag.QRKodu = qRKodlar;
            StringBuilder sb = new StringBuilder();
            foreach (var item in qRKodlar)
            {
                sb.Append(item + "\n");
            }

            var model = new QRKodDetayViewModel()
            {
                QRKodlar = sb.ToString(),
                AlimId = alimId
            };

            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AlimDurumId,QRKodu")] QRKod QRKodu)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _qRKodService.Update(QRKodu);
                    TempData["MessageSuccess"] = "QRKodu başarıyla düzenlenmiştir";
                  
                }
                catch (Exception ex)
                {
                    TempData["MessageDanger"] = "ERROR:" + ex.InnerException.InnerException.Message.ToString();
                }
            }

          
            return View(QRKodu);
        }

    }
}