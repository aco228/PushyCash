using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Direct.Models.Links
{
	public class DirectLinkConversions : DirectModel
	{
		public DirectLinkConversions() : base("LinkConversions")
		{
		}

		public int LinkConversionsID { get; set; } = 0;
		public string LinkName { get; set; } = string.Empty;
		public int TrackMinutes { get; set; } = 0;
		public int Conversions { get; set; } = 0;
		public double AvaragePayout { get; set; } = 0.0;

		public static void Insert(string linkName, int trackMinutes, int conversions, double avaragePayout)
		{
			PushyCashDirect.Instance.Execute("INSERT INTO [].LinkConversions (LinkName, TrackMinutes, Conversions, AvaragePayout)", linkName, trackMinutes, conversions, avaragePayout);
		}

	}
}
