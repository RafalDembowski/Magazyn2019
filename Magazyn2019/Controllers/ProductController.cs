using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Magazyn2019.Controllers
{
    public class ProductController : Controller
    {
        // GET: Products
        public ActionResult Index()
        {
            return View();
        }
    }
}