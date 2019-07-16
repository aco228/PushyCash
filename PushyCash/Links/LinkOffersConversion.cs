using PushyCash.Afflow.Models;
using PushyCash.Direct.Models.Links;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Links
{
	public class LinkOffersConversion
	{
		private object lock_obj = new object();
		private Link Link { get; set; } = null;

		public Dictionary<string, List<LinkConversion>> Conversions { get; protected set; } = new Dictionary<string, List<LinkConversion>>();

		public List<string> Offers
		{
			get
			{
				List<string> offers = new List<string>();
				foreach (var o in this.Conversions) offers.Add(o.Key);
				return offers;
			}
		}

		public int TrackMinutes { get => this.Link.TrackMinutes; }

		public int OneMinuteConversions { get; protected set; } = 0;
		public double OneMinutePayout { get; protected set; } = 0.0;
		public double AvarageOneMinutePayout { get => OneMinutePayout == 0.0 ? 0 : OneMinutePayout / (OneMinuteConversions * 1.0); }

		public int TwoMinuteConversions { get; protected set; } = 0;
		public double TwoMinutePayout { get; protected set; } = 0.0;
		public double AvarageTwoMinutePayout { get => TwoMinutePayout == 0.0 ? 0 : TwoMinutePayout / (TwoMinuteConversions * 1.0); }

		public int ThreeMinuteConversions { get; protected set; } = 0;
		public double ThreeMinutePayout { get; protected set; } = 0.0;
		public double AvarageThreeMinutePayout { get => ThreeMinutePayout == 0.0 ? 0 : ThreeMinutePayout / (ThreeMinuteConversions * 1.0); }

		public int CurrentConversions { get; protected set; }
		public double CurrentPayout { get; protected set; }
		public double AvaragePayout { get => this.CurrentPayout / (this.CurrentConversions * 1.0); }
		public DateTime LastConversion = new DateTime();

		public LinkOffersConversion(Link link)
		{
			this.Link = link;
		}

		public void WhitelistOffersInAfflow(string afflowMapKey)
		{
			string offers = "";
			foreach (var o in this.Offers)
				offers += '"' + o.Replace("#", string.Empty) + '"' + ",";
			offers = "[" + offers.Substring(0, offers.Length - 1) + "]";

			PushyContext.AfflowManager.WhitelistLink(Link.AfflowLinkMap[afflowMapKey], offers);
		}
		public string GetWhitelistedOffersForSlackReport()
		{
			string offers = "";
			foreach (var o in this.Offers)
				offers += o.Replace("#", string.Empty) + ",";
			offers = offers.Substring(0, offers.Length - 1);
			return offers;
		}

		public void Add(string offerID, double payout)
		{
			lock (lock_obj)
			{
				if (this.Conversions.ContainsKey(offerID))
					this.Conversions[offerID].Add(new LinkConversion() { Payout = payout });
				else
					this.Conversions.Add(offerID, new List<LinkConversion>() { new LinkConversion() { Payout = payout } });
			}

			this.CurrentConversions++;
			this.CurrentPayout += payout;
			this.LastConversion = DateTime.Now;
		}
		public void GarbageCollection()
		{
			int oneMinuteConversions = 0, twoMinuteConversions = 0, threeMinutesConversions = 0;
			double oneMinutePayout = 0.0, twoMinutePayout = 0.0, threeMinutePayout = 0.0;

			lock (lock_obj)
			{
				foreach (KeyValuePair<string, List<LinkConversion>> conv in this.Conversions)
				{
					for (int i = conv.Value.Count - 1; i >= 0; i--)
					{
						double totalMinutes = (DateTime.Now - conv.Value.ElementAt(i).Created).TotalMinutes;
						if (totalMinutes >= 3)
							conv.Value.RemoveAt(i);

						else if (totalMinutes <= 1)
						{
							oneMinuteConversions++;
							oneMinutePayout += conv.Value.ElementAt(i).Payout;
						}
						else if (totalMinutes > 1 && totalMinutes <= 2)
						{
							twoMinuteConversions++;
							twoMinutePayout += conv.Value.ElementAt(i).Payout;
						}
						else if (totalMinutes > 2)
						{
							threeMinutesConversions++;
							threeMinutePayout += conv.Value.ElementAt(i).Payout;
						}
					}
				}
			}

			this.OneMinuteConversions = oneMinuteConversions;
			this.OneMinutePayout = oneMinutePayout;
			this.TwoMinuteConversions = twoMinuteConversions;
			this.TwoMinutePayout = twoMinutePayout;
			this.ThreeMinuteConversions = threeMinutesConversions;
			this.ThreeMinutePayout = threeMinutePayout;

			this.CurrentConversions = this.OneMinuteConversions + this.TwoMinuteConversions;
			this.CurrentPayout = this.OneMinutePayout + this.TwoMinutePayout;
		}
		public LinkOffersConversionResult CheckIfLinkShouldStart()
		{

			foreach (var tracking in PushyGlobal.LinkTrackingOptions.Options)
			{
				if ((tracking.Minutes == 1 && this.OneMinuteConversions <= tracking.MaximumConversions && this.OneMinuteConversions > tracking.MinimumConversions))
					return new LinkOffersConversionResult() { ShoulLinkStart = true, Multiplier = tracking.Multiplier, Minutes = tracking.Minutes, NumberOfConversions = this.OneMinuteConversions };
				if (tracking.Minutes == 2 && this.TwoMinuteConversions <= tracking.MaximumConversions && this.TwoMinuteConversions > tracking.MinimumConversions)
					return new LinkOffersConversionResult() { ShoulLinkStart = true, Multiplier = tracking.Multiplier, Minutes = tracking.Minutes, NumberOfConversions = this.TwoMinuteConversions };
				if (tracking.Minutes == 3 && this.ThreeMinuteConversions <= tracking.MaximumConversions && this.ThreeMinuteConversions > tracking.MinimumConversions)
					return new LinkOffersConversionResult() { ShoulLinkStart = true, Multiplier = tracking.Multiplier, Minutes = tracking.Minutes, NumberOfConversions = this.ThreeMinuteConversions };
			}

			return new LinkOffersConversionResult() { ShoulLinkStart = false };
		}
		public void Clear()
		{
			lock (lock_obj)
				this.Conversions.Clear();

			this.CurrentConversions = 0;
			this.CurrentPayout = 0.0;
		}

	}
}
