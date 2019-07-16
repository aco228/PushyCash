using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.TrafficNetworks.Core
{
	public class TrafficNameAnalyser
	{
		public string Country { get; protected set; } = string.Empty;
		public string MobileOperator { get; protected set; } = string.Empty;
		public string Vertical { get; protected set; } = string.Empty;
		public string Device { get; protected set; } = string.Empty;

		public static TrafficNameAnalyser Anylise(string fullName)
		{
			fullName = fullName.Substring(1);
			string[] basicInfo = fullName.Split('.');
			if (basicInfo.Length != 4)
				return null;
			
			string country = basicInfo[0];
			string mobileOperator = basicInfo[1];
			string vertical = basicInfo[2];
			string device = basicInfo[3];
			return new TrafficNameAnalyser() { Country = country, MobileOperator = mobileOperator, Vertical = vertical, Device = device };
		}
	}
}
