using Asp.net_Exercise.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Asp.net_Exercise.Controllers
{
    public class CartController : Controller
    {
        DatabaseEntities DB = new DatabaseEntities();
        
        public ActionResult CarPartial()
        {
            var c = Convert.ToInt32(Session["Cart"].ToString());
            var data = (from Cart in DB.ShoppingCar
                        where Cart.Id == c
                        join Qty in DB.Quantity on c equals Qty.Cid
                        join pf in DB.ProdFeature on Qty.PFid equals pf.Id
                        join prod in DB.Product on pf.Pid equals prod.Id
                        select new
                        {
                            name = prod.Name,
                            quantity = Qty.Qty,
                            color = pf.Color.Name,
                            size = pf.Size.Name,
                            pid = pf.Pid
                        }
                        ).ToList();
            string json = JsonConvert.SerializeObject(data);
            ViewBag.Cdata = json.Replace(" ", "");
            return PartialView();
        }
        public ActionResult CheckOut()
        {
            var d = Convert.ToInt32(Session["Member"].ToString());
            if (DB.Member.Where(m => m.Id == d && m.Enable == 0).FirstOrDefault() != null)
            {
                return RedirectToAction("EmailValidationView");
            }
            if (DB.Member_Store.Where(m => m.Member_Id == d).FirstOrDefault() == null)
            {
                TempData["CartError"] = "請先至會員會員中心選擇711門市";
                return RedirectToAction("Location","store");
            }
            var c = Convert.ToInt32(Session["Cart"].ToString());
            var data = (from Cart in DB.ShoppingCar
                        where Cart.Id == c
                        join Qty in DB.Quantity on c equals Qty.Cid
                        join Pf in DB.ProdFeature on Qty.PFid equals Pf.Id
                        join P in DB.Product on Pf.Pid equals P.Id
                        join PI in DB.Prod_Img on P.Id equals PI.Pid
                        join I in DB.Img on new { A = PI.Mid, B = "previewed" } equals new { A = I.Id, B = I.Type }
                        //多欄位比較時，欄位名及型別必須完全一致(int? != int 必須在此查詢之前先查好丟進變數或直接更改Model型別(前提是該欄位不會使用到null型別)
                        select new
                        {
                            Name = P.Name,
                            Price = P.Price,
                            Feature = Pf.Color.Name + "-" + Pf.Size.Name,
                            Img = I.FileName,
                            Qty = Qty.Qty,
                            Total = P.Price * Qty.Qty
                        }
                        ).ToList();
            var Json = JsonConvert.SerializeObject(data);
            ViewBag.json = Json.Replace(" ", "");
            //------------------------------------------
            var s = DB.Member_Store.Where(m => m.Member_Id == d).ToList();
            var M = DB.Member.Where(m => m.Id == d).FirstOrDefault();
            var S = new List<string>();
            foreach (var x in s)
            {
                S.Add(x.Store.StoreName + "-" + x.Store.StoreAddress);
            }
            var l = new SelectList(S);
            ViewBag.storelist = l;
            ViewBag.Name = M.Name;
            ViewBag.Phone = M.Phone;
            ViewBag.Email = M.Email;
            return View();
        }
        public Quantity SearchQuantity(string Name, string Feature)
        {
            var d = Convert.ToInt32(Session["Member"].ToString());
            var F = Feature.Split('-');
            var color = F[0];
            var size = F[1];
            var S = DB.ShoppingCar.Where(m => m.Userid == d).FirstOrDefault();
            var data = DB.Quantity.Where(m => m.Cid == S.Id && m.ProdFeature.Color.Name == color && m.ProdFeature.Size.Name == size && m.ProdFeature.Product.Name == Name).FirstOrDefault();
            return data;
        }

        public string DeleteCart(string Name, string Feature)
        {
            try
            {
                var data = SearchQuantity(Name, Feature);
                DB.Quantity.Remove(data);
                DB.SaveChanges();
                return "";
            }
            catch (Exception e)
            {
                return "伺服器錯誤 code:" + e.ToString();
            }
        }
        public string QtyAdd(string Name, string Feature)
        {
            try
            {
                var data = SearchQuantity(Name, Feature);
                data.Qty++;
                DB.SaveChanges();
                return "";
            }
            catch (Exception e)
            {
                return "伺服器錯誤 code:" + e.ToString();
            }
        }
        public string QtyCut(string Name, string Feature)
        {
            try
            {
                var data = SearchQuantity(Name, Feature);
                data.Qty--;
                DB.SaveChanges();
                return "";
            }
            catch (Exception e)
            {
                return "伺服器錯誤 code:" + e.ToString();
            }
        }
        [HttpPost]
        public string VerifyOrder(Order order, string Sname)
        {
            if (ModelState.IsValid)
            {
                var t = Sname.Split('-');
                var S = t[0];
                var sd = DB.Store.Where(m => m.StoreName == S).Select(m=>m.StoreId);
                Session["Orderdata"] = order.Name + "," + order.Phone + "," + order.Email + "," + sd;
                return "";
            }
            else
            {
                var e = ModelState.Where(m => m.Value.Errors.Any()).Select(m => new { key = m.Key, message = m.Value.Errors.Select(x => x.ErrorMessage).First() }).ToList();
                var j = JsonConvert.SerializeObject(e);
                return j.Replace(" ", "");//回傳ModelValidate錯誤訊息
            }
        }

        /*public string AddOrder(Order order, string Sname)
        {
            if (ModelState.IsValid)
            {
                var d = Convert.ToInt32(Session["Member"].ToString());
                var s = Convert.ToInt32(Session["cart"].ToString());
                var t = Sname.Split('-');
                var S = t[0];
                var sd = DB.Store.Where(m => m.StoreName == S).FirstOrDefault();
                try
                {
                    order.Store_Id = sd.StoreId;
                    order.User_Id = d;

                    DB.Order.Add(order);
                    DB.SaveChanges();
                    var data = DB.Quantity.Where(m => m.Cid == s).ToList();
                    OrderDetail orderdetail = new OrderDetail();
                    foreach (var i in data)
                    {
                        orderdetail = new OrderDetail()
                        {
                            Quantity_Id = i.Id,
                            Order_Id = order.Id
                        };
                        DB.OrderDetail.Add(orderdetail);
                    }
                    DB.ShoppingCar.Remove(DB.ShoppingCar.Find(s));
                    ShoppingCar cart = new ShoppingCar() { Userid = d };
                    DB.ShoppingCar.Add(cart);
                    DB.SaveChanges();
                    Session["Cart"] = cart.Id;
                    return "";
                }
                catch (Exception e)
                {
                    return "資料庫更新失敗 code:" + e;
                }
            }
            else
            {
                var e = ModelState.Where(m => m.Value.Errors.Any()).Select(m => new { key = m.Key, message = m.Value.Errors.Select(x => x.ErrorMessage).First() }).ToList();
                var j = JsonConvert.SerializeObject(e);
                return j.Replace(" ", "");
            }
        }*/
        public string CreatePaydata()
        {
            var c = Convert.ToInt32(Session["Cart"].ToString());
            var prod = (from Cart in DB.ShoppingCar
                        where Cart.Id == c
                        join Qty in DB.Quantity on Cart.Id equals Qty.Cid
                        join PF in DB.ProdFeature on Qty.PFid equals PF.Id
                        join p in DB.Product on PF.Pid equals p.Id
                        select new
                        {
                            name = p.Name,
                            qty = Qty.Qty,
                            price = p.Price,
                            total = p.Price * Qty.Qty
                        }).ToList();
            int? total = 0; var names = "";

            for(var i = 0; prod.Count > i; i++)
            {
                total += prod[i].total;
                if(i == prod.Count - 1)
                {
                    names += prod[i].name + prod[i].price + "元共" + prod[i].qty + "個";
                }
                else
                {
                    names += prod[i].name + prod[i].price + "元共" + prod[i].qty + "個#";
                }
            }
            var random = new Random();
            obj.Paydata paydata = new obj.Paydata()
            {
                MerchantTradeNo = "Asptest" + random.Next(0, 99999999),
                TotalAmount = total.ToString(),
                ItemName = names
            };
            paydata.CreateCheckMacValue(paydata);
            Session["Check"] = paydata.CheckMacValue;
            string json = JsonConvert.SerializeObject(paydata);
            return json; 
        }
        [HttpPost]
        public string VerifyPay(string checkvalue)
        {
            if (Session["Check"].ToString() == checkvalue)
            {
                try
                {
                    var d = Convert.ToInt32(Session["Member"].ToString());
                    var s = Convert.ToInt32(Session["cart"].ToString());
                    var str = Session["Orderdata"].ToString().Split(',');
                    int sd = Convert.ToInt32(str[3]);
                    Order order = new Order()
                    {
                        Name = str[0],
                        Phone = str[1],
                        Email = str[2],
                        Store_Id = sd

                    };
                    DB.Order.Add(order);
                    DB.SaveChanges();
                    var data = DB.Quantity.Where(m => m.Cid == s).ToList();
                    OrderDetail orderdetail = new OrderDetail();
                    foreach (var i in data)
                    {
                        orderdetail = new OrderDetail()
                        {
                            Quantity_Id = i.Id,
                            Order_Id = order.Id
                        };
                        DB.OrderDetail.Add(orderdetail);
                    }
                    DB.ShoppingCar.Remove(DB.ShoppingCar.Find(s));
                    ShoppingCar cart = new ShoppingCar() { Userid = d };
                    DB.ShoppingCar.Add(cart);
                    DB.SaveChanges();
                    Session["Cart"] = cart.Id;
                }
                catch (Exception e)
                {

                }
                return "1|OK";
            }
            else
            {
                return "checkvalue error";
            }
        }
    }
}