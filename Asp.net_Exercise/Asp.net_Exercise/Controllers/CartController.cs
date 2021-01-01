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
        
        public ActionResult CarPartial()//產生購物車頁面
        {
            var c = Convert.ToInt32(Session["Cart"].ToString());
            var data = (from Cart in DB.ShoppingCar//從DB撈出購物車資料
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
        public string CartList()
        {
            var d = Convert.ToInt32(Session["Member"].ToString());
            var c = Convert.ToInt32(Session["Cart"].ToString());
            //產生購物清單
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
            var Json = JsonConvert.SerializeObject(data);//將該List轉為Json格式
            return Json;
        }
        public ActionResult CheckOut()//結帳頁面
        {
            var d = Convert.ToInt32(Session["Member"].ToString());
            var c = Convert.ToInt32(Session["Cart"].ToString());
            if (DB.Member.Where(m => m.Id == d && m.Enable == 0).FirstOrDefault() != null)//檢查驗證狀態
            {
                return RedirectToAction("EmailValidationView");
            }
            if (DB.Member_Store.Where(m => m.Member_Id == d).FirstOrDefault() == null)//檢查是否設定取貨點
            {
                TempData["CartError"] = "請先至會員會員中心選擇711門市";
                return RedirectToAction("Location","store");
            }
            //-----------------------------------------------------------------------
            //產生購物清單
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
            var Json = JsonConvert.SerializeObject(data);//將該List轉為Json格式
            ViewBag.json = Json.Replace(" ", "");
            //-----------------------------------------------------------------------
            //產生Step2預設資料
            var s = DB.Member_Store.Where(m => m.Member_Id == d).ToList();//撈出該會員選擇的門市
            var M = DB.Member.Where(m => m.Id == d).FirstOrDefault();
            var S = new List<string>();//使用List<string>存取所有StoreName-Address
            foreach (var x in s)
            {
                S.Add(x.Store.StoreName + "-" + x.Store.StoreAddress);
            }
            var l = new SelectList(S);//將List轉為SelectList
            ViewBag.storelist = l;//轉為Viewbag傳至前端
            ViewBag.Name = M.Name;
            ViewBag.Phone = M.Phone;
            ViewBag.Email = M.Email;
            return View();
        }
        public Quantity SearchQuantity(string Name, string Feature)//搜尋該用戶購物車商品
        {
            var d = Convert.ToInt32(Session["Member"].ToString());//當前登入用戶
            var F = Feature.Split('-');//分割字串得到尺寸及特徵
            var color = F[0];
            var size = F[1];
            var S = DB.ShoppingCar.Where(m => m.Userid == d).FirstOrDefault();//購物車編號
            var data = DB.Quantity.Where(m => m.Cid == S.Id && m.ProdFeature.Color.Name == color && m.ProdFeature.Size.Name == size && m.ProdFeature.Product.Name == Name).FirstOrDefault();
            return data;
        }
        
        public string DeleteCart(string Name, string Feature)//刪除購物車商品
        {
            try
            {
                var data = SearchQuantity(Name, Feature);//呼叫自訂function得到商品資料(型別Quantity)
                DB.Quantity.Remove(data);//刪除該筆資料
                DB.SaveChanges();
                return "";
            }
            catch (Exception e)
            {
                return "伺服器錯誤 code:" + e.ToString();
            }
        }
        public string QtyAdd(string Name, string Feature)//購物車商品數量增減
        {
            try
            {
                var data = SearchQuantity(Name, Feature);//呼叫自訂function得到商品資料(型別Quantity)
                data.Qty++;
                DB.SaveChanges();
                return "";
            }
            catch (Exception e)
            {
                return "伺服器錯誤 code:" + e.ToString();//錯誤則提供錯誤訊息
            }
        }
        public string QtyCut(string Name, string Feature)//購物車商品數量增減
        {
            try
            {
                var data = SearchQuantity(Name, Feature);//呼叫自訂function得到商品資料(型別Quantity)
                data.Qty--;
                DB.SaveChanges();
                return "";
            }
            catch (Exception e)
            {
                return "伺服器錯誤 code:" + e.ToString();//錯誤則提供錯誤訊息
            }
        }
        [HttpPost]
        public string VerifyOrder(Order order, string Sname)//檢查Step2資料
        {
            if (ModelState.IsValid)
            {
                return "";
            }
            else
            {
                //從ModelState內撈出錯誤訊息轉為List後序列化為JSON回傳前端
                var e = ModelState.Select(m => new { key = m.Key, message = m.Value.Errors.Select(x => x.ErrorMessage).First() }).ToList();
                var j = JsonConvert.SerializeObject(e);
                return j.Replace(" ", "");//回傳ModelValidate錯誤訊息
            }
        }
        public string CreatePaydata(Order order,string Sname)//create綠界所需參數
        {
            var u = Convert.ToInt32(Session["Member"].ToString());
            var c = Convert.ToInt32(Session["Cart"].ToString());
            ////建立金流訂單資所需參數
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
            //----------------------------------------------------------------------
            int? total = 0; var names = "";
            for(var i = 0; prod.Count > i; i++)//建立綠界頁面商品說明
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
            //----------------------------------------------------------------------
            var random = new Random();
            var o = new obj();
            obj.Paydata paydata = new obj.Paydata()//產生訂單編號
            {
                MerchantTradeNo = "Asptest" + random.Next(10000000, 99999999),
                TotalAmount = total.ToString(),
                ItemName = names
            };
            while (DB.Order.Where(m => m.MerchantTradeNo == paydata.MerchantTradeNo).FirstOrDefault() != null)//檢查訂單編號是否有重複
            {
                paydata.MerchantTradeNo = "Asptest" + random.Next(10000000, 99999999);
            }
            paydata.CheckMacValue = o.CreateCheckMacValue(paydata);//產出所需參數後加密產生驗證碼
            //-------------------------------------------------------------
            //將購物車內容清空後移至DB儲存
            var t = Sname.Split('-');//分割前端商店字串 商店名-地址
            var S = t[0];
            var sd = DB.Store.Where(m => m.StoreName == S).Select(m => m.StoreId).FirstOrDefault();
            try
            {
                //新增訂單資料後儲存至DB產生ID供Detail參考
                order.Store_Id = sd;
                order.User_Id = u;
                order.TradeDate = paydata.MerchantTradeDate;
                order.MerchantTradeNo = paydata.MerchantTradeNo;
                order.Pay = 0;
                DB.Order.Add(order);
                DB.SaveChanges();
                //----------------------------------------------------------------------
                //將購物車內容儲存至DB
                var data = DB.Quantity.Where(m => m.Cid == c).ToList();
                OrderDetail orderdetail = new OrderDetail();
                foreach (var i in data)
                {
                    orderdetail.Quantity_Id = i.Id;
                    orderdetail.Order_Id = order.Id;
                    DB.OrderDetail.Add(orderdetail);
                }
                //----------------------------------------------------------------------
                //移除舊購物車，新增新的購物車編號
                DB.ShoppingCar.Remove(DB.ShoppingCar.Find(c));
                ShoppingCar cart = new ShoppingCar() { Userid = u };
                DB.ShoppingCar.Add(cart);
                DB.SaveChanges();
                Session["MerchantTradeNo"] = order.MerchantTradeNo;
                Session["Cart"] = cart.Id;
            }
            catch(Exception e)
            {
                return e.Message;
            }
            string json = JsonConvert.SerializeObject(paydata);
            return json; 
        }
        public ActionResult Explanation()
        {
            return View();
        }
        public ActionResult RedirectView()
        {
            return View();
        }
        [HttpPost]
        public void VerifyPay(obj.Postback postback)//供綠界呼叫的API;得到綠界回傳訂單內容後將該訂單付款狀態改為已付款
        {
            var o = DB.Order.Where(m => m.MerchantTradeNo == postback.MerchantTradeNo).FirstOrDefault();
            o.TradeNo = postback.TradeNo;//寫入綠界訂單編號
            o.PaymentDate = postback.PaymentDate;//寫入付款時間
            o.Pay = 1;//修改為已付款
            DB.SaveChanges();
            Response.Write("1|OK");//回傳綠界確認參數(由綠界指定)
        }
    }
}