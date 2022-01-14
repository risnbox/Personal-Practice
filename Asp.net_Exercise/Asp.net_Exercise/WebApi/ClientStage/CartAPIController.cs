using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Asp.net_Exercise.Models;

namespace Asp.net_Exercise.WebApi.ClientStage
{
    public class CartAPIController : ApiController
    {
        DatabaseEntities DB = new DatabaseEntities();
        //在WebApicontroller使用Session 參考資料:https://ithelp.ithome.com.tw/articles/10229385
        HttpContext httpContext = HttpContext.Current;
        //---------------------------------------------------------------
        [HttpGet]
        public IHttpActionResult Partial()//回傳購物車資料
        {
            try
            {
                var c = Convert.ToInt32(httpContext.Session["Cart"].ToString());
                var data = (from Qty in DB.Quantity where Qty.Cid == c
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
                return Ok(data);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        public Quantity SearchQuantity(string Name, string Feature)//搜尋該用戶購物車商品
        {
            var d = Convert.ToInt32(httpContext.Session["Member"].ToString());//當前登入用戶
            var F = Feature.Split('-');//分割字串得到尺寸及特徵
            var color = F[0];
            var size = F[1];
            var S = DB.ShoppingCar.Where(m => m.Userid == d).FirstOrDefault();//購物車編號
            var data = DB.Quantity.Where(m => m.Cid == S.Id && m.ProdFeature.Color.Name == color && m.ProdFeature.Size.Name == size && m.ProdFeature.Product.Name == Name).FirstOrDefault();
            return data;
        }
        [HttpGet]
        public IHttpActionResult DeleteCart(string Name, string Feature)//刪除購物車商品
        {
            try
            {
                var data = SearchQuantity(Name, Feature);//呼叫function得到商品資料(型別Quantity)
                DB.Quantity.Remove(data);//刪除該筆資料
                DB.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult QtyAdd(string Name, string Feature)//購物車商品數量增減
        {
            try
            {
                var data = SearchQuantity(Name, Feature);//呼叫自訂function得到商品資料(型別Quantity)
                data.Qty++;
                DB.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);//錯誤則提供錯誤訊息
            }
        }
        [HttpGet]
        public IHttpActionResult QtyCut(string Name, string Feature)//購物車商品數量增減
        {
            try
            {
                var data = SearchQuantity(Name, Feature);//呼叫function得到商品資料(型別Quantity)
                data.Qty--;
                DB.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);//錯誤則提供錯誤訊息
            }
        }
        [HttpGet]
        public IHttpActionResult CheckPay()//確認是否付款成功
        {
            var MTN = httpContext.Session["MerchantTradeNo"].ToString();
            var o = DB.Order.Where(m => m.MerchantTradeNo == MTN && m.Pay == 1).FirstOrDefault();
            if (o != null)
            {
                httpContext.Session.Remove("MerchantTradeNo");
                return Ok("true");
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpGet]
        public IHttpActionResult CartList()//印出最後確認的購物車清單
        {
            var d = Convert.ToInt32(httpContext.Session["Member"].ToString());
            var c = Convert.ToInt32(httpContext.Session["Cart"].ToString());
            try
            {
                //產生購物清單
                var data = (from Qty in DB.Quantity
                            where Qty.Cid == c
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
                return Ok(data);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
            
        }
    }
}
