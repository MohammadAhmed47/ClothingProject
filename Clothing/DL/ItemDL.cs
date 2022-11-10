using Clothing.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Clothing.DL
{
    public class ItemDL : Controller
    {
        // GET: ItemDL
        DatabaseEntities db;
        #region Item
        public List<Item> GetActiveItemsList()
        {
            List<Item> Items;
            DatabaseEntities db = new DatabaseEntities();
            Items = db.Items.Where(x => x.IsActive == 1).ToList();
            //Items = db.ShowAllItems().ToList();
            return Items;
        }

        public List<Item> GetInactiveItems()
        {
            List<Item> Item;
            DatabaseEntities db = new DatabaseEntities();
            Item = db.Items.Where(x => x.IsActive == 0).ToList();
            return Item;
        }

        public Item getItemById(int _Id, DatabaseEntities de = null)
        {
            DatabaseEntities db = new DatabaseEntities();
            if (de != null)
            {
                Item Items = de.Items.Where(x => x.IsActive == 1).FirstOrDefault(x => x.Id == _Id);
                //Item Items = de.GetItemById(_Id).FirstOrDefault();
                return Items;
            }
            Item Item = db.Items.Where(x => x.IsActive == 1).FirstOrDefault(x => x.Id == _Id);
            //Item Item = db.GetItemById(_Id).FirstOrDefault(); ;

            return Item;
        }

        public bool AddItem(Item _Item)
        {
            using (db = new DatabaseEntities())
            {
                db.Items.Add(_Item);
                //db.AddItem(_Item.Brand, _Item.ItemName, _Item.Color, _Item.Size, _Item.Fit, _Item.Material, _Item.Notes, _Item.Image, _Item.Careinstructions, _Item.StoreItemNumber, _Item.CategoriesId, _Item.Userid);
                db.SaveChanges();
            }
            return true;
        }

        public bool UpdateItem(Item _Item, DatabaseEntities de = null)
        {
            try
            {
                if (de != null)
                {
                    de.Entry(_Item).State = System.Data.Entity.EntityState.Modified;
                    //de.UpdateItem(_Item.Id, _Item.Brand, _Item.ItemName, _Item.Color, _Item.Size, _Item.Fit, _Item.Material, _Item.Notes, _Item.Image, _Item.Careinstructions, _Item.StoreItemNumber, _Item.CategoriesId, _Item.IsActive, _Item.Userid);
                    de.SaveChanges();
                    return true;
                }
                using (db = new DatabaseEntities())
                {
                    db.Entry(_Item).State = System.Data.Entity.EntityState.Modified;
                    //db.UpdateItem(_Item.Id, _Item.Brand, _Item.ItemName, _Item.Color, _Item.Size, _Item.Fit, _Item.Material, _Item.Notes, _Item.Image, _Item.Careinstructions, _Item.StoreItemNumber, _Item.CategoriesId, _Item.IsActive, _Item.Userid);
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