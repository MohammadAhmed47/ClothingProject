using Clothing.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Clothing.DL
{
    public class LocationDL : Controller
    {
        // GET: LocationDL
        DatabaseEntities db;
        #region Location
        public List<Location> GetActiveLocationsList()
        {
            List<Location> Locations;
            DatabaseEntities db = new DatabaseEntities();
            Locations = db.Locations.Where(x => x.IsActive == 1).ToList();
            //Locations = db.ShowAllLocation().ToList();
            return Locations;
        }

        public List<Location> GetInactiveLocations()
        {
            List<Location> Location;
            DatabaseEntities db = new DatabaseEntities();
            Location = db.Locations.Where(x => x.IsActive == 0).ToList();
            return Location;
        }

        public Location getLocationById(int _Id, DatabaseEntities de = null)
        {
            DatabaseEntities db = new DatabaseEntities();
            if (de != null)
            {
                Location Locations = de.Locations.Where(x => x.IsActive == 1).FirstOrDefault(x => x.Id == _Id);
                //Location Locations = de.GetLocationById(_Id).FirstOrDefault();
                return Locations;
            }
            Location Location = db.Locations.Where(x => x.IsActive == 1).FirstOrDefault(x => x.Id == _Id);
            //Location Location = db.GetLocationById(_Id).FirstOrDefault();
            return Location;
        }

        public bool AddLocation(Location _Location)
        {
            using (db = new DatabaseEntities())
            {
                db.Locations.Add(_Location);
                //db.AddLocation(_Location.Name, _Location.UserID);
                db.SaveChanges();
            }
            return true;
        }

        public bool UpdateLocation(Location _Location, DatabaseEntities de = null)
        {
            try
            {
                if (de != null)
                {
                    de.Entry(_Location).State = System.Data.Entity.EntityState.Modified;
                    //de.UpdateLocation(_Location.Id, _Location.Name, _Location.UserID, _Location.IsActive);
                    de.SaveChanges();
                    return true;
                }
                using (db = new DatabaseEntities())
                {
                    db.Entry(_Location).State = System.Data.Entity.EntityState.Modified;
                    //db.UpdateLocation(_Location.Id, _Location.Name, _Location.UserID, _Location.IsActive);
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