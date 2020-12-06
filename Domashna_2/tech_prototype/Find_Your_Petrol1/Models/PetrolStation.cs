using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Find_Your_Petrol1.Models
{
    public class PetrolStation
    {
        public int PetrolStationId { get; set; }
        [Display(Name = "Име на бензинската пумпа")]
        public String ImeNaBenzinska { get; set; }
        [Display(Name = "Тип на гориво")]
        public List<String> TipNaGorivo { get; set; }
        [Display(Name = "Работно време" )]
        public String RabotnoVreme { get; set; }
        [Display(Name = "Оддалеченост")]
        public int Oddalecenost { get; set; }
        [Display(Name = "Оцена")]
        public float Ocena { get; set; }
    }
}