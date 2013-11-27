using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GymGame.Models;

namespace GymGame.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection f)
        {
            var accessToken = f["accessToken"];

            var client = new Facebook.FacebookClient(accessToken);
            dynamic result = client.Get("me", new { fields = "name, id" });

            UserModel um = new UserModel();

            ViewBag.status = "loggedIn";
            return View();

        }

    }
}
