using PushyCash.Direct.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PushyCash.Web.Api.Controllers
{
  public class LogController : ApiController
  {
    // POST: Log
    public void Post([FromBody]DirectLog log)
    {
      DirectLog.Add(log);
    }

  }
}
