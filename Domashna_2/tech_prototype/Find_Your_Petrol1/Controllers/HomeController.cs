using Find_Your_Petrol1.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Find_Your_Petrol1.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            FromLocationToDestination model = new FromLocationToDestination();
            ViewBag.PetrolStationNames = db.PetrolStations.ToArray();
            return View(model);
        }

        [HttpPost]
        public ActionResult FromLocationToDestination(FromLocationToDestination model)
        {
            return RedirectToAction("Map", "PetrolStations", model);

        }

        private void insertToDatabase()
        {
            using (StreamReader file = new StreamReader("~/Petrol_Stations Dataset/petrol_stations.csv"))
            {
                int counter = 0;
                string ln;
                db.Fuels.Add(new Fuel("Diesel"));
                db.Fuels.Add(new Fuel("Biodiesel"));
                db.Fuels.Add(new Fuel("Octane 95"));
                db.Fuels.Add(new Fuel("Octane 98"));
                db.Fuels.Add(new Fuel("Octane 100"));
                db.Fuels.Add(new Fuel("LPG"));

                db.SaveChanges();
                
                while ((ln = file.ReadLine()) != null)
                {
                    bool diesel = false, biodiesel = false, octane_95 = false, octane_98 = false
                    , octane_100 = false, lpg = false;
                    string[] splitted = ln.Split(',');
                    string name = splitted[2];
                    List<Fuel> vidoviGorivo = new List<Fuel>();
                    if (splitted[5].Equals("yes"))
                    {
                        vidoviGorivo.Add(db.Fuels.FirstOrDefault(m => m.FuelId == 1));
                        diesel = true;
                    } 
                    if (splitted[6].Equals("yes"))
                    {
                        vidoviGorivo.Add(db.Fuels.FirstOrDefault(m => m.FuelId == 2));
                        biodiesel = true;
                    } 
                    if(splitted[7].Equals("yes"))
                    {
                        vidoviGorivo.Add(db.Fuels.FirstOrDefault(m => m.FuelId == 3));
                        octane_95 = true;
                    }
                    if (splitted[8].Equals("yes")) {
                        vidoviGorivo.Add(db.Fuels.FirstOrDefault(m => m.FuelId == 4));
                        octane_98 = true;
                    }    
                    if (splitted[9].Equals("yes"))
                    {
                        vidoviGorivo.Add(db.Fuels.FirstOrDefault(m => m.FuelId == 5));
                        octane_100 = true;
                    } 
                    if (splitted[10].Equals("yes"))
                    {
                        vidoviGorivo.Add(db.Fuels.FirstOrDefault(m => m.FuelId == 6));
                        lpg = true;
                    }

                    string vreme = splitted[4];
                    float ocena = 0;
                    double shirina = double.Parse(splitted[0]);
                    double dolzhina = double.Parse(splitted[1]);

                    PetrolStation toAdd = new PetrolStation(name, vidoviGorivo, vreme, shirina, dolzhina, ocena);
                    // Fuel f = db.Fuels[0];

                    db.PetrolStations.Add(toAdd);

                    if (diesel)
                        db.Fuels.FirstOrDefault(m => m.FuelId == 1).Petrols.Add(toAdd);
                    if (biodiesel)
                        db.Fuels.FirstOrDefault(m => m.FuelId == 2).Petrols.Add(toAdd);
                    if (octane_95)
                        db.Fuels.FirstOrDefault(m => m.FuelId == 3).Petrols.Add(toAdd);
                    if (octane_98)
                        db.Fuels.FirstOrDefault(m => m.FuelId == 4).Petrols.Add(toAdd);
                    if (octane_100)
                        db.Fuels.FirstOrDefault(m => m.FuelId == 5).Petrols.Add(toAdd);
                    if (lpg)
                        db.Fuels.FirstOrDefault(m => m.FuelId == 6).Petrols.Add(toAdd);
                    Console.WriteLine(ln);
                    counter++;
                }
            }
            db.SaveChanges();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}