using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WM.Northwind.Business.Abstract.IlacTakip;
using WM.Northwind.Entities.Concrete.IlacTakip;

namespace WM.TakasTakip.WebApi.Controllers
{
    public class AlimlarController : ApiController
    {
        private IAlimService _AlimService;

        public AlimlarController(IAlimService AlimService)
        {
            _AlimService = AlimService;
        }

        public List<Alim> Get()
        {
            return _AlimService.GetList();
        }
    }
}
