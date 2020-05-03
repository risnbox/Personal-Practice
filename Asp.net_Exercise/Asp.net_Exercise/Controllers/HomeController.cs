using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Asp.net_Exercise.Models;

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
                    DB.Member.Add(Postback);
                    DB.SaveChanges();
                    TempData["SignUpSuccess"] = "註冊成功,即將進入登入頁面";
                    
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
                    
                    if (DB.Member.Where(m => m.Id==D.Id&m.Email == postback.Email).FirstOrDefault() != null)
                    {
                        if (DB.Member.Where(m => D.Phone == postback.Phone).FirstOrDefault() != null)
                        {
                            postback.Id = D.Id;
                            postback.Enable = D.Enable;
                            DB.Entry(D).CurrentValues.SetValues(postback);
                            try
                            {
                                DB.SaveChanges();
                            }
                            catch(Exception e)
                            {
                                ViewBag.EditErrorMessage = e.Message;
                            }
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
    }
}