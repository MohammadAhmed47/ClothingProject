using Clothing.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Clothing.DL
{
    public class UserDL : Controller
    {
        DatabaseEntities db;
        #region User
        public List<User> GetActiveUsersList()
        {
            List<User> Users;
            DatabaseEntities db = new DatabaseEntities();
            Users = db.Users.Where(x => x.IsActive == 1).ToList();
            //Users = db.ShowAllUser().ToList();
            return Users;
        }

        public List<User> GetInactiveUsers()
        {
            List<User> user;
            DatabaseEntities db = new DatabaseEntities();
            user = db.Users.Where(x => x.IsActive == 3).ToList();
            return user;
        }

        public User getUserById(int _Id, DatabaseEntities de = null)
        {
            DatabaseEntities db = new DatabaseEntities();
            if (de != null)
            {
                User Users = de.Users.Where(x => x.IsActive == 1).FirstOrDefault(x => x.Id == _Id);
                //User Users = de.GetUserById(_Id).FirstOrDefault();
                return Users;
            }
            User User = db.Users.Where(x => x.IsActive == 1).FirstOrDefault(x => x.Id == _Id);
            //User User = db.GetUserById(_Id).FirstOrDefault();
            return User;
        }

        public bool AddUser(User _User)
        {
            using (db = new DatabaseEntities())
            {
                db.Users.Add(_User);
                //db.AddUser(_User.Name, _User.Email, _User.Username, _User.Password, _User.Address, _User.Phone, _User.Role);
                db.SaveChanges();
            }
            return true;
        }

        public bool UpdateUser(User _User, DatabaseEntities de = null)
        {
            try
            {
                if (de != null)
                {
                    de.Entry(_User).State = System.Data.Entity.EntityState.Modified;
                    //de.UpdateUser(_User.Id, _User.Name, _User.Email, _User.Username, _User.Password, _User.Address, _User.Phone, _User.Role, _User.IsActive);
                    de.SaveChanges();
                    return true;
                }
                using (db = new DatabaseEntities())
                {
                    db.Entry(_User).State = System.Data.Entity.EntityState.Modified;
                    //db.UpdateUser(_User.Id, _User.Name, _User.Email, _User.Username, _User.Password, _User.Address, _User.Phone, _User.Role, _User.IsActive);
                    db.SaveChanges();
                }
                return true;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        #endregion
    }
}