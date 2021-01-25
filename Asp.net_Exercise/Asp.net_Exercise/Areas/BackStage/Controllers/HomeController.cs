using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Asp.net_Exercise.Areas.BackStage.Controllers
{
    public class HomeController : Controller
    {
        // GET: BackStage/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}