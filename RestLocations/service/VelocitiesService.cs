using System;
using System.Net;
using System.Collections.Generic;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using ServiceStack.Common.Web;
using ServiceStack.Text;
using ServiceStack.ServiceHost;
using ServiceStack.Logging;
using MySql.Data.MySqlClient;

using RestLocations.Model;
using RestLocations.Db;

namespace RestLocations.Service
{
	public class VelocitiesService : RestServiceBase<VelLocationInTime>
	{
		double totalDist, avgVel;
		public override object OnGet (VelLocationInTime request)
		{
			string sql = "SELECT log_latitud, log_longitud, log_hora FROM log" +
				" WHERE log_dia = '" + request.Day + "'" +
				" AND usr_sim = '" + request.Sim + "'" +
				" AND log_hora BETWEEN " + request.StartTime + 
					" AND " + request.EndTime;
			
			Global.log.Info("[From: " + this.RequestContext.IpAddress + 
			                  "] GET /velocities/" + request.Sim + "/" +
			                  request.Day + "/" + request.StartTime + 
			                  "/" + request.EndTime);
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
				Global.log.Error("[MySQL Err. " + ex.Number + "] Error in " +
					"mysql connection: " + ex.Message);
				return new HttpResult(new ResponseStatus {
					Message = "Error in MySQL Connection"
				}, HttpStatusCode.InternalServerError);
			}
			catch (Exception ex) {
				Global.log.Error("Error: " + ex.Message);
				return new HttpResult( new ResponseStatus {
					 Message = "Error in Server"
				}, HttpStatusCode.InternalServerError);
			} 
			finally {
				DbUtils.closeDbConnLocal();
			}
			
			if (locations.IsNullOrEmpty()) {
				Global.log.Warn("No locations found");
				return new HttpResult( new ResponseStatus {
					 Message = "No Locations found"
				}, HttpStatusCode.NotFound);
			}
			
			List<VelLocation> velocities = new List<VelLocation>(locations.Count);
			velocities.Add(new VelLocation {
				Distance = 0.0,
				Velocity = 0.0,
				Latitude = locations[0].Latitude,
				Longitude = locations[0].Longitude
			});
			double velocity, distance;
			double partialVel, partialDist,partialLat,partialLng;
			Location loc1, loc2; 
			bool v1,v2,v3,v4,v5;
			int countVel=0;
			partialVel=partialDist=partialLat=partialLng=0.0;
			totalDist=avgVel=velocity=distance=0.0;
			v1=v2=v3=v4=v5=false;

			for (int i = 0; i < locations.Count - 1; ++i)
			{
				loc1 = locations[i];
				loc2 = locations[i + 1];
				distance = LocationCalculations.calculateDistance(loc1,loc2);
				velocity = LocationCalculations.calculateVelocity(distance, loc1, loc2);
				// 0-9,10-29, 30-49, 50-79, 80+  
			
				// Velocidad de 0 a <20				
				if (velocity  >=0 && velocity <20) {				
					v1=true;	
					// Si la velocidad anterior es del mismo tipo, acumula.
					if (v2 ==false && v3==false && v4==false && v5==false ){
						partialVel+=velocity;
						partialDist += distance;
						partialLat=loc2.Latitude;
						partialLng=loc2.Longitude;
						countVel++;		
						if ( i == locations.Count - 2) {	// Si son los ultimos puntos, agrega
							velocities.Add(addPartialVel(partialVel,partialDist,
							                         partialLat,partialLng,countVel));
						}
					}
					else {
					// Si la velocidad anterior es de diferente tipo, la agrega.
						velocities.Add(addPartialVel(partialVel,partialDist,
							                         partialLat,partialLng,countVel));
						v2=v3=v4=v5=false;
					// Inicializa los valores de la nueva velocidad.	
						partialVel=velocity;
						partialDist=distance;
						partialLat=loc2.Latitude;
						partialLng=loc2.Longitude;
						countVel=1;
					// Si son los ultimos puntos, agrega.
						if ( i == locations.Count - 2) {
							velocities.Add(addPartialVel(partialVel,partialDist,
							                         partialLat,partialLng,countVel));
						}
					}
				}
					// Velocidad de 20 a <50
				else if (velocity  >=20 && velocity <50) {
					v2=true;
					if (v1 ==false && v3==false && v4==false && v5==false){		
						partialVel+=velocity;
						partialDist += distance;
						partialLat=loc2.Latitude;
						partialLng=loc2.Longitude;
						countVel++;		
						if ( i == locations.Count - 2) {
							velocities.Add(addPartialVel(partialVel,partialDist,
							                         partialLat,partialLng,countVel));
						}
					}
					else{
						velocities.Add(addPartialVel(partialVel,partialDist,
							                         partialLat,partialLng,countVel));
						v1=v3=v4=v5=false;
						partialVel=velocity;
						partialDist=distance;
						partialLat=loc2.Latitude;
						partialLng=loc2.Longitude;
						countVel=1;
						if ( i == locations.Count - 2) {
							velocities.Add(addPartialVel(partialVel,partialDist,
							                         partialLat,partialLng,countVel));
						}
					}
				}
					// Velocidad de 50 a <70
				else if (velocity  >=50 && velocity <70) {
					v3=true;
					if (v1 ==false && v2==false && v4==false && v5==false){		
						partialVel+=velocity;
						partialDist += distance;	
						partialLat=loc2.Latitude;
						partialLng=loc2.Longitude;
						countVel++;
						if ( i == locations.Count - 2) {
							velocities.Add(addPartialVel(partialVel,partialDist,
							                         partialLat,partialLng,countVel));
						}
					}
					else{
						velocities.Add(addPartialVel(partialVel,partialDist,
							                         partialLat,partialLng,countVel));
						v1=v2=v4=v5=false;
						partialVel=velocity;
						partialDist=distance;
						partialLat=loc2.Latitude;
						partialLng=loc2.Longitude;
						countVel=1;
						if ( i == locations.Count - 2) {
							velocities.Add(addPartialVel(partialVel,partialDist,
							                         partialLat,partialLng,countVel));
						}
					}
				}
					// Velocidad de 50 a <70
				/*else if (velocity  >=50 && velocity <70) {
					
				}*/
					// Velocidad >=70
				else if (velocity  >=70) {
					v5=true;
					if (v1 ==false && v2==false && v3==false && v4==false){		
						partialVel+=velocity;
						partialDist += distance;
						partialLat=loc2.Latitude;
						partialLng=loc2.Longitude;
						countVel++;
						if ( i == locations.Count - 2) {
							velocities.Add(addPartialVel(partialVel,partialDist,
							                         partialLat,partialLng,countVel));
						}
					}
					else{
						velocities.Add(addPartialVel(partialVel,partialDist,
							                         partialLat,partialLng,countVel));
						v1=v2=v3=v4=false;
						partialVel=velocity;
						partialDist=distance;
						partialLat=loc2.Latitude;
						partialLng=loc2.Longitude;
						countVel=1;
						if ( i == locations.Count - 2) {
							velocities.Add(addPartialVel(partialVel,partialDist,
							                         partialLat,partialLng,countVel));
						}
					}					
				} 
			} /// Fin del for 
		/*	 	loc1 = locations[i];
				 loc2 = locations[i + 1];
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
			}*/
			VelLocationResponse response = new VelLocationResponse();
			response.AvgVelocity = avgVel / (double) (velocities.Count - 1);
			response.TotalDistance = totalDist;
			response.locations = velocities;
			Global.log.Info("Returning locations with velocity to " + 
			                this.RequestContext.IpAddress);
			return response;
		}
		
		public VelLocation addPartialVel(double partialVel, double partialDist,
			                      double partialLat, double partialLng, int countVel ) {
					
			VelLocation velLoc = new VelLocation();
			velLoc.Velocity = partialVel/ countVel;
			velLoc.Distance = partialDist;
			velLoc.Latitude = partialLat;
			velLoc.Longitude = partialLng;
			totalDist += velLoc.Distance;
			avgVel += velLoc.Velocity;
			return velLoc;
			}
	}
}

