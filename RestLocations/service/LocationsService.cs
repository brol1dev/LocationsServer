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

namespace RestLocations
{
	public class LocationsService : RestServiceBase<Locations>
	{
		public override object OnGet (Locations locations)
		{
			string sql = "SELECT log_id, usr_sim, log_dia, log_hora, " +
				"log_latitud, log_longitud FROM log";
			
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
				return new HttpResult(locations, 
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
			return new LocationsResponse { locations = locations };
		}
		
		public override object OnPost (Locations locations)
		{
			try 
			{
				this.insertLocations(locations, true, false);
			}
			catch (Exception ex) {
				Console.WriteLine("Error description: " + ex.Message);
				return new HttpResult(locations, 
				                      HttpStatusCode.InternalServerError);
			}
			
			return new HttpResult(locations, "application/json", 
			                      HttpStatusCode.Created);
		}
		
		/**
		 * Decides in which databases to insert the locations
		 */ 
		private void insertLocations(Locations locations, bool dbLocal, bool dbRemote)
		{
			if (dbLocal)
				this.insertTransactionLocal(locations);
			if (dbRemote)
			    this.insertTransactionRemote(locations);
		}
		
		/**
		 * Inserts multiple locations in a single transaction into the local database
		 */
		private void insertTransactionLocal(Locations locations)
		{
			string inSql = "INSERT INTO log " +
				"(usr_sim, log_dia, log_hora, log_longitud, log_latitud) " +
				"VALUES (@sim, @day, @time, @longitude, @latitude)";
			MySqlTransaction trans;
			
			try 
			{
				DbUtils.openDbConnLocal();
				MySqlCommand cmd = Global.mySqlConnLocal.CreateCommand();
				trans = Global.mySqlConnLocal.BeginTransaction();
				cmd.Connection = Global.mySqlConnLocal;
				cmd.Transaction = trans;
				cmd.CommandText = inSql;
				
				cmd.Parameters.AddWithValue("@sim", 0);
				cmd.Parameters.AddWithValue("@day", null);
				cmd.Parameters.AddWithValue("@time", (long) 0);
				cmd.Parameters.AddWithValue("@longitude", 0.0);
				cmd.Parameters.AddWithValue("@latitude", 0.0);
				foreach (Location location in locations.locations)
				{
					cmd.Parameters["@sim"].Value = location.Sim;
					cmd.Parameters["@day"].Value = location.Day;
					cmd.Parameters["@time"].Value = location.Time;
					cmd.Parameters["@longitude"].Value = location.Longitude;
					cmd.Parameters["@latitude"].Value = location.Latitude;
					cmd.ExecuteNonQuery();
				}
				
				trans.Commit();
			}
			catch (Exception e) {
				try 
				{
					trans.Rollback();
				}
				catch(MySqlException ex) {
					Console.WriteLine("Error encountered of type " + ex.GetType() +
					    ", while trying to rollback the insert transaction");
					throw ex;
				}
				Console.WriteLine("Error encountered of type " + e.GetType() +
					    ", when trying to insert data. Nothing was inserted");
				throw e;
			} finally {
				DbUtils.closeDbConnLocal();
			}
		}
		
		/**
		 * Inserts multiple locations in a single transaction into the remote database
		 */
		private void insertTransactionRemote(Locations locations)
		{
			string inSql = "INSERT INTO log " +
				"(usr_sim, log_dia, log_hora, log_longitud, log_latitud) " +
				"VALUES (@sim, @day, @time, @longitude, @latitude)";
			MySqlTransaction trans;
			
			try 
			{
				DbUtils.openDbConnRemote();
				MySqlCommand cmd = Global.mySqlConnRemote.CreateCommand();
				trans = Global.mySqlConnRemote.BeginTransaction();
				cmd.Connection = Global.mySqlConnRemote;
				cmd.Transaction = trans;
				cmd.CommandText = inSql;
				
				cmd.Parameters.AddWithValue("@sim", 0);
				cmd.Parameters.AddWithValue("@day", null);
				cmd.Parameters.AddWithValue("@time", (long) 0);
				cmd.Parameters.AddWithValue("@longitude", 0.0);
				cmd.Parameters.AddWithValue("@latitude", 0.0);
				foreach (Location location in locations.locations)
				{
					cmd.Parameters["@sim"].Value = location.Sim;
					cmd.Parameters["@day"].Value = location.Day;
					cmd.Parameters["@time"].Value = location.Time;
					cmd.Parameters["@longitude"].Value = location.Longitude;
					cmd.Parameters["@latitude"].Value = location.Latitude;
					cmd.ExecuteNonQuery();
				}
				
				trans.Commit();
			}
			catch (Exception e) {
				try 
				{
					trans.Rollback();
				}
				catch(MySqlException ex) {
					Console.WriteLine("Error encountered of type " + ex.GetType() +
					    ", while trying to rollback the insert transaction");
					throw ex;
				}
				Console.WriteLine("Error encountered of type " + e.GetType() +
					    ", when trying to insert data. Nothing was inserted");
				throw e;
			} finally {
				DbUtils.closeDbConnRemote();
			}	
		}
	}
}

