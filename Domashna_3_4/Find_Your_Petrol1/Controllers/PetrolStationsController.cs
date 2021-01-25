using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Find_Your_Petrol1.Models;

namespace Find_Your_Petrol1.Controllers
{
    public class PetrolStationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PetrolStations
        public ActionResult Index()
        {
            return View();
        }

        private double CalculateEuqlide(DistanceCalculator model)
        {
            PetrolStation from = db.PetrolStations.FirstOrDefault(m => m.PetrolStationId == model.FromId);
            PetrolStation to = db.PetrolStations.FirstOrDefault(m => m.PetrolStationId == model.ToId);
            double latDistance = Math.Abs(from.Dolzhina - to.Dolzhina);
            double lngDistance = Math.Abs(from.Dolzhina - to.Dolzhina);

            double a = Math.Sin(latDistance / 2) * Math.Sin(latDistance / 2)
              + Math.Cos(from.GeografskaShirochina) * Math.Cos(to.GeografskaShirochina)
              * Math.Sin(lngDistance / 2) * Math.Sin(lngDistance / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return (Math.Round(6371 * c));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AddFavouritePetrol(int id)
        {
            FavouritePetrol fp = db.FavouritePetrols.FirstOrDefault(p => p.CurrentUserUsername.Equals(this.User.Identity.Name));
            if(fp == null)
            {
                db.FavouritePetrols.Add(new FavouritePetrol(this.User.Identity.Name, id));
                db.SaveChanges();

                return Json(db.PetrolStations.FirstOrDefault(p => p.PetrolStationId == id).Prikaz);
            } else
            {
                return Json("Already has!");
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetFavouritePetrol()
        {
            if (this.User.Identity.Name != null && !this.User.Identity.Name.Equals(""))
            {
                FavouritePetrol fp = db.FavouritePetrols.FirstOrDefault(p => p.CurrentUserUsername.Equals(this.User.Identity.Name));
                if (fp != null)
                    return Json(db.PetrolStations.FirstOrDefault(p => p.PetrolStationId == fp.PetrolID).Prikaz);
                else
                    return Json("Not chosen");
            }

            return Json("Not logged");
        }

        [AcceptVerbs(HttpVerbs.Put)]
        public JsonResult RemoveFavouritePetrol()
        {
            FavouritePetrol fp = db.FavouritePetrols.FirstOrDefault(p => p.CurrentUserUsername.Equals(this.User.Identity.Name));
            db.FavouritePetrols.Remove(fp);
            db.SaveChanges();

            return Json("Ok");
        }

        public ActionResult CalculateDistance()
        {
            DistanceCalculator model = new DistanceCalculator();
            model.petrols = db.PetrolStations.ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult CalculateDistance(DistanceCalculator model)
        {
            model.kilometres = CalculateEuqlide(model);
            model.petrols = db.PetrolStations.ToList();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult PostRating(int rating, int mid)
        {
            PetrolStation station = db.PetrolStations.FirstOrDefault(m => m.PetrolStationId == mid);
            station.Ocena = rating;
            db.Entry(station).State = EntityState.Modified;

            db.SaveChanges();
            return Json("You rated this " + rating.ToString() + " star(s)");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetIfAdmin()
        {
            if (this.User.IsInRole("Administrator") == true)
                return Json(true, JsonRequestBehavior.AllowGet);

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetTypesOfOils(int id)
        {
            PetrolStation station = db.PetrolStations.FirstOrDefault(m => m.PetrolStationId == id);
            List<String> types_oils = getFuels(station);
            
            if (types_oils.Count == 0 || types_oils.Contains("Непознато"))
                return Json("Непознато");

            String finalString = "";
            foreach (String s in types_oils)
                finalString += s + ", ";

            return Json(finalString.Substring(0, finalString.Length - 2));
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetUserLogged()
        {
            if (this.User.Identity.Name != null && !this.User.Identity.Name.Equals(""))
                return Json(true, JsonRequestBehavior.AllowGet);

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public List<String> getFuels(PetrolStation petrolStation)
        {
            List<String> fuelList = new List<string>();
            List<int> getAllFuelsWithStationId = db.PetrolStationFuels.Where(r => r.PetrolStation_PetrolStationId == petrolStation.PetrolStationId).
                    Select(r => r.Fuel_FuelId).ToList();

            if (getAllFuelsWithStationId != null)
            {
                foreach (int id in getAllFuelsWithStationId)
                    fuelList.Add(db.Fuels.FirstOrDefault(r => r.FuelId == id).Name);
            }
            else
            {
                fuelList.Add("Непознато");
            }

            return fuelList;
        }

        //GET: PetrolStations/Map
        public ActionResult Map(FromLocationToDestination model)
        {

            var petrolStation = db.PetrolStations.Where(r => r.GeografskaShirochina.Equals(model.stationsLatitude) && r.Dolzhina.Equals(model.stationsLongitude)).FirstOrDefault();
            ViewBag.station = petrolStation;

            List<String> fuelList = getFuels(petrolStation);

            if (this.User.Identity != null && !this.User.Identity.Name.Equals(""))
                ViewBag.isLogged = true;
            else
                ViewBag.isLogged = false;

            ViewBag.fuels = fuelList;

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
