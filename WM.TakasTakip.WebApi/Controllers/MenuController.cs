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
    public class MenuController : ApiController
    {
        private IMenuRoleService _menuRoleService;
        private IUserRoleService _userRoleService;

        public MenuController(IMenuRoleService menuRoleService,
                            IUserRoleService userRoleService
                            )
        {
            _menuRoleService = menuRoleService;
            _userRoleService = userRoleService;
        }
        [HttpGet]
        public List<MenuModel> Get(int id)
        {
            List<MenuRoleDetay> menuRoleDetayList = new List<MenuRoleDetay>();
            int roleId = _userRoleService.GetListByUserId(id).Select(s => s.RoleId).FirstOrDefault();
            menuRoleDetayList = _menuRoleService.GetDetayList(roleId).ToList(); 
            var menuModelList = new List<MenuModel>();

            foreach (var item in menuRoleDetayList)
            {
                MenuModel menuModel = new MenuModel();
                menuModel.MenuId = item.MenuId;
                menuModel.MenuAdi = item.MenuAdi;
                menuModel.UserId = id;
                menuModel.RoleId = item.RoleId;
                menuModelList.Add(menuModel);
                
            }

            return menuModelList;
            //menuId ve menuAdi içeren anonim bir list deönecek
        }
    }
}
