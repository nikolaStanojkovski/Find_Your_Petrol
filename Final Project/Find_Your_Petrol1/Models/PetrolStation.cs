using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Find_Your_Petrol1.Models
{
    public class PetrolStation
    {
        [Key]
        public int PetrolStationId { get; set; }
        [Display(Name = "Име на бензинската пумпа")]
        public String ImeNaBenzinska { get; set; }
        public String TipoviGorivo { get; set; }
        [Display(Name = "Работно време" )]
        public String RabotnoVreme { get; set; }
        [Display(Name = "Географска широчина")]
        public double GeografskaShirochina { get; set; }
        [Display(Name = "Географска должина")]
        public double Dolzhina { get; set; }
        [Display(Name = "Оцена")]
        public float Ocena { get; set; }
        public String Prikaz { get; set; }

        public PetrolStation()
        {
            // TipNaGorivo = new List<Fuel>();
        }
        public PetrolStation(string ime, string tipovi, string vreme, double shirina, double dolzhina, float ocena)
        {
            this.ImeNaBenzinska = ime;
            this.RabotnoVreme = vreme;
            this.TipoviGorivo = tipovi;
            this.GeografskaShirochina = shirina;
            this.Dolzhina = dolzhina;
            this.Ocena = ocena;
            this.Prikaz = ime + " (" + shirina + ", " + dolzhina + ")";
        }
    
    }
}