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

		public int InterestingOffersMinutes { get; protected set; } = 1;
		public int InterestingOffersConversions { get; protected set; } = 10;

		public int MaximumClicksForConversions { get; protected set; } = 1000;
		public int MinimumClicksForConversions { get; protected set; } = 150;
		public double MaximumBudget { get; protected set; } = 15.0;


		public PushyInitialConfiguration()
		{
			string[] allText = File.ReadAllLines(@"D:\Projects\AkoProjects\dot.net\PushyCash\_storage\main_configuration.txt");
			foreach(string line in allText)
			{
				if (string.IsNullOrEmpty(line) || line.Length == 0 || line[0] == '#')
					continue;

				string[] info = line.Replace(@"\t", string.Empty).Split('=');

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
						
				} 
			}
		}


	}
}
