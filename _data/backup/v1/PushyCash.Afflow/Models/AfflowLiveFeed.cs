using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Afflow.Models
{
	public class AfflowLiveFeed
	{
		public enum OperativeSystem { None, Android, iOS, Desktop }

		public int LeadID { get; protected set; } = 0;
		public double Payout { get; protected set; } = 0.0;
		public string Country { get; protected set; } = "";
		public string Vertical { get; protected set; } = "";
		public string OfferID { get; protected set; } = "";
		public string OS { get; protected set; } = "";

		public static AfflowLiveFeed Get(dynamic row)
		{
			AfflowLiveFeed entry = new AfflowLiveFeed();
			entry.LeadID = row.lead_id;
			entry.Country = row.country_code;
			entry.Payout = row.amount;
			entry.Vertical = row.vertical;
			entry.OfferID = "#" + row.offer_id;
			entry.OS = row.os_name.ToString();

			return entry;
		}

	}
}
