using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using Asp.net_Exercise.Models;

namespace Asp.net_Exercise.Controllers
{
    public class StoreAPIController : ApiController
    {
        HttpContext httpContext = HttpContext.Current;
        DatabaseEntities DB = new DatabaseEntities();
        public class postdata
        {
            public string Name { get; set; }
            public int ID { get; set; }
            public string Address { get; set; }
            public string TelNo { get; set; }
        }
        [HttpPost]
        public IHttpActionResult SelectStore(postdata postdata)//選擇商店
        {
            var d = Convert.ToInt32(httpContext.Session["Member"].ToString());
            var D = DB.Member.Include("Member_Store").Where(m => m.Id == d).FirstOrDefault();
            var Sdata = new Store();
            if (DB.Store.Where(m => m.StoreId == postdata.ID).FirstOrDefault() == null) //檢查該門市是否已被新增過
            {
                Sdata.StoreAddress = postdata.Address;
                Sdata.StoreId = postdata.ID;
                Sdata.StoreName = postdata.Name;
                Sdata.StoreTelNo = postdata.TelNo;
                DB.Store.Add(Sdata);
            }
            Sdata = DB.Store.Find(postdata.ID);
            if (DB.Member_Store.Where(m => m.Member_Id == d && m.Store_Id == postdata.ID).FirstOrDefault() != null)//檢查使用者是否已選擇過該門市
            {
                return BadRequest("您已選擇過該門市");
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
                return BadRequest(e.Message);
            }
            return Ok(Sdata.StoreName);
        }
    }
}
