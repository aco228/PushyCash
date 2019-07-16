using PushyCash.Links;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.PushyConsole
{
	internal class PConsoleTableControl : PConsoleControlBase
	{
		private static int Top = 5, Left = 0, Width = 127;
		private static bool IsHeaderPrinted = false;
		private static class TableValues
		{
			public static int
				Country = 2, MobileOperator = 11,
				OneMinConv = 6, OneMinPyt = 6, TwoMinConv = 6, TwoMinPyt = 6,
				Vertical = 10, OS = 5,
				TotalConversions = 4, TotalPayout = 6, LastConversion = 4;
		};

		public PConsoleTableControl() : base(Top, Left, Width) { }
		
		/*
			TABLE CONTENT
		*/

		public void PrintHeader()
		{
			this.PrintRowSeparator();
			this.PrintRow("Country", "MNO", "1mConv", "1mPyt", "2mConv", "2mPyt", "3mConv", "3mPyt", "Vertical", "OS", "TConversions", "TPayout", "LConv");
			this.PrintRowSeparator();
			IsHeaderPrinted = true;
		}
		public void PrintTableData(List<LinkDisplay> list)
		{
			foreach (Links.LinkDisplay display in list)
				PrintRow(display.Country, display.MobileOperator, 
					display.OneMinuteConversions, display.OneMinuteAvaragePayout, display.TwoMinuteConversions, display.TwoMinuteAvaragePayout, display.ThreeMinuteConversions, display.ThreeMinuteAvaragePayout,
					display.Vertical, display.OS, display.TotalConversions, display.TotalPayout, display.LastConversions);
			this.PrintRowSeparator();
			this.WriteLine();
			
		}
		private void PrintRowSeparator()
		{
			string separator = new StringBuilder().Append('=', 20).ToString();
			string result = "";
			result += "==" + this.TrimString(separator, TableValues.Country);
			result += "===" + this.TrimString(separator, TableValues.MobileOperator);
			result += "===" + this.TrimString(separator, TableValues.OneMinConv);
			result += "===" + this.TrimString(separator, TableValues.OneMinPyt);
			result += "===" + this.TrimString(separator, TableValues.OneMinConv);
			result += "===" + this.TrimString(separator, TableValues.OneMinPyt);
			result += "===" + this.TrimString(separator, TableValues.OneMinConv);
			result += "===" + this.TrimString(separator, TableValues.OneMinPyt);
			result += "===" + this.TrimString(separator, TableValues.Vertical);
			result += "===" + this.TrimString(separator, TableValues.OS);
			result += "===" + this.TrimString(separator, TableValues.TotalConversions);
			result += "===" + this.TrimString(separator, TableValues.TotalPayout);
			result += "===" + this.TrimString(separator, TableValues.LastConversion);
			result += "==";
			this.WriteLine(result);
		}
		private void PrintRow(string country, string mobileOperator, string oneMinConv, string oneMinPyt, string twoMinConv, string twoMinPyt, string threeMinConv, string threeMinPyt, string vertical, string os, string totalConversions, string totalPayout,  string lastConversion)
		{
			string result = "";
			result += "| " + this.TrimString(country, TableValues.Country);
			result += " | " + this.TrimString(mobileOperator, TableValues.MobileOperator);
			result += " | " + this.TrimString(oneMinConv, TableValues.OneMinConv);
			result += " | " + this.TrimString(oneMinPyt, TableValues.OneMinPyt);
			result += " | " + this.TrimString(twoMinConv, TableValues.OneMinConv);
			result += " | " + this.TrimString(twoMinPyt, TableValues.OneMinPyt);
			result += " | " + this.TrimString(threeMinConv, TableValues.OneMinConv);
			result += " | " + this.TrimString(threeMinPyt, TableValues.OneMinPyt);
			result += " | " + this.TrimString(vertical, TableValues.Vertical);
			result += " | " + this.TrimString(os, TableValues.OS);
			result += " | " + this.TrimString(totalConversions, TableValues.TotalConversions);
			result += " | " + this.TrimString(totalPayout, TableValues.TotalPayout);
			result += " | " + this.TrimString(lastConversion, TableValues.LastConversion);
			result += " |";
			this.WriteLine(result);
		}


		/*
			CAMPAIGN CONTENT
		*/

		public void PrintCampaigns()
		{
			try
			{
				if (PushyContext.CampaignManager.ActiveCampaigns == null || PushyContext.CampaignManager.ActiveCampaigns.Count == 0)
					return;

				this.WriteLine();
				foreach(PushyCash.Campaigns.PushyCampaign camp in PushyContext.CampaignManager.ActiveCampaigns)
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.Write($" -> Campaign '{camp.Link.FullName}'");
					Console.ResetColor();
					this.PrintCampaignInformation("TMin/TConv", (camp.WebModel.TrackingMinutes + "/" + camp.WebModel.NumberOfConversions));
					this.PrintCampaignInformation("AllowDef", Math.Round(camp.AllowedDeficit, 2).ToString());
					this.PrintCampaignInformation("Clicks", camp.Metrics.LinkRecomendedClicksWithoutConversion.ToString());
					string offset = "    ";

					Console.Write(Environment.NewLine);

					foreach (var ts in camp.PushyNetworkCampaigns)
					{
						Console.Write(offset);

						if (ts.Value.IsStoped)					Console.BackgroundColor = ConsoleColor.DarkRed;
						else														Console.BackgroundColor =	ConsoleColor.DarkGreen;

						Console.Write("   "); // color indicator
						Console.ResetColor();

						Console.BackgroundColor = ConsoleColor.DarkGray;
						Console.ForegroundColor = ConsoleColor.White;
						Console.Write($" {ts.Value.TrafficCampaign.TrafficNetworkName} ");
						Console.ResetColor();
						Console.Write(" ");

						this.PrintCampaignInformation("Budget", Math.Round(ts.Value.Budget, 2).ToString());

						// conversions
						{
							Console.Write("[Convs: ");
							{
								Console.ForegroundColor = ConsoleColor.White;
								if(ts.Value.Conversions > 0)
									Console.ForegroundColor = ConsoleColor.Green;
								Console.Write(ts.Value.Conversions.ToString());
								Console.ResetColor();
							}
							Console.Write("] ");
						}

						this.PrintCampaignInformation("Clicks", ts.Value.Clicks.ToString());
						this.PrintCampaignInformation("Pay/Cost", (Math.Round(ts.Value.Payout, 2) + "/" + Math.Round(ts.Value.Spent, 2)));
						//this.PrintCampaignInformation("ROI", Math.Round(ts.Value.ROI, 2).ToString());

						if (ts.Value.TrafficCampaign.TrafficSource.SupportsAutoBid)
							this.PrintCampaignInformation("Bid/Visib", (ts.Value.TrafficCampaign.BidValue + " / " + ts.Value.TrafficCampaign.LastVisibility));
						
						// EARNED MONEY

						Console.Write("[");
						Console.ForegroundColor = ts.Value.EarnedMoney <= 0 ? ConsoleColor.Red : ConsoleColor.Green;
						Console.Write(Math.Round(ts.Value.EarnedMoney, 2));
						Console.ResetColor();
						Console.Write("$] ");
						

						Console.Write(Environment.NewLine);
						this.CurrentLineCount++;

						Console.ForegroundColor = ConsoleColor.DarkGray;

						int maxNumber = PushyContext.CampaignManager.ActiveCampaigns.Count >= 5 ? (ts.Value.Logs.Count >= 3 ? 3 : ts.Value.Logs.Count) : ts.Value.Logs.Count;
						for (int i = 0; i < maxNumber; i++)
						{
							var log = ts.Value.Logs.ElementAt(i);
							Console.Write(offset + offset);
							Console.Write($"-> [{PushyGlobal.GetDate(log.Created)}]: {log.Text}");
							Console.Write(Environment.NewLine);
							this.CurrentLineCount++;
						}

						Console.ResetColor();
					}
				}
			}
			catch (Exception e) { }
		}

		private void PrintCampaignInformation(string name, string value)
		{
			Console.Write("["+name+": ");
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write(value);
				Console.ResetColor();
			}
			Console.Write("] ");
		}
		


	}
}
