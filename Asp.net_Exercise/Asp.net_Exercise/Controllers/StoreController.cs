using Asp.net_Exercise.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace Asp.net_Exercise.Controllers
{
    public class StoreController : Controller
    {
        DatabaseEntities DB = new DatabaseEntities();
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
            foreach (var i in MS)
            {
                Stores.Add(i.Store);
            }
            string json = JsonConvert.SerializeObject(Stores);
            return json;

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
            foreach (var i in L)
            {
                Town.Add(i.name);//利用loop將資料寫入要list
            }
            var Json = JsonConvert.SerializeObject(Town);//將list轉回JSON回傳至前端供Jquery操作
            return Json;
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

        public void DeleteStore(int store)
        {
            var d = Convert.ToInt32(Session["Member"].ToString());
            var MS = DB.Member_Store.Where(m => m.Member_Id == d).ToList();
            var delete = MS.Where(m => m.Store_Id == store).FirstOrDefault();
            DB.Member_Store.Remove(delete);
            DB.SaveChanges();
        }
        public async Task<string> Get711(string city, string town)
        {
            //利用HttpWeb發出請求取711門市資料
            var url = "https://emap.pcsc.com.tw/EMapSDK.aspx?commandid=SearchStore&city=" + city + "&town=" + town;
            try
            {
                using (var Client = new HttpClient())
                {
                    Client.Timeout = TimeSpan.FromSeconds(30);
                    HttpResponseMessage resmsg = await Client.GetAsync(url);
                    var xml = await resmsg.Content.ReadAsStringAsync();
                    xml = xml.Remove(0, 38);
                    xml = xml.Replace(" ", "");
                    var doc = new XmlDocument();
                    doc.LoadXml(xml);
                    string json = JsonConvert.SerializeXmlNode(doc);
                    return json;
                }
            }
            catch (Exception e)
            {
                return ("請求失敗" + e);
            }
        }
    }
}