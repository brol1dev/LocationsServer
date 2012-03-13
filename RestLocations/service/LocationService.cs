//using System;
//using System.Net;
//using ServiceStack.ServiceInterface;
//using ServiceStack.Common.Web;
//using ServiceStack.Text;
//using ServiceStack.ServiceHost;
//using MySql.Data.MySqlClient;
//
//using RestLocations.Model;
//using RestLocations.Db;
//
//namespace RestLocations.Service
//{
//	public class LocationService : RestServiceBase<Location>
//	{
//		public override object OnGet (Location request)
//		{
//			string sql = "SELECT log_id, usr_sim, log_dia, log_hora, " +
//				"log_latitud, log_longitud FROM log";
//			
//			try {
//				DbUtils.openDbConnLocal();
//				MySqlCommand cmd = new MySqlCommand(sql, Global.mySqlConnLocal);
//				MySqlDataReader reader = cmd.ExecuteReader();
//				while (reader.Read())
//				{
//					
//				}
//				reader.Close();
//			}
//			catch (MySqlException ex) {
//				Console.WriteLine("[MySQL Err. " + ex.Number + "] Error in " +
//					"mysql connection: " + ex.Message);
//				return new HttpResult(request) {
//					StatusCode = HttpStatusCode.InternalServerError,
//				};
//			}
//			catch (Exception ex) {
//				Console.WriteLine("Error: " + ex.Message);
//				return new HttpResult(request) {
//					StatusCode = HttpStatusCode.InternalServerError,
//				};
//			}
//			DbUtils.closeDbConnLocal();
//			return new LocationResponse { location = request };
//		}
//
//		public override object OnPost (Location request)
//		{
//			string inSql = "INSERT INTO log (usr_sim, log_dia, log_hora, " +
//				"log_longitud, log_latitud) VALUES (@sim, @day, @time, " +
//				"@longitude, @latitude)";
//			
//			try {
//				DbUtils.openDbConnLocal();
//				MySqlCommand cmd = new MySqlCommand(inSql, Global.mySqlConnLocal);
//				cmd.Parameters.AddWithValue("@sim", request.Sim);
//				cmd.Parameters.AddWithValue("@day", request.Day);
//				cmd.Parameters.AddWithValue("@time", request.Time);
//				cmd.Parameters.AddWithValue("@longitude", request.Longitude);
//				cmd.Parameters.AddWithValue("@latitude", request.Latitude);
//				cmd.ExecuteNonQuery();
//				
//				request.Id = (int) cmd.LastInsertedId;
//			}
//			catch (MySqlException ex) {
//				Console.WriteLine("[MySQL Err. " + ex.Number + "]Error in " +
//					"mysql connection: " + ex.Message);
//				return new HttpResult(request) {
//					StatusCode = HttpStatusCode.InternalServerError,
//				};
//			}
//			DbUtils.closeDbConnLocal();
//			
//			var newPath = base.RequestContext.AbsoluteUri.WithTrailingSlash() + request.Id;
//			return new HttpResult(request) {
//				StatusCode = HttpStatusCode.Created,
//				Headers = {
//					{ HttpHeaders.Location, newPath }
//				}
//			};
//		}
//
//	}
//}
