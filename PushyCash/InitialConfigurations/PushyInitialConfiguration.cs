using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash
{
	public class PushyInitialConfiguration
	{
		public bool DontStartNewCampaigns { get; protected set; } = false;
		public int TotalNumberOfActivatedCampaigns { get; set; } = 0;
		public int? MaximumNumberOfActiveCampaigns { get; protected set; } = null;
		public int? MaximumNumberOfCampaigns { get; protected set; } = null;
		public List<string> IgnoredCountries { get; protected set; } = new List<string>();

		public int MinutesTimeoutAfterLinkStartedCampaign_WithNoConversions { get; protected set; } = 210;
		public int MinutesTimeoutAfterLinkStartedCampaign_WithConversionsAndMinusROI { get; protected set; } = 0;
		public int MinutesTimeoutAfterLinkStartedCampaign_WithConversionsAndPlusROI { get; protected set; } = 0;

		public int MaximumMinutesForCampaignWithNoConversions { get; protected set; } = 90;

		public int SecondsForTrafficSourceUpdateStats_WithNoActiveCampaign { get; protected set; } = 14;
		public int SecondsForTrafficSourceUpdateStats_WithActiveCampaign { get; protected set; } = 14;

		public int OptimizationMinutesTimeout { get; protected set; } = 5;

		public int InterestingOffersMinutes { get; protected set; } = 1;
		public int InterestingOffersConversions { get; protected set; } = 10;

		public int MaximumClicksForConversions { get; protected set; } = 1000;
		public int MinimumClicksForConversions { get; protected set; } = 150;
		public double MaximumBudget { get; protected set; } = 15.0;
		public double MaximumBudgetForTrafficCampaign { get; protected set; } = 0.0;
		public int MaximumConversionsForDeficitTracking { get; protected set; } = 3;


		public PushyInitialConfiguration()
		{
			string[] allText = File.ReadAllLines(@"D:\Projects\AkoProjects\dot.net\PushyCash\_storage\main_configuration.as");
			foreach(string l in allText)
			{
				string line = l.Replace("\t", string.Empty);
				if (string.IsNullOrEmpty(line) || line.Length == 0 || line[0] == '#')
					continue;

				string[] info = line.Split('=');

				if (info.Length != 2)
					continue;

				int intConvert;
				double doubleConvert;
				DateTime dateTimeConvert;

				switch(info[0].Trim())
				{
					// Link starting configuration

					case "DontStartNewCampaigns":
						this.DontStartNewCampaigns = info[1].Trim().ToLower().Equals("true");
						break;
					case "MaximumNumberOfActiveCampaigns":
						if (int.TryParse(info[1].Trim(), out intConvert))
							this.MaximumNumberOfActiveCampaigns = intConvert;
						break;
					case "MaximumNumberOfCampaigns":
						if (int.TryParse(info[1].Trim(), out intConvert))
							this.MaximumNumberOfCampaigns = intConvert;
						break;
					case "IgnoredCountries":
						string[] countrySplit = info[1].ToLower().Trim().Split(',');
						foreach (string c in countrySplit)
							IgnoredCountries.Add(c.ToUpper());
						break;
					case "MinutesTimeoutAfterLinkStartedCampaign_WithNoConversions":
						if (int.TryParse(info[1].Trim(), out intConvert))
							this.MinutesTimeoutAfterLinkStartedCampaign_WithNoConversions = intConvert;
						break;
					case "MinutesTimeoutAfterLinkStartedCampaign_WithConversionsAndMinusROI":
						if (int.TryParse(info[1].Trim(), out intConvert))
							this.MinutesTimeoutAfterLinkStartedCampaign_WithConversionsAndMinusROI = intConvert;
						break;
					case "MinutesTimeoutAfterLinkStartedCampaign_WithConversionsAndPlusROI":
						if (int.TryParse(info[1].Trim(), out intConvert))
							this.MinutesTimeoutAfterLinkStartedCampaign_WithConversionsAndPlusROI = intConvert;
						break;
					case "MaximumMinutesForCampaignWithNoConversions":
						if (int.TryParse(info[1].Trim(), out intConvert))
							this.MaximumMinutesForCampaignWithNoConversions = intConvert;
						break;

					case "SecondsForTrafficSourceUpdateStats_WithNoActiveCampaign":
						if (int.TryParse(info[1].Trim(), out intConvert))
							this.SecondsForTrafficSourceUpdateStats_WithNoActiveCampaign = intConvert;
						break;
					case "SecondsForTrafficSourceUpdateStats_WithActiveCampaign":
						if (int.TryParse(info[1].Trim(), out intConvert))
							this.SecondsForTrafficSourceUpdateStats_WithActiveCampaign = intConvert;
						break;

					// Traffic network campaign optimization

					case "OptimizationMinutesTimeout":
						if (int.TryParse(info[1].Trim(), out intConvert))
							this.OptimizationMinutesTimeout = intConvert;
						break;

					// Interesting offers

					case "InterestingOffersMinutes":
						if (int.TryParse(info[1].Trim(), out intConvert))
							this.InterestingOffersMinutes = intConvert;
						break;
					case "InterestingOffersConversions":
						if (int.TryParse(info[1].Trim(), out intConvert))
							this.InterestingOffersConversions = intConvert;
						break;

					// PushyNetworkCampaign click management
					
					case "MaximumClicksForConversions":
						if (int.TryParse(info[1].Trim(), out intConvert))
							this.MaximumClicksForConversions = intConvert;
						break;
					case "MinimumClicksForConversions":
						if (int.TryParse(info[1].Trim(), out intConvert))
							this.MinimumClicksForConversions = intConvert;
						break;
					case "MaximumBudget":
						if (double.TryParse(info[1].Trim(), out doubleConvert))
							this.MaximumBudget = doubleConvert;
						break;
					case "MaximumBudgetForTrafficCampaign":
						if (double.TryParse(info[1].Trim(), out doubleConvert))
							this.MaximumBudgetForTrafficCampaign = doubleConvert;
						break;
					case "MaximumConversionsForDeficitTracking":
						if (int.TryParse(info[1].Trim(), out intConvert))
							this.MaximumConversionsForDeficitTracking = intConvert;
						break;
				}
			}
		}


	}
}
