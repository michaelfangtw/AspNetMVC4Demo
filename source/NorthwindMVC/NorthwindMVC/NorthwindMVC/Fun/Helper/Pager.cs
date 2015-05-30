using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fun.Helper
{
    public class Pager
    {
        public int PageSize { get; set; }
        public int RecordCount { get; set; }
        public int PageCount
        {
            get
            {
                return (int) Math.Ceiling((double)RecordCount/PageSize);
            }            
         }
        public List<SelectListItem> PageList
        {
            get 
            {
                    List<SelectListItem> items=new List<SelectListItem>();
                    for (int i = 1; i <= PageCount; i++)
                    {
                        items.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = (PageNumber == i) });
                    }
                    return items;   
             }
        }
        public int PageNumber { get; set; }
        public string SortField { get; set; }
        public string SortOrder { get; set; }
        
    }
}