using Direct.Core;
using Newtonsoft.Json;
using PushyCash.Direct;
using PushyCash.Direct.Models;
using PushyCash.Links;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.ConsoleApp.Tests
{
  class MNO
  {
    public string Afflow = "";
    public string Zeropark = "";
  }


  class Program
  {
    static PushyCashDirect DB = PushyCashDirect.Instance;
    static Dictionary<string, List<MNO>> MAP = new Dictionary<string, List<MNO>>();
    static string TEMPLATE = "{'name':'{NAME}','geoCountry':'{COUNTY}','locationTargetingType':'ALL','demographicTargeting':'','bid':'0.0001','dailyBudgetLimited':'true','dailyBudget':'5','budgetLimited':'false','asapSpending':'false','targetsDailyBudgetValidation':'','targetsDailyBudget':'false','sourcesDailyBudgetValidation':'','sourcesDailyBudget':'false','trafficSourceType':'DOMAIN','uaTargetsValidation':'','trafficType':'MOBILE','mobileTrafficType':'{MOBILE_TRAFFIC_TYPE}','mobileCarriersByCountry':{MOBILE_OPERATOR},'mobileTrafficTypeValidation':'','mobileOSs':[{DEVICE}],'frequencyFilterValidation':'','frequencyFilterInHours':'1','adultFiltersValidation':'','adultFilters':['{VERTICAL}'],'dayPartingValidation':'','dayPartingRaw':['MONDAY:0','MONDAY:1','MONDAY:2','MONDAY:3','MONDAY:4','MONDAY:5','MONDAY:6','MONDAY:7','MONDAY:8','MONDAY:9','MONDAY:10','MONDAY:11','MONDAY:12','MONDAY:13','MONDAY:14','MONDAY:15','MONDAY:16','MONDAY:17','MONDAY:18','MONDAY:19','MONDAY:20','MONDAY:21','MONDAY:22','MONDAY:23','TUESDAY:0','TUESDAY:1','TUESDAY:2','TUESDAY:3','TUESDAY:4','TUESDAY:5','TUESDAY:6','TUESDAY:7','TUESDAY:8','TUESDAY:9','TUESDAY:10','TUESDAY:11','TUESDAY:12','TUESDAY:13','TUESDAY:14','TUESDAY:15','TUESDAY:16','TUESDAY:17','TUESDAY:18','TUESDAY:19','TUESDAY:20','TUESDAY:21','TUESDAY:22','TUESDAY:23','WEDNESDAY:0','WEDNESDAY:1','WEDNESDAY:2','WEDNESDAY:3','WEDNESDAY:4','WEDNESDAY:5','WEDNESDAY:6','WEDNESDAY:7','WEDNESDAY:8','WEDNESDAY:9','WEDNESDAY:10','WEDNESDAY:11','WEDNESDAY:12','WEDNESDAY:13','WEDNESDAY:14','WEDNESDAY:15','WEDNESDAY:16','WEDNESDAY:17','WEDNESDAY:18','WEDNESDAY:19','WEDNESDAY:20','WEDNESDAY:21','WEDNESDAY:22','WEDNESDAY:23','THURSDAY:0','THURSDAY:1','THURSDAY:2','THURSDAY:3','THURSDAY:4','THURSDAY:5','THURSDAY:6','THURSDAY:7','THURSDAY:8','THURSDAY:9','THURSDAY:10','THURSDAY:11','THURSDAY:12','THURSDAY:13','THURSDAY:14','THURSDAY:15','THURSDAY:16','THURSDAY:17','THURSDAY:18','THURSDAY:19','THURSDAY:20','THURSDAY:21','THURSDAY:22','THURSDAY:23','FRIDAY:0','FRIDAY:1','FRIDAY:2','FRIDAY:3','FRIDAY:4','FRIDAY:5','FRIDAY:6','FRIDAY:7','FRIDAY:8','FRIDAY:9','FRIDAY:10','FRIDAY:11','FRIDAY:12','FRIDAY:13','FRIDAY:14','FRIDAY:15','FRIDAY:16','FRIDAY:17','FRIDAY:18','FRIDAY:19','FRIDAY:20','FRIDAY:21','FRIDAY:22','FRIDAY:23','SATURDAY:0','SATURDAY:1','SATURDAY:2','SATURDAY:3','SATURDAY:4','SATURDAY:5','SATURDAY:6','SATURDAY:7','SATURDAY:8','SATURDAY:9','SATURDAY:10','SATURDAY:11','SATURDAY:12','SATURDAY:13','SATURDAY:14','SATURDAY:15','SATURDAY:16','SATURDAY:17','SATURDAY:18','SATURDAY:19','SATURDAY:20','SATURDAY:21','SATURDAY:22','SATURDAY:23','SUNDAY:0','SUNDAY:1','SUNDAY:2','SUNDAY:3','SUNDAY:4','SUNDAY:5','SUNDAY:6','SUNDAY:7','SUNDAY:8','SUNDAY:9','SUNDAY:10','SUNDAY:11','SUNDAY:12','SUNDAY:13','SUNDAY:14','SUNDAY:15','SUNDAY:16','SUNDAY:17','SUNDAY:18','SUNDAY:19','SUNDAY:20','SUNDAY:21','SUNDAY:22','SUNDAY:23'],'destinationUrl':'{DESTINATION_URL}','postbackUrl':'http://postback.zeroredirect1.com/zppostback/5e1985f1-616d-11e7-82e6-0e06c6fba698?cid=','cpaMode':'AUTO','verticalId':'f1420bb0-3339-11e8-9573-b6284209f64b','usingPrelanders':'false','paymentModel':'CPA','initStatus':'PAUSED','type':'RON'}";
    static string ZeroparkPattern = "https://win.ningoffer.club/?utm_medium={HASH}&utm_campaign={long_campaign_id}&1={keyword}&2={match}&3={target}&cid={cid}";

    static string GetZeroparkName(string country, string name)
    {
      if(!MAP.ContainsKey(country))
      {
        int a = 0;
      }
      foreach (var m in MAP[country])
        if (m.Afflow.Equals(name))
          return string.Format("[ \"{0}|{1}\" ]", country, m.Zeropark);

      return "";
    }

    static void Main(string[] args)
    {
      PushyContext.AfflowManager = new Afflow.AfflowManager();

      string[] lines = File.ReadAllLines(@"D:\Projects\AkoProjects\dot.net\PushyCash\_data\networks\zeropark\afflow.mobileoperator.map.txt");
      foreach(string line in lines)
      {
        string[] split = line.Split('#');
        if (!MAP.ContainsKey(split[0]))
          MAP.Add(split[0], new List<MNO>() { new MNO() { Afflow = split[1], Zeropark = split[2] } });
        else
          MAP[split[0]].Add(new MNO() { Afflow = split[1], Zeropark = split[2] });
      }

      string result = "";
      
      foreach(Afflow.Models.AfflowLink link in PushyContext.AfflowManager.GetLinks())
      {
        if (!link.TrafficNetwork.Equals("zeropark"))
          continue;

        bool isWifi = link.AfflowMobileOperator.Contains("WiFi");
        string mmno = isWifi ? "null" : GetZeroparkName(link.Country, link.AfflowMobileOperator);
        if(string.IsNullOrEmpty(mmno))
        {
          int wtf_aa = 0;
        }

        string name = link.FullName.Replace(".zeropark", string.Empty);
        string device = link.Device.Equals("android") ? "[\"ANDROID_PHONE:@ALL\",\"ANDROID_TABLET:@ALL\"]" : "[\"IOS_PHONE:@ALL\",\"IOS_TABLET:@ALL\"]";
        string destination = ZeroparkPattern.Replace("{HASH}", link.hash);
        string vertical = "[" + (link.vertical.Equals("adult") ? "ADULT" : "NON_ADULT") + "]";
        string mobile_traffic_type = isWifi ? "WIFI" : "SELECTED";
        string mobile_operator = isWifi ? "null" : mmno;

        string json2 =  "{" + string.Format("\"name\":\"{0}\",", name)
          + string.Format("\"country\":\"{0}\",", link.Country)
          + string.Format("\"device\":\"{0}\",", device)
          + string.Format("\"destination\":\"{0}\",", destination)
          + string.Format("\"vertical\":\"{0}\",", vertical)
          + string.Format("\"mobile_traffic_type\":\"{0}\",", mobile_traffic_type)
          + string.Format("\"mobile_operator\":\"{0}\"", mobile_operator) + "},";

        //string json = TEMPLATE
        //  .Replace("'", "\"")
        //  .Replace("{NAME}", link.FullName.Replace(".zeropark", string.Empty))
        //  .Replace("{COUNTY}", link.Country)
        //  .Replace("{DEVICE}", link.Device.Equals("android") ? "\"ANDROID_PHONE:@ALL\",\"ANDROID_TABLET:@ALL\"" : "\"IOS_PHONE:@ALL\",\"IOS_TABLET:@ALL\"")
        //  .Replace("{DESTINATION_URL}", ZeroparkPattern.Replace("{HASH}", link.hash))
        //  .Replace("{VERTICAL}", link.vertical.Equals("adult") ? "ADULT" : "NON_ADULT")
        //  .Replace("{MOBILE_TRAFFIC_TYPE}", isWifi ? "WIFI" : "SELECTED")
        //  .Replace("{MOBILE_OPERATOR}", isWifi ? "null" : mmno);

        result += json2 + Environment.NewLine;
      }

      Console.ReadKey();
    }
  }
}
