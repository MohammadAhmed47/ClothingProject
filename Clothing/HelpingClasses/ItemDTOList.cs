using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clothing.HelpingClasses
{
    public class ItemDTOList
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string Fit { get; set; }
        public string Material { get; set; }
        public string Brand { get; set; }
        public string Notes { get; set; }
        public string Careinstructions { get; set; }
        public string StoreItemNumber { get; set; }
        public string DatePurchased { get; set; }
        public double? PurchasePrice { get; set; }
        public string NotesII { get; set; }
        public string QRbarcode { get; set; }
        public string User_Id { get; set; }
        public string CategoriesId { get; set; }
        public string LocationId { get; set; }
        public string Createdat { get; set; }
        public string Image { get; set; }
        public string SubCategory { get; set; }
        public int? ItemId { get; set; }
    }
}