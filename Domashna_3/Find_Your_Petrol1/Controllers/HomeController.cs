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