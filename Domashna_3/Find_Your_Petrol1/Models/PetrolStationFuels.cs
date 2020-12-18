using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Find_Your_Petrol1.Models
{
    public class PetrolStationFuels
    {
        [Key]
        public int PetrolStation_PetrolStationId { get; set; }
        public System.Int64 Fuel_FuelId { get; set; }

        PetrolStationFuels(int station_id, System.Int64 fuel_id)
        {
            this.PetrolStation_PetrolStationId = station_id;
            this.Fuel_FuelId = fuel_id;
        }

        PetrolStationFuels()
        {

        }
    }
}