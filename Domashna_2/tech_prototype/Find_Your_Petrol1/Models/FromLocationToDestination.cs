using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Find_Your_Petrol1.Models
{
    public class FromLocationToDestination
    {
        public double usersLatitude { get; set; }
        public double usersLongitude{ get; set; }
        public double stationsLatitude { get; set; }
        public double stationsLongitude { get; set; }
        public String traffic_model { get; set; }

        public FromLocationToDestination()
        {

        }

        public FromLocationToDestination(double userLat, double usersLon, double stationsLat, double stationsLon, string transit_mode)
        {
            this.usersLongitude = usersLon;
            this.usersLatitude = userLat;
            this.stationsLatitude = stationsLat;
            this.stationsLongitude = stationsLon;
            this.traffic_model = transit_mode;
        }
    }
}