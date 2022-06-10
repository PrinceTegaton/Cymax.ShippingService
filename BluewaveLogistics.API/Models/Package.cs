using System.Xml.Serialization;

namespace BluewaveLogistics.API.Models
{
    public class Package
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public double Length { get; set; }
        public double Weight { get; set; }
    }
}