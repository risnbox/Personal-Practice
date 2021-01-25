﻿using System.Web.Mvc;

namespace Asp.net_Exercise.Areas.BackStage
{
    public class BackStageAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "BackStage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                name: "BackStage_default",
                url: "BackStage/{controller}/{action}/{id}",
                defaults: new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Asp.net_Exercise.Areas.BackStage.Controllers" }
            );
        }
    }
}