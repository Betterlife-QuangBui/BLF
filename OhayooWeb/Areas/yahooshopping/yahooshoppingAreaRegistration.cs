using System.Web.Mvc;

namespace OhayooWeb.Areas.yahooshopping
{
    public class yahooshoppingAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "yahooshopping";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "yahooshopping_default",
                "yahooshopping/{controller}/{action}/{id}/{name}/{cateid}/{catename}",
                new {
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional,
                    name = UrlParameter.Optional,
                    cateid = UrlParameter.Optional,
                    catename = UrlParameter.Optional
                }
            );
        }
    }
}