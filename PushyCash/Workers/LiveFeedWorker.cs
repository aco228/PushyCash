using PushyCash.Links;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Workers
{
	public class LiveFeedWorker : WorkerBase
	{
		public LiveFeedWorker() : base(1000)
		{ }

		public override void OnLoop()
		{
			System.Diagnostics.Debug.WriteLine("");
			System.Diagnostics.Debug.WriteLine("---------------");
			System.Diagnostics.Debug.WriteLine("");

			foreach (Afflow.Models.AfflowLiveFeed feed in PushyContext.AfflowManager.GetLiveFeed())
			{
				List<Link> links = PushyContext.LinkManager.GetLinks(feed.Country, feed.Vertical, feed.OS);
				Snapshots.SnapshotOffer offer = null;

				foreach(Link link in links)
					if (link.OfferSnapshotMap.ContainsKey(feed.OfferID))
					{
						if (link.Country.Equals("IT") && link.AfflowMobileOperator.Equals("WIND") && link.Vertical.Equals("mainstream"))
						{
							int a = 0;
						}

						System.Diagnostics.Debug.WriteLine(feed.LeadID + " - " + link.Name);
						link.AddConversion(feed.OfferID, feed.Payout);
						continue;
					}

				offer = PushyContext.SnapshotManager.GetOffer(feed.Vertical, feed.Country, feed.OfferID);
				if (offer == null)
					continue;

				offer.AddConversion(feed.Payout);
			}
		}
	}
}
