using Clothing.DL;
using Clothing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Clothing.BL
{
    public class CategoriesBL : Controller
    {
        // GET: CategoriesBL
        #region Category
        public List<Category> GetActiveCategoryList()
        {
            return new CategoryDL().GetActiveCategorysList();
        }

        public List<Category> GetInactiveCategoryList()
        {
            return new CategoryDL().GetInactiveCategorys();
        }

        public Category GetCategorybyId(int _id, DatabaseEntities de = null)
        {
            return new CategoryDL().getCategoryById(_id, de);
        }

        public Category getCategoryByIdwithisActive(int _id, DatabaseEntities de = null)
        {
            return new CategoryDL().getCategoryByIdwithisActive(_id, de);
        }

        public bool AddCategory(Category _Category)
        {
            if (_Category.IsActive == null || _Category.CreatedAt == null)
                return false;
            return new CategoryDL().AddCategory(_Category);
        }

        public bool UpdateCategory(Category _Category, DatabaseEntities de = null)
        {
            if (_Category.IsActive == null || _Category.CreatedAt == null)
                return false;
            return new CategoryDL().UpdateCategory(_Category, de);
        }

        #endregion
    }
}