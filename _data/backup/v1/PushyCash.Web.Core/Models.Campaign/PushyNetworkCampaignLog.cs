using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Web.Core.Models
{
	[Serializable()]
	public class PushyNetworkCampaignLog
	{
		public string LinkName { get; set; } = string.Empty;
		public string PushyNetworkCampaignName { get; set; } = string.Empty;

		public string Text { get; set; } = string.Empty;
		public DateTime Created { get; set; } = DateTime.Now;
    public string CreatedTime { get => Created.ToString("HH:mm"); }
  }
}
