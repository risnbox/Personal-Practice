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
        DatabaseEntities DB = new DatabaseEntities();//建立公用DB物件
        public ActionResult Index()
        {
            var count = DB.Product.Count();//得到商品數量
            var products = DB.Product.ToList();
            var random = new Random();
            List<obj.prod_img> pm = new List<obj.prod_img>();//建立List<prod_img>，用以存放隨機後得到的data
            var I = new List<int>();//用來存取隨機數確保不重複
            for (var  i = 0; 4 > i; i++)//只顯示四筆商品
            {
                obj.prod_img data = new obj.prod_img();//用以存放prod及img
                var R = random.Next(count);//從商品數量內得到隨機數
                foreach(var x in I)//確保不會得到重複值
                {
                    while (R == x)
                    {
                        R = random.Next(count);
                    }
                }
                //透過 R 取得商品資料(商品及預覽圖)
                var P = products[R];
                data.prod = P;
                data.img = DB.Prod_Img.Where(m => m.Pid == P.Id && m.Img.Type == "previewed").Select(m => m.Img).FirstOrDefault();
                pm.Add(data);
                I.Add(R);
            }
            //將資料序列後傳回前端
            var json = JsonConvert.SerializeObject(pm).Replace(" ","");
            ViewBag.json = json;
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
            //取得該類型所有商品資訊及預覽圖
            var data = (from PCT in DB.Prod_Class_Type where PCT.Type.TypeName == TYPE
                        join PM in DB.Prod_Img on PCT.Pid equals PM.Pid
                        join Img in DB.Img on new { A = PM.Mid, B = "previewed" } equals new { A = Img.Id, B = Img.Type }
                        select new
                        {
                            img = Img,
                            prod = PM.Product
                        }).ToList();
            //將資料序列後傳回前端
            var json = JsonConvert.SerializeObject(data);
            json = json.Replace(" ", "");
            return json;
        }
        public ActionResult ProdDetails(int Pid)
        {
            //確認是否登入以及是否已新增過該商品
            string keep = null;
            if (Session["Member"] != null)
            {
                var U = Convert.ToInt32(Session["Member"].ToString());
                if (DB.Keep.Where(m => m.Userid == U && m.Prodid == Pid).FirstOrDefault() != null)
                {
                    keep = "true";//此字串用以讓前端辨識初始顯示狀態
                }
            }
            //取得商品資料,圖片,是否新增過
            var data = (from PM in DB.Prod_Img where PM.Pid==Pid
                        join prod in DB.Product on Pid equals prod.Id
                        join Img in DB.Img on PM.Mid equals Img.Id
                        select new
                        {
                            prod = prod,
                            img = Img,
                            keep = keep
                        }
                      ).ToList();
            //將資料序列後傳回前端
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