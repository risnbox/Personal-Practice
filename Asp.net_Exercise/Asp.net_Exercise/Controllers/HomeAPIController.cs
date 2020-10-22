using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;
using Asp.net_Exercise.Models;

namespace Asp.net_Exercise.Controllers
{
    public class HomeAPIController : ApiController
    {
        DatabaseEntities DB = new DatabaseEntities();
        //在WebApicontroller使用Session 參考資料:https://ithelp.ithome.com.tw/articles/10229385
        HttpContext httpContext = HttpContext.Current;
//---------------------------------------------------------------
        [HttpGet]
        public IHttpActionResult Index()//IHttpActionResult是Webapi2新增的回傳類型 參考資料:https://dotblogs.com.tw/grayyin/2016/04/19/144437
        {
            try
            {
                var count = DB.Product.Count();//得到商品數量
                var products = DB.Product.ToList();
                var random = new Random();
                var List = new[] { new { prod = new Product(), img = new Img() } }.ToList();//因使用匿名類型第一筆資料只為宣告用，值為null
                var I = new List<int>();//用來存取隨機數確保不重複
                for (var i = 0; 4 > i; i++)//只顯示四筆商品
                {
                    var R = random.Next(count);//從商品數量內得到隨機數
                    foreach (var x in I)//確保不會得到重複值
                    {
                        while (R == x)
                        {
                            R = random.Next(count);
                        }
                    }
                    //透過 R 取得商品資料(商品及預覽圖)
                    var P = products[R];
                    List.Add(new
                    {
                        prod = P,
                        img = DB.Prod_Img.Where(m => m.Pid == P.Id && m.Img.Type == "previewed").Select(m => m.Img).FirstOrDefault()
                    });
                    I.Add(R);//每迴圈一次就加進數組
                }
                return Ok(List);
            }
            catch (Exception e)//出錯處理
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult GetProd(string TYPE)//用來回傳women,men,kids葉面所需資料
        {
            try
            {
                //取得該類型所有商品資訊及預覽圖
                var d = (from PCT in DB.Prod_Class_Type
                         where PCT.Type.TypeName == TYPE
                         join PM in DB.Prod_Img on PCT.Pid equals PM.Pid
                         join Img in DB.Img on new { A = PM.Mid, B = "previewed" } equals new { A = Img.Id, B = Img.Type }
                         select new
                         {
                             img = Img,
                             prod = PM.Product
                         }).ToList();
                return Ok(d);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult proddetalis(int Pid)
        {
            try
            {
                //確認是否登入以及是否已新增過該商品
                string keep = null;
                if (httpContext.Session["Member"] != null)
                {
                    var U = Convert.ToInt32(httpContext.Session["Member"].ToString());
                    if (DB.Keep.Where(m => m.Userid == U && m.Prodid == Pid).FirstOrDefault() != null)
                    {
                        keep = "true";//此字串用以讓前端辨識初始顯示狀態
                    }
                }
                //取得商品資料,圖片,是否新增過
                var data = (from PM in DB.Prod_Img
                            where PM.Pid == Pid
                            join prod in DB.Product on Pid equals prod.Id
                            join Img in DB.Img on PM.Mid equals Img.Id
                            select new
                            {
                                prod = prod,
                                img = Img,
                                keep = keep
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
        public IHttpActionResult AddCart(int pid, string color, string size)
        {
            try
            {
                if (httpContext.Session["Cart"] == null)//先檢查session是否為null，直接宣告會error
                {
                    throw new Exception("請先登入");
                }
                var d = Convert.ToInt32(httpContext.Session["Member"].ToString());
                var c = Convert.ToInt32(httpContext.Session["Cart"].ToString());//獲取購物車Id
                if (DB.Member.Where(m => m.Id == d && m.Enable == 0).FirstOrDefault() != null)
                {
                    throw new Exception("帳號尚未認證");
                }
                else
                {
                    //取得購物車,尺寸,顏色供後續查詢使用
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
                    //取得商品特徵要是null則新增 p=prod c=color s=size
                    var pf = DB.ProdFeature.Where(m => m.Pid == pid && m.Cid == data.C && m.Sid == data.S).FirstOrDefault();
                    if (pf == null)
                    {
                        pf = new ProdFeature() { Pid = pid, Cid = data.C, Sid = data.S };
                        DB.ProdFeature.Add(pf);
                    }
                    //取得購物車內是否有相同特徵的商品
                    var q = DB.Quantity.Where(m => m.PFid == pf.Id && m.Cid == c).FirstOrDefault();
                    if (q != null)//有則數量欄位++
                    {
                        q.Qty++;
                    }
                    else//無則新增至DB
                    {
                        q = new Quantity() { PFid = pf.Id, Cid = c, Qty = 1 };
                        DB.Quantity.Add(q);
                    }
                    DB.SaveChanges();
                    return Ok();
                }
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}

