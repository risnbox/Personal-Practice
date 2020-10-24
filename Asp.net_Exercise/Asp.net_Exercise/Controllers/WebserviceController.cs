using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Asp.net_Exercise.Controllers
{
    [EnableCors(origins: "https://asptest.ml:45678", headers: "*", methods: "*")]

    public class WebserviceController : ApiController
    {
        public string test()
        {
            return "ewerewr";
        }
    }
}
