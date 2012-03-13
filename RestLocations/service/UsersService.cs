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
	public class UsersService : RestServiceBase<Users>
	{
		public override object OnGet (Users request)
		{
			string sql = "SELECT usr_sim, usr_nombre FROM usuarios";
			
			try {
				DbUtils.openDbConnLocal();
				MySqlCommand cmd = new MySqlCommand(sql, Global.mySqlConnLocal);
				MySqlDataReader reader = cmd.ExecuteReader();
				
				request.users = new List<User>();
				while (reader.Read())
				{
					User user = new User();
					user.Sim = reader.GetString("usr_sim");
					user.Name = reader.GetString("usr_nombre");
					request.users.Add(user);
				}
				reader.Close();
			}
			catch (MySqlException ex) {
				Console.WriteLine("[MySQL Err. " + ex.Number + "] Error in " +
					"mysql connection: " + ex.Message);
				return new HttpResult(request.users, 
				                      HttpStatusCode.InternalServerError);
			}
			catch (Exception ex) {
				Console.WriteLine("Error: " + ex.Message);
				return new HttpResult(request.users, 
				                      HttpStatusCode.InternalServerError);
			} 
			finally {
				DbUtils.closeDbConnLocal();
			}
			
			return new UsersResponse { users = request.users };
		}
	}
}

