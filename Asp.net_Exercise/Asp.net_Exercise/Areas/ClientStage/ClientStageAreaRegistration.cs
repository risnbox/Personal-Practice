using System.Web.Mvc;

namespace Asp.net_Exercise.Areas.ClientStage
{
    public class ClientStageAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ClientStage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                name: "ClientStage_default",
                url: "{controller}/{action}",
                defaults: new { Controller = "Home", action="Index" },
                namespaces: new[] { "Asp.net_Exercise.Areas.ClientStage.Controllers" }
            );
        }
    }
}