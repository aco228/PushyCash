using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Direct.Models
{
  public class DirectLog
  {
    public int LogID { get; set; }
    public string Level { get; set; }
    public string Text { get; set; }
    public DateTime Created { get; set; }

    public static void Add(DirectLog log)
    {
      PushyCashDirect db = PushyCashDirect.Instance;
      string sql = $"INSERT INTO PushyCash.core.Log (Level, Text, Created) VALUES ('{log.Level}', '{log.Text}', '{DateTime.Now}')";
      db.Execute(sql);
    }
  }
}
