using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Asp.net_Exercise.Models;
using System.Web.Http.Cors;

namespace Asp.net_Exercise.WebApi.ClientStage
{
    public class ProdAPIrController : ApiController
    {
        DatabaseEntities DB = new DatabaseEntities();
        [HttpGet]
        public IHttpActionResult SearchProd(string Stype)
        {
            try
            {
                var data = (from PCT in DB.Prod_Type
                            join T in DB.Type on PCT.Type.TypeName equals Stype
                            join P in DB.Product on PCT.Pid equals P.Id
                            where PCT.Tid == T.Id && PCT.Pid == P.Id
                            select new
                            {
                                Prod = PCT.Product,
                                type = PCT.Type
                            }
                        ).ToList();
                return Ok(data);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult DelProd(string name)
        {
            try
            {
                int? data = null;
                foreach (var i in DB.Prod_Type)
                {
                    if (i.Product.Name == name)
                    {
                        data = i.Pid;
                    }
                }
                var Delete1 = DB.Product.Where(m => m.Id == data).FirstOrDefault();
                var Delete2 = DB.Prod_Type.Where(m => m.Pid == data).FirstOrDefault();
                DB.Product.Remove(Delete1);
                DB.Prod_Type.Remove(Delete2);
                DB.SaveChanges();
                return Ok();
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
