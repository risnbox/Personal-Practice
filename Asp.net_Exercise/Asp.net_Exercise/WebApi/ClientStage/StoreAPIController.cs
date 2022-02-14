using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using Asp.net_Exercise.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Data.SqlClient;

namespace Asp.net_Exercise.WebApi.ClientStage
{
    public class StoreAPIController : ApiController
    {
        HttpContext httpContext = HttpContext.Current;
        DatabaseEntities DB = new DatabaseEntities();
        [HttpPost]

        public IHttpActionResult SelectStore(Store postdata)//選擇商店
        {
            var d = Convert.ToInt32(httpContext.Session["Member"].ToString());
            if (DB.Store.Where(m => m.StoreId == postdata.StoreId).FirstOrDefault() == null) //檢查該門市是否已被新增過
            {
                DB.Store.Add(postdata);
            }
            if (DB.Member_Store.Where(m => m.Member_Id == d && m.Store_Id == postdata.StoreId).FirstOrDefault() != null)//檢查使用者是否已選擇過該門市
            {
                throw new HttpRequestException("已選擇過該門市");
            }
            var Linkdata = new Member_Store();//新增至會員資料
            Linkdata.Member_Id = Convert.ToInt32(d);
            Linkdata.Store_Id = postdata.StoreId;
            DB.Member_Store.Add(Linkdata);
            try
            {
                DB.SaveChanges();
            }
            catch (SqlException e)
            {
                return BadRequest(e.Message);
            }
            return Ok(postdata.StoreName);
        }
        [HttpGet]
        public async Task<IHttpActionResult> Get7111(string city, string town)
        {
            //發出請求取711門市資料
            var url = "https://emap.pcsc.com.tw/EMapSDK.aspx?commandid=SearchStore&city=" + city + "&town=" + town;
            try
            {
                using (var Client = new HttpClient())
                {
                    Client.Timeout = TimeSpan.FromSeconds(30);
                    HttpResponseMessage resmsg = await Client.GetAsync(url);
                    //711回傳的格式是XML，因此將前面版本及格式說明移除
                    var xml = await resmsg.Content.ReadAsStringAsync();
                    xml = xml.Remove(0, 38);
                    xml = xml.Replace(" ", "");
                    var doc = new XmlDocument();
                    doc.LoadXml(xml);//轉為XML物件
                    return Json(doc);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult ViewStore()//回傳已選擇的商店
        {
            var d = Convert.ToInt32(httpContext.Session["Member"].ToString());
            var MS = DB.Member_Store.Where(m => m.Member_Id == d).ToList();
            var Stores = new List<Store>();
            foreach (var i in MS)
            {
                Stores.Add(i.Store);
            }
            return Json(Stores);
        }
        [HttpGet]
        public void DeleteStore(int store)
        {
            var d = Convert.ToInt32(httpContext.Session["Member"].ToString());
            var MS = DB.Member_Store.Where(m => m.Member_Id == d).ToList();
            var delete = MS.Where(m => m.Store_Id == store).FirstOrDefault();
            DB.Member_Store.Remove(delete);
            DB.SaveChanges();
        }
    }
}
