using System.Web.Mvc;

namespace OhayooWeb.Areas.rakuten
{
    public class rakutenAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "rakuten";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "rakuten_default",
                "rakuten/{controller}/{action}/{id}/{name}/{cateid}/{catename}",
                new
                {
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