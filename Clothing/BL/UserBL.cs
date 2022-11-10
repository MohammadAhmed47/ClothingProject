using Clothing.DL;
using Clothing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Clothing.BL
{
    public class UserBL : Controller
    {
        // GET: UserBL
        #region User
        public List<User> GetActiveUserList()
        {
            return new UserDL().GetActiveUsersList();
        }

        public List<User> GetInactiveUserList()
        {
            return new UserDL().GetInactiveUsers();
        }

        public User GetUserbyId(int _id, DatabaseEntities de = null)
        {
            return new UserDL().getUserById(_id, de);
        }

        public bool AddUser(User _User)
        {
            if (_User.IsActive == null || _User.CreatedAt == null)
                return false;
            return new UserDL().AddUser(_User);
        }

        public bool UpdateUser(User _User, DatabaseEntities de = null)
        {
            if (_User.IsActive == null || _User.CreatedAt == null)
                return false;
            return new UserDL().UpdateUser(_User, de);
        }

        #endregion
    }
}