using Clothing.DL;
using Clothing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Clothing.BL
{
    public class ItemBL : Controller
    {
        // GET: ItemBL
        #region Item
        public List<Item> GetActiveItemList()
        {
            return new ItemDL().GetActiveItemsList();
        }

        public List<Item> GetInactiveItemList()
        {
            return new ItemDL().GetInactiveItems();
        }

        public Item GetItembyId(int _id, DatabaseEntities de = null)
        {
            return new ItemDL().getItemById(_id, de);
        }

        public bool AddItem(Item _Item)
        {
            if (_Item.IsActive == null || _Item.CreatedAt == null)
                return false;
            return new ItemDL().AddItem(_Item);
        }

        public bool UpdateItem(Item _Item, DatabaseEntities de = null)
        {
            if (_Item.IsActive == null || _Item.CreatedAt == null)
                return false;
            return new ItemDL().UpdateItem(_Item, de);
        }

        #endregion
    }
}