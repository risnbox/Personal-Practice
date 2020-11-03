using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Asp.net_Exercise.Models;
using Newtonsoft.Json;

namespace Asp.net_Exercise.Controllers
{
    public class MembersController : Controller
    {
        DatabaseEntities DB = new DatabaseEntities();
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(Member Postback, string check)
        {
            var Phone = Postback.Phone;
            var Email = Postback.Email;
            //檢查是否通過模型驗證
            if (ModelState.IsValid)
            {
                //檢查信箱及手機是否已註冊過
                if (DB.Member.Where(m => m.Email == Email).FirstOrDefault() != null)
                {
                    ViewBag.MemberErrorMessage = "該信箱已使用";
                    return View();
                }
                if (DB.Member.Where(m => m.Phone == Phone).FirstOrDefault() != null)
                {
                    ViewBag.MemberErrorMessage = "該手機已被使用";
                    return View();
                }
                //註冊成功後寫入TempData供登入頁面使用,壽命剛好至登入頁面後即會清除,Bag則因壽命問題無法套用在此處                                                        
                else
                {
                    if (Postback.Password != check)
                    {
                        ViewBag.MemberErrorMessage = "兩次輸入密碼不一致";
                        return View();
                    }
                    //加入啟用狀態以及驗證碼錯誤次數初始化
                    Postback.ErrorCount = 0;
                    Postback.Enable = 0;
                    DB.Member.Add(Postback);
                    DB.SaveChanges();
                    //創造驗證碼
                    var code = Randomcode();
                    Session["Veriflcationcode"] = code;
                    //使用System.Net.Mail來寄出驗證碼
                    EmailValidation("帳號驗證", Postback.Email, "您的驗證碼是:" + code + "<br />Time:" + DateTime.Now);
                    //寫入TempData傳入SignInView來Alert提示使用者
                    TempData["SignUpSuccess"] = "註冊成功,已寄出認證信,請收信後登入輸入驗證碼";
                    return RedirectToAction("SignIn");
                }
            }
            else
            {
                return View();
            }
        }
        public ActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignIn(string User, string Psw)
        {
            //判斷是否為管理員
            if (string.Compare(User, "Admin", false) == 0 && string.Compare(Psw, "Admin01", false) == 0)
            {
                Session["Member"] = 12;
                Session["MemberName"] = "Admin";
                return RedirectToAction("NewProd1","product");
            }
            //檢查帳號是否存在,若無則顯示無此帳號
            var data = DB.Member.Where(m => m.Email == User | m.Phone == User).FirstOrDefault();
            if (data != null)
            {
                if (data.Password == Psw && data.GoogleId == null) 
                {
                    Session["Member"] = data.Id;
                    Session["MemberName"] = data.Name;
                    Session["MemberEmail"] = data.Email;
                    //檢查該帳號是否已有購物車id，無則新增
                    if (DB.ShoppingCar.Where(m => m.Userid == data.Id).FirstOrDefault() == null)
                    {
                        var cart = new ShoppingCar() { Userid = data.Id };
                        DB.ShoppingCar.Add(cart);
                        DB.SaveChanges();
                    }
                    var C = DB.ShoppingCar.Where(m => m.Userid == data.Id).FirstOrDefault();
                    Session["Cart"] = C.Id;
                    //檢查該帳號的啟用狀態,或未啟用則進入驗證View
                    if (DB.Member.Where(m => m.Id == data.Id && m.Enable == 0).FirstOrDefault() != null)
                    {
                        Session["EnableMessage"] = "驗證尚未完成,即將前往驗證頁面";
                        return RedirectToAction("EmailValidationView");
                    }
                    return RedirectToAction("Index","Home");
                }
                else if (data.GoogleId != null)
                {
                    ViewBag.SignInErrormessage = "該帳號為Google連動帳號,請使用Google登入";
                    return View();
                }
                else
                {
                    ViewBag.SignInErrormessage = "密碼輸入錯誤";
                    return View();
                }
            }
            else
            {
                ViewBag.SignInErrormessage = "查無此帳號";
                return View();
            }
        }
        
        public ActionResult SignOut()
        {
            //將Session清空辨別會員狀態
            Session.RemoveAll();
            return RedirectToAction("Index","home");
        }
        public ActionResult MemberCenter()
        {
            //若Session為空或該會員是未啟用狀態則導回登入頁面,避免未登入或是未啟用帳戶透過網址進入
            if (Session["Member"] == null)
            {
                TempData["SignUpSuccess"] = "您尚未登入";
                return RedirectToAction("SignIn");
            }
            var d = Convert.ToInt32(Session["Member"].ToString());
            var D = DB.Member.Where(m => m.Id == d).FirstOrDefault();
            //確認驗證狀態
            if (DB.Member.Where(m => m.Id == D.Id && m.Enable == 0).FirstOrDefault() != null)
            {
                TempData["SignUpSuccess"] = "您尚未啟用";
                return RedirectToAction("EmailValidationView");
            }
            //顯示會員已選擇的門市
            var Store = DB.Member_Store.Where(m => m.Member_Id == d).ToList();
            List<string> str = new List<string>();
            foreach (var i in Store)
            {
                var Lt = DB.Store.Where(m => m.StoreId == i.Store_Id).ToList();
                foreach (var x in Lt)
                {
                    str.Add(x.StoreName);
                }
            }
            ViewBag.Store = string.Join(",", str);
            D.Password = null;
            return View(D);
        }
        [HttpPost]
        public ActionResult MemberCenter(Member postback)
        {
            var d = Convert.ToInt32(Session["Member"].ToString());
            var D = DB.Member.Where(m => m.Id == d).FirstOrDefault();
            if (ModelState.IsValid)
            {
                //檢查密碼是否正確=>信箱、手機是否有重複
                if (DB.Member.Where(m => D.Password == postback.Password).FirstOrDefault() != null)
                {
                    if (DB.Member.Where(m => m.Id == D.Id && m.Email == postback.Email).FirstOrDefault() != null)//信箱無更動 判斷手機
                    {
                        if (DB.Member.Where(m => D.Phone == postback.Phone).FirstOrDefault() != null)//手機無更動則通過驗證
                        {
                            //postback並無Id,Enable,ErrorCount的資料,必須先指定在寫入DB
                            postback.Id = D.Id;
                            postback.Enable = D.Enable;
                            postback.ErrorCount = D.ErrorCount;
                            DB.Entry(D).CurrentValues.SetValues(postback);
                            DB.SaveChanges();
                            ViewBag.EditErrorMessage = "修改成功";
                            Session["MemberName"] = postback.Name;
                            return View();
                        }
                        if (DB.Member.Where(m => m.Phone == postback.Phone).FirstOrDefault() != null)//信箱無更動手機重複
                        {
                            ViewBag.EditErrorMessage = "該手機已被使用";
                            return View();
                        }
                        //信箱無更動手機變動無重複通過驗證
                        postback.Id = D.Id;
                        postback.Enable = D.Enable;
                        postback.ErrorCount = D.ErrorCount;
                        DB.Entry(D).CurrentValues.SetValues(postback);
                        ViewBag.EditErrorMessage = "修改成功";
                        Session["MemberName"] = postback.Name;
                        return View();
                    }

                    if (DB.Member.Where(m => m.Email == postback.Email).FirstOrDefault() != null)//信箱變動且重複
                    {
                        ViewBag.EditErrorMessage = "該信箱已被使用";
                        return View();
                    }

                    if (DB.Member.Where(m => m.Id == D.Id && m.Phone == postback.Phone).FirstOrDefault() != null)//信箱,手機都變動且無重複
                    {
                        postback.Id = D.Id;
                        postback.Enable = D.Enable;
                        postback.ErrorCount = D.ErrorCount;
                        DB.Entry(D).CurrentValues.SetValues(postback);
                        ViewBag.EditErrorMessage = "修改成功";
                        Session["MemberName"] = postback.Name;
                        return View();
                    }
                    if (DB.Member.Where(m => m.Phone == postback.Phone).FirstOrDefault() != null)//信箱,手機變動且手機重複
                    {
                        ViewBag.EditErrorMessage = "該手機已被使用";
                        return View();
                    }
                }
                ViewBag.EditErrorMessage = "密碼錯誤";
                return View();
            }
            return View();
        }
        public string RepeatEmailValidation()//重新寄出驗證信
        {
            try
            {
                var randomcode = Randomcode();
                EmailValidation("帳號驗證", Session["MemberEmail"] as string, "您的驗證碼是:" + randomcode + "<br />Time:" + DateTime.Now);
                Session["Veriflcationcode"] = randomcode;
                return "成功";
            }
            catch (Exception e)
            {
                return "寄出失敗 code:" + e;
            }
        }
        public void EmailValidation(string Title, string Email, string Body)//透過Gmail寄出郵件
        {
            try
            {
                //設定郵件相關參數
                MailMessage Mail = new MailMessage();
                Mail.To.Add(Email);//收件人
                //參數屬性是寄件人名字及地址但地址似乎會被Gmail覆蓋,此處我的寄件人地址是User但實際收件顯示的依然是Host
                Mail.From = new MailAddress(Email, "作品測試", Encoding.UTF8);
                Mail.Subject = Title;//標題
                Mail.SubjectEncoding = Encoding.UTF8;
                Mail.Body = Body;//內容,如定義為Html信件則可加入Html語法(CSS及Javescript未試過)
                Mail.BodyEncoding = Encoding.UTF8;
                Mail.IsBodyHtml = true;//是否定義為Html信件

                //寄出參數
                SmtpClient Smtp = new SmtpClient();
                //登入信箱用參數,在信箱內部設定:低安全性應用程式→開啟較低的應用程式存取權限,需打開則無法引用
                Smtp.Credentials = new NetworkCredential("risnbox@gmail.com", "RISNBOX8520");
                Smtp.Host = ("smtp.gmail.com");//SMTP(簡易郵件傳輸協定)主機地址
                Smtp.Port = 25;//連接埠
                Smtp.EnableSsl = true;//驗證開啟(SSL)
                Smtp.Send(Mail);//寄出
                Smtp.Dispose();//關閉連線
                TempData["MailValidation"] = "驗證碼寄出成功";
            }
            catch (Exception e)
            {
                TempData["MailValidation"] = "驗證碼寄出失敗 code:" + e;
            }
        }
        //產生驗證碼 說明在Google書籤
        public string Randomcode()
        {
            //定義隨機字串內的內容
            string allowwords = "QWERTYUIOPLKJHGFDSAZXCVBNMqwertyuioplkjhgfdsazxcvbnm0123456789";
            char[] Chr = new char[6];//字串長度
            Random rd = new Random();

            for (var i = 0; i < 6; i++)//使用for迴圈獲得隨機碼
            {
                Chr[i] = allowwords[rd.Next(0, 61)];//定義亂數範圍
            }
            string code = new string(Chr);//將char轉回string
            return code;
        }
        public ActionResult EmailValidationView()
        {
            return View();
        }
        [HttpPost]
        public ActionResult EmailValidationView(string Veriflcationcode)
        {
            var d = Convert.ToInt32(Session["Member"].ToString());
            var data = DB.Member.Where(m => m.Id == d).FirstOrDefault();
            Veriflcationcode.Trim();//去除頭尾空白避免字串比較因空白出錯
            if (string.Equals(Session["Veriflcationcode"] as string, Veriflcationcode))
            {

                TempData["ValidationErrorMessage"] = "恭喜您已完成信箱驗證!即將返回首頁";
                data.Enable = 1;//改為啟用
                data.ErrorCount = 0;//初始化錯誤次數
                //修改資料庫內資料已測試過重複變數一樣能作用,前者data是索引作用後者才是複寫索引到的data
                DB.Entry(data).CurrentValues.SetValues(data);
                DB.SaveChanges();
                Session.Remove("Veriflcationcode");//清空驗證碼session
                return RedirectToAction("Index","home");
            }
            if (data.ErrorCount >= 3)
            {
                var code = Randomcode();//呼叫產生亂數方法
                Session["Veriflcationcode"] = code;//由於重寄驗證碼所以Session內容須重置
                ViewBag.ValidationErrorMessage = "驗證碼輸入錯誤次數超過3次,已重新寄出驗證信";
                EmailValidation("帳號驗證", data.Email, Session["Veriflcationcode"] as string);
                data.ErrorCount = 0;
                DB.Entry(data).CurrentValues.SetValues(data);
                DB.SaveChanges();
                return View();
            }
            ViewBag.ValidationErrorMessage = "驗證碼輸入錯誤,錯誤達三次則須重新收取驗證碼";
            data.ErrorCount++;
            DB.Entry(data).CurrentValues.SetValues(data);
            DB.SaveChanges();
            return View();
        }
        public ActionResult KeepView()
        {
            var d = Convert.ToInt32(Session["Member"].ToString());
            //取得要在View顯示的資料
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
            var json = JsonConvert.SerializeObject(data);
            json = json.Replace(" ", "");
            ViewBag.json = json;
            return View();
        }
        public ActionResult OrderVIew()
        {
            var d = Convert.ToInt32(Session["Member"].ToString());
            //取得要在View顯示的資料
            var order = DB.Order.Where(m => m.User_Id == d).Select(m => new { name = m.Name, email = m.Email, phone = m.Phone, tradeNo = m.MerchantTradeNo, store = m.Store.StoreName, time = m.TradeDate, oid = m.Id, pay = m.Pay }).ToList();
            var j = JsonConvert.SerializeObject(order);
            ViewBag.order = j.Replace(" ", "");
            return View();
        }
    }
}