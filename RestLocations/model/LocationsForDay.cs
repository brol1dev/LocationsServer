using System.Collections.Generic;

namespace RestLocations.Model
{
	public class LocationsForDay
	{
		public string Sim { get; set; }
		public string Day { get; set; }
	}
	
	public class LocationsForDayResponse
	{
		public List<Location> locations { get; set; }
	}
}