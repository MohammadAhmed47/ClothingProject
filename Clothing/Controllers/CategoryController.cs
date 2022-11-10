using Clothing.BL;
using Clothing.HelpingClasses;
using Clothing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using OfficeOpenXml;
using System.Data.Entity;

namespace Clothing.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category
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

        public ActionResult AddCategory(string msg = "", string errmsg = "")
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            ViewBag.msgs = msg;
            ViewBag.ermsg = errmsg;
            return View();
        }

        public ActionResult PostAddCategory(Category category)
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            int Count = new CategoriesBL().GetActiveCategoryList().Where(x => x.Name.ToUpper() == category.Name.ToUpper()).Count();
            if (Count > 0)
            {
                return RedirectToAction("AddLocation", "Location", new { msg = "Location already exist" });
            }
            Category _category = new Category()
            {
                Name = category.Name,
                IsActive = 1,
                CreatedAt = DateTime.Now,
                UserId = validateUser().Id,
                SubCategory = 0
            };

            if (new CategoriesBL().AddCategory(_category))
            {
                return RedirectToAction("ShowAllCategories", "Category", new { msg = "Category Added Successfully" });
            }
            else
            {
                return RedirectToAction("ShowAllCategories", "Category", new { msg = "Category Can't be Added" });
            }
        }

        public ActionResult ShowAllCategories(string msg = "", string errmsg = "")
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            List<Category> cgi;
            cgi = new CategoriesBL().GetActiveCategoryList().Where(x => x.Id != validateUser().Id && x.IsActive == 1 && x.SubCategory == 0).OrderBy(x => x.CreatedAt).ToList();
            List<Category> cg = new List<Category>();
            

            ViewBag.Category = cgi;
            ViewBag.msgs = msg;
            ViewBag.ermsg = errmsg;

            //foreach(Category x in cgi)
            //{
            //    Category categoryDTO = new Category()
            //    {
            //        Id = x.Id,
            //        Name = x.Name,
            //        IsActive = x.IsActive,
            //        CreatedAt = x.CreatedAt,
            //        SubCategory = x.SubCategory,
            //        UserId = x.UserId,
            //    };
            //    cg.Add(categoryDTO);
            //}
            //ViewBag.cate = cg;
            return View();
        }
        [HttpPost]
        public ActionResult AllCategories(string msg = "")
        {
            if (validateUser() != null)
            {
                List<Category> _Category;
                _Category = new CategoriesBL().GetActiveCategoryList().Where(x => x.IsActive == 1 && x.SubCategory == 0).OrderBy(x => x.CreatedAt).ToList();
                

                int start2 = Convert.ToInt32(Request["start"]);
                int length = Convert.ToInt32(Request["length"]);
                string searchValue = Request["search[value]"];
                string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
                string sortDirection = Request["order[0][dir]"];

                int totalrows = _Category.Count();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    _Category = _Category.Where(x => x.Name.ToString().Contains(searchValue.ToLower()) || x.Name.ToLower().Contains(searchValue.ToLower())).ToList();
                }
                int totalrowsafterfilterinig = _Category.Count();

                List<Category> CategoryDTOList = new List<Category>();

                _Category = _Category.Skip(start2).Take(length).ToList();

                Category c = new Category();
                c = new CategoriesBL().GetCategorybyId(c.Id);
                var i = 0;

                foreach (Category x in _Category)
                {
                    Category LocationDTO = new Category()
                    {
                        Id = x.Id,
                        Name = x.Name,
                    };
                    CategoryDTOList.Add(LocationDTO);
                }
                return Json(new { data = CategoryDTOList, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        public ActionResult PostDeleteCategory(int Id = -1, string msg= "", string errmsg = "")
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            ViewBag.msgs = msg;
            ViewBag.ermsg = errmsg;
            DatabaseEntities dc = new DatabaseEntities();
            Category category = new CategoriesBL().GetCategorybyId(Id, dc);
            Category c = new CategoriesBL().GetActiveCategoryList().Where(x => x.SubCategory == Id).FirstOrDefault();
            
            if (category != null)
            {
                if (c != null)
                {
                    if (c.SubCategory == Id)
                    {
                        return RedirectToAction("ShowAllCategories", "Category", new { errmsg = "This Category Cant be Deleted, Subcategoies of this Category Existed" });
                    }
                }
                category.IsActive = 0;
                if (new CategoriesBL().UpdateCategory(category, dc))
                {
                    return RedirectToAction("ShowAllCategories", "Category", new { errmsg = "Category Deleted Successfully" });
                }
                else
                {
                    return RedirectToAction("ShowAllCategories", "Category", new { errmsg = "Category Cant Be Deleted" });
                }
            }
            else
            {
                return RedirectToAction("ShowAllCategories", "Category", new { errmsg = "Unable to Locate Category" });
            }

        }

        public ActionResult UpdateCategory(int Id, string msg = "", string errmsg = "")
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            Category category = new CategoriesBL().GetCategorybyId(Id);
            ViewBag.msgs = msg;
            ViewBag.ermsg = errmsg;
            return View(category);
        }

        public ActionResult PostUpdateCategory(Category _category)
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            Category category = new CategoriesBL().GetCategorybyId(_category.Id);

            Category updatedCategory = new Category()
            {
                Name = _category.Name,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                Id = category.Id,
                UserId = category.UserId,
                SubCategory = category.SubCategory

            };

            if (new CategoriesBL().UpdateCategory(updatedCategory) == true)
            {
                return RedirectToAction("ShowAllCategories", "Category", new { msg = "Category Successfully Updated" });
            }
            else
            {
                return RedirectToAction("ShowAllCategories", "Category", new { errmsg = "Category Can't be Updated" });
            }
        }

        //SubCategories

        public ActionResult AddSubCategory(string msg = "", string errmsg = "")
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            List<Category> c = new CategoriesBL().GetActiveCategoryList().Where(x => x.IsActive == 1).ToList();
            //List<Category> sc = new CategoriesBL().GetActiveCategoryList().Where(x => x.IsActive == 1 && x.SubCategory != null).ToList();
            //ViewBag.subcategories = sc;
            ViewBag.categories = c;
            ViewBag.msgs = msg;
            ViewBag.ermsg = errmsg;
            return View();
        }

        public ActionResult PostAddSubCategory(Category category)
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            DatabaseEntities db = new DatabaseEntities();
            int Count = new CategoriesBL().GetActiveCategoryList().Where(x => x.Name.ToUpper() == category.Name.ToUpper()).Count();
            if (Count > 0)
            {
                return RedirectToAction("AddSubCategory", "Category", new { errmsg = "Category already exist" });
            }
            Category _subcategory = new Category()
            {
                Name = category.Name,
                IsActive = 1,
                CreatedAt = DateTime.Now,
                UserId = validateUser().Id,
                SubCategory = category.SubCategory,
                CatPath = "0"
            };
            
            if (new CategoriesBL().AddCategory(_subcategory))
            {
                //Category obj = new CategoriesBL().GetActiveCategoryList().LastOrDefault();

                //Category cv = new CategoriesBL().GetCategorybyId(category.SubCategory.GetValueOrDefault(), db);

                //obj.CatPath = cv.CatPath + "," +obj.Id;

                
                //bool check = new CategoriesBL().UpdateCategory(obj) == true;
                return RedirectToAction("ShowAllSubCategories", "Category", new { msg = "Category Added SuccessFully" });
            }
            else
            {
                return RedirectToAction("ShowAllSubCategories", "Category", new { errmsg = "Category Can't be Added" });
            }
        }

        public ActionResult ShowAllSubCategories(string msg = "", string errmsg = "")
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            List<Category> cg = new List<Category>();
            cg = new CategoriesBL().GetActiveCategoryList().Where(x => x.SubCategory == 0 && x.IsActive == 1).OrderBy(x => x.CreatedAt).ToList();
            List<CategoryDTOList> CategoryDTO = new List<CategoryDTOList>();
            var i = 0;
            //foreach (Category x in cg)
            //{
            //    CategoryDTOList categoryDTO = new CategoryDTOList()
            //    {
            //        Id = x.Id,
            //        Name = x.Name,
            //        SubCategory = new CategoriesBL().GetCategorybyId(x.SubCategory.GetValueOrDefault()).Name,
            //        catPath = x.CatPath
            //    };

            //    CategoryDTO.Add(categoryDTO);
            //    //List<Category> cgg = new CategoriesBL().GetActiveCategoryList().Where(y => y.SubCategory == x.Id && y.IsActive == 1).OrderBy(y => y.CreatedAt).ToList();
            //    //foreach (Category xx in cgg)
            //    //{
            //    //    CategoryDTOList categoryDTOs = new CategoryDTOList()
            //    //    {
            //    //        Id = x.Id,
            //    //        Name = x.Name,
            //    //        //SubCategory = new CategoriesBL().GetCategorybyId(x.SubCategory.GetValueOrDefault()).Name,
            //    //        catPath = x.CatPath
            //    //    };
            //    //    CategoryDTO.Add(categoryDTOs);
            //    //}
            //}
            ViewBag.Category = cg;
            ViewBag.msgs = msg;
            ViewBag.ermsg = errmsg;
            return View();
        }

        [HttpPost]
        public ActionResult AllSubCategories(string msg = "")
        {
            if (validateUser() != null)
            {
                List<Category> _Category;
                _Category = new CategoriesBL().GetActiveCategoryList().Where(x => x.IsActive == 1 && x.SubCategory != 0).OrderBy(x => x.CreatedAt).ToList();

                int start2 = Convert.ToInt32(Request["start"]);
                int length = Convert.ToInt32(Request["length"]);
                string searchValue = Request["search[value]"];
                string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
                string sortDirection = Request["order[0][dir]"];

                int totalrows = _Category.Count();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    _Category = _Category.Where(x => x.Name.ToString().Contains(searchValue.ToLower()) || x.Name.ToLower().Contains(searchValue.ToLower())).ToList();
                }
                int totalrowsafterfilterinig = _Category.Count();

                List<CategoryDTOList> CategoryDTOLists = new List<CategoryDTOList>();

                _Category = _Category.Skip(start2).Take(length).ToList();

                var i = 0;
                foreach (Category x in _Category)
                {

                    CategoryDTOList LocationDTO = new CategoryDTOList()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        SubCategory =new CategoriesBL().GetCategorybyId(x.SubCategory.GetValueOrDefault()).Name,
                        
                    };

                    CategoryDTOLists.Add(LocationDTO);
                }
                return Json(new { data = CategoryDTOLists, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        public ActionResult PostDeleteSubCategory(int Id = -1, string msg = "", string errmsg = "")
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            ViewBag.msgs = msg;
            ViewBag.ermsg = errmsg;
            DatabaseEntities dc = new DatabaseEntities();
            Category category = new CategoriesBL().GetCategorybyId(Id, dc);
            if (category != null)
            {
                category.IsActive = 0;
                if (new CategoriesBL().UpdateCategory(category, dc))
                {
                    return RedirectToAction("ShowAllSubCategories", "Category", new { errmsg = "SubCategory Deleted Successfully" });
                }
                else
                {
                    return RedirectToAction("ShowAllSubCategories", "Category", new { errmsg = "Category Cant Be Deleted" });
                }
            }
            else
            {
                return RedirectToAction("ShowAllSubCategories", "Category", new { errmsg = "Unable to Locate Category" });
            }
        }

        public ActionResult UpdateSubCategory(int Id, string msg = "", string errmsg = "")
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            Category category = new CategoriesBL().GetCategorybyId(Id);
            List<Category> c = new CategoriesBL().GetActiveCategoryList().Where(x => x.IsActive == 1 && x.SubCategory == 0).ToList();
            ViewBag.categories = c;
            ViewBag.msg = msg;
            ViewBag.ermsg = errmsg;
            return View(category);
        }

        public ActionResult PostUpdateSubCategory(Category _category)
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            Category category = new CategoriesBL().GetCategorybyId(_category.Id);

            Category updatedCategory = new Category()
            {
                Name = _category.Name,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                Id = category.Id,
                UserId = category.UserId,
                SubCategory = _category.SubCategory

            };

            if (new CategoriesBL().UpdateCategory(updatedCategory) == true)
            {
                return RedirectToAction("ShowAllSubCategories", "Category", new { msg = "Category Successfully Updated" });
            }
            else
            {
                return RedirectToAction("ShowAllSubCategories", "Category", new { errmsg = "Category Can't be Updated" });
            }
        }

        public ActionResult GetCategoryResults(int Id, string msg = "", string errmsg = "")
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            List<Category> cg = new List<Category>();
            cg = new CategoriesBL().GetActiveCategoryList().Where(x => x.SubCategory == 0 && x.IsActive == 1).OrderBy(x => x.CreatedAt).ToList();
            List<CategoryDTOList> CategoryDTO = new List<CategoryDTOList>();
            List<Item> cit = new ItemBL().GetActiveItemList().Where(x => x.IsActive != 0 && x.CategoriesId == Id).ToList();

            List<ItemDTOList> CategoryDTOss = new List<ItemDTOList>();
            string str = "";
            foreach (Item i in cit)
            {
                if (i.Image != null)
                {
                    str = "<td> <a target='_blank' href=" + i.Image + ">Picture</a> </td>";
                }
                else
                {
                    str = "No Image Found";
                }
                ItemDTOList itemDTOs = new ItemDTOList()
                {
                    Id = i.Id,
                    ItemName = i.ItemName,
                    Brand = i.Brand,
                    Careinstructions = i.Careinstructions,
                    Color = i.Color,
                    Fit = i.Fit,
                    Material = i.Material,
                    Notes = i.Notes,
                    Size = i.Size,
                    StoreItemNumber = i.StoreItemNumber,
                    Image = str
                };
                if (i.Category != null)
                {
                    if (i.Category.IsActive != 0)
                    {
                        if (i.Category.SubCategory != 0)
                        {
                            var firstcategory = new CategoriesBL().GetCategorybyId(i.Category.SubCategory.GetValueOrDefault());
                            if (firstcategory.SubCategory != 0)
                            {
                                var secondcategory = firstcategory.Name + "-" + i.Category.Name;

                                var thirdcategory = new CategoriesBL().GetCategorybyId(firstcategory.SubCategory.GetValueOrDefault()).Name + " - " + secondcategory;
                                itemDTOs.CategoriesId = thirdcategory;
                            }
                            else
                            {
                                //ItemDTO.CategoriesId = x.Category.Name;
                                itemDTOs.CategoriesId = new CategoriesBL().GetCategorybyId(i.Category.SubCategory.GetValueOrDefault()).Name + " - " + i.Category.Name;
                            }
                        }
                        else
                        {
                            itemDTOs.CategoriesId = i.Category.Name;
                        }
                    }
                    else
                    {
                        if (i.Category.SubCategory != 0)
                        {
                            itemDTOs.CategoriesId = new CategoriesBL().GetCategorybyId(i.Category.SubCategory.GetValueOrDefault()).Name + " - <br>" + i.Category.Name;
                            itemDTOs.CategoriesId = itemDTOs.CategoriesId + " <br><br> <span style='background:#ce2029; color:#ffffff; padding:4px 4px; text-align:center'>This Category or Subcategory is no longer Exists</span>";
                        }
                        else
                        {
                            itemDTOs.CategoriesId = i.Category.Name;
                            itemDTOs.CategoriesId = itemDTOs.CategoriesId + " <br><br> <span style='background:#ce2029; color:#ffffff; padding:4px 4px; text-align:center'>This Category or Subcategory is no longer Exists</span>";
                        }
                    }
                }
                else
                {
                    itemDTOs.CategoriesId = "N/A";
                }
                CategoryDTOss.Add(itemDTOs);
            }

            ViewBag.CategoryItems = CategoryDTOss;
            ViewBag.d = Id;
            ViewBag.msgs = msg;
            ViewBag.ermsg = errmsg;
            return View();
        }
        [HttpPost]
        public ActionResult GetCategory(int Id = -1)
        {
            if (validateUser() != null)
            {
                //List<Category> _Category;
                //_Category = new CategoriesBL().GetActiveCategoryList().Where(x => x.IsActive == 1 && x.SubCategory == 0).OrderBy(x => x.CreatedAt).ToList();

                List<Category> cg = new List<Category>();
                cg = new CategoriesBL().GetActiveCategoryList().Where(x => x.SubCategory == 0 && x.IsActive == 1).OrderBy(x => x.CreatedAt).ToList();

                int start2 = Convert.ToInt32(Request["start"]);
                int length = Convert.ToInt32(Request["length"]);
                string searchValue = Request["search[value]"];
                string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
                string sortDirection = Request["order[0][dir]"];

                int totalrows = cg.Count();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    cg = cg.Where(x => x.Name.ToString().Contains(searchValue.ToLower()) || x.Name.ToLower().Contains(searchValue.ToLower())).ToList();
                }
                int totalrowsafterfilterinig = cg.Count();

                List<ItemDTOList> CategoryDTOLists = new List<ItemDTOList>();

                cg = cg.Skip(start2).Take(length).ToList();
                List<Item> cit = new ItemBL().GetActiveItemList().Where(x => x.IsActive != 0 && x.CategoriesId == Id).ToList();

                string str = "";
                foreach (Item i in cit)
                {
                    if (i.Image != null)
                    {
                        str = "<a target='_blank' href=" + i.Image + ">Picture</a>";
                    }
                    else
                    {
                        str = "No Image Found";
                    }
                    ItemDTOList itemDTOs = new ItemDTOList()
                    {
                        Id = i.Id,
                        ItemName = i.ItemName,
                        Brand = i.Brand,
                        Careinstructions = i.Careinstructions,
                        Color = i.Color,
                        Fit = i.Fit,
                        Material = i.Material,
                        Notes = i.Notes,
                        Size = i.Size,
                        StoreItemNumber = i.StoreItemNumber,
                        Image = str
                    };
                    if (i.Category != null)
                    {
                        if (i.Category.IsActive != 0)
                        {
                            if (i.Category.SubCategory != 0)
                            {
                                var ct = new CategoriesBL().GetCategorybyId(i.Category.SubCategory.GetValueOrDefault());
                                var abc = ct.Name + " - " + i.Category.Name;
                                if (ct.SubCategory != 0)
                                {
                                    itemDTOs.CategoriesId = new CategoriesBL().GetCategorybyId(ct.SubCategory.GetValueOrDefault()).Name + " - " + abc;
                                }
                                else
                                {
                                    itemDTOs.CategoriesId = new CategoriesBL().GetCategorybyId(i.Category.SubCategory.GetValueOrDefault()).Name + " - " + i.Category.Name;
                                }
                            }
                        }
                    }
                    CategoryDTOLists.Add(itemDTOs);
                }
                return Json(new { data = CategoryDTOLists, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        public ActionResult CsvUpload(FormCollection formCollection)
        {
            DatabaseEntities db = new DatabaseEntities();
            var id = validateUser();
            if (id == null)
            {
                return RedirectToAction("Login", "Auth", new { loginErrMsg = "Please login to get access to home page!" });
            }
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if (file.ContentType == "application/vnd.ms-excel" || file.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" || file.ContentType == ".csv")
                {
                    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                    {
                        using (var context = new DatabaseEntities())
                        {
                            context.Database.Log = Console.Write;
                            using (DbContextTransaction transaction = context.Database.BeginTransaction())
                            {
                                try
                                {
                                    string filename = file.FileName;
                                    string fileContentType = file.ContentType;
                                    byte[] fileBytes = new byte[file.ContentLength];
                                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                                    //ExcelPackage.LicenseContext = LicenseContext.Commercial;
                                    using (var package = new ExcelPackage(file.InputStream))
                                    {
                                        var currentSheet = package.Workbook.Worksheets;
                                        var workSheet = currentSheet.First();
                                        var noOfCol = workSheet.Dimension.End.Column;
                                        var noOfRow = workSheet.Dimension?.End.Row;
                                        var check = workSheet.Cells;
                                        List<string> ColumnNames = new List<string>();
                                        string[] copySubCategoriesList = null;
                                        for (int i = 1; i <= workSheet.Dimension.End.Row; i++)
                                        {
                                            string str = workSheet.Cells[i, 1].Value.ToString();

                                            string[] strlist = str.Split('-');

                                            string categoryName = strlist[0];

                                            string[] subCategoriesList = strlist[1].Split('/');

                                            if (i == 1)
                                            {
                                                //adding category

                                                Category sku = new Category()
                                                {
                                                    Name = categoryName.ToUpper(),
                                                    CreatedAt = DateTime.Now,
                                                    IsActive = 1,
                                                    UserId = validateUser().Id,
                                                    SubCategory = 0
                                                };

                                                new CategoriesBL().AddCategory(sku);
                                                Category parentCategory = new CategoriesBL().GetActiveCategoryList().LastOrDefault();

                                                //adding sub categories

                                                foreach (string cat in subCategoriesList)
                                                {
                                                    Category sub = new Category()
                                                    {
                                                        Name = cat.ToUpper(),
                                                        SubCategory = parentCategory.Id,
                                                        CreatedAt = DateTime.Now,
                                                        IsActive = 1,
                                                        UserId = validateUser().Id
                                                    };
                                                    new CategoriesBL().AddCategory(sub);
                                                }

                                                copySubCategoriesList = subCategoriesList;

                                            }
                                            else
                                            {
                                                bool flag = true;

                                                foreach (string sub in copySubCategoriesList)
                                                {
                                                    if (sub.Equals(categoryName))
                                                    {
                                                        flag = false;
                                                        //adding category
                                                        int Count = new CategoriesBL().GetActiveCategoryList().Where(x => x.Name.ToUpper() == categoryName.ToUpper()).FirstOrDefault().Id;
                                                        //int countid = new CategoriesBL().GetActiveCategoryList().Where(x => x.Id = categoryName.FirstOrDefault()).FirstOrDefault();
                                                        if (Count > 0)
                                                        {
                                                            //adding sub categories

                                                            foreach (string cat in subCategoriesList)
                                                            {
                                                                Category subs = new Category()
                                                                {
                                                                    Name = cat.ToUpper(),
                                                                    SubCategory = Count,
                                                                    CreatedAt = DateTime.Now,
                                                                    IsActive = 1,
                                                                    UserId = validateUser().Id
                                                                };
                                                                new CategoriesBL().AddCategory(subs);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Category sku = new Category()
                                                            {
                                                                Name = categoryName.ToUpper(),
                                                                CreatedAt = DateTime.Now,
                                                                IsActive = 1,
                                                                UserId = validateUser().Id
                                                            };

                                                            new CategoriesBL().AddCategory(sku);
                                                            Category parentCategory = new CategoriesBL().GetActiveCategoryList().LastOrDefault();

                                                            //adding sub categories

                                                            foreach (string cat in subCategoriesList)
                                                            {
                                                                Category subs = new Category()
                                                                {
                                                                    Name = cat.ToUpper(),
                                                                    SubCategory = parentCategory.Id,
                                                                    CreatedAt = DateTime.Now,
                                                                    IsActive = 1,
                                                                    UserId = validateUser().Id
                                                                };
                                                                new CategoriesBL().AddCategory(subs);
                                                            }
                                                        }
                                                    }
                                                }

                                                if (flag)
                                                {
                                                    //adding category

                                                    Category skus = new Category()
                                                    {
                                                        Name = categoryName.ToUpper(),
                                                        SubCategory = 0,
                                                        CreatedAt = DateTime.Now,
                                                        IsActive = 1,
                                                        UserId = validateUser().Id
                                                    };

                                                    new CategoriesBL().AddCategory(skus);
                                                    Category parentCategorys = new CategoriesBL().GetActiveCategoryList().LastOrDefault();

                                                    //adding sub categories

                                                    foreach (string cat in subCategoriesList)
                                                    {
                                                        Category subs = new Category()
                                                        {
                                                            Name = cat.ToUpper(),
                                                            SubCategory = parentCategorys.Id,
                                                            CreatedAt = DateTime.Now,
                                                            IsActive = 1,
                                                            UserId = validateUser().Id
                                                        };
                                                        new CategoriesBL().AddCategory(subs);
                                                    }
                                                    copySubCategoriesList = subCategoriesList;
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    transaction.Rollback();
                                    return RedirectToAction("ShowAllSubCategories", "Category", new { type = "danger", errmsg = "Error! Your File is not in correct Format" });
                                }
                            }

                        }
                    }
                    else
                    {
                        return RedirectToAction("ShowAllSubCategories", "Category", new { errmsg = "Please choose excel file!", type = "danger" });
                    }
                }
                else
                {
                    return RedirectToAction("ShowAllSubCategories", "Category", new { errmsg = "Invalid file format!", type = "danger" });
                }
            }
            else
            {
                return RedirectToAction("ShowAllSubCategories", "Category", new { errmsg = "Please choose excel file!", type = "danger" });
            }

            return RedirectToAction("ShowAllSubCategories", "Category", new { msg = "File is uploaded and read successfully!", type = "success" });
        }

        //public ActionResult CsvUpload(FormCollection formCollection)
        //{
        //    DatabaseEntities db = new DatabaseEntities();
        //    var id = validateUser();
        //    if (id == null)
        //    {
        //        return RedirectToAction("Login", "Auth", new { loginErrMsg = "Please login to get access to home page!" });
        //    }
        //    if (Request != null)
        //    {
        //        HttpPostedFileBase file = Request.Files["UploadedFile"];
        //        if (file.ContentType == "application/vnd.ms-excel" || file.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" || file.ContentType == ".csv")
        //        {
        //            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
        //            {
        //                using (var context = new DatabaseEntities())
        //                {
        //                    context.Database.Log = Console.Write;
        //                    using (DbContextTransaction transaction = context.Database.BeginTransaction())
        //                    {
        //                        try
        //                        {
        //                            string filename = file.FileName;
        //                            string fileContentType = file.ContentType;
        //                            byte[] fileBytes = new byte[file.ContentLength];
        //                            var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
        //                            //ExcelPackage.LicenseContext = LicenseContext.Commercial;
        //                            using (var package = new ExcelPackage(file.InputStream))
        //                            {
        //                                var currentSheet = package.Workbook.Worksheets;
        //                                var workSheet = currentSheet.First();
        //                                var noOfCol = workSheet.Dimension.End.Column;
        //                                var noOfRow = workSheet.Dimension?.End.Row;
        //                                var check = workSheet.Cells;
        //                                List<string> ColumnNames = new List<string>();
        //                                string[] copySubCategoriesList = null;
        //                                for (int i = 1; i <= workSheet.Dimension.End.Row; i++)
        //                                {
        //                                    string str = workSheet.Cells[i, 1].Value.ToString();

        //                                    string[] strlist = str.Split('-');

        //                                    string categoryName = strlist[0];

        //                                    string[] subCategoriesList = strlist[1].Split('/');

        //                                    if (i == 1)
        //                                    {
        //                                        //adding category

        //                                        Category sku = new Category()
        //                                        {
        //                                            Name = categoryName.ToUpper(),
        //                                            CreatedAt = DateTime.Now,
        //                                            IsActive = 1,
        //                                            UserId = validateUser().Id,
        //                                            SubCategory = 0
        //                                        };

        //                                        new CategoriesBL().AddCategory(sku);
        //                                        Category parentCategory = new CategoriesBL().GetActiveCategoryList().LastOrDefault();

        //                                        //adding sub categories

        //                                        foreach (string cat in subCategoriesList)
        //                                        {
        //                                            Category sub = new Category()
        //                                            {
        //                                                Name = cat.ToUpper(),
        //                                                SubCategory = parentCategory.Id,
        //                                                CreatedAt = DateTime.Now,
        //                                                IsActive = 1,
        //                                                UserId = validateUser().Id
        //                                            };
        //                                            new CategoriesBL().AddCategory(sub);
        //                                        }

        //                                        copySubCategoriesList = subCategoriesList;

        //                                    }
        //                                    else
        //                                    {
        //                                        bool flag = true;

        //                                        foreach (string sub in copySubCategoriesList)
        //                                        {
        //                                            if (sub.Equals(categoryName))
        //                                            {
        //                                                flag = false;
        //                                                //adding category
        //                                                int Count = new CategoriesBL().GetActiveCategoryList().Where(x => x.Name.ToUpper() == categoryName.ToUpper()).FirstOrDefault().Id;
        //                                                //int countid = new CategoriesBL().GetActiveCategoryList().Where(x => x.Id = categoryName.FirstOrDefault()).FirstOrDefault();
        //                                                if (Count > 0)
        //                                                {
        //                                                    //adding sub categories

        //                                                    foreach (string cat in subCategoriesList)
        //                                                    {
        //                                                        Category subs = new Category()
        //                                                        {
        //                                                            Name = cat.ToUpper(),
        //                                                            SubCategory = Count,
        //                                                            CreatedAt = DateTime.Now,
        //                                                            IsActive = 1,
        //                                                            UserId = validateUser().Id
        //                                                        };
        //                                                        new CategoriesBL().AddCategory(subs);
        //                                                    }
        //                                                }
        //                                                else
        //                                                {
        //                                                    Category sku = new Category()
        //                                                    {
        //                                                        Name = categoryName.ToUpper(),
        //                                                        CreatedAt = DateTime.Now,
        //                                                        IsActive = 1,
        //                                                        UserId = validateUser().Id
        //                                                    };

        //                                                    new CategoriesBL().AddCategory(sku);
        //                                                    Category parentCategory = new CategoriesBL().GetActiveCategoryList().LastOrDefault();

        //                                                    //adding sub categories

        //                                                    foreach (string cat in subCategoriesList)
        //                                                    {
        //                                                        Category subs = new Category()
        //                                                        {
        //                                                            Name = cat.ToUpper(),
        //                                                            SubCategory = parentCategory.Id,
        //                                                            CreatedAt = DateTime.Now,
        //                                                            IsActive = 1,
        //                                                            UserId = validateUser().Id
        //                                                        };
        //                                                        new CategoriesBL().AddCategory(subs);
        //                                                        transaction.Commit();
        //                                                    }
        //                                                }
        //                                            }
        //                                        }

        //                                        if (flag)
        //                                        {
        //                                            //adding category

        //                                            Category skus = new Category()
        //                                            {
        //                                                Name = categoryName.ToUpper(),
        //                                                SubCategory = 0,
        //                                                CreatedAt = DateTime.Now,
        //                                                IsActive = 1,
        //                                                UserId = validateUser().Id
        //                                            };

        //                                            new CategoriesBL().AddCategory(skus);
        //                                            Category parentCategorys = new CategoriesBL().GetActiveCategoryList().LastOrDefault();

        //                                            //adding sub categories

        //                                            foreach (string cat in subCategoriesList)
        //                                            {
        //                                                Category subs = new Category()
        //                                                {
        //                                                    Name = cat.ToUpper(),
        //                                                    SubCategory = parentCategorys.Id,
        //                                                    CreatedAt = DateTime.Now,
        //                                                    IsActive = 1,
        //                                                    UserId = validateUser().Id
        //                                                };
        //                                                new CategoriesBL().AddCategory(subs);
        //                                                transaction.Commit();
        //                                            }
        //                                            copySubCategoriesList = subCategoriesList;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        catch (Exception)
        //                        {
        //                            transaction.Rollback();
        //                            return RedirectToAction("ShowAllSubCategories", "Category", new { type = "danger", errmsg = "Error! Your File is not in correct Format" });
        //                        }
        //                    }

        //                }
        //            }
        //            else
        //            {
        //                return RedirectToAction("ShowAllSubCategories", "Category", new { errmsg = "Please choose excel file!", type = "danger" });
        //            }
        //        }
        //        else
        //        {
        //            return RedirectToAction("ShowAllSubCategories", "Category", new { errmsg = "Invalid file format!", type = "danger" });
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("ShowAllSubCategories", "Category", new { errmsg = "Please choose excel file!", type = "danger" });
        //    }

        //    return RedirectToAction("ShowAllSubCategories", "Category", new { msg = "File is uploaded and read successfully!", type = "success" });
        //}

    }
}