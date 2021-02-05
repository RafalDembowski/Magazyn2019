using Magazyn2019.Models;
using Magazyn2019.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Magazyn2019.Controllers
{
    public class HomeController : Controller
    {
        private UnitOfWork unitOfWork;

        public HomeController()
        {
            unitOfWork = new UnitOfWork(new Magazyn2019Entities());
        }
        [HttpGet]
        public ActionResult Login()
        {
            if (Session["ActiveUserId"] == null)
            {
                return View();
            }
            return RedirectToAction("Index", "HomePage");
        }
        [HttpPost]
        public ActionResult Login(User _user)
        {
            var user = unitOfWork.UserRepository.GetAll().Where(u => u.userName == _user.userName && u.password == _user.password).FirstOrDefault();

            if (user != null)
            {
                Session["ActiveUserId"] = user.id_user;
                Session["ActiveUserFullName"] = user.fullName;
                Session["ActiveUserPosition"] = user.position;
                return RedirectToAction("Index", "HomePage");
            }
            ViewBag.Error = true;
            return View(_user);
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}