using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using WM.Northwind.Business.Abstract.Authorization;
using WM.Northwind.Business.Abstract.IlacTakip;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using WM.Northwind.Entities.Concrete.IlacTakip;
using WM.TakasTakip.WebApi.Models;

namespace WM.TakasTakip.WebApi.Controllers
{
    public class AccountController : ApiController
    {
        #region ctor
        private IUserRoleService _userRoleService;
        private IUserService _userService;
        private IGrupService _grupService;
        private IEczaneUserService _eczaneUserService;
        private IEczaneGrupService _eczaneGrupService;
        private IEczaneService _eczaneService;

        public AccountController(IUserRoleService userRoleService, IUserService userService,
            IEczaneUserService eczaneUserService,
            IEczaneGrupService eczaneGrupService,
            IEczaneService eczaneService,
            IGrupService grupService)
        {
            _userRoleService = userRoleService;
            _userService = userService;
            _eczaneUserService = eczaneUserService;
            _eczaneGrupService = eczaneGrupService;
            _eczaneService = eczaneService;
            _grupService = grupService;
        }
        #endregion
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
            string token = string.Empty;
            string pwd = string.Empty;
            if (headers.Contains("username"))
            {
                token = headers.GetValues("username").First();
            }
            if (headers.Contains("password"))
            {
                pwd = headers.GetValues("password").First();
            }
            //code to authenticate and return some thing
            int userId = _userService.GetByUserNameAndPassword(token, pwd).Id;
        

            if (userId == 0)
            {
                return NotFound();
            }
            return Ok(userId);
        }
    //    [HttpPost]//post
    //    public AccountModel Get(HttpRequestMessage request)
    //    {
    //        IEnumerable<string> headerValues = request.Headers.GetValues("Authorization");
    //        var id = headerValues.FirstOrDefault();

    //        byte[] data = Convert.FromBase64String(id);
    //        string decodedString = Encoding.UTF8.GetString(data);
    //        string[] str = decodedString.Split(':');
    //        string userName = "";
    //        string password = "";
    //        int i = 0;
    //        foreach (var item in str)
    //        {
    //            if (i == 0)
    //                userName = item.ToString();
    //            else
    //                password = item.ToString();
    //            i++;
    //        }
    //        int userId = _userService.GetByUserNameAndPassword(userName, password).Id;
    //        int eczaneId = _eczaneUserService.GetListByUserId(userId).Select(s => s.EczaneId).FirstOrDefault();
    //        string eczaneAdi = _eczaneService.GetById(eczaneId).Adi;
    //        int roleId = _userRoleService.GetListByUserId(userId).Select(s => s.RoleId).FirstOrDefault();
    //        List<int> grupIdList = _grupService.GetListByUser(_userService.GetByUserNameAndPassword(userName, password)).Select(s=>s.Id).ToList();

    //        var model = new AccountModel()
    //        {
    //            UserId = userId,
    //            RoleId = roleId,
    //            EczaneId = eczaneId,
    //            EczaneAdi = eczaneAdi,
    //            UserName = userName,
    //            GrupIdList = grupIdList
    //        };

    //        return model;
    //    }
    }
}
