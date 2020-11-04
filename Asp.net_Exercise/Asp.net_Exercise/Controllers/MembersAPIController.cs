using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.Threading.Tasks;
using Asp.net_Exercise.Models;
using Newtonsoft.Json;

namespace Asp.net_Exercise.Controllers
{
    public class MembersAPIController : ApiController
    {
        DatabaseEntities DB = new DatabaseEntities();
        //在WebApicontroller使用Session 參考資料:https://ithelp.ithome.com.tw/articles/10229385
        HttpContext httpContext = HttpContext.Current;
        //---------------------------------------------------------------
        public class Postdata
        {
            public string gender { get; set; }
            public string phone { get; set; }
            public string id_token { get; set; }
        }
        [HttpPost]//WebAPI無法獨立解析參數 須建立類
        public async Task<IHttpActionResult> GooglesignUp(Postdata postdata)
        {
            try
            {
                var result = "";
                var url = "https://oauth2.googleapis.com/tokeninfo?id_token=" + postdata.id_token;
                using (var Client = new HttpClient())
                {
                    Client.Timeout = TimeSpan.FromSeconds(30);//避免資源浪費設定逾時，除非流量用量大不然其實沒必要
                    var Response = await Client.GetAsync(url);//非同步發出請求，得到回應後才會往下 參考資料:https://docs.microsoft.com/zh-tw/dotnet/csharp/programming-guide/concepts/async/
                    result = await Response.Content.ReadAsStringAsync();//非同步取得回應內資料，完成才會往下
                }
                //解析json成匿名型別物件
                var json = JsonConvert.DeserializeAnonymousType(result, new { email = "", sub = "", name = "" });
                var member = new Member()//新增會員資料及免除驗證
                {
                    Email = json.email,
                    Name = json.name,
                    Phone = postdata.phone,
                    Gender = postdata.gender,
                    GoogleId = json.sub,
                    Enable = 1
                };
                //查詢是否已註冊過
                var d = DB.Member.Where(m => m.GoogleId == member.GoogleId && m.Email == member.Email).FirstOrDefault();
                if (d == null)//要是無則註冊
                {
                    d = member;
                    DB.Member.Add(member);
                    DB.SaveChanges();
                }
                httpContext.Session["Member"] = d.Id;
                httpContext.Session["MemberName"] = d.Name;
                httpContext.Session["MemberEmail"] = d.Email;
                if (DB.ShoppingCar.Where(m => m.Userid == d.Id).FirstOrDefault() == null)
                {
                    var cart = new ShoppingCar() { Userid = d.Id };
                    DB.ShoppingCar.Add(cart);
                    DB.SaveChanges();
                }
                var C = DB.ShoppingCar.Where(m => m.Userid == d.Id).FirstOrDefault();
                httpContext.Session["Cart"] = C.Id;
                if (d.Phone == null) { throw new Exception("Google帳號未含手機號碼,請至會員中心填寫"); }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet]
        public void Keep(int Pid)
        {
            var keep = new Keep() { Userid = Convert.ToInt32(httpContext.Session["Member"].ToString()), Prodid = Pid };
            if (DB.Keep.Where(m => m.Userid == keep.Userid && m.Prodid == keep.Prodid).FirstOrDefault() == null)
            {
                DB.Keep.Add(keep);
                DB.SaveChanges();
            }
        }
        [HttpGet]
        public void DelKeep(int Pid)
        {
            var U = Convert.ToInt32(httpContext.Session["Member"].ToString());
            var data = DB.Keep.Where(m => m.Prodid == Pid && m.Userid == U).FirstOrDefault();
            DB.Keep.Remove(data);
            DB.SaveChanges();
        }
        [HttpGet]
        public IHttpActionResult Keepdata()
        {
            try
            {
                var d = Convert.ToInt32(httpContext.Session["Member"].ToString());
                var data = (from keep in DB.Keep
                            where keep.Userid == d
                            join prod in DB.Product on keep.Prodid equals prod.Id
                            join PM in DB.Prod_Img on keep.Prodid equals PM.Pid
                            join img in DB.Img on new { a = PM.Mid, b = "previewed" } equals new { a = img.Id, b = img.Type }
                            select new
                            {
                                prod = prod,
                                img = img
                            }).ToList();
                return Ok(data);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult GetOrderDetail(int oid)
        {
            try
            {
                var data = (from order in DB.Order
                            where order.Id == oid
                            join OD in DB.OrderDetail on order.Id equals OD.Order_Id
                            join Qty in DB.Quantity on OD.Quantity_Id equals Qty.Id
                            join PF in DB.ProdFeature on Qty.PFid equals PF.Id
                            join PI in DB.Prod_Img on PF.Pid equals PI.Pid
                            join img in DB.Img on new { A = PI.Mid, B = "previewed" } equals new { A = img.Id, B = img.Type }
                            select new
                            {
                                name = PF.Product.Name,
                                price = PF.Product.Price,
                                qty = Qty.Qty,
                                img = img.FileName,
                                total = PF.Product.Price * Qty.Qty,
                                feature = PF.Size.Name + "-" + PF.Color.Name
                            }
                            ).ToList();
                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
