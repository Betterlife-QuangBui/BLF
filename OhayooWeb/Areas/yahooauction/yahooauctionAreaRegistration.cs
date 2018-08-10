using System.Web.Mvc;

namespace OhayooWeb.Areas.yahooauction
{
    public class yahooauctionAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "yahooauction";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "yahooauction_default",
                "yahooauction/{controller}/{action}/{id}/{name}/{cateid}/{catename}",
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