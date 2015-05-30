using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Data;
using Fun.DAL;
using Fun.Helper;

namespace Fun.BAL
{
    public class CategoryBAL: IRespository<Category>
    {
        public string LastError { get; set; }
        private CategoryDAL categoryDAL = new CategoryDAL();

        public List<Category> Select(string sortField, string sortOrder,int pageNumber,int pageSize,ref int recordCount)
        {
             List<Category> categoryList= categoryDAL.Select(sortField, sortOrder,pageNumber,pageSize,ref recordCount);
             this.LastError = categoryDAL.LastError;
             return categoryList;           
        }

        public Category Select(int categoryID)
        {
            Category category=categoryDAL.Select(categoryID);
            this.LastError = categoryDAL.LastError;
            return category;
        }

        public bool Insert(Category category)
        {
            bool result=categoryDAL.Insert(category);
            this.LastError = categoryDAL.LastError;
            return result;
        }

        public bool Update(Category category)
        {
            bool result = categoryDAL.Update(category);
            this.LastError = categoryDAL.LastError;
            return result;
        }

        public bool Delete(int categoryID)
        {
            bool result = categoryDAL.Delete(categoryID);
            this.LastError = categoryDAL.LastError;
            return result;
        }
    }
}