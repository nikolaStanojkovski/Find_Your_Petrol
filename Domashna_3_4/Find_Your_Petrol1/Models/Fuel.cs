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

        public Fuel(string name)
        {
            this.Name = name;
        }

        public Fuel()
        {
            // Petrols = new List<PetrolStation>();
        }
    }
}