namespace GreenExpress.API.Models
{
    public class Location
    {
        public IEnumerable<LocationState> State { get; set; }
    }

    public class LocationState
    {
        public string State { get; set; }
        public IEnumerable<LocationCity> Cities { get; set; }
    }

    public class LocationCity
    {
        public string Name { get; set; }
        public double Point { get; set; }
    }

    public class AddressBlock
    {
        public string Country { get; set; } = "Canada";
        public string State { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public double Point { get; set; }
    }
}