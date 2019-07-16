using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Web.Core.Models
{
	[Serializable()]
	public class DebugWebModel
	{
    public DateTime Date { get; set; } = DateTime.Now;
    public string Level { get; set; } = "";
		public string Text { get; set; } = "";
    public string CreatedTime { get => Date.ToString("HH:mm"); }
  }
}
