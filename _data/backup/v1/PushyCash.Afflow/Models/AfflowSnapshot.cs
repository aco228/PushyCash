using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Afflow.Models
{
	[Serializable()]
	public class AfflowSnapshot
	{
		public string Country = string.Empty;
		public string MobileOperator = string.Empty;
		public string Vertical = string.Empty;
		public string OfferID = string.Empty;

		public static AfflowSnapshot Get(dynamic row, string vertical)
		{
			AfflowSnapshot result = new AfflowSnapshot();
			string[] split = row.keyword.ToString().Split(',');

			result.Country = split[0].Substring(0, 2).Trim();
			result.MobileOperator = split[0].Substring(2, split[0].Length - 2).Trim();
			result.OfferID = split[1].Replace(" ", string.Empty).Trim();
			result.Vertical = vertical;

			return result;
		}

	}
}
