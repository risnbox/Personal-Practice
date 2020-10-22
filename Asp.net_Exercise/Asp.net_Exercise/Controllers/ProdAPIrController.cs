using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Asp.net_Exercise.Models;

namespace Asp.net_Exercise.Controllers
{
    public class ProdAPIrController : ApiController
    {
        DatabaseEntities DB = new DatabaseEntities();
        [HttpGet]
        public IHttpActionResult SearchProd(string Stype, string SClass)
        {
            try
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
                foreach (var i in DB.Prod_Class_Type)
                {
                    if (i.Product.Name == name)
                    {
                        data = i.Pid;
                    }
                }
                var Delete1 = DB.Product.Where(m => m.Id == data).FirstOrDefault();
                var Delete2 = DB.Prod_Class_Type.Where(m => m.Pid == data).FirstOrDefault();
                DB.Product.Remove(Delete1);
                DB.Prod_Class_Type.Remove(Delete2);
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
