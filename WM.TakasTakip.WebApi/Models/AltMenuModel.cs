using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WM.TakasTakip.WebApi.Models
{
    public class AltMenuModel
    {
        public int MenuId { get; set; }
        public string AltMenuAdi { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }

    }
}