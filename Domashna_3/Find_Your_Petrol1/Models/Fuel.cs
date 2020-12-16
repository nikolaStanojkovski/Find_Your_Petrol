using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Find_Your_Petrol1.Models
{
    public class Fuel
    {
        [Key]
        public long FuelId { get; set; }
        public string Name { get; set; }
        public List<PetrolStation> Petrols { get; set; }

        public Fuel(string name)
        {
            this.Name = name;
            Petrols = new List<PetrolStation>();
        }

        public Fuel()
        {
            // Petrols = new List<PetrolStation>();
        }
    }
}