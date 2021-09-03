﻿namespace QRCoder2.Payloads
{
    public class GeolocationPayload : PayloadBase
    {
        private readonly string latitude, longitude;
        private readonly GeolocationEncoding encoding;

        /// <summary>
        /// Generates a geo location payload. Supports raw location (GEO encoding) or Google Maps link (GoogleMaps encoding)
        /// </summary>
        /// <param name="latitude">Latitude with . as splitter</param>
        /// <param name="longitude">Longitude with . as splitter</param>
        /// <param name="encoding">Encoding type - GEO or GoogleMaps</param>
        public GeolocationPayload(string latitude, string longitude, GeolocationEncoding encoding = GeolocationEncoding.GEO)
        {
            this.latitude = latitude.Replace(",",".");
            this.longitude = longitude.Replace(",", ".");
            this.encoding = encoding;
        }

        public override string ToString()
        {
            switch (this.encoding)
            {
                case GeolocationEncoding.GEO:
                    return $"geo:{this.latitude},{this.longitude}";
                case GeolocationEncoding.GoogleMaps:
                    return $"http://maps.google.com/maps?q={this.latitude},{this.longitude}";
                default:
                    return "geo:";
            }
        }

        public enum GeolocationEncoding
        {
            GEO,
            GoogleMaps
        }
    }
}