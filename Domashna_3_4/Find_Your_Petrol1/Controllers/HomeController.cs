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
            ViewBag.Message = "Опис на апликацијата";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.NotLogged = false;
            ViewBag.Message = "Контакт страна";
            UserFeedback uf = db.UserFeedbacks.FirstOrDefault(p => p.CurrentUserUsername.Equals(this.User.Identity.Name));
            if (uf != null)
                ViewBag.AlreadySent = "yes";
            else
                ViewBag.AlreadySent = "no";

            ViewBag.Feedbacks = db.UserFeedbacks.ToList();
            if (this.User.Identity.Name != null && !this.User.Identity.Name.Equals(""))
            {
                UserFeedback model = new UserFeedback();
                model.CurrentUserUsername = this.User.Identity.Name;

                return View(model);
            } else
            {
                ViewBag.NotLogged = true;
                ViewBag.Message = "Не можете да оставите повратна информација доколку не сте логирани !!!";

                return View();
            }
            
        }

        [HttpPost]
        public ActionResult Contact(UserFeedback model)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Your contact page";
                Session["DoneFeedback"] = "yes";
                db.UserFeedbacks.Add(model);
                db.SaveChanges();
                return RedirectToAction("Contact");
            }

            ViewBag.Feedbacks = db.UserFeedbacks.ToList();
            return View(model);
        }
    }
}