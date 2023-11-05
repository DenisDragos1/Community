using Community.Data;
using Community.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Community.Controllers
{
    public class RegisterController : Controller
    {

        private CommunityEntities db = new CommunityEntities();


        public RegisterController()
        {
            db = new CommunityEntities();
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View("Add");
        }


        //GET: Users

        public ActionResult Add(int id = 1)
        {
            //var db = new CommunityEntities();
            // User user = new User();
            return View();
        }
        [HttpPost]
        public ActionResult Add(User user)
        {
            using (CommunityEntities db = new CommunityEntities())
            {
                //db.Users.Add(user);

                //db.SaveChanges();

                user.EmailVerification = false;
                var IsExists = IsEmailExists(user.Email);
                if (IsExists)
                {
                    ModelState.AddModelError("EmailExists", "Email Already Exists");
                    return View();
                }
                //it generate unique code

                user.ActivationCode = Guid.NewGuid();
                //password convert
                user.Password = Community.Models.encryptPassword.textToEncrypt(user.Password);
                db.Users.Add(user);
                db.SaveChanges();
            }
            #region Send email verification link
            SendEmailToUser(user.Email, user.ActivationCode.ToString());
            var Message = "Registration Completed. Please check your email :" + user.Email;
            ViewBag.Message = "Message";
            #endregion
            return View("Add");

        }

        // GET: Register
        #region entity connection
        // private CommunityEntities db = new CommunityEntities();
        #endregion

        public ActionResult Index()
        {
            return View();
        }
        #region Check Email Exists or not in DB  
        public bool IsEmailExists(string eMail)
        {
            var IsCheck = db.Users.Where(email => email.Email == eMail).FirstOrDefault();
            return IsCheck != null;
        }
        #endregion
        #region Registration post method for data save  
        [HttpPost]
        public ActionResult Index(User objUsr)
        {

            // email not verified on registration time  
            objUsr.EmailVerification = false;
            var IsExists = IsEmailExists(objUsr.Email);
            if (IsExists)
            {
                ModelState.AddModelError("EmailExists", "Email Already Exists");
                return View();
            }
            //it generate unique code 

            objUsr.ActivationCode = Guid.NewGuid();
            //password convert
            objUsr.Password = Community.Models.encryptPassword.textToEncrypt(objUsr.Password);
            db.Users.Add(objUsr);
            db.SaveChanges();
            // Session["ID"]= objUsr.ID;
            #region Send email verification link
            SendEmailToUser(objUsr.Email, objUsr.ActivationCode.ToString());
            var Message = "Registration Completed. Please check your email :" + objUsr.Email;
            ViewBag.Message = "Message";
            #endregion
            return View("Registration");
        }
        #endregion
        public void SendEmailToUser(string emailId, string activationCode)
        {
            var GenarateUserVerificationLink = "/Register/UserVerification/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, GenarateUserVerificationLink);

            var fromMail = new MailAddress("community123401@gmail.com", "Denis Dragos"); // set your email  
            var fromEmailpassword = "*********"; // Set your password   
            var toEmail = new MailAddress(emailId);

            var smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(fromMail.Address, fromEmailpassword);

            var Message = new MailMessage(fromMail, toEmail);
            Message.Subject = "Registration Completed-Demo";
            Message.Body = "<br/> Your registration completed succesfully." +
                           "<br/> please click on the below link for account verification" +
                           "<br/><br/><a href=" + link + ">" + link + "</a>";
            Message.IsBodyHtml = true;
            smtp.Send(Message);
        }
        #region Verification from Email Account.  
        public ActionResult UserVerification(string id)
        {
            //bool Status = false;

            db.Configuration.ValidateOnSaveEnabled = false; // Ignor to password confirmation   
            var IsVerify = db.Users.Where(u => u.ActivationCode == new Guid(id)).FirstOrDefault();

            if (IsVerify != null)
            {
                IsVerify.EmailVerification = true;
                db.SaveChanges();
                ViewBag.Message = "Email Verification completed";
                //Status = true;
            }
            else
            {
                ViewBag.Message = "Invalid Request...Email not verify";
                ViewBag.Status = false;
            }

            return View();
        }
        #endregion
        #region User Login
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(UserLogin LgnUsr, User user)
        {
            var _passWord = Community.Models.encryptPassword.textToEncrypt(LgnUsr.Password);
            bool Isvalid = db.Users.Any(x => x.Email == LgnUsr.EmailId && x.EmailVerification == true &&
            x.Password == _passWord);
            
            //var userDetail = db.Users.Where(x => x.Email == user.Email && x.Password == user.Password).FirstOrDefault();
            if (Isvalid)
            {

                int timeout = LgnUsr.Rememberme ? 60 : 5; // Timeout in minutes, 60 = 1 hour.  
                var ticket = new FormsAuthenticationTicket(LgnUsr.EmailId, false, timeout);
                string encrypted = FormsAuthentication.Encrypt(ticket);
                //var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                //cookie.Expires = System.DateTime.Now.AddMinutes(timeout);
                //cookie.HttpOnly = true;
                //Response.Cookies.Add(cookie);
                var userDetail = db.Users.Where(x => x.Email == LgnUsr.EmailId && x.EmailVerification == true && x.Password == _passWord).FirstOrDefault();

                FormsAuthentication.SetAuthCookie(user.ID.ToString(), false);
                Session["ID"] = userDetail.ID;
                Session["FirstName"] = userDetail.FirstName;



                return RedirectToAction("Index", "Home");

            }
            else
            {
                ModelState.AddModelError("", "Invalid Information... Please try again!");
            }
            return View();
        }
        #endregion
        public ActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgetPassword(ForgetPassword pass)
        {
            var IsExists = IsEmailExists(pass.EmailId);
            if (!IsExists)
            {
                ModelState.AddModelError("EmailNotExists", "This email is not exists");
                return View();
            }
            var objUsr = db.Users.Where(x => x.Email == pass.EmailId).FirstOrDefault();

            // Genrate OTP   
            string OTP = GeneratePassword();

            objUsr.ActivationCode = Guid.NewGuid();
            objUsr.OTP = OTP;
            db.Entry(objUsr).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            ForgetPasswordEmailToUser(objUsr.Email, objUsr.ActivationCode.ToString(), objUsr.OTP);
            return View();
        }
        public string GeneratePassword()
        {
            string OTPLength = "4";
            string OTP = string.Empty;

            string Chars = string.Empty;
            Chars = "1,2,3,4,5,6,7,8,9,0";

            char[] seplitChar = { ',' };
            string[] arr = Chars.Split(seplitChar);
            string NewOTP = "";
            string temp = "";
            Random rand = new Random();
            for (int i = 0; i < Convert.ToInt32(OTPLength); i++)
            {
                temp = arr[rand.Next(0, arr.Length)];
                NewOTP += temp;
                OTP = NewOTP;
            }
            return OTP;
        }
        public void ForgetPasswordEmailToUser(string emailId, string activationCode, string OTP)
        {
            var GenerateUserVerificationLink = "/Register/ChangePassword/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, GenerateUserVerificationLink);

            //var fromMail = new MailAddress("community123401@gmail.com", "Denis Dragos");
            //var fromEmailpassword = "*********";
            //var toEmail = new MailAddress(emailId);



            //var smpt = new SmtpClient();
            //smpt.Host = "smpt.gmail.com";
            //smpt.Port = 587;
            //smpt.EnableSsl = true;
            //smpt.DeliveryMethod = SmtpDeliveryMethod.Network;
            //smpt.UseDefaultCredentials = false;
            //smpt.Credentials = new NetworkCredential(fromMail.Address, fromEmailpassword);

            //var Message = new MailMessage(fromMail, toEmail);

            //Message.Subject = "Password Reset-Demo";
            //Message.Body = "<br/> Please click on the below link for password change" +
            //               "<br/><br/><a href=" + link + ">" + link + "</a> " +
            //               "<br/> OTP for password change :" + OTP;
            //Message.IsBodyHtml = true;
            ////smpt.Send(SendEmailToUser(emailId, activationCode), Message);
            //smpt.Send(Message);
            MailMessage mail = new MailMessage();
            mail.To.Add(emailId);
            mail.From = new MailAddress("community123401@gmail.com");
            //mail.Subject = Subject;

            //string userMessage = "";
            //userMessage = "<br/>Name :" + Name;
            //userMessage = userMessage + "<br/>Email Id: " + EmailId;
            //userMessage = userMessage + "<br/>Phone No: " + PhoneNo;
            //userMessage = userMessage + "<br/>Message: " + Message;
            //string Body = "Hi, <br/><br/> A new enquiry by user. Detail is as follows:<br/><br/> " + OTP + "<br/><br/>Thanks";
            string Body = "<br/> Please click on the below link for password change" +
                           "<br/><br/><a href=" + link + ">" + link + "</a> " +
                           "<br/> OTP for password change :" + OTP;

            mail.Body = Body;
            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            //SMTP Server Address of gmail
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.Credentials = new System.Net.NetworkCredential("community123401@gmail.com", "RaAEp9ZU7HfgJ@K");
            // Smtp Email ID and Password For authentication
            smtp.EnableSsl = true;
            smtp.Send(mail);

        }
        public ActionResult ChangePassword()
        {
            return View();

        }
        [HttpPost]
        public ActionResult ChangePassword(ChangePassword model)
        {
            var message = "";
            if(ModelState.IsValid)
            {
                using (CommunityEntities dc = new CommunityEntities())
                {
                    var user=dc.Users.Where(a=>a.OTP==model.OTP).FirstOrDefault();
                    if(user !=null)
                    {
                        user.Password = Community.Models.encryptPassword.textToEncrypt(model.Password);
                        user.OTP = "";
                        dc.Configuration.ValidateOnSaveEnabled = false;
                        dc.SaveChanges();
                        message = "new password update succesfully";
                    }
                    else
                    {
                        ViewBag.Message = "Invalid OTP";
                    }
                }
                

            }
            else
            {
                message = "Something invalid";
            }
            ViewBag.Message = message;
            return View(model);
            return RedirectToAction("Login", "Register");

        }

    }
}