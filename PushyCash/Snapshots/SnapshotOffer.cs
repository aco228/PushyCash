using Direct.Core;
using PushyCash.Direct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Snapshots
{
	public class SnapshotOffer
	{
		public readonly object LockObj = new object();
		public Afflow.Models.AfflowSnapshot AfflowSnapshotData { get; private set; } = null;

		public string FullName { get => string.Format("{0}.{1}.{2}.{3}.{4}", this.Country, this.MobileOperator, this.Vertical, this.Device, this.OfferID); }
		public bool OfferIsMapped = false;

		public string Country { get { return this.AfflowSnapshotData.Country; } }
		public string MobileOperator { get { return this.AfflowSnapshotData.MobileOperator; } }
		public string OfferID { get { return this.AfflowSnapshotData.OfferID; } }
		public string Vertical { get { return this.AfflowSnapshotData.Vertical; } }
		public string Device { get; protected set; } = string.Empty;

		public List<SnapshotConversion> Conversions { get; protected set; } = new List<SnapshotConversion>();
		public int TotalConversions { get; protected set; } = 0;
		public double TotalPayout { get; protected set; } = 0.0;

		public int CurrentConversions { get { return this.Conversions.Count; } }
		public double CurrentPayout
		{
			get
			{
				lock(this.LockObj)
				{
					double payout = 0.0;
					foreach (SnapshotConversion c in this.Conversions)
						payout += c.Payout;
					return payout;
				}
			}
		}
		public double AvaragePayout
		{
			get
			{
				double payout = 0.0;
				foreach (SnapshotConversion c in this.Conversions)
					payout += c.Payout;
				return Math.Round(payout / (this.Conversions.Count * 1.0), 2);
			}
		}
		public DateTime? FirstConversationCreated { get; protected set; } = null;
		public DateTime? LastConversionCreated
		{
			get
			{
				lock (this.LockObj)
				{
					DateTime? result = null;
					foreach (SnapshotConversion c in this.Conversions)
						if (result == null || c.Created > result.Value)
							result = c.Created;
					return result;
				}
			}
		}

		public List<string> MapTest = new List<string>();

		public SnapshotOffer(Afflow.Models.AfflowSnapshot data)
		{
			this.AfflowSnapshotData = data;
		}
		
		public void AddConversion(double payout, string device, Afflow.Models.AfflowLiveFeed feed)
		{
			if (this.OfferIsMapped)
				return;

			lock (LockObj)
				this.Conversions.Add(new SnapshotConversion() { Created = DateTime.Now, Payout = payout });

			if (!this.FirstConversationCreated.HasValue)
				this.FirstConversationCreated = DateTime.Now;

			this.Device = device;
			this.TotalConversions++;
			this.TotalPayout += payout;
			PConsole.AddUntrackedConvertion(payout);
		}

		public void ClearOldConversions(int minutes)
		{
			lock(this.LockObj)
			{
				for(int i = this.Conversions.Count -1; i >= 0; i--)
					if ((int)(DateTime.Now - this.Conversions.ElementAt(i).Created).TotalMinutes >= minutes)
						this.Conversions.RemoveAt(i);
			}
		}

		public void ClearAllConversions()
		{
			lock (this.LockObj)
			{
				this.Conversions.Clear();
				this.FirstConversationCreated = null;
			}
		}
		
		public void CheckIfOfferIsInteresting()
		{
			if (this.OfferIsMapped || this.Conversions.Count == 0 || this.CurrentConversions < PushyGlobal.InitialConfiguration.InterestingOffersConversions)
				return;

			double minutesPassedFromFirstConversion = Math.Round((DateTime.Now - this.Conversions.ElementAt(0).Created).TotalMinutes, 2);
			
			PLogger.InterestingOffer(this.Country, this.MobileOperator, this.Vertical, this.Device, 
				this.AvaragePayout, this.OfferID, minutesPassedFromFirstConversion,
				this.Conversions.Count, PushyGlobal.InitialConfiguration.InterestingOffersMinutes);

			PushyCashDirect db = PushyCashDirect.Instance;
			DirectContainer dc = db.LoadContainer("SELECT CountryID, MobileOperatorID FROM [].MobileOperator WHERE Name={0} AND CountryName={1}", this.MobileOperator, this.Country);
			if (!dc.HasValue)
				return;

			int deviceID = this.Device.Equals("Android") ? 1 : 2;

			db.Execute(
				@"INSERT INTO [].InterestingOffer (CountryID, MobileOperatorID, DeviceID, CountryName, MobileOperator, Device, OfferID, AvaragePayout, Seconds, Conversions, TrackingMinutes, TrackingConversions, TrackingPayout)",
				dc.GetInt("CountryID").Value,
				dc.GetInt("MobileOperatorID").Value,
				deviceID, 
				this.Country,
				this.MobileOperator,
				this.Device,
				this.OfferID,
				this.AvaragePayout,
				(DateTime.Now - this.Conversions.ElementAt(0).Created).TotalSeconds,
				this.CurrentConversions,
				PushyGlobal.InitialConfiguration.InterestingOffersMinutes,
				PushyGlobal.InitialConfiguration.InterestingOffersConversions,
				0.0);

			this.ClearAllConversions();
		}

	}
}
