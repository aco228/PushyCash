using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Direct.Models
{
	public class DirectLink
	{
		public int LinkID { get; set; } = 0;
		public string Name { get; set; } = string.Empty;
		public string Country { get; set; } = string.Empty;
		public string MobileOperator { get; set; } = string.Empty;
		public string Vertical { get; set; } = string.Empty;
		public string Device { get; set; } = string.Empty;

		public int TrackingConversions { get; set; } = 5;
		public double TrackingMinutes { get; set; } = 1.0;

		public static DirectLink LoadOrInsert(string name, string country, string mobileOperator, string vertical, string device)
		{
			DirectContainer dc = PushyCashDirect.Instance.LoadContainer(
				@"SELECT TOP 1 * FROM [].Link 
            WHERE Name={0}, Country={2}, MobileOperator={3}, Vertical={4}, Device={5}
            ORDER BY LinkID DESC", name, country, mobileOperator, vertical, device);

			if (dc != null && dc.HasValue)
				return dc.Convert<DirectLink>();

			int? id = PushyCashDirect.Instance.Execute(
					@"INSERT INTO [].Link (LinkName, Country, MobileOperator,Vertical, Device)", name, country, mobileOperator, vertical, device);

			if (!id.HasValue) return null;

			var result = new DirectLink() { Name = name, Country = country, MobileOperator = mobileOperator, Vertical = vertical, Device = device };
			result.LinkID = id.Value;
			return result;
		}

	}
}
