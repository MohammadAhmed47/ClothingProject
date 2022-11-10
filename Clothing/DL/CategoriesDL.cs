using Clothing.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Clothing.DL
{
    public class CategoryDL : Controller
    {
        // GET: CategoryDL
        DatabaseEntities db;
        #region Category
        public List<Category> GetActiveCategorysList()
        {
            List<Category> Categorys;
            DatabaseEntities db = new DatabaseEntities();
            Categorys = db.Categories.Where(x => x.IsActive == 1).ToList();
            //Categorys = db.ShowAllCategories().ToList();
            return Categorys;
        }

        public List<Category> GetInactiveCategorys()
        {
            List<Category> Category;
            DatabaseEntities db = new DatabaseEntities();
            Category = db.Categories.Where(x => x.IsActive == 0).ToList();
            return Category;
        }

        public Category getCategoryById(int _Id, DatabaseEntities de = null)
        {
            DatabaseEntities db = new DatabaseEntities();
            if (de != null)
            {
                Category Categorys = de.Categories.Where(x => x.IsActive == 1).FirstOrDefault(x => x.Id == _Id);
                //Category Categorys = de.GetCategoryById(_Id).FirstOrDefault();
                return Categorys;
            }
            Category Category = db.Categories.Where(x => x.IsActive == 1).FirstOrDefault(x => x.Id == _Id);

            return Category;
        }

        public Category getCategoryByIdwithisActive(int _Id, DatabaseEntities de = null)
        {
            DatabaseEntities db = new DatabaseEntities();
            if (de != null)
            {
                Category Categorys = de.Categories.Where(x => x.IsActive != 2).FirstOrDefault(x => x.Id == _Id);
                //Category Categorys = de.GetCategoryById(_Id).FirstOrDefault();
                return Categorys;
            }
            Category Category = db.Categories.Where(x => x.IsActive != 2).FirstOrDefault(x => x.Id == _Id);

            return Category;
        }


        public bool AddCategory(Category _Category)
        {
            using (db = new DatabaseEntities())
            {
                db.Categories.Add(_Category);
                //db.AddCategory(_Category.Name, _Category.SubCategory, _Category.CatPath, _Category.UserId );
                db.SaveChanges();
            }
            return true;
        }

        public bool UpdateCategory(Category _Category, DatabaseEntities de = null)
        {
            try
            {
                if (de != null)
                {
                    de.Entry(_Category).State = System.Data.Entity.EntityState.Modified;
                    //de.UpdateCategories(_Category.Id, _Category.Name, _Category.UserId, _Category.SubCategory, _Category.CatPath, _Category.IsActive);
                    de.SaveChanges();
                    return true;
                }
                using (db = new DatabaseEntities())
                {
                    db.Entry(_Category).State = System.Data.Entity.EntityState.Modified;
                    //db.UpdateCategories(_Category.Id, _Category.Name, _Category.UserId, _Category.SubCategory, _Category.CatPath, _Category.IsActive);
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