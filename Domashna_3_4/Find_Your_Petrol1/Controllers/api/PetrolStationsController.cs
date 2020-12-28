using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using Find_Your_Petrol1.Models;

namespace Find_Your_Petrol1.Controllers.api
{
    public class PetrolStationsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/PetrolStations
        public IQueryable<PetrolStation> GetPetrolStations()
        {
            return db.PetrolStations;
        }

        // GET: api/PetrolStations/5
        [ResponseType(typeof(PetrolStation))]
        public IHttpActionResult GetPetrolStation(int id)
        {
            PetrolStation petrolStation = db.PetrolStations.Find(id);
            if (petrolStation == null)
            {
                return NotFound();
            }

            return Ok(petrolStation);
        }

        // PUT: api/PetrolStations/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPetrolStation(int id, string name, string work_time, string geo_length, string geo_width, string rating)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            PetrolStation petrolStation = db.PetrolStations.FirstOrDefault(petrol => petrol.PetrolStationId == id);
            petrolStation.ImeNaBenzinska = name;
            petrolStation.RabotnoVreme = work_time;
            petrolStation.Dolzhina = Double.Parse( geo_length );
            petrolStation.GeografskaShirochina = Double.Parse(geo_width);
            petrolStation.Ocena = float.Parse(rating);

            db.Entry(petrolStation).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PetrolStationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/PetrolStations
        [ResponseType(typeof(PetrolStation))]
        public IHttpActionResult PostPetrolStation(PetrolStation petrolStation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PetrolStations.Add(petrolStation);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = petrolStation.PetrolStationId }, petrolStation);
        }

        // DELETE: api/PetrolStations/5
        [ResponseType(typeof(PetrolStation))]
        public IHttpActionResult DeletePetrolStation(int id)
        {
            PetrolStation petrolStation = db.PetrolStations.Find(id);
            if (petrolStation == null)
            {
                return NotFound();
            }

            db.PetrolStations.Remove(petrolStation);
            db.SaveChanges();

            return Ok(petrolStation);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PetrolStationExists(int id)
        {
            return db.PetrolStations.Count(e => e.PetrolStationId == id) > 0;
        }
    }
}