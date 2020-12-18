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
            return View(db.PetrolStations.ToList());
        }

        // GET: PetrolStations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PetrolStation petrolStation = db.PetrolStations.Find(id);
            if (petrolStation == null)
            {
                return HttpNotFound();
            }
            return View(petrolStation);
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

        //GET: PetrolStations/Map
        public ActionResult Map(FromLocationToDestination model)
        {

            var petrolStation = db.PetrolStations.Where(r => r.GeografskaShirochina.Equals(model.stationsLatitude) && r.Dolzhina.Equals(model.stationsLongitude)).FirstOrDefault();
            ViewBag.station = petrolStation;

            var stationId = petrolStation.PetrolStationId;
            var getAllFuelsWithStationId = db.PetrolStationFuels.Where(r => r.PetrolStation_PetrolStationId.Equals(stationId)).Select(r => r.Fuel_FuelId).ToList();

            List<String> fuelList = new List<string>();

            if (getAllFuelsWithStationId.Count != 0)
            {
                foreach (var fuel_id in getAllFuelsWithStationId)
                {
                    fuelList.Add(db.Fuels.Where(r => r.FuelId.Equals(fuel_id)).FirstOrDefault().Name);
                }
            }
            else
            {
                fuelList.Add("Непознато");
            }

            ViewBag.fuels = fuelList;

            return View(model);
        }

        // GET: PetrolStations/Create
        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: PetrolStations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PetrolStationId,ImeNaBenzinska,RabotnoVreme,Oddalecenost,Ocena")] PetrolStation petrolStation)
        {
            if (ModelState.IsValid)
            {
                db.PetrolStations.Add(petrolStation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(petrolStation);
        }

        // GET: PetrolStations/Edit/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PetrolStation petrolStation = db.PetrolStations.Find(id);
            if (petrolStation == null)
            {
                return HttpNotFound();
            }
            return View(petrolStation);
        }

        // POST: PetrolStations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PetrolStationId,ImeNaBenzinska,RabotnoVreme,Oddalecenost,Ocena")] PetrolStation petrolStation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(petrolStation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(petrolStation);
        }

        // GET: PetrolStations/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PetrolStation petrolStation = db.PetrolStations.Find(id);
            if (petrolStation == null)
            {
                return HttpNotFound();
            }
            return View(petrolStation);
        }

        // POST: PetrolStations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PetrolStation petrolStation = db.PetrolStations.Find(id);
            db.PetrolStations.Remove(petrolStation);
            db.SaveChanges();
            return RedirectToAction("Index");
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
