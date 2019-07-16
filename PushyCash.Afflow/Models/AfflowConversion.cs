using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Afflow.Models
{
	/*
	{  
      "lead_id":"234331557",
      "amount":"0.09",
      "country_code":"TR",
      "vertical":"aggressive",
      "lead_time":"1529519366",
      "nid":"9803",
      "rcid":"a67c6dc0-6e33-11e8-9ab3-0e9c191f3024",
      "offer_id":"8f5cba",
      "os_name":"Android",
      "browser":"Chrome Mobile",
      "is_omega":"0",
      "is_vauto":"0"
   },
	 */
	public class AfflowConversion
	{

		public int LeadID { get; protected set; } = 0;
		public double Payout { get; protected set; } = 0.0;
		public string Country { get; protected set; } = "";
		public string Vertical { get; protected set; } = "";
		public string OfferID { get; protected set; } = "";
		public string OS { get; protected set; } = "";
		public int NID { get; protected set; } = 0;
		public string RCID { get; protected set; } = "";
		public string Browser { get; protected set; } = "";
		public DateTime Created { get; protected set; }


		public static AfflowConversion Get(dynamic row)
		{
			AfflowConversion entry = new AfflowConversion();
			entry.LeadID = row.lead_id;
			entry.Country = row.country_code;
			entry.Payout = row.amount;
			entry.Vertical = row.vertical;
			entry.OfferID = "#" + row.offer_id;
			entry.OS = row.os_name.ToString();
			entry.NID = row.nid;
			entry.RCID = row.rcid.ToString();
			entry.Browser = row.browser;
			entry.Created = UnixTimeStampToDateTime((int)row.lead_time);

			return entry;
		}

		private static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
		{
			// Unix timestamp is seconds past epoch
			System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
			return dtDateTime;
		}

	}
}
