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
    public class AdminController : Controller
    {
        // GET: Admin
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

        public ActionResult AddUser(string msg = "", string errmsg = "")
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            ViewBag.msgs = msg;
            ViewBag.ermsg = errmsg;
            return View();
        }

        public ActionResult PostAddUser(User user)
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            int Count = new UserBL().GetActiveUserList().Where(x => x.Username.ToUpper() == user.Username.ToUpper() || x.Email.ToUpper() == user.Email.ToUpper()).Count();
            if (Count > 0)
            {
                return RedirectToAction("AddUser", "Admin", new { errmsg = "User already exist" });
            }
            User _user = new User()
            {
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                Phone = user.Phone,
                Address = user.Address,
                IsActive = 1,
                CreatedAt = DateTime.Now,
                Username = user.Username,
                Role = user.Role,
            };
            
            if (new UserBL().AddUser(_user))
            {
                if (_user.Role == 2)
                {
                    return RedirectToAction("ShowAllUsers", "Admin", new { msg = "User Added SuccessFully" });
                }
                else
                {
                    return RedirectToAction("ShowAllAdmins", "Admin", new { msg = "Admin Added SuccessFully" });
                }
            }
            else
            {
                if (_user.Role == 2)
                {
                    return RedirectToAction("ShowAllUsers", "Admin", new { errmsg = "User Can't Added" });
                }
                else
                {
                    return RedirectToAction("ShowAllAdmins", "Admin", new { errmsg = "Admin Can't Added" });
                }
            }
        }

        public ActionResult ShowAllUsers(string msg = "", string errmsg = "")
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            List<User> us = new List<User>();
            us = new UserBL().GetActiveUserList().Where(x => x.Role == 2 && x.IsActive == 1).OrderBy(x => x.CreatedAt).ToList();
            ViewBag.user = us;
            ViewBag.msgs = msg;
            ViewBag.ermsg = errmsg;
            return View();
        }
        [HttpPost]
        public ActionResult AllUsers()
        {
            if (validateUser() != null)
            {
                List<User> _user;
                //_user = new UserBL().GetActiveUserList().Where(x => x.Id != validateUser().Id).OrderBy(x => x.CreatedAt).ToList();
                _user = new UserBL().GetActiveUserList().Where(x => x.Role == 2 && x.IsActive == 1).OrderBy(x => x.CreatedAt).ToList();

                int start2 = Convert.ToInt32(Request["start"]);
                int length = Convert.ToInt32(Request["length"]);
                string searchValue = Request["search[value]"];
                string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
                string sortDirection = Request["order[0][dir]"];

                int totalrows = _user.Count();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    _user = _user.Where(x => x.Name.ToString().Contains(searchValue.ToLower()) || x.Email.ToLower().Contains(searchValue.ToLower()) || x.Username.ToLower().Contains(searchValue.ToLower())).ToList();
                }
                int totalrowsafterfilterinig = _user.Count();

                List<User> userDTOList = new List<User>();

                _user = _user.Skip(start2).Take(length).ToList();

                var i = 0;
                foreach (User x in _user)
                {

                    User userDTO = new User()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Password = x.Password,
                        Email = x.Email,
                        Phone = x.Phone,
                        Address = x.Address,
                        Username = x.Username
                    };
                    userDTOList.Add(userDTO);
                }
                return Json(new { data = userDTOList, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        public ActionResult ShowAllAdmins(string msg = "", string errmsg = "")
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            List<User> us = new List<User>();
            us = new UserBL().GetActiveUserList().Where(x => x.Role == 1).OrderBy(x => x.CreatedAt).ToList();
            ViewBag.user = us;
            ViewBag.msgs = msg;
            ViewBag.ermsg = errmsg;
            return View();
        }
        [HttpPost]
        public ActionResult AllAdmins()
        {
            if (validateUser() != null)
            {
                List<User> _user;
                _user = new UserBL().GetActiveUserList().Where(x => x.Role == 1 && x.IsActive == 1 && x.Role == 1).OrderBy(x => x.CreatedAt).ToList();
                //_user = new UserBL().GetActiveUserList().Where(x => x.Role == 2 && x.IsActive == 1).OrderBy(x => x.CreatedAt).ToList();

                int start2 = Convert.ToInt32(Request["start"]);
                int length = Convert.ToInt32(Request["length"]);
                string searchValue = Request["search[value]"];
                string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
                string sortDirection = Request["order[0][dir]"];

                int totalrows = _user.Count();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    _user = _user.Where(x => x.Name.ToString().Contains(searchValue.ToLower()) || x.Email.ToLower().Contains(searchValue.ToLower()) || x.Username.ToLower().Contains(searchValue.ToLower())).ToList();
                }
                int totalrowsafterfilterinig = _user.Count();

                List<User> userDTOList = new List<User>();

                _user = _user.Skip(start2).Take(length).ToList();

                var i = 0;
                foreach (User x in _user)
                {

                    User userDTO = new User()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Password = x.Password,
                        Email = x.Email,
                        Phone = x.Phone,
                        Address = x.Address,
                        Username = x.Username
                    };
                    userDTOList.Add(userDTO);
                }
                return Json(new { data = userDTOList, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        public ActionResult PostDeleteUser(int Id = -1, string msg= "", string errmsg = "")
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            
            DatabaseEntities dc = new DatabaseEntities();
            User user = new UserBL().GetUserbyId(Id, dc);
            ViewBag.msgs = msg;
            ViewBag.ermsg = errmsg;
            if (user != null)
            {
                user.IsActive = 0;
                if (new UserBL().UpdateUser(user, dc))
                {
                    if (user.Role == 2)
                    {
                        return RedirectToAction("ShowAllUsers", "Admin", new { errmsg = "User Deleted Successfully" });
                    }
                    else
                    {
                        return RedirectToAction("ShowAllAdmins", "Admin", new { errmsg = "User Deleted Successfully" });
                    }
                }
                else
                {
                    if (user.Role == 2)
                    {
                        return RedirectToAction("ShowAllUsers", "Admin", new { errmsg = "User Cant Be Deleted" });
                    }
                    else
                    {
                        return RedirectToAction("ShowAllAdmins", "Admin", new { errmsg = "User Cant Be Deleted" });
                    }
                }
            }
            else
            {
                if (user.Role == 2)
                {
                    return RedirectToAction("ShowAllUsers", "Admin", new { errmsg = "Unable to Locate User" });
                }
                else
                {
                    return RedirectToAction("ShowAllAdmins", "Admin", new { errmsg = "Unable to Locate User" });
                }
            }

        }

        public ActionResult UpdateUser(int Id, string msg = "", string errmsg = "")
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            User user = new UserBL().GetUserbyId(Id);
            ViewBag.msgs = msg;
            ViewBag.ermsg = errmsg;
            return View(user);
        }

        public ActionResult UserProfile(int Id, string msg = "", string errmsg = "")
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            User user = new UserBL().GetUserbyId(Id);
            ViewBag.msgs = msg;
            ViewBag.ermsg = errmsg;
            return View(user);
        }

        public ActionResult PostUpdateUser(User _user, int Id = -1)
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            DatabaseEntities dg = new DatabaseEntities();
            User user = new UserBL().GetUserbyId(Id, dg);
            if (Id == validateUser().Id)
            {
                user.Id = user.Id;
                user.Username = _user.Username;
                user.Name = _user.Name;
                user.Email = _user.Email;
                user.Phone = _user.Phone;
                user.Address = _user.Address;
                if(new UserBL().UpdateUser(user, dg) == true)
                {
                    return RedirectToAction("UserProfile", "Admin", new { Id = user.Id, msg = "Profile Successfully Updated" });
                }
                else
                {
                    return RedirectToAction("UserProfile", "Admin", new { Id = user.Id, msg = "Profile is not Updated" });
                }
            }
            User updateduser = new User()
            {
                Username = _user.Username,
                Name = _user.Name,
                Email = _user.Email,
                Password = _user.Password,
                Phone = _user.Phone,
                Address = _user.Address,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                Id = user.Id,
                Role = user.Role,
            };

            if (new UserBL().UpdateUser(updateduser) == true)
            {
                if (user.Role == 2)
                {
                    return RedirectToAction("ShowAllUsers", "Admin", new { msg = "User Successfully Updated" });
                }
                else
                {
                    return RedirectToAction("ShowAllAdmins", "Admin", new { msg = "User Successfully Updated" });
                }
            }
            else
            {
                if (user.Role == 2)
                {
                    return RedirectToAction("ShowAllUsers", "Admin", new { errmsg = "User Name Can't Updated" });
                }
                else
                {
                    return RedirectToAction("ShowAllAdmins", "Admin", new { errmsg = "User Name Can't Updated" });
                }
            }
        }

        public ActionResult ChangePassword(int Id = -1, string msg = "", string errmsg = "")
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            User user = new UserBL().GetUserbyId(Id);
            ViewBag.msgs = msg;
            ViewBag.ermsg = errmsg;
            return View(user);
        }

        public ActionResult PostChangePassword(User _user)
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            DatabaseEntities dg = new DatabaseEntities();
            User user = new UserBL().GetUserbyId(_user.Id, dg);

            user.Password = _user.Password;
            if (new UserBL().UpdateUser(user, dg) == true)
            {
                return RedirectToAction("UserProfile", "Admin", new { Id = user.Id, msg = "Password Successfully Updated" });
            }
            else
            {
                return RedirectToAction("UserProfile", "Admin", new { Id = user.Id, msg = "Password is not Updated" });
            }
        }
    }
}