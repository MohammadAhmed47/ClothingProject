using Clothing.DL;
using Clothing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Clothing.BL
{
    public class InfoBL : Controller
    {
        // GET: InfoBL
        #region Info
        public List<Info> GetActiveInfoList(DatabaseEntities de = null)
        {
            return new InfoDL().GetActiveInfoesList(de);
        }

        public List<Info> GetInactiveInfoList()
        {
            return new InfoDL().GetInactiveInfoes();
        }

        public Info GetInfobyId(int _id, DatabaseEntities de = null)
        {
            return new InfoDL().getInfoById(_id, de);
        }

        public bool AddInfo(Info _Info)
        {
            if (_Info.IsActive == null || _Info.CreatedAt == null)
                return false;
            return new InfoDL().AddInfo(_Info);
        }

        public bool UpdateInfo(Info _Info, DatabaseEntities de = null)
        {
            if (_Info.IsActive == null || _Info.CreatedAt == null)
                return false;
            return new InfoDL().UpdateInfo(_Info, de);
        }

        #endregion
    }
}