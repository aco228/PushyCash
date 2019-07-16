using PushyCash.Web.Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PushyCash.Web.Portal.Controllers.Api
{
  public class DebugController : ApiController
  {
    public List<DebugViewModel> Get()
    {
      List<DebugViewModel> model = new List<DebugViewModel>();
      foreach (var item in PushyPortalApplication.DebugWebModel)
      {
        DebugViewModel debug = new DebugViewModel();
        debug.Date = item.Date;
        debug.Level = item.Level;
        debug.Text = item.Text;

        model.Add(debug);
      }

      return model;
    }
  }
}
