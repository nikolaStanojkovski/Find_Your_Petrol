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
    /// <summary>
    /// Класата <c>HomeController</c>
    /// се справува со барањата поврзани
    /// со погледот Home
    /// </summary>
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Методот <c>Index</c>
        /// се спавува со GET барањата на патеката /Home/Index
        /// го враќа стандардниот поглед заедно со објект
        /// </summary>
        /// <returns>
        /// Враќа поглед со објект од типот FromLocationToDestination
        /// </returns>
        public ActionResult Index()
        {
            FromLocationToDestination model = new FromLocationToDestination();
            ViewBag.PetrolStationNames = db.PetrolStations.ToArray();
            return View(model);
        }
        /// <summary>
        /// Методот <c>FromLocationToDestination</c>
        /// се повикува при POST барање на патеката /Home/FromLocationToDestination
        /// </summary>
        /// <param name="model">Објект од типот FromLocationToDestination</param>
        /// <returns>
        /// Враќа редирекција до друг метод во класата
        /// </returns>
        [HttpPost]
        public ActionResult FromLocationToDestination(FromLocationToDestination model)
        {
            return RedirectToAction("Map", "PetrolStations", model);

        }
        /// <summary>
        /// Методот <c>About</c>
        /// се справува со барањето на За нас страната
        /// </summary>
        /// <returns>
        /// Враќа поглед
        /// </returns>
        public ActionResult About()
        {
            ViewBag.Message = "Опис на апликацијата";
            return View();
        }
        /// <summary>
        /// Методот <c>Contact</c>
        /// се справува со GET барањата на патеката /Home/Contact и
        /// проверува дали најавениот корисник ја има оценето нашата апликација
        /// и соодветно враќа поглед
        /// </summary>
        /// <returns>
        /// Доколку корисникот не е најавен се враќа поглед од Контакт страната,
        /// во спротивно се враќа Поглед заедно со објект од типот UserFeedback
        /// </returns>
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
        /// <summary>
        /// Методот <c>Contact</c>
        /// се справува со POST барањата на патеката /Home/Contact
        /// </summary>
        /// <param name="model">Објект од типот UserFeedback</param>
        /// <returns>
        /// Доколку корисникот внел валидни податоци за објектот од тип UserFeedback, тогаш враќа редирекција до GET барањето на Contact методот
        /// во спротивно враќа поглед со истиот модел од влезниот параметар
        /// </returns>
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