using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WM.TakasTakip.WebApi.Models
{
    public class AccountModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int RoleId { get; set; }
        public int EczaneId { get; set; }
        public List<int> GrupIdList { get; set; }
        public string EczaneAdi { get; set; }


    }
}