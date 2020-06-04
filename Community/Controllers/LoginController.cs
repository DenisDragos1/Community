using Community.Data;
using Community.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Community.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Authorise(User user, UserLogin LgnUsr)
        {
            using (CommunityEntities db = new CommunityEntities()) //Modifica ProjectXEntities cu numele entitiului programului tau
            {
                var userDetail = db.Users.Where(x => x.Email == user.Email && x.Password == user.Password).FirstOrDefault();

                if (userDetail == null)
                {
                    user.LoginError = "Invalid Account";
                    return View("Index", user);
                }
                else
                {
                    // FormsAuthentication.SetAuthCookie(user.ID.ToString(), false);
                    Session["ID"] = userDetail.ID;
                    Session["FirstName"] = userDetail.FirstName;
                    return RedirectToAction("Index", "Home");
                }
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User objUser)
        {
            if (ModelState.IsValid)
            {
                using (CommunityEntities db = new CommunityEntities())
                {
                    var obj = db.Users.Where(a => a.FirstName.Equals(objUser.FirstName) && a.Password.Equals(objUser.Password)).FirstOrDefault();
                    if (obj != null)
                    {
                        Session["ID"] = obj.ID.ToString();
                        Session["FirstName"] = obj.FirstName.ToString();
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View(objUser);
        }
        //public ActionResult Login()
        //{
        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Login(User objUser)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        using (CommunityEntities db = new CommunityEntities())
        //        {
        //            var obj = db.Users.Where(a => a.FirstName.Equals(objUser.FirstName) && a.Password.Equals(objUser.Password)).FirstOrDefault();
        //            if (obj != null)
        //            {
        //                Session["ID"] = obj.ID.ToString();
        //                Session["FirstName"] = obj.FirstName.ToString();
        //                return RedirectToAction("Index","Home");
        //            }
        //        }
        //    }
        //    return View(objUser);
        //}
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}