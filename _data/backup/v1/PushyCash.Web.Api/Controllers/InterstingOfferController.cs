using PushyCash.Direct.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PushyCash.Web.Api.Controllers
{
  public class InterstingOfferController : ApiController
  {
    // POST : InterstingOffer
    public void Post([FromBody] DirectInterestingOffer interestingOffer)
    {
      DirectInterestingOffer.Add(interestingOffer);
    }
  }
}