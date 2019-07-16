using PushyCash.Links;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.PushyConsole
{
	internal class PConsoleHeaderControl : PConsoleControlBase
	{
		private static int Top = 0, Left = 0, Width = 102;
		private static class HeaderValues { public static int Margins = 9, Spacing = 3, TextCount = 16; }

		public PConsoleHeaderControl() : base(Top, Left, Width) { }


		#region # Header printing #

		public void PrintHeader()
		{
			string pattern = "{margin}{key1}{spacing}:{spacing}{val1}{spacing}|{spacing}{key2}{spacing}:{spacing}{val2}{margin}"
				.Replace("{margin}", new StringBuilder().Append(' ', HeaderValues.Margins).ToString())
				.Replace("{spacing}", new StringBuilder().Append(' ', HeaderValues.Spacing).ToString());
			
			this.WriteLine();

			this.WriteLine(this.ConstructHeaderWord(pattern,
				"AppStarted", PushyGlobal.ApplicationStartedString,
				"AppStartPass", PushyGlobal.TimeElapsed + ", " + PushyGlobal.GetDate(DateTime.Now)));
			
			this.WriteLine(this.ConstructHeaderWord(pattern,
				"Tracking", PushyGlobal.TrackedConversions.ToString() + "," + PushyGlobal.UntrackedConversions,
				"Payout & Cost", string.Format("{0} / {1}", Math.Round(PushyGlobal.Payout, 2), Math.Round(PushyGlobal.Costs, 2))));

			this.WriteLine(this.ConstructHeaderWord(pattern,
				"Conv & Click", string.Format("{0} / {1}", PushyGlobal.Conversions, PushyGlobal.Clicks),
				"Revenue today", Math.Round(PushyGlobal.Revenue, 2) + "$"));

			this.WriteLine();
		}

		public string ConstructHeaderWord(string pattern, string info1, string value1, string info2, string value2)
		{
			string result = pattern.Replace("{key1}", this.TrimString(info1, HeaderValues.TextCount))
					.Replace("{val1}", this.TrimString(value1, HeaderValues.TextCount))
					.Replace("{key2}", this.TrimString(info2, HeaderValues.TextCount))
					.Replace("{val2}", this.TrimString(value2, HeaderValues.TextCount));
			return result;
		}

		#endregion
		

	}
}
