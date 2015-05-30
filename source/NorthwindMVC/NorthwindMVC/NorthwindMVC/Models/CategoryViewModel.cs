using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fun.BAL;
using Fun.Helper;
namespace Northwind.Models
{
    public class CategoryViewModel
    {
        public Pager Pager { get; set; }
        public List<Category> CategoryList { get; set; }

        public string Message { get; set; }
        public Category Category { get; set; }    
    }
}