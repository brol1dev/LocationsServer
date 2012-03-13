using System.Collections.Generic;

namespace RestLocations.Model
{
	public class LocationsInTime : LocationsForDay
	{
		public int StartTime { get; set; }
		public int EndTime { get; set; }
	}
}

