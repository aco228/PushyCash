using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PushyCash.Snapshots
{
	public class SnapshotManager
	{
		private static readonly string _lock_key = "lockKey";
		private string[] _supportedVerticals = new string[] { "adult", "mainstream", "aggressive" };
		private DateTime _lastUpdateDate = DateTime.Now;
		private bool _currentlyUpdating = false;

		// {vertical} / {offerID}.offer
		public Dictionary<string, Dictionary<string, SnapshotOffer>> Offers { get; protected set; } = new Dictionary<string, Dictionary<string, SnapshotOffer>>();
		public Action OnLoadFinished = null;

		public SnapshotManager() { }

		public Dictionary<string, SnapshotOffer> GetOffersDictonary(string vertical, string country, string mobileOperator)
		{
			Dictionary<string, SnapshotOffer> result = new Dictionary<string, SnapshotOffer>();
			foreach (SnapshotOffer offer in this.GetOffers(vertical, country, mobileOperator))
				result.Add(offer.OfferID, offer);
			return result;
		}
		public IEnumerable<SnapshotOffer> GetOffers(string vertical, string country, string mobileOperator)
		{
			lock(_lock_key)
			{
				if (!this.Offers.ContainsKey(vertical))
					yield break;

				var type = this.Offers[vertical];
				foreach(KeyValuePair<string, Snapshots.SnapshotOffer> o in type)
					if (o.Value.Country.Equals(country) && o.Value.MobileOperator.Equals(mobileOperator))
						yield return o.Value;

				yield break;
			}
		}
		public SnapshotOffer GetOffer(string vertical, string country, string offerID)
		{
			try
			{
				if (!this.Offers.ContainsKey(vertical))
					return null;
				var type = this.Offers[vertical];
				string key = string.Format("{0}_{1}", country, offerID);
				return type.ContainsKey(key) ? type[key] : null;
			}
			catch (Exception e) { return null; }
		}
		public void Update()
		{
			if (this._currentlyUpdating) return;
			this._currentlyUpdating = true;

			new Thread(() =>
			{
				Dictionary<string, Dictionary<string, SnapshotOffer>> new_data = new Dictionary<string, Dictionary<string, SnapshotOffer>>();
				bool shouldWriteToExternalStorage = false;

				foreach (string vertical in this._supportedVerticals)
				{
					if (!new_data.ContainsKey(vertical))
						new_data.Add(vertical, new Dictionary<string, SnapshotOffer>());

					var type = new_data[vertical];
					FileInfo info = PushyExternalStorage.GetFileInfo(@"afflow\snapshot_" + vertical);
					if (shouldWriteToExternalStorage == false && !info.Exists)
						shouldWriteToExternalStorage = true;
					
					foreach (Afflow.Models.AfflowSnapshot afs in (!info.Exists ? PushyContext.AfflowManager.GetSnapshotData(vertical) : this.GetStorageSnapshotData(info)))
					{
						string key = string.Format("{0}_{1}", afs.Country, afs.OfferID);
						if (type.ContainsKey(key))
							continue;
						type.Add(key, new SnapshotOffer(afs));
					}

					this._lastUpdateDate = DateTime.Now;
					this._currentlyUpdating = false;
				}
				
				lock (_lock_key)
					this.Offers = new_data;

				if (shouldWriteToExternalStorage)
					this.SaveStorageSnapshotData();

				PushyContext.OnLoadFinished("SnapshotManager");
				OnLoadFinished?.Invoke();
			}).Start();
		}

		#region # External Storage #

		private IEnumerable<Afflow.Models.AfflowSnapshot> GetStorageSnapshotData(FileInfo file)
		{
			List<Afflow.Models.AfflowSnapshot> data = PushyExternalStorage.ReadFromBinaryFile<List<Afflow.Models.AfflowSnapshot>>(file.FullName);
			foreach (var d in data)
				yield return d;
			yield break;
		}

		public void SaveStorageSnapshotData()
		{
			foreach(string vertical in this._supportedVerticals)
			{
				if (!this.Offers.ContainsKey(vertical))
					continue;

				List<Afflow.Models.AfflowSnapshot> data = new List<Afflow.Models.AfflowSnapshot>();
				foreach (var o in this.Offers[vertical])
					data.Add(o.Value.AfflowSnapshotData);

				FileInfo info = PushyExternalStorage.GetFileInfo(@"afflow\snapshot_" + vertical);
				PushyExternalStorage.WriteToBinaryFile<List<Afflow.Models.AfflowSnapshot>>(info.FullName, data);
			}
		}

	#endregion

		// SUMMARY: On main thread update
		public void OnGarbageCollector()
		{
			foreach (KeyValuePair<string, Dictionary<string, SnapshotOffer>> vertical in this.Offers)
				foreach (KeyValuePair<string, SnapshotOffer> offer in vertical.Value)
				{
					offer.Value.ClearOldConversions(2); // 2 minutes is default value
					offer.Value.CheckIfOfferIsInteresting();
				}
		}

	}
}
