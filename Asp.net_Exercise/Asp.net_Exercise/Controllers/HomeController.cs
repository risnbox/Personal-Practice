using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Asp.net_Exercise.Models;
using System.Net.Mail;

namespace Asp.net_Exercise.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        DatabaseEntities DB = new DatabaseEntities();
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(Member Postback,string check)
        {
            var Phone = Postback.Phone;
            var Email = Postback.Email;
            //檢查是否通過模型驗證
            if (ModelState.IsValid == true)
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
                //註冊成功後寫入TempData供登入頁面使用,壽命剛好至登入頁面後即會清除,Bag則因壽命問題無法套用在此處?
                //                                                                 (暫不確定壽命是由寫入的此刻開始算還是Return後的頁面開始算)
                else
                {
                    if (Postback.Password != check)
                    {
                        ViewBag.MemberErrorMessage = "兩次輸入密碼不一致";
                        return View();
                    }
                    //加入啟用狀態以及驗證碼錯誤次數
                    Postback.ErrorCount = 0;
                    Postback.Enable = 0;
                    DB.Member.Add(Postback);
                    DB.SaveChanges();
                    var code = Randomcode();
                    Session["Veriflcationcode"] = code;
                    EmailValidation(Postback.Email, Session["Veriflcationcode"] as string);
                    TempData["SignUpSuccess"] = "註冊成功,已寄出認證信,請收信登入輸入驗證碼";
                    
                    return RedirectToAction("SignIn");
                }
            }
            else
            {

            }
            {
                return View();
            }
        }
        public ActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignIn(string User,string Psw)
        {
            //檢查帳號是否存在,若無則顯示無此帳號
            var data = DB.Member.Where(m => m.Email == User | m.Phone == User).FirstOrDefault();
            if (data != null) 
            {
                if (data.Password == Psw) 
                {
                    Session["Member"] = data.Id;
                    Session["MemberName"] = data.Name;
                    if (DB.Member.Where(m => m.Id==data.Id && m.Enable == 0).FirstOrDefault()!=null)
                    {
                        return RedirectToAction("EmailValidationView");
                    }
                    return RedirectToAction("Index");
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
            //將Session清空已利辨別會員狀態
            Session["Member"] = null;
            Session["MemberName"] = null;
            return RedirectToAction("Index");
        }
        public ActionResult MemberCentre()
        {
            //若Session為空則導回登入頁面,避免未登入卻透過網址進入
            if(Session["Member"]==null)
            {
                return RedirectToAction("SignIn");
            }
            var e = Session["Member"];
            var d = Convert.ToInt32(Session["Member"].ToString());
            var D = DB.Member.Where(m => m.Id == d).FirstOrDefault();
            D.Password = null;
            return View(D);
        }
        [HttpPost]
        public ActionResult MemberCentre(Member postback)
        {

            var d = Convert.ToInt32(Session["Member"].ToString());
            var D = DB.Member.Where(m => m.Id == d).FirstOrDefault();
            var t = DB.Member.Where(m => m.Id == D.Id && m.Email == postback.Email).FirstOrDefault();
            if (ModelState.IsValid == true)
            {
                if (DB.Member.Where(m => D.Password == postback.Password).FirstOrDefault() != null)
                {

                    if (DB.Member.Where(m => m.Id == D.Id && m.Email == postback.Email).FirstOrDefault() != null) 
                    {
                        if (DB.Member.Where(m => D.Phone == postback.Phone).FirstOrDefault() != null)
                        {
                            postback.Id = D.Id;
                            postback.Enable = D.Enable;
                            postback.ErrorCount = D.ErrorCount;
                            DB.Entry(D).CurrentValues.SetValues(postback);
                            DB.SaveChanges();
                            ViewBag.EditErrorMessage = "修改成功";
                            return View();
                        }

                        if (DB.Member.Where(m => m.Phone == postback.Phone).FirstOrDefault() != null)
                        {
                            ViewBag.EditErrorMessage = "該手機已被使用";
                            return View();
                        }
                        D = postback;
                        DB.SaveChanges();
                        ViewBag.EditErrorMessage = "修改成功";
                        return View();
                    }

                    if (DB.Member.Where(m => m.Email == postback.Email).FirstOrDefault() != null)
                    {
                        ViewBag.EditErrorMessage = "該信箱已被使用";
                        return View();
                    }

                    if (DB.Member.Where(m => m.Id == D.Id && m.Phone == postback.Phone).FirstOrDefault() != null)
                    {
                        D = postback;
                        DB.SaveChanges();
                        ViewBag.EditErrorMessage = "修改成功";
                        return View();
                    }

                    if (DB.Member.Where(m => m.Phone == postback.Phone).FirstOrDefault() != null)
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
        public void EmailValidation(string Email,string Veriflcationcode)
        {
            try
            {
                //設定郵件相關參數
                MailMessage Mail = new MailMessage();
                Mail.To.Add(Email);
                Mail.From = new MailAddress(Email, "Lativ", System.Text.Encoding.UTF8);
                Mail.Subject = "帳號驗證";
                Mail.SubjectEncoding = System.Text.Encoding.UTF8;
                Mail.Body = Veriflcationcode;
                Mail.BodyEncoding = System.Text.Encoding.UTF8;
                Mail.IsBodyHtml = true;

                //寄出參數
                SmtpClient Smtp = new SmtpClient();
                Smtp.Credentials = new System.Net.NetworkCredential("risnbox@gmail.com", "RISNFOX753159");
                Smtp.Host = ("smtp.gmail.com");
                Smtp.Port = 25;
                Smtp.EnableSsl = true;
                Smtp.Send(Mail);
                Smtp.Dispose();
                TempData["MailValidation"] = "驗證碼寄出成功";
            }
            catch(Exception)
            {
                TempData["MailValidation"] = "驗證碼寄出失敗";
            }
        }
        //產生驗證碼 說明在Google書籤
        public string Randomcode()
        {
            string allowwords = "QWERTYUIOPLKJHGFDSAZXCVBNMqwertyuioplkjhgfdsazxcvbnm0123456789";
            char[] Chr = new char[6];
            Random rd = new Random();

            for(var i = 0; i < 6; i++)
            {
                Chr[i] = allowwords[rd.Next(0, 61)];
            }
            string code = new string(Chr);
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

            if(Session["Veriflcationcode"] as string == Veriflcationcode)
            {
                ViewBag.ValidationErrorMessage = "恭喜您已完成信箱驗證!即將返回首頁";
                data.Enable = 1;
                data.ErrorCount = 0;
                DB.Entry(data).CurrentValues.SetValues(data);
                DB.SaveChanges();
                RedirectToAction("Index");
            }
            if (data.ErrorCount >= 3)
            {
                var code = Randomcode();
                Session["Veriflcationcode"] = code;
                ViewBag.ValidationErrorMessage = "驗證碼輸入錯誤次數超過3次,已重新寄出驗證信";
                EmailValidation(data.Email, Session["Veriflcationcode"] as string);
                var Newdata = data;
                Newdata.ErrorCount=0;
                DB.Entry(data).CurrentValues.SetValues(Newdata);
                DB.SaveChanges();
                return View();
            }
            ViewBag.ValidationErrorMessage = "驗證碼輸入錯誤,錯誤達三次則須重新收取驗證碼";
            var newdata = data;
            newdata.ErrorCount++;
            DB.Entry(data).CurrentValues.SetValues(newdata);
            DB.SaveChanges();
            return View();
        }
    }
}