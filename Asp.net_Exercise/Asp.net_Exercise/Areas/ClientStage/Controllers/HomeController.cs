using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Asp.net_Exercise.Models;
using Newtonsoft.Json;
using System.Threading;

namespace Asp.net_Exercise.Areas.ClientStage.Controllers
{
    public class HomeController : Controller
    {
        DatabaseEntities DB = new DatabaseEntities();//建立公用DB物件
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Women()
        {
            return View();
        }
        public ActionResult Man()
        {
            return View();
        }
        public ActionResult Kids()
        {
            return View();
        }
        public ActionResult ProdDetails(int pid)
        {
            ViewBag.pid = pid;
            return View();
        }
    }    
}