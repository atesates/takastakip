using System.Web.Mvc;

namespace WM.UI.Mvc.Areas.Kullanici
{
    public class KullaniciAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Kullanici";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {

            context.MapRoute(
                "Create",
                "Kullanici/{controller}/Ekle",
                new { action = "Create" }
            );

            context.MapRoute(
                "Edit",
                "Kullanici/{controller}/Duzenle/{id}",
                new { action = "Edit", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Delete",
                "Kullanici/{controller}/Sil/{id}",
                new { action = "Delete", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Details",
                "Kullanici/{controller}/Detaylar/{id}",
                new { action = "Details", id = UrlParameter.Optional }
            );

          
            context.MapRoute(
                "Kullanici_default",
                "Kullanici/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}