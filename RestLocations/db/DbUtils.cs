using System;
using MySql.Data.MySqlClient;

namespace RestLocations.Db
{
	public class DbUtils
	{
		public const string connStringLocal = "server=127.0.0.1;" +
			"uid=mobileuser;pwd=pass123;database=mobile_lab4";
		public const string connStringRemote = "server=mysql2.000webhost.com;" +
			"uid=a7991869_movil;pwd=cmovil123;database=a7991869_points";
		
		public static void openDbConnLocal() 
		{
			try {
				Global.mySqlConnLocal.Open();
			}
			catch (MySqlException ex) {
				throw ex;		
			}
		}
		
		public static void closeDbConnLocal()
		{
			Global.mySqlConnLocal.Close();
		}
		
		public static void openDbConnRemote() 
		{
			try {
				Global.mySqlConnRemote.Open();
			}
			catch (MySqlException ex) {
				throw ex;		
			}
		}
		
		public static void closeDbConnRemote()
		{
			Global.mySqlConnRemote.Close();
		}
	}
}
