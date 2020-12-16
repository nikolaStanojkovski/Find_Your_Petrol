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

        //GET: PetrolStations/Map

        public ActionResult Map(FromLocationToDestination model)
        {
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
