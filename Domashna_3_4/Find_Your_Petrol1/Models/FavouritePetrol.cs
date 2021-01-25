using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Find_Your_Petrol1.Models
{
    public class FavouritePetrol
    {
        [Key]
        public string CurrentUserUsername { get; set; }
        public int PetrolID { get; set; }

        public FavouritePetrol(string username, int petrol)
        {
            this.CurrentUserUsername = username;
            this.PetrolID = petrol;
        }

        public FavouritePetrol()
        {

        }
    }
}