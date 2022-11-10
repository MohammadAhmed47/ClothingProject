using Clothing.BL;
using Clothing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Clothing.HelpingClasses;

namespace Clothing.Controllers
{
    public class LocationController : Controller
    {
        // GET: Location
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

        public ActionResult AddLocation(string msg = "", string errmsg = "")
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            ViewBag.msgs = msg;
            ViewBag.ermsg = errmsg;
            return View();
        }

        public ActionResult PostAddLocation(Location location)
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            Location l = new LocationBL().GetActiveLocationList().Where(x => x.UserID == validateUser().Id).FirstOrDefault();
            //int Count = new LocationBL().GetActiveLocationList().Where(x => x.Name.ToUpper() == location.Name.ToUpper() ).Count();
            if(l != null)
            {
                int Count = new LocationBL().GetActiveLocationList().Where(x => x.Name.ToUpper() == location.Name.ToUpper()).Count();
                if (Count > 0)
                {
                    return RedirectToAction("AddLocation", "Location", new { errmsg = "Location already exist" });
                }
            }
            
            Location _location = new Location()
            {
                Name = location.Name,
                IsActive = 1,
                CreatedAt = DateTime.Now,
                UserID = validateUser().Id

            };

            if (new LocationBL().AddLocation(_location))
            {
                return RedirectToAction("ShowAllLocations", "Location", new { msg = "Location Added SuccessFully" });
            }
            else
            {
                return RedirectToAction("ShowAllLocations", "Location", new { errmsg = "Location Can't be Added" });
            }
        }

        public ActionResult ShowAllLocations(string msg = "", string errmsg = "")
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (validateUser().Role == 1)
            {
                List<Location> ls = new List<Location>();
                ls = new LocationBL().GetActiveLocationList().Where(x => x.IsActive == 1).OrderBy(x => x.CreatedAt).ToList();
                ViewBag.location = ls;
                ViewBag.msgs = msg;
                ViewBag.ermsg = errmsg;
                return View();
            }
            else
            {
                List<Location> ls = new List<Location>();
                ls = new LocationBL().GetActiveLocationList().Where(x => x.UserID == validateUser().Id && x.IsActive == 1).OrderBy(x => x.CreatedAt).ToList();
                ViewBag.location = ls;
                ViewBag.msgs = msg;
                ViewBag.ermsg = errmsg;
                return View();
            }
        }
        [HttpPost]
        public ActionResult AllLocation()
        {
            if (validateUser() != null)
            {
                List<Location> _location;
                if (validateUser().Role == 1)
                {
                    _location = new LocationBL().GetActiveLocationList().Where(x => x.IsActive == 1).OrderBy(x => x.CreatedAt).ToList();
                }
                else
                {
                    _location = new LocationBL().GetActiveLocationList().Where(x => x.UserID == validateUser().Id && x.IsActive == 1).OrderBy(x => x.CreatedAt).ToList();
                }
                int start2 = Convert.ToInt32(Request["start"]);
                int length = Convert.ToInt32(Request["length"]);
                string searchValue = Request["search[value]"];
                string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
                string sortDirection = Request["order[0][dir]"];

                int totalrows = _location.Count();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    _location = _location.Where(x => x.Name.ToString().Contains(searchValue.ToLower()) || x.Name.ToLower().Contains(searchValue.ToLower())).ToList();
                }
                int totalrowsafterfilterinig = _location.Count();

                List<LocationDTOList> LocationDTOLists = new List<LocationDTOList>();

                _location = _location.Skip(start2).Take(length).ToList();

                var i = 0;
                foreach (Location x in _location)
                {

                    LocationDTOList LocationDTO = new LocationDTOList()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        UserId = x.User.Username,
                    };
                    LocationDTOLists.Add(LocationDTO);
                }
                return Json(new { data = LocationDTOLists, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        public ActionResult PostDeleteLocation(int Id = -1)
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            DatabaseEntities dc = new DatabaseEntities();
            Location location = new LocationBL().GetLocationbyId(Id, dc);
            if (location != null)
            {
                location.IsActive = 0;
                if (new LocationBL().UpdateLocation(location, dc))
                {
                    return RedirectToAction("ShowAllLocations", "Location", new { errmsg = "Location Deleted Successfully" });
                }
                else
                {
                    return RedirectToAction("ShowAllLocations", "Location", new { errmsg = "Location Cant Be Deleted" });
                }
            }
            else
            {
                return RedirectToAction("ShowAllLocations", "Location", new { errmsg = "Unable to Locate Location" });
            }
        }

        public ActionResult UpdateLocation(int Id, string msg = "", string errmsg = "")
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            Location location = new LocationBL().GetLocationbyId(Id);
            ViewBag.msgs = msg;
            ViewBag.ermsg = errmsg;
            return View(location);
        }

        public ActionResult PostUpdateLocation(Location _location)
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            Location location = new LocationBL().GetLocationbyId(_location.Id);

            Location updatedlocation = new Location()
            {
                Name = _location.Name,
                IsActive = location.IsActive,
                CreatedAt = location.CreatedAt,
                Id = location.Id,
                UserID = location.UserID
                
            };

            if (new LocationBL().UpdateLocation(updatedlocation) == true)
            {
                return RedirectToAction("ShowAllLocations", "Location", new { msg = "Location Successfully Updated" });
            }
            else
            {
                return RedirectToAction("ShowAllLocations", "Location", new { errmsg = "Location Can't be Updated" });
            }
        }
    }
}