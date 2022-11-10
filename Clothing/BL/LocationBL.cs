using Clothing.DL;
using Clothing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Clothing.BL
{
    public class LocationBL : Controller
    {
        // GET: LocationBL
        #region Location
        public List<Location> GetActiveLocationList()
        {
            return new LocationDL().GetActiveLocationsList();
        }

        public List<Location> GetInactiveLocationList()
        {
            return new LocationDL().GetInactiveLocations();
        }

        public Location GetLocationbyId(int _id, DatabaseEntities de = null)
        {
            return new LocationDL().getLocationById(_id, de);
        }

        public bool AddLocation(Location _Location)
        {
            if (_Location.IsActive == null || _Location.CreatedAt == null)
                return false;
            return new LocationDL().AddLocation(_Location);
        }

        public bool UpdateLocation(Location _Location, DatabaseEntities de = null)
        {
            if (_Location.IsActive == null || _Location.CreatedAt == null)
                return false;
            return new LocationDL().UpdateLocation(_Location, de);
        }

        #endregion
    }
}