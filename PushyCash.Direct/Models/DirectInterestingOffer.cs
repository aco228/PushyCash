using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Direct.Models
{
  public class DirectInterestingOffer
  {
    public int InterestringOfferID { get; set; }
    public int CountryID { get; set; }
    public int MobileOperatorID { get; set; }
    public int? DeviceID { get; set; }
    public string CountryName { get; set; }
    public string MobileOperator { get; set; }
    public string Device { get; set; }
    public string OfferID { get; set; }
    public double AvaragePayout { get; set; }
    public int Seconds { get; set; }
    public int Conversions { get; set; }
    public int TrackingMinutes { get; set; }
    public int TrackingConversions { get; set; }
    public double TrackingPayout { get; set; }


    public static void Add(DirectInterestingOffer interestingOffer)
    {
      PushyCashDirect db = PushyCashDirect.Instance;

      var deviceID = interestingOffer.DeviceID != null ? interestingOffer.DeviceID.Value : (int?)null;
      var device = interestingOffer.Device == null ? interestingOffer.Device : "NULL";

      string sql = DirectHelper.ConstructQuery("INSERT INTO PushyCash.core.InterestingOffer VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13})",
        interestingOffer.CountryID,
        interestingOffer.MobileOperatorID,
        deviceID,
        interestingOffer.CountryName,
        interestingOffer.MobileOperator,
        interestingOffer.Device,
        interestingOffer.OfferID,
        interestingOffer.AvaragePayout,
        interestingOffer.Seconds,
        interestingOffer.Conversions,
        interestingOffer.TrackingMinutes,
        interestingOffer.TrackingConversions,
        interestingOffer.TrackingPayout,
        DateTime.Now);
      db.Execute(sql);
    }
  }
}
