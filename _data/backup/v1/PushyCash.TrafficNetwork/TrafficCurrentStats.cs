using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.TrafficNetworks.Core
{
	public class TrafficCurrentStats
	{
		public double Spent { get; set; } = 0.0;
		public double Payout { get; set; } = 0.0;
		public int Clicks { get; set; } = 0;
		public int Conversions { get; set; } = 0;
	}
}
