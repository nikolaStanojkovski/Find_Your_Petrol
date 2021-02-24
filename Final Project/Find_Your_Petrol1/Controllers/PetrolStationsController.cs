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
    /// <summary>
    /// Класата <c>PetrolStationsController</c>
    /// управува со барањата кои пристигаат до нашата апликација
    /// </summary>
    public class PetrolStationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        /// <summary>
        /// Методот <c>Index</c> ми го враќа стандардниот поглед за овој контролер
        /// </summary>
        /// <returns>
        /// Враќа поглед
        /// </returns>
        // GET: PetrolStations
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Методот <c>CalculateEuqlide</c> 
        /// ми го пресметува евклидовото растојание од бензинска A до
        /// бензинска Б
        /// </summary>
        /// <param name="model">Влезен аргумент од типот <c>DistanceCalculator</c></param>
        /// <returns>
        /// Враќа double вредност што ни претставува евклидово растојание
        /// </returns>
        private double CalculateEuqlide(DistanceCalculator model)
        {
            //Ги зема објектите од тип PetrolStation од базата на податоци со дадено Id
            PetrolStation from = db.PetrolStations.FirstOrDefault(m => m.PetrolStationId == model.FromId);
            PetrolStation to = db.PetrolStations.FirstOrDefault(m => m.PetrolStationId == model.ToId);

            //Во продолжение следува алгоритам за пресметување на евклидово растојание
            double latDistance = Math.Abs(from.Dolzhina - to.Dolzhina);
            double lngDistance = Math.Abs(from.Dolzhina - to.Dolzhina);

            double a = Math.Sin(latDistance / 2) * Math.Sin(latDistance / 2)
              + Math.Cos(from.GeografskaShirochina) * Math.Cos(to.GeografskaShirochina)
              * Math.Sin(lngDistance / 2) * Math.Sin(lngDistance / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return (Math.Round(6371 * c));
        }

        /// <summary>
        /// Методот <c>AddFavouritePetrol</c>
        /// се справува со POST барањата на патеката /PetrolStations/AddFavouritePetrol и 
        /// на корисникот којшто е најавен му доделува бензинска станица со даденото Id
        /// </summary>
        /// <param name="PetrolStationId">Id-то на објектот од тип PetrolStation</param>
        /// <returns>
        /// Доколку корисникот веќе ја има доделено бензинската со дадено Id, враќа Json објект со порака дека корисникот веќе ја има доделено омилената бензинска станица,
        /// во спротивно ја додава новата омилена бензинска станица на корисникот и ја враќа во Json oбјект
        /// </returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AddFavouritePetrol(int PetrolStationId)
        {
            FavouritePetrol fp = db.FavouritePetrols.FirstOrDefault(p => p.CurrentUserUsername.Equals(this.User.Identity.Name));
            if(fp == null)
            {
                db.FavouritePetrols.Add(new FavouritePetrol(this.User.Identity.Name, PetrolStationId));
                db.SaveChanges();

                return Json(db.PetrolStations.FirstOrDefault(p => p.PetrolStationId == PetrolStationId).Prikaz);
            } else
            {
                return Json("Already has!");
            }
        }
        /// <summary>
        /// Методот <c>GetFavouritePetrol</c>
        /// се справува со POST барања на патеката /PetrolStations/GetFavouritePetrol и 
        /// ги враќа омилените бензински станици за најавениот корисник
        /// </summary>
        /// <returns>
        /// Доколку корисникот не е најавен враќа Json објект со порака дека не е најавен,
        /// доколку тој корисник има додадено омилена бензинска, тогаш ја враќа таа бензинска во Json објект,
        /// во спротивно враќа Json објект со порака дека сеуште нема избрано омилена бензинска
        /// </returns>
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
        /// <summary>
        /// Методот <c>RemoveFavouritePetrol</c>
        /// се справува со PUT барања на патеката /PetrolStations/RemoveFavouritePetrol и 
        /// ја брише доделената омилена бензинска станица за најавениот корисник
        /// </summary>
        /// <returns>
        /// Враќа Json oбјект со порака Ок, доколку е избришана успешно
        /// </returns>

        [AcceptVerbs(HttpVerbs.Put)]
        public JsonResult RemoveFavouritePetrol()
        {
            FavouritePetrol fp = db.FavouritePetrols.FirstOrDefault(p => p.CurrentUserUsername.Equals(this.User.Identity.Name));
            db.FavouritePetrols.Remove(fp);
            db.SaveChanges();

            return Json("Ok");
        }
        /// <summary>
        /// Методот <c>CalculateDistance</c>
        /// се справува со GET барањата на патека /PetrolStations/CalculateDistance и 
        /// ги зема сите објекти од типот PetrolStation од базата на податоци
        /// и ги враќа во поглед
        /// </summary>
        /// <returns>
        /// Враќа поглед со модел во којшто се сите објекти од типот PetrolStation од базата на податоци
        /// </returns>
        public ActionResult CalculateDistance()
        {
            DistanceCalculator model = new DistanceCalculator();
            model.petrols = db.PetrolStations.ToList();
            return View(model);
        }
        /// <summary>
        /// Методот <c>CalculateDistance</c>
        /// ми ги прифаќа POST барањата на патеката /PetrolStation/CalculateDistance и
        /// пресметува евклидово растојание помеѓу 2 бензински станици
        /// </summary>
        /// <param name="model">Влезен аргумент од типот DistanceCalculator</param>
        /// <returns>
        /// Враќа поглед со модел, во којшто е пресметено растојанието помеѓу 2-те бензински станици
        /// </returns>
        [HttpPost]
        public ActionResult CalculateDistance(DistanceCalculator model)
        {
            model.kilometres = CalculateEuqlide(model);
            model.petrols = db.PetrolStations.ToList();
            return View(model);
        }
        /// <summary>
        /// Методот <c>PostRating</c>
        /// се справува со POST барања на патеката /PetrolStations/PostRating и 
        /// го ажурира параметарот Ocena на објектот од тип PetrolStation од базата на податоци со дадено Id
        /// </summary>
        /// <param name="Ocena">Доделената оцена за бензинската станица</param>
        /// <param name="PetrolStationId">Id-то за бензинската станица</param>
        /// <returns>
        /// Враќа Јson објект со соодветна порака
        /// </returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult PostRating(int Ocena, int PetrolStationId)
        {
            PetrolStation station = db.PetrolStations.FirstOrDefault(m => m.PetrolStationId == PetrolStationId);
            station.Ocena = Ocena;
            db.Entry(station).State = EntityState.Modified;

            db.SaveChanges();
            return Json("You rated this " + Ocena.ToString() + " star(s)");
        }
        /// <summary>
        /// Методот <c>GetIfAdmin</c>
        /// се справува со GET барањата на патека /PetrolStation/GetIfAdmin
        /// проверува дали сега најавениот корисник е во улога Administrator
        /// </summary>
        /// <returns>
        /// Враќа Јson објект
        /// </returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetIfAdmin()
        {
            if (this.User.IsInRole("Administrator") == true)
                return Json(true, JsonRequestBehavior.AllowGet);

            return Json(false, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Методот <c>GetTypesOfOils</c>
        /// се справува со POST барањата на патека /PetrolStation/GetTypesOfOils
        /// ги наоѓа типовите на гориво за објектот од тип PetrolStation од базата на податоци со дадено Id
        /// </summary>
        /// <param name="PetrolStationId">Id-то на објектот од тип PetrolStation</param>
        /// <returns>
        /// Доколку за бензинската станица со даденото Id, нема типови на гориво или доколку е непознато,
        /// тогаш враќа Json објект со порака Непознато, 
        /// во спротивно враќа Json објект со сите типови на гориво специфицирани во String
        /// </returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetTypesOfOils(int PetrolStationId)
        {
            PetrolStation station = db.PetrolStations.FirstOrDefault(m => m.PetrolStationId == PetrolStationId);
            List<String> types_oils = GetFuels(station);
            
            if (types_oils.Count == 0 || types_oils.Contains("Непознато"))
                return Json("Непознато");

            String finalString = "";
            foreach (String s in types_oils)
                finalString += s + ", ";

            return Json(finalString.Substring(0, finalString.Length - 2));
        }
        /// <summary>
        /// Методот <c>GetUserLogged</c>
        /// се справува со GET барањата на патека /PetrolStations/GetUserLogged
        /// проверува дали корисникот е најавен
        /// </summary>
        /// <returns>
        /// Враќа Json објект соодветно дали е најавен или не
        /// </returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetUserLogged()
        {
            if (this.User.Identity.Name != null && !this.User.Identity.Name.Equals(""))
                return Json(true, JsonRequestBehavior.AllowGet);

            return Json(false, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Методот <c>GetFuels</c>
        /// ги наоѓа сите типови на гориво
        /// за дадената бензинска станица
        /// </summary>
        /// <param name="petrolStation">Објект од типот PetrolStation за којшто бараме типови на гориво</param>
        /// <returns>
        /// Враќа листа со имиња од сите типови на гориво коишто се присутни во дадената бензинска станица
        /// </returns>
        public List<String> GetFuels(PetrolStation petrolStation)
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
        /// <summary>
        /// Методот <c>Map</c>
        /// се справува со GET барањата на патека /PetrolStations/Map
        /// ми враќа поглед со модел во којшто се специфицирани објекти од каде тргнува корисникот и до каде треба да оди
        /// </summary>
        /// <param name="model">Влезен аргумент од типот FromLocationToDestination</param>
        /// <returns>
        /// Враќа поглед со модел од типот <c>FromLocationToDestination</c>
        /// </returns>
        //GET: PetrolStations/Map
        public ActionResult Map(FromLocationToDestination model)
        {

            var petrolStation = db.PetrolStations.Where(r => r.GeografskaShirochina.Equals(model.stationsLatitude) && r.Dolzhina.Equals(model.stationsLongitude)).FirstOrDefault();
            ViewBag.station = petrolStation;

            List<String> fuelList = GetFuels(petrolStation);

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
