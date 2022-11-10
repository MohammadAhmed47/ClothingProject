using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clothing.HelpingClasses
{
    public class CategoryDTOList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SubCategory { get; set; }
        public string catPath { get; set; }
        public string UserID { get; set; }
        
    }
}