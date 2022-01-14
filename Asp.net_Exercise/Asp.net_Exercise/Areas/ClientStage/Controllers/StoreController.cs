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

namespace Asp.net_Exercise.Areas.ClientStage.Controllers
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
        public string Gettown(string city)//顯示該城市的鄉鎮
        {
            //Server.MapPath可取得專案實體位置
            var data = System.IO.File.ReadAllText(Server.MapPath("~/Models/Taiwantown.json"), Encoding.UTF8);
            List<data2> d2 = JsonConvert.DeserializeObject<List<data2>>(data);//data2型別為VS複製json資料後選擇性貼丄json格式產生
            var City = d2.Where(m => m.name == city).FirstOrDefault();
            var L = City.districts.ToList();//將該城市清單轉為List
            var Town = new List<string>();//創造新的StringList
            foreach (var i in L)
            {
                Town.Add(i.name);//利用loop將資料寫入List
            }
            var Json = JsonConvert.SerializeObject(Town);//將list轉回JSON回傳至前端供Jquery操作
            return Json;
        }
    }
}