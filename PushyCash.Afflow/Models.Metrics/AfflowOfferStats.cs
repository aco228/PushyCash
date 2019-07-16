using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Afflow.Models
{
	public class AfflowOfferStats
	{
		private int? _clicksForConversion = null;

		public string Keyword { get; set; } = string.Empty;
		public int Clicks { get; set; } = 0;
		public int Conversions { get; set; } = 0;
		public double Revenue { get; set; } = 0.0;
		public double CR { get; set; } = 0.0;
		public double OfferEPC { get; set; } = 0.0;
		public double eCPM { get => OfferEPC * 1000; }

		public double ConversionCost { get => this.Revenue / (this.Conversions * 1.0); }
		public double CompareFactor { get => ((this.CR * 10.0) * this.ConversionCost); }

		public int ClicksForConversion
		{
			get
			{
				if (this._clicksForConversion.HasValue)
					return this._clicksForConversion.Value;
				this._clicksForConversion = (int)Math.Floor((this.Clicks * 1.0) / (this.Conversions * 1.0));
				this._clicksForConversion += (int)Math.Floor((this._clicksForConversion.Value * 1.0) * 0.3);
				return this._clicksForConversion.Value;
			}
		}

	}
}
