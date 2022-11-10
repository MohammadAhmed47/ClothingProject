using Clothing.BL;
using Clothing.Models;
using QRCoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Clothing.HelpingClasses;
using System.Drawing;

namespace Clothing.Controllers
{
    public class ItemController : Controller
    {
        // GET: Item
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

        public ActionResult AddItem(string msg = "", string errmsg = "")
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            List<Category> c = new CategoriesBL().GetActiveCategoryList().Where(x => x.IsActive == 1 && x.SubCategory != 0).ToList();
            ViewBag.categories = c;

            List<Location> l = new LocationBL().GetActiveLocationList().Where(x => x.IsActive == 1 && x.UserID == validateUser().Id).OrderBy(x => x.CreatedAt).ToList();
            ViewBag.location = l;

            ViewBag.msgs = msg;
            ViewBag.ermsg = errmsg;
            return View();
        }

        public ActionResult PostAddItem(Item item, HttpPostedFileBase IdentityDocument1)
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            
            Item _item = new Item()
            {
                ItemName = item.ItemName,
                Brand = item.Brand,
                Careinstructions = item.Careinstructions,
                Color = item.Color,
                //DatePurchased = item.DatePurchased,
                Fit = item.Fit,
                Material = item.Material,
                Notes = item.Notes,
                //NotesII = item.NotesII,
                //PurchasePrice = item.PurchasePrice,
                Size = item.Size,
                StoreItemNumber = item.StoreItemNumber,
                IsActive = 1,
                CreatedAt = DateTime.Now,
                //User_Id = validateUser().Id,
                CategoriesId = item.CategoriesId,
                //LocationId = item.LocationId,
               Userid = validateUser().Id
            };

            if (IdentityDocument1 != null)
            {
                string filenamenoext = System.IO.Path.GetFileNameWithoutExtension(IdentityDocument1.FileName);
                string ext = System.IO.Path.GetExtension(System.IO.Path.GetFileName(IdentityDocument1.FileName));
                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".JPG" || ext == ".JPEG" || ext == ".PNG")
                {
                    filenamenoext = "_" + DateTime.Now.ToString("yymmddfff") + "_" + IdentityDocument1.FileName;
                    string paths = Path.Combine(Server.MapPath("~/Content/UploadImage"), filenamenoext);
                    // file is uploaded
                    IdentityDocument1.SaveAs(paths);
                    _item.Image = "/Content/UploadImage/" + filenamenoext;
                }
                else
                {
                    ViewBag.wrongFile = "Wrong file format!";
                    return View("AddUser", item);
                }
            }

            if (new ItemBL().AddItem(_item))
            {
                return RedirectToAction("ShowAllItems", "Item", new { msg = "Item Added SuccessFully" });
            }
            else
            {
                return RedirectToAction("ShowAllItems", "Item", new { errmsg = "Item Can't be Added" });
            }
        }

        public ActionResult ShowAllItems(string msg = "", string errmsg = "")
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            //List<Info> ls = new List<Info>();
            List<Item> ls = new List<Item>();
            List<Location> l = new List<Location>();
            if (validateUser().Role == 1)
            {
                ls = new ItemBL().GetActiveItemList().Where(x => x.IsActive == 1).OrderBy(x => x.CreatedAt).ToList();
                l = new LocationBL().GetActiveLocationList().Where(x => x.IsActive == 1).OrderBy(x => x.CreatedAt).ToList();
            }
            else
            {
                //ls = new InfoBL().GetActiveInfoList().Where(x => x.User_Id == validateUser().Id && x.IsActive == 1).OrderBy(x => x.CreatedAt).ToList();
                ls = new ItemBL().GetActiveItemList().Where(x => x.IsActive == 1 && x.Userid == validateUser().Id).OrderBy(x => x.CreatedAt).ToList();
                l = new LocationBL().GetActiveLocationList().Where(x => x.IsActive == 1 && x.UserID == validateUser().Id).OrderBy(x => x.CreatedAt).ToList();
            }
            ViewBag.Item = ls;
            ViewBag.location = l;
            ViewBag.msgs = msg;
            ViewBag.ermsg = errmsg;
            return View();
        }
        [HttpPost]
        public ActionResult AllItem(string msg = "")
        {
            if (validateUser() != null)
            {
                List<Item> _info;
                //List<Info> _info;
                List<Location> l = new List<Location>();
                if (validateUser().Role == 1)
                {
                    _info = new ItemBL().GetActiveItemList().Where(x => x.IsActive == 1).OrderBy(x => x.CreatedAt).ToList();
                    //_info = new InfoBL().GetActiveInfoList().Where(x => x.IsActive == 1).OrderBy(x => x.CreatedAt).ToList();
                    l = new LocationBL().GetActiveLocationList().Where(x => x.IsActive == 1).OrderBy(x => x.CreatedAt).ToList();
                }
                else
                {
                    _info = new ItemBL().GetActiveItemList().Where(x => x.IsActive == 1 && x.Userid == validateUser().Id).OrderBy(x => x.CreatedAt).ToList();
                    //_info = new InfoBL().GetActiveInfoList().Where(x => x.IsActive == 1).OrderBy(x => x.CreatedAt).ToList();
                    l = new LocationBL().GetActiveLocationList().Where(x => x.IsActive == 1 && x.UserID == validateUser().Id).OrderBy(x => x.CreatedAt).ToList();
                }

                int start2 = Convert.ToInt32(Request["start"]);
                int length = Convert.ToInt32(Request["length"]);
                string searchValue = Request["search[value]"];
                string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
                string sortDirection = Request["order[0][dir]"];

                int totalrows = _info.Count();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    _info = _info.Where(x => x.ItemName.ToString().Contains(searchValue.ToLower()) || x.Category.Name.ToLower().Contains(searchValue.ToLower())).ToList();
                }
                int totalrowsafterfilterinig = _info.Count();

                List<ItemDTOList> ItemDTOLists = new List<ItemDTOList>();

                _info = _info.Skip(start2).Take(length).ToList();

                var i = 0;
                string str = "";
                foreach (Item x in _info)
                {
                    if (x.Image != null)
                    {
                        str = "<td> <a target='_blank' href=" + x.Image + ">Picture</a> </td>";
                    }
                    else
                    {
                        str = "No Image";
                    }

                    ItemDTOList ItemDTO = new ItemDTOList()
                    {
                        Id = x.Id,
                        ItemName = x.ItemName,
                        Brand = x.Brand,
                        Careinstructions = x.Careinstructions,
                        Color = x.Color,
                        Fit = x.Fit,
                        Material = x.Material,
                        Notes = x.Notes,
                        Size = x.Size,
                        StoreItemNumber = x.StoreItemNumber,
                        Image = str,
                        User_Id = x.User.Username
                    };
                    if (x.Category != null)
                    {
                        if (x.Category.IsActive != 0)
                        {
                            if (x.Category.SubCategory != 0)
                            {
                                var firstcategory = new CategoriesBL().GetCategorybyId(x.Category.SubCategory.GetValueOrDefault());
                                if(firstcategory.SubCategory != 0)
                                {
                                    var secondcategory = firstcategory.Name + "-" + x.Category.Name;

                                    var thirdcategory = new CategoriesBL().GetCategorybyId(firstcategory.SubCategory.GetValueOrDefault()).Name + " - " + secondcategory;
                                    ItemDTO.CategoriesId = thirdcategory;
                                }
                                else
                                {
                                    //ItemDTO.CategoriesId = x.Category.Name;
                                    ItemDTO.CategoriesId = new CategoriesBL().GetCategorybyId(x.Category.SubCategory.GetValueOrDefault()).Name + " - " + x.Category.Name;
                                }
                            }
                            else
                            {
                                ItemDTO.CategoriesId = x.Category.Name;
                            }
                        }
                        else
                        {
                            if (x.Category.SubCategory != 0)
                            {
                                ItemDTO.CategoriesId = new CategoriesBL().getCategoryByIdwithisActive(x.Category.SubCategory.GetValueOrDefault()).Name + " - <br>" + x.Category.Name;
                                ItemDTO.CategoriesId = ItemDTO.CategoriesId + " <br><br> <span style='background:#ce2029; color:#ffffff; padding:4px 4px; text-align:center'>This Category is no longer Exists</span>";
                            }
                            else
                            {
                                ItemDTO.CategoriesId = x.Category.Name;
                                ItemDTO.CategoriesId = ItemDTO.CategoriesId + " <br><br> <span style='background:#ce2029; color:#ffffff; padding:4px 4px; text-align:center'>This Category is no longer Exists</span>";
                            }
                        }
                    }
                    else
                    {
                        ItemDTO.CategoriesId = "N/A";
                    }

                    ItemDTOLists.Add(ItemDTO);
                }
                return Json(new { data = ItemDTOLists, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        public ActionResult PostDeleteItem(int Id = -1)
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            DatabaseEntities dc = new DatabaseEntities();
            Item item = new ItemBL().GetItembyId(Id, dc);
            if (item != null)
            {
                item.IsActive = 0;
                if (new ItemBL().UpdateItem(item, dc))
                {
                    return RedirectToAction("ShowAllItems", "Item", new { errmsg = "Item Deleted Successfully" });
                }
                else
                {
                    return RedirectToAction("ShowAllItems", "Item", new { errmsg = "Item Cant Be Deleted" });
                }
            }
            else
            {
                return RedirectToAction("ShowAllItems", "Item", new { errmsg = "Unable to Locate Item" });
            }
        }

        public ActionResult UpdateItem(int Id = -1, int IId = -1, string msg = "", string errmsg = "")
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            Item item = new ItemBL().GetItembyId(Id);
            //Info info = new InfoBL().GetInfobyId(Id);
            List<Category> c = new CategoriesBL().GetActiveCategoryList().Where(x => x.IsActive == 1 && x.SubCategory != 0).ToList();
            ViewBag.categories = c;

            List<Location> l = new LocationBL().GetActiveLocationList().Where(x => x.IsActive == 1 && x.UserID == validateUser().Id).OrderBy(x => x.CreatedAt).ToList();
            ViewBag.location = l;
            ViewBag.msgs = msg;
            ViewBag.ermsg = errmsg;
            return View(item);
        }

        public ActionResult PostUpdateItem(Info _info, Item _item, HttpPostedFileBase Document1)
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            DatabaseEntities db = new DatabaseEntities();


            Item item = new ItemBL().GetItembyId(_item.Id);

            int cid;

            if (_item.CategoriesId != null)
            {
                cid = (int)_item.CategoriesId;
            }
            else
            {
                cid = (int)item.CategoriesId;
            }

            Item Updateditem = new Item()
            {
                Id = item.Id,
                IsActive = item.IsActive,
                CreatedAt = item.CreatedAt,
                ItemName = _item.ItemName,
                Brand = _item.Brand,
                Color = _item.Color,
                Size = _item.Size,
                Fit = _item.Fit,
                Material = _item.Material,
                Notes = _item.Notes,
                Careinstructions = _item.Careinstructions,
                StoreItemNumber = _item.StoreItemNumber,
                CategoriesId = cid,
                Userid = item.Userid
            };

            if (Document1 != null)
            {
                string filenamenoext = System.IO.Path.GetFileNameWithoutExtension(Document1.FileName);
                string ext = System.IO.Path.GetExtension(System.IO.Path.GetFileName(Document1.FileName));
                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".JPG" || ext == ".JPEG" || ext == ".PNG")
                {
                    filenamenoext = "_" + DateTime.Now.ToString("yymmddfff") + "_" + Document1.FileName;
                    string path = Path.Combine(Server.MapPath("~/Content/UploadImage"), filenamenoext);
                    // file is uploaded
                    Document1.SaveAs(path);
                    Updateditem.Image = "/Content/UploadImage/" + filenamenoext;
                }
                else
                {
                    ViewBag.wrongFile = "Wrong file format!";
                    return View("UpdateProfile", item);
                }
            }
            else
            {
                Item items = new ItemBL().GetItembyId(item.Id);
                Updateditem.Image = items.Image;
            }

            if (new ItemBL().UpdateItem(Updateditem))
            {
                return RedirectToAction("ShowAllItems", "Item", new { msg = "Item Info Updated Successfull" });
            }
            else
            {
                return RedirectToAction("ShowAllItems", "Item", new { errmsg = "Error in Updating Item" });
            }
            ////////////////////////////////
            //DatabaseEntities de = new DatabaseEntities();
            //List<Info> lst = new InfoBL().GetActiveInfoList(de).Where(x => x.ItemId == item.Id).ToList();
            //int count = 0;
            //foreach(var x in lst)
            //{
            //    QRCodeGenerator ObjQr = new QRCodeGenerator();

            //    //string data = "Item Id: " + x.Id + "\n " +
            //    //    "Item Name: " + Updateditem.ItemName + "\n " +
            //    //    "Category: " + item.Category.Name + "\n" +
            //    //    "Color: " + Updateditem.Color + "\n" +
            //    //    "Brand: " + Updateditem.Brand + "\n" +
            //    //    "Careinstructions: " + Updateditem.Careinstructions + "\n" +
            //    //    "Location: " + x.Location.Name + "\n" +
            //    //    "Purchase Price: " + x.PurchasePrice + "\n" +
            //    //    "Notes:" + Updateditem.Notes + "\n" +
            //    //    "NotesII:" + x.NotesII + "\n" +
            //    //    "Date Purchased:" + x.DatePurchased + "\n" +
            //    //    "Fit:" + Updateditem.Fit + "\n" +
            //    //    //"Notes:" + i.Image + "\n" +
            //    //    "Material:" + Updateditem.Material + "\n" +
            //    //    "Size:" + Updateditem.Size + "\n" +
            //    //    "Username:" + x.User.Name;
            //    string data = "Item No: " + i.Id + " \n";

            //    QRCodeData qrCodeData = ObjQr.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);

            //    Bitmap bitMap = new QRCode(qrCodeData).GetGraphic(20);
            //    var imageurl = "";
            //    using (MemoryStream ms = new MemoryStream())

            //    {

            //        bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            //        byte[] byteImage = ms.ToArray();

            //        byte[] bytes = Convert.FromBase64String(Convert.ToBase64String(byteImage));
            //        imageurl = "data:image/png;base64," + Convert.ToBase64String(byteImage);

            //    }

            //    //updateditem.QRbarcode = imageurl;

            //    x.QRbarcode = imageurl;
            //    if(new InfoBL().UpdateInfo(x, de))
            //    {
            //        count++; 
            //    }
            //}
            //if (count == lst.Count)
            //{
            //    return RedirectToAction("ShowAllItems", "Item", new { msg = "Item Info Updated Successfull" });
            //}
            
            //return RedirectToAction("ShowAllItems", "Item", new { errmsg = "Error in Updating Item" });
        }

        public ActionResult UpdateAllocatedItems(int Id, string msg = "")
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            Info info = new InfoBL().GetInfobyId(Id);
            List<Category> c = new CategoriesBL().GetActiveCategoryList().Where(x => x.IsActive == 1 && x.SubCategory != 0).ToList();
            ViewBag.categories = c;

            List<Location> l = new LocationBL().GetActiveLocationList().Where(x => x.IsActive == 1 && x.UserID == validateUser().Id).OrderBy(x => x.CreatedAt).ToList();
            ViewBag.location = l;
            ViewBag.msg = msg;
            return View(info);
        }

        public ActionResult PostUpdateAllocatedItems(Info _item)
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            Info item = new InfoBL().GetInfobyId(_item.Id);


            int lid;

            if (_item.LocationId != null)
            {
                lid = (int)_item.LocationId;
            }
            else
            {
                lid = (int)item.LocationId;
            }


            Info updateditem = new Info()
            {
                DatePurchased = _item.DatePurchased,
                IsActive = item.IsActive,
                CreatedAt = item.CreatedAt,
                Id = item.Id,
                ItemId = item.ItemId,
                User_Id = item.User_Id,
                LocationId = lid,
                PurchasePrice = _item.PurchasePrice,
                NotesII = _item.NotesII,
            };

            QRCodeGenerator ObjQr = new QRCodeGenerator();
            string data = item.Id + "\n  ";
            //string data = "Item Id: " + item.Id + "\n  " +
            //    "Item Name: " + item.Item.ItemName + "\n " +
            //    "Category: " + item.Item.Category.Name + "\n " +
            //    "Color: " + item.Item.Color + "\n " +
            //    "Brand: " + item.Item.Brand + "\n " +
            //    "Careinstructions: " + item.Item.Careinstructions + "\n " +
            //    "Location: " + item.Location.Name + "\n " +
            //    "Purchase Price: " + item.PurchasePrice + "\n " +
            //    "Notes:" + item.Item.Notes + "\n " +
            //    "NotesII:" + item.NotesII + "\n " +
            //    "Date Purchased:" + item.DatePurchased + "\n " +
            //    "Fit:" + item.Item.Fit + "\n " +
            //    //"Notes:" + i.Image + "\n" +
            //    "Material:" + item.Item.Material + "\n " +
            //    "Size:" + item.Item.Size + "\n " +
            //    "Username:" + item.User.Name;

            QRCodeData qrCodeData = ObjQr.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);

            Bitmap bitMap = new QRCode(qrCodeData).GetGraphic(20);
            var imageurl = "";

            using (MemoryStream ms = new MemoryStream())
            {
                bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] byteImage = ms.ToArray();
                byte[] bytes = Convert.FromBase64String(Convert.ToBase64String(byteImage));
                imageurl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
            }

            updateditem.QRbarcode = imageurl;


            if (new InfoBL().UpdateInfo(updateditem) == true)
            {
                return RedirectToAction("AllLocatedItems", "Item", new { msg = "Item Successfully Updated" });
            }
            else
            {
                return RedirectToAction("AllLocatedItems", "Item", new { errmsg = "Item Can't be Updated" });
            }


        }

        public ActionResult AssignLocation(Info info, int sId = -1, string msg = "", string errmsg = "")
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            
            Info i = new Info() {
                LocationId = info.LocationId,
                DatePurchased = info.DatePurchased,
                PurchasePrice = info.PurchasePrice,
                NotesII = info.NotesII,
                User_Id = validateUser().Id,
                ItemId = sId,
                IsActive = 1,
                CreatedAt = DateTime.Now,
            };

            QRCodeGenerator ObjQr = new QRCodeGenerator();

            Location l = new LocationBL().GetLocationbyId((int)info.LocationId);
            Item it = new ItemBL().GetItembyId(sId);
            User u = new UserBL().GetUserbyId(validateUser().Id);

            //string data = "Item Id: " + i.Id + "\n " +
            //    "Item Name: " + it.ItemName + "\n " +
            //    "Category: " + it.Category.Name + "\n" +
            //    "Color: " + it.Color + "\n" +
            //    "Brand: " + it.Brand + "\n" +
            //    "Careinstructions: " + it.Careinstructions + "\n" +
            //    "Location: " + l.Name + "\n <br>" +
            //    "Purchase Price: " + info.PurchasePrice + "\n" +
            //    "Notes:" + it.Notes + "\n" +
            //    "NotesII:" + info.NotesII + "\n" +
            //    "Date Purchased:" + info.DatePurchased + "\n" +
            //    "Fit:" + it.Fit + "\n" +
            //    "Material:" + it.Material + "\n" +
            //    "Size:" + it.Size + "\n" +
            //    "Username:" + u.Name;

           

            if (new InfoBL().AddInfo(i))
            {
                DatabaseEntities de = new DatabaseEntities();
                string data = i.Id + " \n";

                QRCodeData qrCodeData = ObjQr.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);

                Bitmap bitMap = new QRCode(qrCodeData).GetGraphic(20);
                var imageurl = "";
                using (MemoryStream ms = new MemoryStream())

                {
                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] byteImage = ms.ToArray();
                    byte[] bytes = Convert.FromBase64String(Convert.ToBase64String(byteImage));
                    imageurl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                }

                i.QRbarcode = imageurl;
                bool check = new InfoBL().UpdateInfo(i, de) == true;
                return RedirectToAction("AllLocatedItems", "Item", new { msg = "Item Located SuccessFully"});
            }
            else
            {
                return RedirectToAction("AllocatedItems", "Item", new { errmsg = "Item Can't be Added"});
            }
        }

        public ActionResult AllLocatedItems(string msg = "", string errmsg = "")
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            List<Info> ls = new List<Info>();
            //List<Item> ls = new List<Item>();
            List<Location> l = new List<Location>();
            if (validateUser().Role == 1)
            {
                ls = new InfoBL().GetActiveInfoList().Where(x => x.IsActive == 1 && x.LocationId != null).OrderBy(x => x.CreatedAt).ToList();
                l = new LocationBL().GetActiveLocationList().Where(x => x.IsActive == 1).OrderBy(x => x.CreatedAt).ToList();
            }
            else
            {
                ls = new InfoBL().GetActiveInfoList().Where(x => x.User_Id == validateUser().Id && x.IsActive == 1 && x.LocationId != null).OrderBy(x => x.CreatedAt).ToList();
                l = new LocationBL().GetActiveLocationList().Where(x => x.IsActive == 1 && x.UserID == validateUser().Id).OrderBy(x => x.CreatedAt).ToList();
            }
            ViewBag.Item = ls;
            ViewBag.location = l;
            ViewBag.msgs = msg;
            ViewBag.ermsg = errmsg;
            return View();
        }
        [HttpPost]
        public ActionResult AllLocatedItem(string msg = "")
        {
            if (validateUser() != null)
            {
                List<Info> _item;
                List<Location> l = new List<Location>();
                if (validateUser().Role == 1)
                {
                    _item = new InfoBL().GetActiveInfoList().Where(x => x.IsActive == 1 && x.LocationId != null).OrderBy(x => x.CreatedAt).ToList();
                    l = new LocationBL().GetActiveLocationList().Where(x => x.IsActive == 1).OrderBy(x => x.CreatedAt).ToList();
                }
                else
                {
                    _item = new InfoBL().GetActiveInfoList().Where(x => x.User_Id == validateUser().Id && x.IsActive == 1 && x.LocationId != null).OrderBy(x => x.CreatedAt).ToList();
                    l = new LocationBL().GetActiveLocationList().Where(x => x.IsActive == 1 && x.UserID == validateUser().Id).OrderBy(x => x.CreatedAt).ToList();
                }
                ViewBag.location = l;
                int start2 = Convert.ToInt32(Request["start"]);
                int length = Convert.ToInt32(Request["length"]);
                string searchValue = Request["search[value]"];
                string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
                string sortDirection = Request["order[0][dir]"];

                int totalrows = _item.Count();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    _item = _item.Where(x => x.Item.ItemName.ToString().Contains(searchValue.ToLower()) || x.Item.Category.Name.ToLower().Contains(searchValue.ToLower()) || x.Location.Name.ToLower().Contains(searchValue.ToLower())).ToList();
                }
                int totalrowsafterfilterinig = _item.Count();

                List<ItemDTOList> ItemDTOLists = new List<ItemDTOList>();

                _item = _item.Skip(start2).Take(length).ToList();

                var i = 0;
                string str = "";
                var str1 = "";
                foreach (Info x in _item)
                {
                    if (x.Item.Image != null)
                    {
                        str = "<td> <a target='_blank' href=" + x.Item.Image + ">Picture</a> </td>";
                    }
                    else
                    {
                        str = "No Image";
                    }

                    if (x.QRbarcode != null)
                    {
                        str1 = "<td>  <img src="+ x.QRbarcode +" style='height:100px; width: 100px;' />" + "<br>" + x.Id + "</td>";
                    }
                    else
                    {
                        str1 = "No Image";
                    }

                    ItemDTOList ItemDTO = new ItemDTOList()
                    {
                        Id = x.Id,
                        //ItemName = x.Item.ItemName,
                        Brand = x.Item.Brand,
                        Careinstructions = x.Item.Careinstructions,
                        Color = x.Item.Color,
                        //DatePurchased = x.DatePurchased.Value.ToString("dd/MM/yyyy hh:mm tt"),
                        DatePurchased = x.DatePurchased.Value.ToString("dd/MM/yyyy"),
                        Fit = x.Item.Fit,
                        Material = x.Item.Material,
                        Notes = x.Item.Notes,
                        Size = x.Item.Size,
                        StoreItemNumber = x.Item.StoreItemNumber,
                        PurchasePrice = x.PurchasePrice,
                        NotesII = x.NotesII,
                        //CategoriesId = x.Category.Name,
                        //LocationId = x.Location.Name,
                        Image = str,
                        QRbarcode = str1,
                        User_Id = x.User.Username
                    };

                    if (x.Item.IsActive != 0)
                    {
                        ItemDTO.ItemName = x.Item.ItemName;
                    }
                    else
                    {
                        ItemDTO.ItemName = x.Item.ItemName + "<br><br><span style='background:#ce2029; color:#ffffff; padding:4px 4px; text-align:center'>This Item is no longer exists</span>";
                    }


                    if(x.LocationId != null)
                    {
                        if(x.Location.IsActive == 0)
                        {
                            ItemDTO.LocationId = x.Location.Name + "<br><br> <span style='background:#ce2029; color:#ffffff; padding:4px 4px;'>This Location is no Longer Exists.</span>";
                        }
                        else
                        {
                            ItemDTO.LocationId = x.Location.Name;
                        }
                    }

                    if (x.Item.Category != null)
                    {
                        if (x.Item.Category.IsActive != 0)
                        {
                            var firstcategory = new CategoriesBL().GetCategorybyId(x.Item.Category.SubCategory.GetValueOrDefault());
                            if (firstcategory.SubCategory != 0)
                            {
                                var secondcategory = firstcategory.Name + "-" + x.Item.Category.Name;

                                var thirdcategory = new CategoriesBL().GetCategorybyId(firstcategory.SubCategory.GetValueOrDefault()).Name + " - " + secondcategory;
                                ItemDTO.CategoriesId = thirdcategory;
                            }
                            else
                            {
                                //ItemDTO.CategoriesId = x.Category.Name;
                                ItemDTO.CategoriesId = new CategoriesBL().GetCategorybyId(x.Item.Category.SubCategory.GetValueOrDefault()).Name + " - " + x.Item.Category.Name;
                            }
                            //if (x.Item.Category.SubCategory != 0)
                            //{
                            //    ItemDTO.CategoriesId = new CategoriesBL().GetCategorybyId(x.Item.Category.SubCategory.GetValueOrDefault()).Name + " - " + x.Item.Category.Name;
                            //}
                            //else
                            //{
                            //    ItemDTO.CategoriesId = x.Item.Category.Name;
                            //}
                        }
                        else
                        {
                            if (x.Item.Category.SubCategory != 0)
                            {
                                ItemDTO.CategoriesId = new CategoriesBL().getCategoryByIdwithisActive(x.Item.Category.SubCategory.GetValueOrDefault()).Name + " - <br>" + x.Item.Category.Name;
                                ItemDTO.CategoriesId =  ItemDTO.CategoriesId + " <br><br> <span style='background:#ce2029; color:#ffffff; padding:4px 4px; text-align:center'>This Category or Subcategory is no longer Exists</span>";
                            }
                            else
                            {
                                ItemDTO.CategoriesId = x.Item.Category.Name;
                                ItemDTO.CategoriesId = ItemDTO.CategoriesId + " <br><br> <span style='background:#ce2029; color:#ffffff; padding:4px 4px; text-align:center'>This Category or Subcategory is no longer Exists</span>";
                            }
                        }
                    }
                    else
                    {
                        ItemDTO.CategoriesId = "N/A";
                    }

                    ItemDTOLists.Add(ItemDTO);
                }
                return Json(new { data = ItemDTOLists, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        public ActionResult QRCodeItem(int Id = -1)
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            //Item i = new ItemBL().GetItembyId(Id);
            //ViewBag.item = i;
            return View();
        }

        public ActionResult UpdateLocation(Info info, int sId = -1, string msg = "", string errmsg = "")
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            DatabaseEntities de = new DatabaseEntities();
            Info i = new InfoBL().GetInfobyId((int)sId, de);

            i.LocationId = info.LocationId;


            //QRCodeGenerator ObjQr = new QRCodeGenerator();

            //Location l = new LocationBL().GetLocationbyId((int)info.LocationId);

            //string data = "Item Id: " + i.Id + "\n " +
            //    "Item Name: " + i.Item.ItemName + "\n  " +
            //    "Category: " + i.Item.Category.Name + "\n " +
            //    "Color: " + i.Item.Color + "\n" +
            //    "Brand: " + i.Item.Brand + "\n" +
            //    "Careinstructions: " + i.Item.Careinstructions + "\n" +
            //    "Location: " + l.Name + "\n" +
            //    "Purchase Price: " + i.PurchasePrice + "\n" +
            //    "Notes:" + i.Item.Notes + "\n" +
            //    "NotesII:" + i.NotesII + "\n" +
            //    "Date Purchased:" + i.DatePurchased + "\n" +
            //    "Fit:" + i.Item.Fit + "\n" +
            //    "Material:" + i.Item.Material + "\n" +
            //    "Size:" + i.Item.Size + "\n" +
            //    "Username:" + i.User.Name;

            //string data = "Item No: " + i.Id + " \n";

            //QRCodeData qrCodeData = ObjQr.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);

            //Bitmap bitMap = new QRCode(qrCodeData).GetGraphic(20);
            //var imageurl = "";
            //using (MemoryStream ms = new MemoryStream())

            //{

            //    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            //    byte[] byteImage = ms.ToArray();

            //    byte[] bytes = Convert.FromBase64String(Convert.ToBase64String(byteImage));
            //    imageurl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
            //}
            //i.QRbarcode = imageurl;


            if (new InfoBL().UpdateInfo(i, de))
            {
                return RedirectToAction("AllLocatedItems", "Item", new { msg = "Item Located SuccessFully" });
            }
            else
            {
                return RedirectToAction("AllocatedItems", "Item", new { errmsg = "Item Can't be Added" });
            }
        }

        public ActionResult PostDeleteAllocated(int Id = -1)
        {
            if (validateUser() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            DatabaseEntities dc = new DatabaseEntities();
            Info info = new InfoBL().GetInfobyId(Id, dc);
            if (info != null)
            {
                info.IsActive = 0;
                if (new InfoBL().UpdateInfo(info, dc))
                {
                    return RedirectToAction("AllLocatedItems", "Item", new { errmsg = "Item Deleted Successfully" });
                }
                else
                {
                    return RedirectToAction("AllLocatedItems", "Item", new { errmsg = "Item Cant Be Deleted" });
                }
            }
            else
            {
                return RedirectToAction("AllLocatedItems", "Item", new { errmsg = "Unable to Locate Item" });
            }
        }
    }
}