using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace PushyCash.Web.Api
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
      // Web API configuration and services

      // Web API routes
      EnableCorsAttribute cors = new EnableCorsAttribute("*", "*","GET,POST,PUT,DELETE");
      config.EnableCors(cors);

      config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
					name: "DefaultApi",
					routeTemplate: "api/{controller}/{id}",
					defaults: new { id = RouteParameter.Optional }
			);

			config.Formatters.Remove(config.Formatters.XmlFormatter);
		}
	}
}
