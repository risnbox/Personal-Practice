using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Asp.net_Exercise.Models;

namespace Asp.net_Exercise.WebApi.BackStage
{
    public class BMemberAPIController : ApiController
    {
        DatabaseEntities db = new DatabaseEntities();
        [HttpGet]
        public IHttpActionResult ChangeStatus(int Id,int Status)
        {
            try
            {
                db.Member.Find(Id).Enable = Status;
                db.SaveChanges();
                var L = db.Member.Find(Id);
                return Ok();
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult Search(string gender, int fb, int google, int status)
        {
            var Mlist = db.Member.Where(m => (gender == "不限" || m.Gender == gender) &&
            (fb == 0 ? true : fb == 1 ? m.FacebookId != null : m.FacebookId == null) &&
            (google == 0 ? true : google == 1 ? m.GoogleId != null : m.GoogleId == null) &&
            (status == 3 ? true : m.Enable == status)
            ).Select(m => new
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Phone = m.Phone,
                Gender = m.Gender,
                Join = m.Joindate,
                Google = m.GoogleId,
                FB = m.FacebookId,
                Status = m.Enable,
                Order = m.Order.Count
            }).ToList();
            return Ok(Mlist);
            /*var Mlist = db.Member.Where(m => (gender == "不限" || m.Gender == gender) &&
            (fb == 0 || fb == 1 ? m.FacebookId != null : false || m.FacebookId == null) &&
            ).Select(m => new
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Phone = m.Phone,
                Gender = m.Gender,
                Join = m.Joindate,
                Google = m.GoogleId,
                FB = m.FacebookId,
                Status = m.Enable,
                Order = m.Order.Count
            }).ToList();
            return Ok(Mlist);*/
        }
    }
}
