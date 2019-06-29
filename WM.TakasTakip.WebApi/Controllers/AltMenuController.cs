using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Web.Http;
using WM.Northwind.Business.Abstract.Authorization;
using WM.Northwind.Business.Abstract.IlacTakip;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using WM.Northwind.Entities.Concrete.IlacTakip;
using WM.TakasTakip.WebApi.Models;

namespace WM.TakasTakip.WebApi.Controllers
{
    public class AltMenuController : ApiController
    {
        private IMenuAltRoleService _menuAltRoleService;
        private IUserRoleService _userRoleService;

        public AltMenuController(IMenuAltRoleService menuAltRoleService,
                            IUserRoleService userRoleService)
        {
            _menuAltRoleService = menuAltRoleService;
            _userRoleService = userRoleService;
        }
        [HttpGet]
        public List<AltMenuModel> Get(int id)
        {
            List<MenuAltRoleDetay> menuAltRoleDetayList = new List<MenuAltRoleDetay>();
            int roleId = _userRoleService.GetListByUserId(id).Select(s => s.RoleId).FirstOrDefault();
            menuAltRoleDetayList = _menuAltRoleService.GetDetayList(roleId).ToList();

            var altMenuModelList = new List<AltMenuModel>();
            foreach (var item in menuAltRoleDetayList)
            {
                AltMenuModel altMenuModel = new AltMenuModel();
                altMenuModel.MenuId = item.MenuId;
                altMenuModel.AltMenuAdi = item.MenuAltAdi;
                altMenuModel.UserId = id;
                altMenuModel.RoleId = item.RoleId;
                altMenuModelList.Add(altMenuModel);
            }

            return altMenuModelList;
            //menuId ve menuAdi içeren anonim bir list deönecek
        }
    }
}
