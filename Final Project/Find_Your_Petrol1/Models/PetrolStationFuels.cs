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
        public int Fuel_FuelId { get; set; }

        public PetrolStationFuels(int station_id, int fuel_id)
        {
            this.PetrolStation_PetrolStationId = station_id;
            this.Fuel_FuelId = fuel_id;
        }

        PetrolStationFuels()
        {

        }
    }
}