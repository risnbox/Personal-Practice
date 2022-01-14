using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Asp.net_Exercise.Models;

namespace Asp.net_Exercise.WebApi.BackStage
{
    public class BOrderAPIController : ApiController
    {
        DatabaseEntities db = new DatabaseEntities();
        [HttpGet]
        public IHttpActionResult All(DateTime? Startdate, DateTime? Enddate, string Oid, string Mid, string Tid, string Money)
        {
            
            var mid = Convert.ToInt32(Mid);
            int money = Convert.ToInt32(Money);
            //條件為空則無視條件
            var list = db.Order.Where(
                    m => (Startdate == null || m.TradeDate >= Startdate) && (Enddate == null || m.TradeDate <= Enddate) &&
                    (Oid == null || m.MerchantTradeNo == Oid) && (Tid == null || m.TradeNo == Tid) &&
                    (Mid == null || m.User_Id == mid) && (Money == null || m.Total == money)
                ).ToList();
            return Ok(list);
        }
    }
}
