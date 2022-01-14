using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Asp.net_Exercise.Models;
using Newtonsoft.Json;

namespace Asp.net_Exercise.Areas.BackStage.Controllers
{
    public class MemberController : Controller
    {
        DatabaseEntities DB = new DatabaseEntities();
        // GET: BackStage/Member
        public ActionResult Index()
        {
            var list = DB.Member.Select(m => new
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Phone = m.Phone,
                Gender = m.Gender,
                Join = m.Joindate,
                Google = m.GoogleId,
                FB = m.FacebookId,
                Status = m.Enable,
                Order = m.Order.Count
            }).ToList();
            ViewBag.data = JsonConvert.SerializeObject(list);
            ViewBag.active = "member";
            return View();
        }
    }
}