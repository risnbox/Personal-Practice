using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;

namespace Asp.net_Exercise
{
    public class Global : HttpApplication
    {
        void Session_Start(object sender, EventArgs e)//新使用者請求session時執行
        {
            Application.Lock();
            Application["count"] = Convert.ToInt32(Application["count"].ToString()) + 1;
            Application.UnLock();
            Session["Count"] = Application["count"];
        }
        void Application_Start(object sender, EventArgs e)
        {
            // 應用程式啟動時執行的程式碼
            Application["count"] = 0;
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        private bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
        }

        void Application_PostAuthorizeRequest()
        {
            if (IsWebApiRequest())
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }
    }
}