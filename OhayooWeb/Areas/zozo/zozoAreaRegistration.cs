using System.Web.Mvc;

namespace OhayooWeb.Areas.zozo
{
    public class zozoAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "zozo";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "zozo_default",
                "zozo/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}