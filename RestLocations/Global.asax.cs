using System;
using System.Collections;
using System.Web;
using System.Web.SessionState;
using Funq;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.Common.Utils;
using ServiceStack.Logging;
using ServiceStack.Logging.Support.Logging;
using MySql.Data.MySqlClient;

using RestLocations.Service;
using RestLocations.Model;
using RestLocations.Db;


namespace RestLocations
{
	public class AppHost : AppHostBase
	{
		public AppHost() : base("Mobile Location REST Services", 
		                        typeof(LocationsService).Assembly){ }
		
		public override void Configure (Container container)
		{
			SetConfig(new EndpointHostConfig
	        {
	            GlobalResponseHeaders =
	            {
	                { "Access-Control-Allow-Origin", "*" },
	                { "Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS" },
					{ "Access-Control-Allow-Headers", "Accept" },
	            },
	        });
			
			// Logging
			//LogManager.LogFactory = new Log4NetFactory(false);
			LogManager.LogFactory = new ConsoleLogFactory();
			Global.log = LogManager.GetLogger(GetType());
			
			// Get MySQL Connection
			Global.mySqlConnLocal = new MySqlConnection(DbUtils.connStringLocal);
			Global.mySqlConnRemote = new MySqlConnection(DbUtils.connStringRemote);
			
			Routes
				.Add<Locations>("/locations", "GET, POST")
				.Add<LocationsForDay>("/locations/{Day}", "GET")
				.Add<LocationsForDay>("/locations/{Sim}/{Day}", "GET")
				.Add<LocationsInTime>("/locations/{Sim}/{Day}/" +
					"{StartTime}/{EndTime}", "GET")
				.Add<LocationsInTime>("/locations/{Day}/" +
					"{StartTime}/{EndTime}", "GET")
				.Add<Users>("/users", "GET")
				.Add<VelLocationInTime>("/velocities/{Sim}/{Day}/" +
					"{StartTime}/{EndTime}", "GET");
		}
	}

	public class Global : System.Web.HttpApplication
	{
		public static MySqlConnection mySqlConnLocal { get; set; }
		public static MySqlConnection mySqlConnRemote { get; set; }
		public static ILog log { get; set; }
		
		protected virtual void Application_Start (Object sender, EventArgs e)
		{
			new AppHost().Init();
		}
	}
}
