namespace RestLocations.Model
{
	public class Location
	{
		public int Id { get; set; }
		public string Sim { get; set; }
		public double Longitude { get; set; }
		public double Latitude { get; set; }
		public string Day { get; set; }
		public long Time { get; set; }
		
	}
	
	public class LocationResponse
	{
		public Location location { get; set; }
	}
}
