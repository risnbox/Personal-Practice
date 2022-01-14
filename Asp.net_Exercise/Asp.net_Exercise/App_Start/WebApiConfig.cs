using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Asp.net_Exercise
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 路由
            config.MapHttpAttributeRoutes();//啟用屬性路由

            //Webapi路由設定，與一般Route不同MapHttpRoute將無視命名空間尋找控制器且無法添加namespaces參數                                
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}"
            );
        }
    }
}
