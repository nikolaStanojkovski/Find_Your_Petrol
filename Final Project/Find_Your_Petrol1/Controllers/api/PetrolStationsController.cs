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
    /// <summary>
    /// Класата <c>PetrolStationsController</c> управува 
    /// со асинхрони барањата кои пристигнуваат до нашата апликација
    /// </summary>
    public class PetrolStationsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        /// <summary>
        /// Методот <c>GetPetrolStations</c>,
        /// се справува со GET барањата на патеката api/PetrolStations и
        /// ги наоѓа сите објекти од тип PetrolStations во базата на податоци
        /// и нема влезни параметри
        /// </summary>
        /// <returns>
        /// Ги враќа сите објекти од тип PetrolStations во базата на податоци
        /// </returns>
        // GET: api/PetrolStations
        public IQueryable<PetrolStation> GetPetrolStations()
        {
            // Ги наоѓа сите објекти од тип PetrolStations во базата на податоци и ги враќа
            return db.PetrolStations;
        }
        /// <summary>
        /// Методот <c>GetPetrolStation</c>,
        /// се справува со GET барањата на патека api/PetrolStations/ID и
        /// го зема објектот од тип PetrolStation со дадено Id
        /// </summary>
        /// <param name="PetrolStationId">Id-то на објектот од тип PetrolStation</param>
        /// <returns>
        /// Враќа објект од типот PetrolStation доколку постои таков во базата на податоци,
        /// или враќа NotFound објект
        /// </returns>

        // GET: api/PetrolStations/5
        [ResponseType(typeof(PetrolStation))]
        public IHttpActionResult GetPetrolStation(int PetrolStationId)
        {
            //Го наоѓа објектот со специфицираното Id
            PetrolStation petrolStation = db.PetrolStations.Find(PetrolStationId);
            //Проверува дали таков објект постои во базата на податоци, со тоа што проверува дали вратениот објект е null
            if (petrolStation == null)
            {
                //If the specified petrol station is not present in the database, then it returns NotFound object
                //Доколку не постои таков објект враќа NotFound објект
                return NotFound();
            }
            //Доколку постои таков објект, врати го него
            return Ok(petrolStation);
        }
        /// <summary>
        /// Методот <c>PutPetrolStation</c>,
        /// се справува со PUT барањата на патека api/PetrolStations/ID и
        /// го ажурира објектот од тип PetrolStation во базата на податоци
        /// со дадено Id
        /// </summary>
        /// <param name="PetrolStationId">Id-то на објектот</param>
        /// <param name="ImeNaBenzinska">Име на објектот</param>
        /// <param name="RabotnoVreme">Работно време на објектот</param>
        /// <param name="Dolzhina">Географска должина на објектот</param>
        /// <param name="GeografskaShirochina">Географска широчина на објектот</param>
        /// <param name="TipoviGorivo">Типови на гориво на објектот</param>
        /// <param name="Ocena">Оцена на објектот</param>
        /// <returns>
        /// Доколку сите влезни аргументи се not null вредности, тогаш враќа void,
        /// во спротивно враќа BadRequest објект
        /// </returns>
        /// <exception cref="DbUpdateConcurrencyException">Доколку настане проблем со ажурирање на податоците со базата на податоци, тогаш се фрла исклучок</exception>
        // PUT: api/PetrolStations/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPetrolStation(int PetrolStationId, string ImeNaBenzinska, string RabotnoVreme, string Dolzhina, string GeografskaShirochina, string TipoviGorivo, string Ocena)
        {
            //Проверува дали моделот е валиден
            if (!ModelState.IsValid)
            {
                //Враќа BadRequest доколку моделот не е валиден
                return BadRequest(ModelState);
            }
            //Find the petrol station with specified id or find default
            //Најди го објектот од тип PetrolStation во базата на бодатоци со дадено Id
            PetrolStation petrolStation = db.PetrolStations.FirstOrDefault(petrol => petrol.PetrolStationId == PetrolStationId);
            //Ажурирај ги податоците
            petrolStation.ImeNaBenzinska = ImeNaBenzinska;
            petrolStation.RabotnoVreme = RabotnoVreme;
            petrolStation.Dolzhina = Double.Parse(Dolzhina);
            petrolStation.GeografskaShirochina = Double.Parse(GeografskaShirochina);
            petrolStation.TipoviGorivo = TipoviGorivo;
            if (Ocena.Contains("Not Rated"))
                petrolStation.Ocena = 0;
            else
                petrolStation.Ocena = float.Parse(Ocena);

            db.Entry(petrolStation).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PetrolStationExists(PetrolStationId))
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
        /// <summary>
        /// Методот <c>PostPetrolStation</c> зачувува
        /// се справува со POST барањата на патека api/PetrolStations и
        /// нов објект од типот PetrolStation во базата на податоци
        /// </summary>
        /// <param name="ImeNaBenzinska">Име на бензинската станица</param>
        /// <param name="RabotnoVreme">Работно време на бензинската станица</param>
        /// <param name="Dolzhina">Географска должина на локацијата на бензинската станица</param>
        /// <param name="GeografskaShirochina">Географска широчина на локацијата на бензинската станица</param>
        /// <param name="TipoviGorivo">Типови на гориво на бензинската станица</param>
        /// <param name="Ocena">Оцена на бензинската станица</param>
        /// <returns>
        /// Го враќа ново креираниот објект
        /// </returns>
        // POST: api/PetrolStations
        [ResponseType(typeof(PetrolStation))]
        public IHttpActionResult PostPetrolStation(string ImeNaBenzinska, string RabotnoVreme, string Dolzhina, string GeografskaShirochina, string TipoviGorivo, string Ocena)
        {
            PetrolStation petrolStation = new PetrolStation(ImeNaBenzinska, TipoviGorivo, RabotnoVreme, Double.Parse(GeografskaShirochina), Double.Parse(Dolzhina), float.Parse(Ocena));

            db.PetrolStations.Add(petrolStation);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = petrolStation.PetrolStationId }, petrolStation);
        }

        /// <summary>
        /// Методот <c>DeletePetrolStation</c> 
        /// се справува со DELETE барањата на патека api/PetrolStations/ID и 
        /// брише објект од типот PetrolStation од базата на податоци со дадено Id
        /// </summary>
        /// <param name="PetrolStationId">Id-то на објектот</param>
        /// <returns>
        /// Доколку постои објект со тоа Id, тогаш го враќа избришаниот објект,
        /// во спротивно враќа NotFound објект
        /// </returns>
        // DELETE: api/PetrolStations/5
        [ResponseType(typeof(PetrolStation))]
        public IHttpActionResult DeletePetrolStation(int PetrolStationId)
        {
            //Го бара објектот со даденото Id во базата на податоци
            PetrolStation petrolStation = db.PetrolStations.Find(PetrolStationId);
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

        /// <summary>
        /// Методот <c>PetrolStationExists</c>
        /// проверува дали објект со дадено Id постои во базата на податоци
        /// </summary>
        /// <param name="PetrolStationId"></param>
        /// <returns>
        /// Враќа true доколку објектот со дадено Id се појавува повеќе или токму еднаш во базата на податоци,
        /// во спротивно враќа false
        /// </returns>
        private bool PetrolStationExists(int PetrolStationId)
        {
            return db.PetrolStations.Count(e => e.PetrolStationId == PetrolStationId) > 0;
        }
    }
}