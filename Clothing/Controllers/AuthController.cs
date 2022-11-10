using Clothing.BL;
using Clothing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Clothing.Controllers
{
    public class AuthController : Controller
    {
        // GET: Auth
        public User validateUser()
        {
            //Get the current claims principal
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

            // Get the claims values
            var userId = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                  .Select(c => c.Value).SingleOrDefault();

            User loggedInUser = new UserBL().GetUserbyId(Convert.ToInt32(userId));

            return loggedInUser;
        }

        public ActionResult Login(string loginErrMsg = "", string passSet = "", string loginSuccessMsg = "")
        {

            if (validateUser() != null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ConfirmPasswordUpdate = passSet;
            ViewBag.ConfirmloginErrMsg = loginErrMsg;
            ViewBag.ConfirmloginSuccessMsg = loginSuccessMsg;

            return View();
        }

        public ActionResult PostLogin(User User)
        {

            //User user = new UserBL().GetActiveUserList().Where(x => (x.Username.ToUpper() == User.Username.ToUpper() || x.Email.ToUpper() == User.Username.ToUpper()) && StringCipher.Decrypt(x.Password, "wallet") == User.Password).FirstOrDefault();
            User user = new UserBL().GetActiveUserList().Where(x => (x.Email.ToUpper() == User.Email.ToUpper()) && x.Password == User.Password).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Login", new { loginErrMsg = "Incorrect Email / Username or Password!" });
            }

            var identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name,user.Name),
                     new Claim(ClaimTypes.Email,user.Email),
                     new Claim(ClaimTypes.Sid,user.Id.ToString()),
                     new Claim("Username", user.Username.ToString()),
                     new Claim("CreatedAt", user.CreatedAt.ToString()),
                     new Claim("Role", user.Role.ToString()),
                   }, "ApplicationCookie");

            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Set current principal  StringCipher.Decrypt(x.Password, "wallet")
            Thread.CurrentPrincipal = claimsPrincipal;
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;
            authManager.SignIn(identity);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;
            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("Login");
        }

        public ActionResult Register()
        {
            return View();
        }

        //public bool EmailSend(string email)
        //{
        //    MailMessage msg = new MailMessage();
        //    string text = "<link href='https://fonts.googleapis.com/css?family=Bree+Serif' rel='stylesheet'><style>  * {";
        //    text += "  font-family: 'Bree Serif', serif; }";
        //    text += " .list-group-item {       border: none;  }    .hor {      border-bottom: 5px solid black;   }";
        //    text += " .line {       margin-bottom: 20px; }";
        //    msg.From = new MailAddress("info@nodlays.com");
        //    msg.To.Add(email);
        //    msg.Subject = "Password Reset";
        //    msg.IsBodyHtml = true;
        //    string temp = "<html>" +
        //            "<head></head>" +
        //            "<body>" +
        //            "<center>" + "<div> <h1 class='text-center'>Hi "+ email +" </h1> " +
        //            "<p class='text-center'> " +
        //                  "You are Getting this Email Because You Requested To Reset Your Account Password.<br>Click the Button Below To Change Your Password" +
        //            " </p>" +
        //            "<p class='text-center'>" +
        //                    "If you did not request a password reset, Please Ignore This Email" +
        //            "</p>" +
        //            "<h4>"+"Thanks"+"</h4>"+
        //            "<br/>" +
        //            "<button style='background-color: #ce2029; padding:12px 16px; border:1px solid #ce2029; border-radius:3px;'>" +
        //                    "<a href='LINKFORFORGOTPASSWORD' style='text-decoration:none; font-size:15px; color:white;'> Reset Password </a>" +
        //            "</button>" + "</div>" + "</center>";
        //    temp += "<script src = 'https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js' ></ script ></ body ></ html >";

        //    //string link = "https://clothingproject.nodlays.com/Auth/UpdatePassword?email=" + email;
        //    string link = "http://localhost:63969/Auth/UpdatePassword?email=" + email;
        //    link = link.Replace("+", "%20");
        //    temp = temp.Replace("LINKFORFORGOTPASSWORD", link);
        //    msg.Body = temp;
        //    using (SmtpClient client = new SmtpClient())
        //    {
        //        client.EnableSsl = false;
        //        client.UseDefaultCredentials = true;
        //        client.Credentials = new NetworkCredential("no-reply@nodlays.com", "nodlays123");
        //        client.Host = "webmail.nodlays.com";
        //        client.Port = 25;
        //        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        client.Send(msg);
        //    }

        //    return true;
        //}

        public bool EmailSend(string email)
        {
            User u = new UserBL().GetActiveUserList().Where(x => x.Email == email).FirstOrDefault();
            MailMessage msg = new MailMessage();
            string text = "<link href='https://fonts.googleapis.com/css?family=Bree+Serif' rel='stylesheet'><style>  * {";
            text += "  font-family: 'Bree Serif', serif; }";
            text += " .list-group-item {       border: none;  }    .hor {      border-bottom: 5px solid black;   }";
            text += " .line {       margin-bottom: 20px; }";
            msg.From = new MailAddress("no-reply@nodlays.com");
            msg.To.Add(email);
            msg.Subject = "Forgot Password!";
            msg.IsBodyHtml = true;
            string temp = "<html>" +
                    "<head></head>" +
                    "<body>" +
                    "<center>" + "<div> <h1 class='text-center' style='color:#000000'>Hi " + u.Name + " </h1> " +
                    "<p class='text-center' style='color:#000000'> " +
                          "You are Getting this Email Because You Requested To Reset Your Account Password.<br>Click the Button Below To Change Your Password" +
                    " </p>" +
                    "<p style='color:#000000' class='text-center'>" +
                            "If you did not request a password reset, Please Ignore This Email" +
                    "</p>" +
                    "<h3 style='color:#000000'>" + "Thanks" + "</h3>" +
                    "<br/>" +
                    "<button style='background-color: #ce2029; padding:12px 16px; border:1px solid #ce2029; border-radius:3px;'>" +
                            "<a href='LINKFORFORGOTPASSWORD' style='text-decoration:none; font-size:15px; color:white;'> Reset Password </a>" +
                    "</button>" +
                    "</div>" + "</center>";
            temp += "<script src = 'https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js' ></ script ></ body ></ html >";
            string link = "https://clothingproject.nodlays.com/Auth/UpdatePassword?email=" + email;
            //string link = "http://localhost:63969/Auth/UpdatePassword?email=" + email;

            link = link.Replace("+", "%20");
            temp = temp.Replace("LINKFORFORGOTPASSWORD", link);
            msg.Body = temp;
            using (SmtpClient client = new SmtpClient())
            {
                client.EnableSsl = false;
                client.UseDefaultCredentials = true;
                client.Credentials = new NetworkCredential("no-reply@nodlays.com", "nodlays123");
                client.Host = "webmail.nodlays.com";
                client.Port = 25;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(msg);
            }
            //using (SmtpClient smt = new SmtpClient())
            //{
            //    smt.Host = "smtp.gmail.com";
            //    System.Net.NetworkCredential ntwd = new NetworkCredential();
            //    ntwd.UserName = "muhammad.hassan93b@gmail.com"; //Your Email ID
            //    ntwd.Password = "qwerty_123!@#"; // Your Password
            //    smt.UseDefaultCredentials = true;
            //    smt.Credentials = ntwd;
            //    smt.Port = 587;
            //    smt.DeliveryMethod = SmtpDeliveryMethod.Network;
            //    smt.EnableSsl = true;
            //    smt.Send(msg);
            //}
            return true;
        }

        public ActionResult UpdatePassword(string email, string t)
        {
            string decodeEmail = email;
            User user = new UserBL().GetActiveUserList().Where(x => x.Email == decodeEmail).FirstOrDefault();
            ViewBag.PasswordNotSet = "";
            if (user == null)
            {
                return RedirectToAction("RecoverPassword", new { uMsg = "User not found!" });
            }
            return View(user);
        }

        public ActionResult ResetPassword(string Email, string uMsg = "")
        {
            User user = new UserBL().GetActiveUserList().Where(x => x.Email == Email && x.IsActive == 1).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("RecoverPassword", new { uMsg = "User Can't Found" });
            }
            else
            {
                if (EmailSend(Email) == false)
                {
                    return RedirectToAction("RecoverPassword", new { uMsg = "Error in sending email!" });
                }
                else
                {
                    return RedirectToAction("RecoverPassword", new { errMsg = "Email sent sucessfully!" });
                }
            }
            
            
        }

        public ActionResult RecoverPassword(string errMsg = "", string uMsg = "")
        {
            ViewBag.ConfirmNotSentMsg = errMsg;
            ViewBag.uMsg = uMsg;
            return View();
        }

        public ActionResult SetPassword(string password, string confirmpassword, int userId)
        {
            User user = new UserBL().GetActiveUserList().Where(x => x.Id == userId).FirstOrDefault();
            ViewBag.PasswordNotSet = "";

            User updateUser = new User()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Username = user.Username,
                Password = password,
                Address = user.Address,
                Phone = user.Phone,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                Role = user.Role,
            };

            bool updatedUser = new UserBL().UpdateUser(updateUser);

            if (!updatedUser)
            {
                return RedirectToAction("Login", "Auth", new { loginErrMsg = "Error in updating user!" });
            }
            return RedirectToAction("Login", "Auth", new { passSet = "Password Updated!" });
        }
    }
}