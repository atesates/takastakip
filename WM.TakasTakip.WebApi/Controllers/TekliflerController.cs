using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Web.Http;
using WM.Northwind.Business.Abstract.IlacTakip;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using WM.Northwind.Entities.Concrete.IlacTakip;

namespace WM.TakasTakip.WebApi.Controllers
{
    public class TekliflerController : ApiController
    {
        private ITeklifService _teklifService;

        public TekliflerController(ITeklifService teklifService)
        {
            _teklifService = teklifService;
        }
        [HttpGet]
        public List<TeklifDetay> Get()
        {
            return _teklifService.GetDetaylar();
        }
        [HttpPost]
        public void Ekle(Teklif teklif)
        {
            if (ModelState.IsValid)
            {
                _teklifService.Insert(teklif);
            }
        }
    }
}
