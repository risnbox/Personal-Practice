using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Asp.net_Exercise.Models;
using Newtonsoft.Json;


namespace Asp.net_Exercise.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        DatabaseEntities DB = new DatabaseEntities();
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
        public string GetProd(string TYPE)
        {
            var data = (from Img in DB.Img
                        join PCT in DB.Prod_Class_Type on TYPE equals PCT.Type.TypeName
                        join PM in DB.Prod_Img on PCT.Pid equals PM.Pid
                        where Img.Id == PM.Mid && Img.Type == "previewed"
                        select new
                        {
                            img = Img,
                            prod = PM.Product
                        }).ToList();
            var json = JsonConvert.SerializeObject(data);
            json = json.Replace(" ", "");
            return json;
        }
        public ActionResult ProdDetails(int Pid)
        {
            string keep = null;
            if (Session["Member"] != null)
            {
                var U = Convert.ToInt32(Session["Member"].ToString());
                if (DB.Keep.Where(m => m.Userid == U && m.Prodid == Pid).FirstOrDefault() != null)
                {
                    keep = "true";
                }
            }
            var data = (from img in DB.Img
                        join prod in DB.Product on Pid equals prod.Id
                        join PM in DB.Prod_Img on Pid equals PM.Pid
                        where img.Id == PM.Mid
                        select new
                        {
                            prod = prod,
                            img = img,
                            keep = keep
                        }
                      ).ToList();
            string json = JsonConvert.SerializeObject(data);
            var V = json.Replace(" ","");
            ViewBag.json = V;
            return View();
        }
        public int AddCart(int pid, string color, string size)
        {
            if (Session["Cart"] == null)
            {
                return 1;
            }
            var d = Convert.ToInt32(Session["Member"].ToString());
            if (DB.Member.Where(m => m.Id ==  d && m.Enable == 0).FirstOrDefault() != null)
            {
                return 2;
            }
            else
            {
                var c = Convert.ToInt32(Session["Cart"].ToString());
                var data = (from Cart in DB.ShoppingCar
                            where Cart.Id == c
                            join C in DB.Color on color equals C.Name
                            join S in DB.Size on size equals S.Name
                            select new
                            {
                                Cart = Cart,
                                C = C.Id,
                                S = S.Id
                            }).FirstOrDefault();
                var pf = DB.ProdFeature.Where(m => m.Pid == pid && m.Cid == data.C && m.Sid == data.S).FirstOrDefault();
                if (pf == null)
                {
                    pf = new ProdFeature() { Pid = pid, Cid = data.C, Sid = data.S };
                    DB.ProdFeature.Add(pf);
                }
                var q = DB.Quantity.Where(m => m.PFid == pf.Id && m.Cid == c).FirstOrDefault();
                if (q != null)
                {
                    q.Qty++;
                }
                else
                {
                    q = new Quantity() { PFid = pf.Id, Cid = c, Qty = 1 };
                    DB.Quantity.Add(q);
                }
                DB.SaveChanges();
                return 0;
            }
            
        }
    }    
}