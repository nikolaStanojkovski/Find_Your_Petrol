using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Find_Your_Petrol1.Models
{
    public class DistanceCalculator
    {
        public long FromId { get; set; }
        public long ToId { get; set; }
        public List<PetrolStation> petrols { get; set; }
        public double kilometres { get; set; }

        public DistanceCalculator()
        {
            this.FromId = 0;
            this.ToId = 0;
            petrols = new List<PetrolStation>();
            this.kilometres = 0;
        }
    }
}