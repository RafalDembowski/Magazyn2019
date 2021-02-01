using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Magazyn2019.Controllers
{
    public class MovementController : Controller
    {
        // GET: Movement
        public ActionResult Index(int id)
        {
            return View();
        }
        public ActionResult TransferNote()
        {
            return View();
        }
    }
}