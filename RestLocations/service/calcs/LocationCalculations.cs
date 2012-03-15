using System;

using RestLocations.Model;

namespace RestLocations.Service
{
	public class LocationCalculations
	{
		private const double DEG_RAD 	= 0.01745329251994;
		private const double R_EARTH 	= 6367.45;
		private const double SEG_TO_HR 	= 60.0 * 60.0;
		
		public static double calculateDistance(Location l1, Location l2)
		{
			double haversine, distance;
			double dLat, dLon;
			
			dLat = (l2.Latitude - l1.Latitude) * DEG_RAD;
			dLon = (l2.Longitude - l1.Longitude) * DEG_RAD;
			haversine = Math.Sin(dLat * 0.5) * Math.Sin(dLat * 0.5) +
				Math.Sin(dLon * 0.5) * Math.Sin(dLon * 0.5) *
				Math.Cos(l2.Latitude * DEG_RAD)	*
				Math.Cos(l1.Latitude * DEG_RAD);
			distance = 2 * Math.Atan2(Math.Sqrt(haversine), 
			                          Math.Sqrt(1 - haversine)) * R_EARTH;
			
			return distance;
		}
		
		public static double calculateVelocity(double distance,
		                                       Location l1, Location l2)
		{
			double velocity;
			double realTime;
			
			realTime = (double) (l2.Time - l1.Time) / SEG_TO_HR;
			velocity = distance / (double) realTime;
			return velocity;
		}
	}
}

