using PushyCash.Direct.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PushyCash.Web.Api.Controllers
{
	public class TrackingOptionsController : ApiController
	{
		// GET: api/TrackingOptions
		public List<DirectTrackingOptions> Get()
		{
			var result = DirectTrackingOptions.GetAll();
			return result;
		}

    [HttpGet]
    //GET: api/TrackingOptions?country={cid}&mobileOperator={mo}&vertical={v}&device={d}
    public List<DirectTrackingOptions> Get([FromUri] string country, [FromUri] string mobileOperator, [FromUri] string vertical, [FromUri] string device)
    {
      return DirectTrackingOptions.GetQuery(country, mobileOperator, vertical, device);
    }

		// GET: api/TrackingOptions/5
		public DirectTrackingOptions Get(int id)
		{
      return DirectTrackingOptions.Get(id);
		}

		// POST: api/TrackingOptions
		public int Post([FromBody]DirectTrackingOptions trackingOptions)
		{
      return DirectTrackingOptions.Update(trackingOptions);
		}

		// PUT: api/TrackingOptions/5
		public void Put(int id, [FromBody]DirectTrackingOptions trackingOptions)
		{
      DirectTrackingOptions.Update(id, trackingOptions);
		}

		// DELETE: api/TrackingOptions/5
		public void Delete(int id)
		{
      DirectTrackingOptions.Delete(id);
		}
	}
}
