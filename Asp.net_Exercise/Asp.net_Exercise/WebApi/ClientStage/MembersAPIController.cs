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
using System.Web.Http.Cors;
using System.Data.Entity.Validation;

namespace Asp.net_Exercise.WebApi.ClientStage
{
    public class MembersAPIController : ApiController
    {
        DatabaseEntities DB = new DatabaseEntities();
        //在WebApicontroller使用Session 參考資料:https://ithelp.ithome.com.tw/articles/10229385
        HttpContext httpContext = HttpContext.Current;
        //---------------------------------------------------------------
        [HttpPost]
        public IHttpActionResult GooglesignUp(obj.Postdata postdata)//接收前端參數後至Google請求使用者資訊
        {
            try
            {
                //利用後端+token取得資料的方式 (如要使用await必須在Func加上async非同步修飾詞及類型改為Task< >來支援非同步result)
                /*string result;
                var url = "https://oauth2.googleapis.com/tokeninfo?id_token=" + postdata.id_token;
                using (var Client = new HttpClient())
                {
                    Client.Timeout = TimeSpan.FromSeconds(30);
                    var Response = await Client.GetAsync(url);//非同步發出請求，得到回應後才會往下
                    result = await Response.Content.ReadAsStringAsync();//非同步取得回應內資料，完成才會往下
                }
                //解析json成匿名型別物件
                var json = JsonConvert.DeserializeAnonymousType(result, new { email = "", sub = "", name = "" });*/
                //查詢是否已註冊過
                var d = DB.Member.Where(m => m.Email == postdata.email).FirstOrDefault();
                if (d == null)//要是無則註冊
                {
                    var member = new Member()//新增會員資料及免除驗證
                    {
                        Email = postdata.email,
                        Name = postdata.name,
                        Phone = postdata.phone,
                        Gender = postdata.gender,
                        GoogleId = postdata.Gid,
                        Enable = 1,
                        Joindate = DateTime.Now
                    };
                    DB.Member.Add(member);
                    DB.SaveChanges();
                    d = member;
                    var cart = new ShoppingCar() { Userid = member.Id };
                    DB.ShoppingCar.Add(cart);
                    DB.SaveChanges();
                }
                if (d.Enable == 2) { throw new Exception("該帳號已被停權"); }
                else
                {
                    d.GoogleId = postdata.Gid;
                    d.Enable = 1;
                    DB.SaveChanges();
                }
                httpContext.Session["Member"] = d.Id;
                httpContext.Session["MemberName"] = d.Name;
                var C = DB.ShoppingCar.Where(m => m.Userid == d.Id).FirstOrDefault();
                httpContext.Session["Cart"] = C.Id;
                return Ok();
            }

            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult FbLogin(obj.Fbdata fbdata)
        {
            try
            {
                if (fbdata.Email == null)//FB信箱若未驗證無法使用FBAPI進行登入
                {
                    throw new Exception("FB信箱未通過驗證，請至FB驗證完畢後再登入或使用本站註冊");
                }
                var d = DB.Member.Where(m => m.Email == fbdata.Email).FirstOrDefault();
                if (d == null)//無註冊過則註冊 註冊過則新增fbId
                {
                    Member member = new Member()
                    {
                        Email = fbdata.Email,
                        Name = fbdata.Name,
                        Enable = 1,
                        FacebookId = fbdata.Fbid,
                        Joindate = DateTime.Now
                    };
                    DB.Member.Add(member);
                    try
                    {
                        DB.SaveChanges();
                        var cart = new ShoppingCar() { Userid = member.Id };
                        DB.ShoppingCar.Add(cart);
                        DB.SaveChanges();
                    }
                    catch (DbEntityValidationException e)//資料庫的錯誤型別要引用Entity.validtion才能抓到EntityValidationErrors這個屬性
                    {
                        var msg = e.EntityValidationErrors.Select(m => m.ValidationErrors.Select(x => x.ErrorMessage).First()).First();
                        return BadRequest(msg);
                    }
                    d = member;
                }
                if (d.Enable == 2) { throw new Exception("該帳號已被停權"); }
                else
                {
                    d.FacebookId = fbdata.Fbid;
                    d.Enable = 1;
                    try
                    {
                        DB.SaveChanges();
                    }
                    catch (DbEntityValidationException e)
                    {
                        var msg = e.EntityValidationErrors.Select(m => m.ValidationErrors.Select(x => x.ErrorMessage).First()).First();
                        return BadRequest(msg);
                    }
                }
                var c = DB.ShoppingCar.Where(m => m.Userid == d.Id).FirstOrDefault();
                httpContext.Session["Member"] = d.Id;
                httpContext.Session["MemberName"] = d.Name;
                httpContext.Session["Cart"] = c.Id;
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet]
        public void Keep(int Pid)//新增至收藏
        {
            var keep = new Keep() { Userid = Convert.ToInt32(httpContext.Session["Member"].ToString()), Prodid = Pid };
            if (!DB.Keep.Where(m => m.Userid == keep.Userid && m.Prodid == keep.Prodid).Any())
            {
                DB.Keep.Add(keep);
                DB.SaveChanges();
            }
        }
        [HttpGet]
        public void DelKeep(int Pid)//刪除該收藏
        {
            var U = Convert.ToInt32(httpContext.Session["Member"].ToString());
            var data = DB.Keep.Where(m => m.Prodid == Pid && m.Userid == U).FirstOrDefault();
            DB.Keep.Remove(data);
            DB.SaveChanges();
        }
        [HttpGet]
        public IHttpActionResult Keepdata()//收藏頁面所需資料
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
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult GetOrderDetail(int oid)//訂單頁面所需資料
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
        [HttpPost]
        public IHttpActionResult ResetPassword(obj.Postdata postdata)
        {
            try
            {
                //取得帳號訊息
                var d = Convert.ToInt32(httpContext.Session["ResetPswId"].ToString());
                var data = DB.Member.Where(m => m.Id == d).FirstOrDefault();
                data.Password = postdata.psw;
                try
                {
                    DB.SaveChanges();
                }
                catch (DbEntityValidationException e)//資料庫的錯誤型別要引用Entity.validtion才能抓到EntityValidationErrors這個屬性
                {
                    //撈出模型驗證錯誤訊息 此例只有輸入密碼因此只抓第一筆
                    var msg = e.EntityValidationErrors.Select(m => m.ValidationErrors.Select(x => x.ErrorMessage).First()).First();
                    return BadRequest(msg);
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }   
    }
}
