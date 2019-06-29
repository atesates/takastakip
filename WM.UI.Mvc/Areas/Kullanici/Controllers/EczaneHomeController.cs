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

namespace WM.UI.Mvc.Areas.Kullanici.Controllers
{
    [Authorize(Roles = "Admin,Eczane,Grup Yöneticisi")]
    public class EczaneHomeController : Controller
    {
        // GET: Kullanici/teklif
        #region ctor
        private ITeklifService _TeklifService;
        private IEczaneService _eczaneService;
        private ITeklifService _teklifService;
        private IEczaneGrupService _eczaneGrupService;
        private IEczaneUserService _eczaneUserService;
        private IUserService _userService;
        private ITeklifTurService _teklifTurService;
        private IGrupService _grupService;
        private ITeklifDurumService _teklifDurumService;
        private IYayinlamaTurService _yayinlamaTurService;
        private IAlimService _alimService;

        public EczaneHomeController(IEczaneGrupService eczaneGrupService,
                                IEczaneUserService eczaneUserService,
                                ITeklifService TeklifService,
                                IEczaneService eczaneService,
                                ITeklifService teklifService,
                                IUserService userService,
                                ITeklifTurService teklifTurService,
                                IGrupService grupService,
                                IYayinlamaTurService yayinlamaTurService,
                                ITeklifDurumService teklifDurumService,
                                IAlimService alimService)
        {
            _TeklifService = TeklifService;
            _teklifService = teklifService;
            _eczaneUserService = eczaneUserService;
            _eczaneGrupService = eczaneGrupService;
            _userService = userService;
            _grupService = grupService;
            _eczaneService = eczaneService;
            _teklifTurService = teklifTurService;
            _teklifDurumService = teklifDurumService;
            _yayinlamaTurService = yayinlamaTurService;
            _alimService = alimService;
        }
        #endregion

        public ActionResult Index(int? id)
        {
            int Id = Convert.ToInt32(id);
            var model = new EczaneRaporViewModel();

            if (Id == 0)
            {//tüm gruplar için
                var user = _userService.GetByUserName(User.Identity.Name);

                var eczaneGruplar = _eczaneGrupService.GetMyDetayListByUser(user);
                var eczaneGrupIdler = eczaneGruplar.Select(s => s.Id).Distinct().ToList();
                var grupIdler = eczaneGruplar.Select(s => s.GrupId).Distinct().ToList();
                var gruplar = _grupService.GetList().Where(w => grupIdler.Contains(w.Id)).ToList();

                #region teklifler
                var teklifIdler = _teklifService.GetList()
                    .Where(w => eczaneGrupIdler.Contains(w.TeklifiVerenEczaneGrupId))
                    .Select(s => s.Id).ToList();
                var alimlar = _alimService.GetDetayListByTeklifler(teklifIdler);

                var eczaneninYaptigiTekliflerdenYapilanAlimlar = alimlar.Where(w => w.AlimDurumId == 4).ToList();
                var tekliftenYapilanAlimTutari = eczaneninYaptigiTekliflerdenYapilanAlimlar.Sum(s => s.NetFiyat * s.Miktar);
                #endregion

                #region Alımlar
                var yapilanAlimIdler = _alimService.GetList()
                    .Where(w => eczaneGrupIdler.Contains(w.EczaneGrupId))
                    .Select(s => s.Id).ToList();
                var yapilanAlimlar = _alimService.GetDetaylar().Where(w => yapilanAlimIdler.Contains(w.Id)).ToList();
                var eczaneninYaptigiAlimlar = yapilanAlimlar.Where(w => w.AlimDurumId == 4).ToList();
                var eczaneninYaptigiAlimtutari = eczaneninYaptigiAlimlar.Sum(s => s.NetFiyat * s.Miktar);
                #endregion

                #region dropDownList

                // eczaneGruplar.Add(new Northwind.Entities.ComplexTypes.IlacTakip.EczaneGrupDetay { Id = 0, Adi = "Hepsi" });
                ViewBag.EczaneGrupId = new SelectList(gruplar, "Id", "Adi", Id);

                #endregion

                model = new EczaneRaporViewModel()
                {
                    Giderler = eczaneninYaptigiAlimtutari,
                    Gelirler = tekliftenYapilanAlimTutari,
                    EczaneGrupDetaylar = eczaneGruplar,
                    Bakiye = tekliftenYapilanAlimTutari - eczaneninYaptigiAlimtutari
                };
            }
            else
            {//seçili grup için
                var user = _userService.GetByUserName(User.Identity.Name);
                var eczaneGruplar = _eczaneGrupService.GetMyDetayListByUser(user).ToList();
                var eczaneGrupId = Id;

                var grupIdler = eczaneGruplar.Select(s => s.GrupId).Distinct().ToList();
                var gruplar = _grupService.GetList().Where(w => grupIdler.Contains(w.Id)).ToList();


                #region teklifler
                var teklifIdler = _teklifService.GetList()
                    .Where(w => w.TeklifiVerenEczaneGrupId == eczaneGrupId)
                    .Select(s => s.Id).ToList();
                var alimlar = _alimService.GetDetayListByTeklifler(teklifIdler).ToList();

                var eczaneninYaptigiTekliflerdenYapilanAlimlar = alimlar.Where(w => w.AlimDurumId == 4).ToList();
                var tekliftenYapilanAlimTutari = eczaneninYaptigiTekliflerdenYapilanAlimlar.Sum(s => s.NetFiyat * s.Miktar);
                #endregion

                #region Alımlar
                var yapilanAlimIdler = _alimService.GetList()
                    .Where(w => w.EczaneGrupId == eczaneGrupId)
                    .Select(s => s.Id).ToList();
                var yapilanAlimlar = _alimService.GetDetaylar().Where(w => yapilanAlimIdler.Contains(w.Id)).ToList();
                var eczaneninYaptigiAlimlar = yapilanAlimlar.Where(w => w.AlimDurumId == 4).ToList();
                var eczaneninYaptigiAlimtutari = eczaneninYaptigiAlimlar.Sum(s => s.NetFiyat * s.Miktar);
                #endregion

                #region dropDownList

                // eczaneGruplar.Add(new Northwind.Entities.ComplexTypes.IlacTakip.EczaneGrupDetay { Id = 0, Adi = "Hepsi" });
                ViewBag.EczaneGrupId = new SelectList(gruplar, "Id", "Adi", Id);

                #endregion

                model = new EczaneRaporViewModel()
                {
                    Giderler = eczaneninYaptigiAlimtutari,
                    Gelirler = tekliftenYapilanAlimTutari,
                    EczaneGrupDetaylar = eczaneGruplar,
                    Bakiye = tekliftenYapilanAlimTutari - eczaneninYaptigiAlimtutari
                };
            }
            return View(model);

        }
        public JsonResult getPivotRapor(int Id)
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var eczaneGruplar = _eczaneGrupService.GetMyDetayListByUser(user).ToList();
            var eczaneGrupId = Id;

            var grupIdler = eczaneGruplar.Select(s => s.GrupId).Distinct().ToList();
            var gruplar = _grupService.GetList().Where(w => grupIdler.Contains(w.Id)).ToList();


            #region teklifler
            var teklifIdler = _teklifService.GetList()
                .Where(w => w.TeklifiVerenEczaneGrupId == eczaneGrupId)
                .Select(s => s.Id).ToList();
            var alimlar = _alimService.GetDetayListByTeklifler(teklifIdler).ToList();

            var eczaneninYaptigiTekliflerdenYapilanAlimlar = alimlar.Where(w => w.AlimDurumId == 4).ToList();
            var tekliftenYapilanAlimTutari = eczaneninYaptigiTekliflerdenYapilanAlimlar.Sum(s => s.NetFiyat * s.Miktar);
            #endregion

            #region Alımlar
            var yapilanAlimIdler = _alimService.GetList()
                .Where(w => w.EczaneGrupId == eczaneGrupId)
                .Select(s => s.Id).ToList();
            var yapilanAlimlar = _alimService.GetDetaylar().Where(w => yapilanAlimIdler.Contains(w.Id)).ToList();
            var eczaneninYaptigiAlimlar = yapilanAlimlar.Where(w => w.AlimDurumId == 4).ToList();
            var eczaneninYaptigiAlimtutari = eczaneninYaptigiAlimlar.Sum(s => s.NetFiyat * s.Miktar);
            #endregion

            #region dropDownList

            // eczaneGruplar.Add(new Northwind.Entities.ComplexTypes.IlacTakip.EczaneGrupDetay { Id = 0, Adi = "Hepsi" });
            ViewBag.EczaneGrupId = new SelectList(gruplar, "Id", "Adi", Id);

            #endregion

            var model = new EczaneRaporViewModel()
            {
                Giderler = eczaneninYaptigiAlimtutari,
                Gelirler = tekliftenYapilanAlimTutari,
                EczaneGrupDetaylar = eczaneGruplar,
                Bakiye = tekliftenYapilanAlimTutari - eczaneninYaptigiAlimtutari
            };

            var jsonResult = Json(model, JsonRequestBehavior.AllowGet);
            return jsonResult;

        }
        public JsonResult getAyrintiliRapor(int Id)
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var eczaneGruplar = _eczaneGrupService.GetDetayListByUser(user).ToList();
            var grupIdler = eczaneGruplar.Select(s => s.GrupId).Distinct().ToList();
            var gruplar = _grupService.GetList().Where(w => grupIdler.Contains(w.Id)).ToList();
            ViewBag.EczaneGrupId = new SelectList(gruplar, "Id", "Adi", Id);

            var eczaneGruplarSeciliEczaneGrupIdIcin = _eczaneGrupService.GetDetaylar().Where(w => w.GrupId == Id).ToList();
            //var grupId = Id;
            var eczaneGrupIdler = eczaneGruplar.Where(w => w.GrupId == Id).Select(s => s.Id).Distinct().ToList();
            var teklifIdler = _teklifService.GetList()
                .Where(w => eczaneGrupIdler.Contains(w.TeklifiVerenEczaneGrupId))
                .Select(s => s.Id).ToList();
            var alimlar = _alimService.GetDetayListByTeklifler(teklifIdler).Where(w => w.AlimDurumId == 4).ToList();//stoğa giren ilaçlar değerlendirmeye alınacak
            var teklifler = _teklifService.GetMyDetayListByEczaneGruplar(eczaneGruplarSeciliEczaneGrupIdIcin).ToList();
            var eczaneler = _eczaneService.GetListByUser(user);


            var jsonResult = Json(alimlar, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }


        public JsonResult getAyrintiliRapor2(int Id)//eczaneGrupId
        {
            var user = _userService.GetByUserName(User.Identity.Name);
            var eczaneGruplar = _eczaneGrupService.GetDetayListByUser(user).ToList();
            var grupIdler = eczaneGruplar.Select(s => s.GrupId).Distinct().ToList();
            var gruplar = _grupService.GetList().Where(w => grupIdler.Contains(w.Id)).ToList();
            ViewBag.EczaneGrupId = new SelectList(gruplar, "Id", "Adi", Id);

            var eczaneGruplarSeciliEczaneGrupIdIcin = _eczaneGrupService.GetDetaylar().Where(w => w.GrupId == Id).ToList();
            //var grupId = Id;
            var eczaneGrupIdler = eczaneGruplar.Where(w => w.GrupId == Id).Select(s => s.Id).Distinct().ToList();
            var teklifIdler = _teklifService.GetList()
                .Where(w => eczaneGrupIdler.Contains(w.TeklifiVerenEczaneGrupId))
                .Select(s => s.Id).ToList();
            var alimlar = _alimService.GetDetayListByTeklifler(teklifIdler).Where(w => w.AlimDurumId == 4).ToList();//stoğa giren ilaçlar değerlendirmeye alınacak
            var teklifler = _teklifService.GetMyDetayListByEczaneGruplar(eczaneGruplarSeciliEczaneGrupIdIcin).ToList();
            var eczaneler = _eczaneService.GetListByUser(user);
            var eczaneGrupId = _eczaneGrupService.GetListByUser(user).Where(w => w.GrupId == Id).Select(s => s.Id).FirstOrDefault();


            //sadece o eczanenin yaptığı alımlar:
            var alimlarForAlimTip = alimlar.Where(w => w.EczaneGrupId == eczaneGrupId).ToList();
            var modelAlimlar = new List<TeklifAlimViewModel>();
            foreach (var item in alimlarForAlimTip)
            {
                modelAlimlar.Add(new TeklifAlimViewModel
                {
                    Eczane1Adi = item.EczaneAdi,
                    Eczane2Adi = item.TeklifVerenEczaneAdi,
                    IlacAdi = item.IlacAdi,
                    Miktari = item.Miktar * -1,
                    NetFiyat = item.NetFiyat,
                    AlimId = item.Id,
                    TeklifId = item.TeklifId,
                    Tip = "Alım"
                });
            }
            //sadece o eczanede yapılan alımlar
            var alimlarForTeklifTip = alimlar.Where(w => w.TeklifVerenEczaneGrupId == eczaneGrupId).ToList();
            foreach (var item in alimlarForTeklifTip)
            {
                modelAlimlar.Add(new TeklifAlimViewModel
                {
                    Eczane1Adi = item.TeklifVerenEczaneAdi,
                    Eczane2Adi = item.EczaneAdi,
                    IlacAdi = item.IlacAdi,
                    Miktari = item.Miktar,
                    NetFiyat = item.NetFiyat,
                    AlimId = item.Id,
                    TeklifId = item.TeklifId,
                    Tip = "Teklif"

                });
            }

            var jsonResult = Json(modelAlimlar, JsonRequestBehavior.AllowGet);
            return jsonResult;
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
    }
}