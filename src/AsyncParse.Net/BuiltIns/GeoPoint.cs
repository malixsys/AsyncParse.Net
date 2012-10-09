using System;

namespace AsyncParse.Net.BuiltIns
{
    public class GeoPoint

    {
        private static readonly System.Globalization.CultureInfo CultureInfo = new System.Globalization.CultureInfo("en-CA");

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public static GeoPoint FromLatLong(string latitude, string longitude)
        {

            return new GeoPoint
                       {
                           Latitude = Decimal.Parse(latitude, CultureInfo),
                           Longitude = Decimal.Parse(longitude, CultureInfo)
                       };
        }
    }
}