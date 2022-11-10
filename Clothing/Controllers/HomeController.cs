using Clothing.BL;
using Clothing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Clothing.Controllers
{
    public class HomeController : Controller
    {
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

        public ActionResult Index()
        {
            if (validateUser() != null)
            {
                //int count = new UserBL().GetActiveUserList().Where(x => x.IsActive == 1 && x.Role != 1).ToList().Count();
                //ViewBag.user = count;

                int count = new InfoBL().GetActiveInfoList().Where(x => x.IsActive == 1 && x.User_Id == validateUser().Id && x.Item.IsActive == 1).Count();
                ViewBag.item = count;

                int count1 = new LocationBL().GetActiveLocationList().Where(x => x.IsActive == 1 && x.UserID == validateUser().Id).Count();
                ViewBag.location = count1;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}