using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Afflow.MobileOperators
{
	public class AfflowMobileOperatorManager
	{
		public Dictionary<string, List<string>> CountryOperatorMap { get; protected set; } = new Dictionary<string, List<string>>();

		public AfflowMobileOperatorManager()
		{
			foreach (string data in AfflowMobileOperatorData.Data)
			{
				string country = data.Substring(0, 3).Replace(":", string.Empty);
				string operatorsData = data.Substring(3, data.Length - 3);
				string[] operators = operatorsData.Split(',');
				List<string> list = new List<string>();
				foreach (string o in operators)
					list.Add(o.Trim());

				if (CountryOperatorMap.ContainsKey(country))
					CountryOperatorMap[country].AddRange(list);
				else
					this.CountryOperatorMap.Add(country, list);
			}
		}

		public string GetName(string country, string reference)
		{
			if (!this.CountryOperatorMap.ContainsKey(country.ToUpper()))
				return string.Empty;

			foreach (string mno in this.CountryOperatorMap[country.ToUpper()])
				if (mno.ToLower().Contains(reference.Trim().ToLower()))
					return mno;

			return string.Empty;
		}

		public string GetNameDeepSearch(string country, string reference)
		{
			if (!this.CountryOperatorMap.ContainsKey(country.ToUpper()))
				return string.Empty;

			foreach (string mno in this.CountryOperatorMap[country.ToUpper()])
			{
				string refMno = mno.Trim().ToLower();
				string refRef = reference.Trim().ToLower();

				if (refMno.Contains(refRef) || refRef.Contains(refMno))
					return mno;
				
			}

			return string.Empty;
		}

	}
}
