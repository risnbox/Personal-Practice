using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Asp.net_Exercise.Models;
using Newtonsoft.Json;

namespace Asp.net_Exercise.Areas.BackStage.Controllers
{
    public class OrderController : Controller
    {
        DatabaseEntities db = new DatabaseEntities();
        public ActionResult Index()
        {
            ViewBag.active = "order";
            return View();
        }
        public ActionResult Window(string Oid)
        {
            var list = (from order in db.Order
                        where order.MerchantTradeNo == Oid
                        join detail in db.OrderDetail on order.Id equals detail.Order_Id
                        join quantity in db.Quantity on detail.Quantity_Id equals quantity.Id
                        select new
                        {
                            name = quantity.ProdFeature.Product.Name,
                            color_size = quantity.ProdFeature.Color.Name + "-" + quantity.ProdFeature.Size.Name,
                            price = quantity.ProdFeature.Product.Price,
                            quantity = quantity.Qty,
                            total = order.Total
                        }).ToList();
            ViewBag.data = JsonConvert.SerializeObject(list);
            ViewBag.active = "oreder";
            return View();
        }
    }
}