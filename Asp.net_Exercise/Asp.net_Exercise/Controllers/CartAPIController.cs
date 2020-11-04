using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Asp.net_Exercise.Models;

namespace Asp.net_Exercise.Controllers
{
    [EnableCors(origins: "https://asptest.ml:45678", headers: "*", methods: "*")]
    public class CartAPIController : ApiController
    {
        DatabaseEntities DB = new DatabaseEntities();
        //在WebApicontroller使用Session 參考資料:https://ithelp.ithome.com.tw/articles/10229385
        HttpContext httpContext = HttpContext.Current;
        //---------------------------------------------------------------
        [HttpGet]
        public IHttpActionResult Partial()
        {
            try
            {
                var c = Convert.ToInt32(httpContext.Session["Cart"].ToString());
                var data = (from Cart in DB.ShoppingCar
                            where Cart.Id == c
                            join Qty in DB.Quantity on c equals Qty.Cid
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
                return Ok(data);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }


    }
}
