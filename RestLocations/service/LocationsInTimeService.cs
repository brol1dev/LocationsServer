using System;
using System.Net;
using System.Collections.Generic;
using ServiceStack.ServiceInterface;
using ServiceStack.Common.Web;
using ServiceStack.Text;
using ServiceStack.ServiceHost;
using MySql.Data.MySqlClient;

using RestLocations.Model;
using RestLocations.Db;

namespace RestLocations.Service
{
	public class LocationsInTimeService : RestServiceBase<LocationsInTime>
	{
		public override object OnGet (LocationsInTime request)
		{
			string sql = "SELECT log_id, usr_sim, log_dia, log_hora, " +
				"log_latitud, log_longitud FROM log " +
				"WHERE log_dia = '" + request.Day + "' " +
				"AND log_hora BETWEEN " + request.StartTime + 
					" AND " + request.EndTime;
			if (request.Sim != null)
				sql += " AND usr_sim = '" + request.Sim + "'";
			
			Locations locations = new Locations();
			try {
				DbUtils.openDbConnLocal();
				MySqlCommand cmd = new MySqlCommand(sql, Global.mySqlConnLocal);
				MySqlDataReader reader = cmd.ExecuteReader();
				
				locations.locations = new List<Location>();
				while (reader.Read())
				{
					Location location = new Location();
					location.Id = reader.GetInt32("log_id");
					location.Sim = reader.GetString("usr_sim");
					location.Day = reader.GetString("log_dia");
					location.Time = reader.GetInt64("log_hora");
					location.Latitude = reader.GetDouble("log_latitud");
					location.Longitude = reader.GetDouble("log_longitud");
					locations.locations.Add (location);
				}
				reader.Close();
			}
			catch (MySqlException ex) {
				Console.WriteLine("[MySQL Err. " + ex.Number + "] Error in " +
					"mysql connection: " + ex.Message);
				return new HttpResult(locations.locations, 
				                      HttpStatusCode.InternalServerError);
			}
			catch (Exception ex) {
				Console.WriteLine("Error: " + ex.Message);
				return new HttpResult(locations.locations, 
				                      HttpStatusCode.InternalServerError);
			} 
			finally {
				DbUtils.closeDbConnLocal();
			}
			return new LocationsForDayResponse { locations = locations.locations };
		}
	}
}

