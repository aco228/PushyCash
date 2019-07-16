using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Direct.Models
{
	public class DirectTrackingOptions
	{
		public int TrackingOptionsID { get; set; }
		public string CountryName { get; set;}
		public string MobileOperator { get; set;}
		public string Vertical { get; set;}
		public string Device { get; set;}
		public int? TrackingConversions { get; set;}
		public double? TrackingPayout { get; set;}
		public int? TrackingMinutes { get; set;}
    public int? ClicksPerConversion { get; set; }
    public double? ToleratedDeficit { get; set; }


		public static List<DirectTrackingOptions> GetAll()
		{
			PushyCashDirect db = PushyCashDirect.Instance;
			DirectContainer dc = db.LoadContainer("SELECT * FROM PushyCash.core.TrackingOptions");
			return dc.ConvertList<DirectTrackingOptions>();
		}

    public static List<DirectTrackingOptions> GetQuery(string country, string mobileOperator, string vertical, string device)
    {
      PushyCashDirect db = PushyCashDirect.Instance;
      List<string> conditions = new List<string>();

      string query = "";
      
      if(country != null ) conditions.Add($"CountryName = '{country}'");
      if(mobileOperator != null) conditions.Add($"MobileOperator  = '{mobileOperator}'");
      if(vertical != null)  conditions.Add($"Vertical = '{vertical}'");
      if(device != null)  string.Format($" AND Device = '{device}'");

      if (conditions.Any())
        query = " WHERE " + string.Join(" AND ", conditions.ToArray()) + " OR TrackingOptionsID = 1";
      
      string sql = $"SELECT * FROM PushyCash.core.TrackingOptions {query}";
      DirectContainer dc = db.LoadContainer(sql);

      return dc.ConvertList<DirectTrackingOptions>();
    }

    public static void Delete(int id)
    {
      PushyCashDirect db = PushyCashDirect.Instance;
      db.Execute("DELETE FROM PushyCash.core.TrackingOptions WHERE TrackingOptionsID = " + id);
    }

    public static DirectTrackingOptions Get(int id)
    {
      PushyCashDirect db = PushyCashDirect.Instance;
      DirectContainer dc = db.LoadContainer("SELECT * FROM PushyCash.core.TrackingOptions WHERE TrackingOptionsID = " + id);
      return dc.Convert<DirectTrackingOptions>();
    }

    public static int Update(DirectTrackingOptions trackingOptions)
    {
      PushyCashDirect db = new PushyCashDirect();

      var countryName = trackingOptions.CountryName == null ? "is null" : $" = '{trackingOptions.CountryName}'";
      var mobileOperator = trackingOptions.MobileOperator == null ? "is null" : $" = '{trackingOptions.MobileOperator}'";
      var vertical = trackingOptions.Vertical == null ? "is null" : $" = '{trackingOptions.Vertical}'";
      var device = trackingOptions.Device == null ? "is null" : $" = '{trackingOptions.Device}'";

      var trackingConversions = trackingOptions.TrackingConversions != null ? trackingOptions.TrackingConversions.Value : (int?)null;
      var trackingPayout = trackingOptions.TrackingPayout != null ? trackingOptions.TrackingPayout.Value : (double?)null;
      var trackingMinutes = trackingOptions.TrackingMinutes != null ? trackingOptions.TrackingMinutes.Value : (int?)null;
      var clicksPerCovnbersions = trackingOptions.ClicksPerConversion != null ? trackingOptions.ClicksPerConversion.Value : (int?)null;
      var toleratedDeficit = trackingOptions.ToleratedDeficit != null ? trackingOptions.ToleratedDeficit.Value : (double?)null;

      string sql = $"SELECT TOP 1 * FROM PushyCash.core.TrackingOptions WHERE CountryName {countryName} AND MobileOperator {mobileOperator} " +
                   $" AND Vertical {vertical} AND Device {device}";
      DirectContainer dc = db.LoadContainer(sql);

      if(dc.RowsCount == 1)
      {
        DirectTrackingOptions to = dc.Convert<DirectTrackingOptions>();
        string sqlUpdate = DirectHelper.ConstructQuery("UPDATE PushyCash.core.TrackingOptions SET TrackingConversions = {0}, TrackingPayout = {1}, TrackingMinutes = {2}," +
                           " ClicksPerConversion = {3}, ToleratedDeficit = {4}, Updated = {5} WHERE TrackingOptionsID = {5}", trackingConversions, trackingPayout, trackingMinutes, clicksPerCovnbersions, toleratedDeficit, DateTime.Now,to.TrackingOptionsID);
        db.Execute(sqlUpdate);
        return to.TrackingOptionsID;
      }
      else
      {
        string sqlInsert = DirectHelper.ConstructQuery("INSERT INTO PushyCash.core.TrackingOptions VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9},{10})", trackingOptions.CountryName, trackingOptions.MobileOperator, trackingOptions.Vertical, trackingOptions.Device, trackingConversions, trackingPayout, trackingMinutes, clicksPerCovnbersions, toleratedDeficit, DateTime.Now, DateTime.Now);
        return db.Execute(sqlInsert).Value;
      }
    }

    public static void Update(int id, DirectTrackingOptions trackingOptions)
    {
      PushyCashDirect db = new PushyCashDirect();

      var trackingConversions = trackingOptions.TrackingConversions != null ? trackingOptions.TrackingConversions.Value : (int?)null;
      var trackingPayout = trackingOptions.TrackingPayout != null ? trackingOptions.TrackingPayout.Value : (double?)null;
      var trackingMinutes = trackingOptions.TrackingMinutes != null ? trackingOptions.TrackingMinutes.Value : (int?)null;
      var clicksPerCovnbersions = trackingOptions.ClicksPerConversion != null ? trackingOptions.ClicksPerConversion.Value : (int?)null;
      var toleratedDeficit = trackingOptions.ToleratedDeficit != null ? trackingOptions.ToleratedDeficit.Value : (double?)null;

      string sql = $"SELECT TOP 1 TrackingOptionsID FROM PushyCash.core.TrackingOptions WHERE TrackingOptionsID = {id}";
      int? trID = db.LoadInt(sql);

      string sqlUpdate = DirectHelper.ConstructQuery("UPDATE PushyCash.core.TrackingOptions SET TrackingConversions = {0}, TrackingPayout = {1}, TrackingMinutes = {2}," +
                           " ClicksPerConversion = {3}, ToleratedDeficit = {4}, Updated = {5} WHERE TrackingOptionsID = {6}", trackingConversions, trackingPayout, trackingMinutes, clicksPerCovnbersions, toleratedDeficit, DateTime.Now, id);
      db.Execute(sqlUpdate);
    }
	}
}
