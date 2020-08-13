using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Asp.net_Exercise.Models;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Xml;
using System.Text;
using Newtonsoft.Json;


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
                    EmailValidation(Postback.Email, Session["Veriflcationcode"] as string);
                    //寫入TempData傳入SignInView來Alert提示使用者
                    TempData["SignUpSuccess"] = "註冊成功,已寄出認證信,請收信後登入輸入驗證碼";
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
        public ActionResult SignIn(string User, string Psw)
        {
            //判斷是否為管理員
            if (string.Compare(User, "Admin", false) == 0 && string.Compare(Psw, "Admin01", false) == 0)
            {
                Session["Member"] = 12;
                Session["MemberName"] = "Admin";
                return RedirectToAction("NewProd1");
            }
            //檢查帳號是否存在,若無則顯示無此帳號
            var data = DB.Member.Where(m => m.Email == User | m.Phone == User).FirstOrDefault();
            if (data != null)
            {
                if (data.Password == Psw)
                {
                    Session["Member"] = data.Id;
                    Session["MemberName"] = data.Name;
                    Session["MemberEmail"] = data.Email;
                    if (DB.ShoppingCar.Where(m => m.Userid == data.Id).FirstOrDefault() == null)
                    {
                        var cart = new ShoppingCar() { Userid = data.Id, Pay = "false", Guid = Guid.NewGuid().ToString() };
                        DB.ShoppingCar.Add(cart);
                        var detail = new Details();
                        DB.Details.Add(detail);
                        DB.SaveChanges();
                        Cart_Detail CD = new Cart_Detail() { Cid = cart.Id, Did = detail.Id };
                        DB.Cart_Detail.Add(CD);
                        DB.SaveChanges();
                    }
                    var C = DB.ShoppingCar.Where(m => m.Userid == data.Id).FirstOrDefault();
                    Session["Cart"] = C.Id;
                    //檢查該帳號的啟用狀態,或未啟用則進入驗證View
                    if (DB.Member.Where(m => m.Id == data.Id && m.Enable == 0).FirstOrDefault() != null)
                    {
                        TempData["EnableMessage"] = "驗證尚未完成,即將前往驗證頁面";
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
            //將Session清空辨別會員狀態
            Session["Member"] = null;
            Session["MemberName"] = null;
            return RedirectToAction("Index");
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
            if (DB.Member.Where(m => m.Id == D.Id && m.Enable == 0).FirstOrDefault() != null)
            {
                @TempData["SignUpSuccess"] = "您尚未啟用";
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

                if (DB.Member.Where(m => D.Password == postback.Password).FirstOrDefault() != null)
                {
                    if (DB.Member.Where(m => m.Id == D.Id && m.Email == postback.Email).FirstOrDefault() != null)
                    {
                        if (DB.Member.Where(m => D.Phone == postback.Phone).FirstOrDefault() != null)
                        {
                            //postback並無Id,Enable,ErrorCount的資料,必須先指定在寫入DB
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
                        postback.Id = D.Id;
                        postback.Enable = D.Enable;
                        postback.ErrorCount = D.ErrorCount;
                        DB.Entry(D).CurrentValues.SetValues(postback);
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
                        postback.Id = D.Id;
                        postback.Enable = D.Enable;
                        postback.ErrorCount = D.ErrorCount;
                        DB.Entry(D).CurrentValues.SetValues(postback);
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
        public string RepeatEmailValidation()
        {
            try
            {
                EmailValidation(Session["MemberEmail"] as string, Randomcode());
                return "成功";
            }
            catch (Exception e)
            {
                return "寄出失敗 code:" + e ;
            }
        }
        public void EmailValidation(string Email, string Veriflcationcode)
        {
            try
            {
                //設定郵件相關參數
                MailMessage Mail = new MailMessage();
                Mail.To.Add(Email);//收件人
                //參數屬性是寄件人名字及地址但地址似乎會被Gmail覆蓋,此處我的寄件人地址是User但實際收件顯示的依然是Host
                Mail.From = new MailAddress(Email , "Lativ", System.Text.Encoding.UTF8);
                Mail.Subject = "帳號驗證";//標題
                Mail.SubjectEncoding = System.Text.Encoding.UTF8;
                Mail.Body = "您的驗證碼是:" + Veriflcationcode;//內容,如定義為Html信件則可加入Html語法(CSS及Javescript未試過)
                Mail.BodyEncoding = System.Text.Encoding.UTF8;
                Mail.IsBodyHtml = true;//是否定義為Html信件

                //寄出參數
                SmtpClient Smtp = new SmtpClient();
                //登入信箱用參數,在信箱內部設定:低安全性應用程式→開啟較低的應用程式存取權限,需打開則無法引用
                Smtp.Credentials = new System.Net.NetworkCredential("risnbox@gmail.com", "RISNBOX8520");
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

            for (var i = 0; i < 6; i++)//使用for迴圈執行六次來獲得六碼隨機碼
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
                return RedirectToAction("Index");
            }
            if (data.ErrorCount >= 3)
            {
                var code = Randomcode();//呼叫產生亂數方法寫入code
                Session["Veriflcationcode"] = code;//由於重寄驗證碼所以Session內容須重置
                ViewBag.ValidationErrorMessage = "驗證碼輸入錯誤次數超過3次,已重新寄出驗證信";
                EmailValidation(data.Email, Session["Veriflcationcode"] as string);
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
        public ActionResult Location()
        {
            //新增City欄位項目表(使用Enum)
            var x = new SelectList(Enum.GetValues(typeof(CitySelect)));
            ViewBag.CitySelect = x;
            return View();
        }
        public string ViewStore()
        {
            var d = Convert.ToInt32(Session["Member"].ToString());
            var MS = DB.Member_Store.Where(m => m.Member_Id == d).ToList();
            var Stores = new List<Store>();
            foreach(var i in MS)
            {
                Stores.Add(i.Store);
            }
            string json = JsonConvert.SerializeObject(Stores);
            return json;

        }
        public int SelectStore1(string Name, int ID, string Address, string TelNo)
        {
            var d = Convert.ToInt32(Session["Member"].ToString());
            var D = DB.Member.Include("Member_Store").Where(m => m.Id == d).FirstOrDefault();
            var Sdata = new Store();
            if (DB.Store.Where(m => m.StoreId == ID).FirstOrDefault() == null) //檢查該門市是否已被新增過
            {
                Sdata.StoreAddress = Address;
                Sdata.StoreId = ID;
                Sdata.StoreName = Name;
                Sdata.StoreTelNo = TelNo;
                DB.Store.Add(Sdata);
            }
            Sdata = DB.Store.Find(ID);
            if (DB.Member_Store.Where(m => m.Member_Id == d && m.Store_Id == ID).FirstOrDefault() != null)//檢查使用者是否已選擇過該門市
            {
                TempData["SelectError"] = "您已選擇過該門市";
                return 1;//由於使用AJAX因此轉址部分需透過Jquery 所以回傳int讓Jquery判斷情況為何
            }
            var Linkdata = new Member_Store();
            Linkdata.Member = D;
            Linkdata.Store = Sdata;
            DB.Member_Store.Add(Linkdata);
            try
            {
                DB.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                TempData["SelectError"] = "選擇門市失敗,ErrorCode:" + e;
                return 2;//由於使用AJAX因此轉址部分需透過Jquery 所以回傳int讓Jquery判斷情況為何
            }
            TempData["SelectError"] = "選擇門市成功,門市名稱為" + Sdata.StoreName;
            return 0;//由於使用AJAX因此轉址部分需透過Jquery 所以回傳int讓Jquery判斷情況為何
        }
        [HttpPost]
        public int SelectStore(string Name, int ID, string Address, string TelNo)
        {
            var d = Convert.ToInt32(Session["Member"].ToString());
            var D = DB.Member.Include("Member_Store").Where(m => m.Id == d).FirstOrDefault();
            var Sdata = new Store();
            if (DB.Store.Where(m => m.StoreId == ID).FirstOrDefault() == null) //檢查該門市是否已被新增過
            {
                Sdata.StoreAddress = Address;
                Sdata.StoreId = ID;
                Sdata.StoreName = Name;
                Sdata.StoreTelNo = TelNo;
                DB.Store.Add(Sdata);
            }
            Sdata = DB.Store.Find(ID);
            if(DB.Member_Store.Where(m => m.Member_Id == d && m.Store_Id == ID).FirstOrDefault() != null)//檢查使用者是否已選擇過該門市
            {
                TempData["SelectError"] = "您已選擇過該門市";
                return 1;//由於使用AJAX因此轉址部分需透過Jquery 所以回傳int讓Jquery判斷情況為何
            }
            var Linkdata = new Member_Store();
            Linkdata.Member = D;
            Linkdata.Store = Sdata;
            DB.Member_Store.Add(Linkdata);
            try
            {
                DB.SaveChanges();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                TempData["SelectError"] = "選擇門市失敗,ErrorCode:" + e;
                return 2;//由於使用AJAX因此轉址部分需透過Jquery 所以回傳int讓Jquery判斷情況為何
            }
            TempData["SelectError"] = "選擇門市成功,門市名稱為" + Sdata.StoreName;
            return 0;//由於使用AJAX因此轉址部分需透過Jquery 所以回傳int讓Jquery判斷情況為何
        }
        public string Get711(string city, string town)
        {
            //利用HttpWeb發出請求取711門市資料
            var url = "https://emap.pcsc.com.tw/EMapSDK.aspx?commandid=SearchStore&city=" + city + "&town=" + town;
            try
            {
                var request = WebRequest.Create(url) as HttpWebRequest;//發送請求
                var response = request.GetResponse() as HttpWebResponse;//取得回應
                var stream = response.GetResponseStream();//取得回應資料流
                StreamReader streamReader = new StreamReader(stream);//讀取資料流內容
                var xml = streamReader.ReadToEnd();//將內容寫入變數
                streamReader.Close();
                stream.Close();//關閉資料流
                xml = xml.Remove(0, 38);//因傳入的資料是XML格式 此處用處是刪除XML固定標頭 版本號及編碼類別
                xml = xml.Replace(" ", "");//清除資料內的空格避免轉為JSON格式錯誤
                Console.WriteLine(xml);
                //將XML轉為JSON
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);//利用XMLDoc將傳入的xml字串轉為xml物件
                string json = JsonConvert.SerializeXmlNode(doc);
                return json;
            }
            catch (Exception e)
            {
                return ("請求失敗" + e);
            }
        }

        public string Gettown(string city)
        {
            //Server.MapPath可取得專案實體位置
            var data = System.IO.File.ReadAllText(Server.MapPath("~/Models/Taiwantown.json"), Encoding.UTF8);
            List<data2> d2 = JsonConvert.DeserializeObject<List<data2>>(data);//將JSON轉成List物件操作
            List<data1> d1 = JsonConvert.DeserializeObject<List<data1>>(data);//因此JSON為多層結構所以需要轉多層
            var City = d1.Where(m => m.name == city).FirstOrDefault();//配對城市名稱
            var L = City.districts.ToList();//將該城市清單轉為List
            var Town = new List<string> { };//創造新的StringList
            foreach(var i in L)
            {
                Town.Add(i.name);//利用loop將資料寫入要list
            }
            var Json = JsonConvert.SerializeObject(Town);//將list轉回JSON回傳至前端供Jquery操作
            return Json;
        }
        
        public void DeleteStore(int store)
        {
            var d = Convert.ToInt32(Session["Member"].ToString());
            var MS = DB.Member_Store.Where(m => m.Member_Id == d).ToList();
            var delete = MS.Where(m => m.Store_Id == store).FirstOrDefault();
            DB.Member_Store.Remove(delete);
            DB.SaveChanges();
        } 
        
        public ActionResult KeepView()
        {
            var d = Convert.ToInt32(Session["Member"].ToString());
            var data = (from keep in DB.Keep
                        where keep.Userid == d
                        join prod in DB.Product on keep.Prodid equals prod.Id
                        join PM in DB.Prod_Img on keep.Prodid equals PM.Pid
                        join img in DB.Img on PM.Mid equals img.Id
                        where img.Type == "previewed"
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

        public void Keep(int Pid)
        {
            
            var keep = new Keep() { Userid = Convert.ToInt32(Session["Member"].ToString()),Prodid=Pid };
            if (DB.Keep.Where(m => m.Userid == keep.Userid && m.Prodid == keep.Prodid).FirstOrDefault() == null)
            {
                DB.Keep.Add(keep);
                DB.SaveChanges();
            }
            
        }

        public void DelKeep(int Pid)
        {
            var U = Convert.ToInt32(Session["Member"].ToString());
            var data = DB.Keep.Where(m => m.Prodid == Pid && m.Userid == U).FirstOrDefault();
            DB.Keep.Remove(data);
            DB.SaveChanges();
        }

        public ActionResult NewProd1()
        {
            if (Convert.ToInt32(Session["Member"].ToString()) != 12&&Session["Member"]==null)
            {
                Session["Member"] = "";
                return RedirectToAction("Index");
            }

            return View();
        }
        
        public bool UpdataFile(HttpPostedFileBase file, string Type)
        {
            var E = file.FileName.Split('.');//分割字串
            string[] VD = { "jpg", "img", "png", "jpeg" };//驗證副檔名
            var bl = false;
            if (E.Length > 2) 
            {
                return false;
            }
            else
            {
                foreach (var i in VD)
                {
                    if (E[1] == i)
                    {
                        bl = true;
                    }
                }
                if (bl == false)
                {
                    return false;
                }
                else
                {
                    var PH = Path.Combine(Server.MapPath("~/UpdataFiles/"), file.FileName);//編輯儲存路徑
                    file.SaveAs(PH);//執行儲存
                    var Img = new Img() { Type = Type, FileName = file.FileName };
                    DB.Img.Add(Img);
                    return true;
                }
            }
        }
        [HttpPost]
        public ActionResult NewProd1(string name, int price, string type, string Class, HttpPostedFileBase previewed ,HttpPostedFileBase title, HttpPostedFileBase content)
        {
            if (type == null && Class == null)
            {
                ViewBag.error = "請選擇種類和類別";
                return View();
            }
            if ((UpdataFile(title, "title")&UpdataFile(previewed, "previewed")&UpdataFile(content, "content"))==false)
            {
                ViewBag.error = "新增失敗,只接受圖檔並且檔名不可包含'.'請重新操作";
                return View();
            }
            var E = previewed.FileName.Split('.');
            var prod = new Product() { Name = name, Price = price};
            var cid = (int)Enum.Parse(typeof(ClassSelect), Class);
            int tid = (int)Enum.Parse(typeof(TypeSelect), type);
            DB.Product.Add(prod);
            var PC = new Prod_Class_Type() { Cid = cid, Product = prod, Tid = tid };
            DB.Prod_Class_Type.Add(PC);
            DB.SaveChanges();
            var P = DB.Product.Where(m => m.Name == name).FirstOrDefault();
            var Pid = P.Id;
            var I = DB.Img.Where(m => m.FileName == title.FileName).FirstOrDefault();
            var Iid = I.Id;
            var M = DB.Img.Where(m => m.FileName == previewed.FileName).FirstOrDefault();
            var Mid = M.Id;
            var G = DB.Img.Where(m => m.FileName == content.FileName).FirstOrDefault();
            var Gid = G.Id; 
            Prod_Img[] PM = new Prod_Img[3];
            for(int i = 0; PM.Length > i; i++)
            {
                PM[i] = new Prod_Img();
                PM[i].Pid = Pid;
                if (i == 0) { PM[i].Mid = Iid; }
                if (i == 1) { PM[i].Mid = Mid; }
                if (i == 2) { PM[i].Mid = Gid; }
                DB.Prod_Img.Add(PM[i]);
            }
            var p = PM;
            DB.SaveChanges();
            ViewBag.error = "新增成功";
            return View();
        }
        public string SearchProd(string Stype, string SClass)
        {
            var data = (from PCT in DB.Prod_Class_Type
                        join C in DB.Class on PCT.Class.ClassName equals SClass
                        join T in DB.Type on PCT.Type.TypeName equals Stype
                        join P in DB.Product on PCT.Pid equals P.Id
                        where PCT.Cid == C.Id && PCT.Tid == T.Id && PCT.Pid == P.Id
                        select new
                        {
                            Prod = PCT.Product,
                            Clas = PCT.Class,
                            type = PCT.Type
                        }
                        ).ToList();

            var json = JsonConvert.SerializeObject(data);
            return json;
        }

        public int DelProd(string name)
        {
            int? data = null;
            foreach (var i in DB.Prod_Class_Type)
            {
                if (i.Product.Name == name)
                {
                    data = i.Pid;
                }
            }
            var Delete1 = DB.Product.Where(m => m.Id == data).FirstOrDefault();
            var Delete2 = DB.Prod_Class_Type.Where(m => m.Pid == data).FirstOrDefault();
            try
            {
                DB.Product.Remove(Delete1);
                DB.Prod_Class_Type.Remove(Delete2);
                DB.SaveChanges();
                return 1;
            }
            catch
            {

                return 0;
            }
        }

        public ActionResult Women()
        {
            return View();
        }
        public string GetProd(string TYPE)
        {
            var data = (from Img in DB.Img
                        join PCT in DB.Prod_Class_Type on TYPE equals PCT.Type.TypeName
                        join PM in DB.Prod_Img on PCT.Pid equals PM.Pid
                        where Img.Id == PM.Mid && Img.Type == "previewed"
                        select new
                        {
                            img = Img,
                            prod = PM.Product

                        }).ToList();
            var json = JsonConvert.SerializeObject(data);
            json = json.Replace(" ", "");
            return json;
        }
        public ActionResult ProdDetails(int Pid)
        {
            string keep = null;
            if (Session["Member"] != null)
            {
                var U = Convert.ToInt32(Session["Member"].ToString());
                if (DB.Keep.Where(m => m.Userid == U && m.Prodid == Pid).FirstOrDefault() != null)
                {
                    keep = "true";
                }
            }
            var data = (from img in DB.Img
                        join prod in DB.Product on Pid equals prod.Id
                        join PM in DB.Prod_Img on Pid equals PM.Pid
                        where img.Id == PM.Mid
                        select new
                        {
                            prod = prod,
                            img = img,
                            keep = keep
                        }
                      ).ToList();
            string json = JsonConvert.SerializeObject(data);
            var V = json.Replace(" ","");
            ViewBag.json = V;
            return View();
        }
        public string AddCart(int pid, string color, string size)
        {
            if (Session["Cart"] == null)
            {
                return "false";
            }
            var c = Convert.ToInt32(Session["Cart"].ToString());
            var data = (from Cart in DB.ShoppingCar
                        where Cart.Id == c
                        join C in DB.Color on color equals C.Name
                        join S in DB.Size on size equals S.Name
                        join CD in DB.Cart_Detail on c equals CD.Cid
                        select new
                        {
                            Cart = Cart,
                            CD = CD,
                            C = C.Id,
                            S = S.Id
                        }).FirstOrDefault();
            var pf = DB.ProdFeature.Where(m => m.Pid == pid && m.Cid == data.C && m.Sid == data.S).FirstOrDefault();
            if (pf == null)
            {
                pf = new ProdFeature() { Pid = pid, Cid = data.C, Sid = data.S };
                DB.ProdFeature.Add(pf);
            }
            var q = DB.Quantity.Where(m => m.PFid == pf.Id && m.Did == data.CD.Did).FirstOrDefault();
            if (q != null)
            {
                q.Qty++;
            }
            else
            {
                q = new Quantity() { PFid = pf.Id, Did = data.CD.Did, Qty = 1 };
                DB.Quantity.Add(q);
            }
            DB.SaveChanges();
            return "";
        }
        public ActionResult CarPartial()
        {
            var c = Convert.ToInt32(Session["Cart"].ToString());
            var data = (from Cart in DB.ShoppingCar
                        where Cart.Id == c
                        join CD in DB.Cart_Detail on Cart.Id equals CD.Cid
                        join Qty in DB.Quantity on CD.Did equals Qty.Did
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
            ViewBag.json = json.Replace(" ", "");
            return PartialView();
        }

    }    
}