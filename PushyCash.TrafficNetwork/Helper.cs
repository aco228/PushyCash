using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.TrafficNetworks.Core
{
	internal static class Helper
	{

		public static List<string> ToStringList(this List<TrafficTargetsInfo> list)
		{
			List<string> result = new List<string>();
			foreach (var l in list) result.Add(l.Name);
			return result;
		}
	}
}
