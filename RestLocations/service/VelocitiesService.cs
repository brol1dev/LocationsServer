using System;
using System.Net;
using System.Collections.Generic;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using ServiceStack.Common.Web;
using ServiceStack.Text;
using ServiceStack.ServiceHost;
using MySql.Data.MySqlClient;

using RestLocations.Model;
using RestLocations.Db;

namespace RestLocations.Service
{
	public class VelocitiesService : RestServiceBase<VelLocationInTime>
	{
		public override object OnGet (VelLocationInTime request)
		{
			string sql = "SELECT log_latitud, log_longitud, log_hora FROM log" +
				" WHERE log_dia = '" + request.Day + "'" +
				" AND usr_sim = '" + request.Sim + "'" +
				" AND log_hora BETWEEN " + request.StartTime + 
					" AND " + request.EndTime;
			
			List<Location> locations = new List<Location>();
			try {
				DbUtils.openDbConnLocal();
				MySqlCommand cmd = new MySqlCommand(sql, Global.mySqlConnLocal);
				MySqlDataReader reader = cmd.ExecuteReader();
				
				while (reader.Read())
				{
					Location location = new Location();
					location.Time = reader.GetInt64("log_hora");
					location.Latitude = reader.GetDouble("log_latitud");
					location.Longitude = reader.GetDouble("log_longitud");
					locations.Add(location);
				}
				reader.Close();

			}
			catch (MySqlException ex) {
				Console.WriteLine("[MySQL Err. " + ex.Number + "] Error in " +
					"mysql connection: " + ex.Message);
				return new HttpResult(new ResponseStatus {
					Message = "Error in MySQL Connection"
				}, HttpStatusCode.InternalServerError);
			}
			catch (Exception ex) {
				Console.WriteLine("Error: " + ex.Message);
				return new HttpResult( new ResponseStatus {
					 Message = "Error in Server"
				}, HttpStatusCode.InternalServerError);
			} 
			finally {
				DbUtils.closeDbConnLocal();
			}
			
			if (locations.IsNullOrEmpty())
				return new HttpResult( new ResponseStatus {
					 Message = "No Locations found"
				}, HttpStatusCode.NotFound);
			
			List<VelLocation> velocities = new List<VelLocation>(locations.Count);
			velocities.Add(new VelLocation {
				Distance = 0.0,
				Velocity = 0.0,
				Latitude = locations[0].Latitude,
				Longitude = locations[0].Longitude
			});
			double totalDist = 0.0;
			double avgVel = 0.0;
			for (int i = 0; i < locations.Count - 1; ++i)
			{
				Location loc1 = locations[i];
				Location loc2 = locations[i + 1];
				VelLocation velLoc = new VelLocation();
				velLoc.Distance = LocationCalculations.calculateDistance(loc1, 
					loc2);
				velLoc.Velocity = LocationCalculations.calculateVelocity(
					velLoc.Distance, loc1, loc2);
				velLoc.Latitude = loc2.Latitude;
				velLoc.Longitude = loc2.Longitude;
				velocities.Add(velLoc);
				
				totalDist += velLoc.Distance;
				avgVel += velLoc.Velocity;
			}
			
			VelLocationResponse response = new VelLocationResponse();
			response.AvgVelocity = avgVel / (double) (velocities.Count - 1);
			response.TotalDistance = totalDist;
			response.locations = velocities;
			return response;
		}
	}
}

