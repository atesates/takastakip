using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Core.Entities;
using WM.Northwind.Entities.Concrete.IlacTakip;

namespace WM.Northwind.Entities.ComplexTypes.IlacTakip
{
    public class UserRoleDetay : IComplexType
    {
        public int Id { get; set; }
        [Display(Name = "Kullanıcı Adı")]
        public string KullaniciAdi { get; set; }
        [Display(Name = "Rol")]
        public string RoleName { get; set; }
        [Display(Name = "Açıklama")]
        public int RoleId { get; set; }
        public int UserId { get; set; }
    }
}
