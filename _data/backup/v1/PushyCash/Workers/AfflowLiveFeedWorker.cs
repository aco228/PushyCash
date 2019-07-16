using PushyCash.Links;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Workers
{
	public class AfflowLiveFeedWorker : WorkerBase
	{
		public AfflowLiveFeedWorker() : base("afflow", "leads", 1000)
		{ }

		public override void OnLoop()
		{

			foreach (Afflow.Models.AfflowLiveFeed feed in PushyContext.AfflowManager.GetLiveFeed())
			{
				List<Link> links = PushyContext.LinkManager.GetLinks(feed.Country, feed.Vertical, feed.OS);
				Snapshots.SnapshotOffer offer = null;

				foreach(Link link in links)
					if (link.ContainsOffer(feed.OfferID))
					{
						link.AddConversion(feed.OfferID, feed.Payout);
						continue;
					}

				offer = PushyContext.SnapshotManager.GetOffer(feed.Vertical, feed.Country, feed.OfferID);
				if (offer == null)
					continue;

				offer.AddConversion(feed.Payout, feed.OS, feed);
			}
		}
	}
}
