using System.Collections.Generic;

namespace RestLocations.Model
{
	public class Locations
	{
		public List<Location> locations { get; set; }
	}
	
	public class LocationsResponse
	{
		public Locations locations { get; set; }
	}
}

