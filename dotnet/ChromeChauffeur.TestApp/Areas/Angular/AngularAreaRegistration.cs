using System.Web.Mvc;

namespace ChromeChauffeur.TestApp.Areas.Angular
{
    public class AngularAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Angular";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Angular_default",
                "Angular/{controller}/{action}/{id}",
                new { controller = "AngularHome", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}