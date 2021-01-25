using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Asp.net_Exercise.Models;

namespace Asp.net_Exercise.Controllers
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
                return Ok(data);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet]
        public string CheckPay()//確認是否付款成功
        {
            var MTN = httpContext.Session["MerchantTradeNo"].ToString();
            var o = DB.Order.Where(m => m.MerchantTradeNo == MTN && m.Pay == 1).FirstOrDefault();
            if (o != null)
            {
                httpContext.Session.Remove("MerchantTradeNo");
                return "true";
            }
            else
            {
                return "";
            }
        }
        [HttpGet]
        public IHttpActionResult CartList()//印出最後確認的購物車清單
        {
            var d = Convert.ToInt32(httpContext.Session["Member"].ToString());
            var c = Convert.ToInt32(httpContext.Session["Cart"].ToString());
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
            return Ok(data);
        }
    }
}
