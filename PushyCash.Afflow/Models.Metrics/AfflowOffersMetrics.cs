using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Afflow.Models
{
	/*
		Vraca podatke o radu Offera u proteklih 7 dana (konverzije, klikove, CR)

		GET https://api.monetizer.co/data/report.php?nid={0}&rcid={1}&order=clicks&direction=desc&keyword=ts&start_ts={2}&end_ts={3}&tz=Europe/Belgrade

	*/

	public class AfflowOffersMetrics
	{
		public List<AfflowOfferStats> OfferStats { get; protected set; } = new List<AfflowOfferStats>();

		public AfflowOfferStats this[string offerID]
		{
			get
			{
				if (!offerID.Contains("#"))
					offerID = "#" + offerID;
				return (from o in this.OfferStats where o.Keyword.Equals(offerID) select o).FirstOrDefault();
			}
		}
		public ulong Clicks
		{
			get
			{
				ulong result = 0;
				foreach (var s in this.OfferStats) result += (ulong)s.Clicks;
				return (ulong)Math.Floor((result * 1.0) / (this.OfferStats.Count * 1.0));
			}
		}
		public ulong Conversions
		{
			get
			{
				ulong result = 0;
				foreach (var s in this.OfferStats) result += (ulong)s.Conversions;
				return (ulong)Math.Floor((result * 1.0) / (this.OfferStats.Count * 1.0));
			}
		}
		public double Revenue
		{
			get
			{
				double result = 0;
				foreach (var s in this.OfferStats) result += s.Revenue;
				return result / (this.OfferStats.Count * 1.0);
			}
		}
		public double AvarageCostPerClick { get; protected set; }
		
		public double MaxECPM
		{
			get
			{
				double maxEcmp = 0.0;
				foreach (var o in this.OfferStats)
					if (o.eCPM > maxEcmp)
						maxEcmp = o.eCPM;
				return maxEcmp;
			}
		}

		//  uzima broj klikova od onog offera koji zahtjeva najvise klikova
		public int ClickForConversionMax 
		{
			get
			{
				int maxClicksForConv = 0;
				foreach (var o in this.OfferStats)
					if (o.ClicksForConversion > maxClicksForConv)
						maxClicksForConv = o.ClicksForConversion;
				return maxClicksForConv;
			}
		} // clicks for conversions for highes offer

		public int RecomendedClicksWithoutConversion { get => this.ClickForConversionMax + (int)Math.Floor(this.ClickForConversionMax * 0.10); }

		// OLD WAY
		//public double RecomendedBudget { get => this.RecomendedClicksWithoutConversion * this.AvarageCostPerClick; }

		public double RecomendedBudget { get => this.MaxECPM + (this.MaxECPM * 0.20); }

		public double LinkRecomendedBudget { get; set; } = 0.0;
		public int LinkRecomendedClicksWithoutConversion { get; set; } = 0;
		
		public AfflowOffersMetrics(double avarageCostPerClick)
		{
			this.AvarageCostPerClick = avarageCostPerClick;
		}

		public void Add(AfflowOfferStats stat)
		{
			this.OfferStats.Add(stat);
		}
		
		
	}
}
