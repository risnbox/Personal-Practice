using Asp.net_Exercise.Models;
using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Asp.net_Exercise.Areas.BackStage.Controllers
{
    public class HomeController : Controller
    {
        DatabaseEntities DB = new DatabaseEntities();
        // GET: BackStage/Home
        public ActionResult Index()
        {
            //訂單,營業額
            var oList = DB.Order.ToList();
            var o7 = new List<Order>();
            var o31 = new List<Order>();
            int? t7d = 0;
            int? t31d = 0;
            foreach (var o in oList)
            {
                for (int i = -6; 0 >= i; i++)
                {
                    if (DateTime.Now.AddDays(i).Date.Equals(Convert.ToDateTime(o.PaymentDate).Date))
                    {
                        o7.Add(o);
                        o31.Add(o);
                        t7d += o.Total;
                        t31d += o.Total;
                    }
                }
                for (int k = -30; -7 >= k; k++)
                {
                    if (DateTime.Now.AddDays(k).Date.Equals(Convert.ToDateTime(o.PaymentDate).Date))
                    {
                        t31d += o.Total;
                        o31.Add(o);
                    }
                }
            }
            //會員
            var mList =  DB.Member.Select(m=>m.Joindate).ToList();
            int t = 0;
            int d = 0;
            foreach(var x in mList)
            {
                for(int i = -6; 0 >= i; i++)
                {
                    if (DateTime.Now.AddDays(i).Date.Equals(Convert.ToDateTime(x).Date)) { t++; d++; }
                }
                for(int k = -30; -7 >= k; k++)
                {
                    if (DateTime.Now.AddDays(k).Date.Equals(Convert.ToDateTime(x).Date)) { d++; }
                }
            }
            ViewBag.t7d = t7d;
            ViewBag.t31d = t31d;
            ViewBag.O7d = o7.Count;
            ViewBag.O31d = o31.Count;
            ViewBag.M31d = d;
            ViewBag.M7d = t;
            ViewBag.active = "index";
            return View();
        }
    }
}