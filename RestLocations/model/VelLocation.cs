using System.Collections.Generic;
using ServiceStack.ServiceInterface.ServiceModel;

namespace RestLocations.Model
{
	public class VelLocationInTime : LocationsInTime { }
	
	public class VelLocation
	{
		public double Longitude { get; set; }
		public double Latitude { get; set; }
		public double Distance { get; set; }
		public double Velocity { get; set; }
	}
	
	public class VelLocationResponse : IHasResponseStatus
	{
		public List<VelLocation> locations { get; set; }
		public double TotalDistance { get; set; }
		public double AvgVelocity { get; set; }
		public ResponseStatus ResponseStatus { get; set; }
	}
}

