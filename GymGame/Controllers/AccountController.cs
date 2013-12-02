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

            //get name and id from facebookClient
            var client = new Facebook.FacebookClient(accessToken);
            dynamic result = client.Get("me", new { fields = "first_name, last_name, id" });

            //access the user model
            UserModel um = new UserModel();

            //check if user exists, if exists: login, if not exists: create
            User u = new User();
            u.Firstname = result.first_name;
            u.Lastname = result.last_name;
            u.FK_Role = 1;
            u.FB_UserId = result.id;
            
            //important! fb id is now int 64,
            //but cannot convert dynamic result to int, so we use string (for now)
            Boolean exists = um.userExists(u.FB_UserId);
            if (!exists)
            {
                //user does not exist yet, create and log in
                try
                {
                    u = um.saveUser(u);
                    //if the user's id has been set: success
                    if (u.User_Id == 0)
                    {
                        throw new Exception("User wasn't saved into database");
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ViewBag.status = "error";
                    return View();
                }
            }
            else{
                //login the user
                try
                {
                    u = um.logIn(u);
                    //if the user's id has been set: success
                    if (u.User_Id == 0)
                    {
                        throw new Exception("User couldn't be logged in");
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ViewBag.status = "error";
                    return View();
                }
            }


            //set the facebook id in the session: set that the user is logged in
            Session["firstname"] = u.Firstname;
            Session["lastname"] = u.Lastname;
            Session["fbUserId"] = result.id;
            Session["userId"] = u.User_Id;
            ViewBag.status = "loggedIn";

            return View();

        }

    }
}
