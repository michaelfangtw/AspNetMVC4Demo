using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Data;
using Fun.BAL;
using Fun.DAO;
using Fun.Helper;

namespace Fun.DAL
{
    public class CategoryDAL : IRespository<Category>
    {

        public string NorthWindDB{ 
            get
            {
                return ConfigurationManager.ConnectionStrings["NorthWind"].ConnectionString;
            } 
        }
        public string LastError { get; set; }
       


        public List<Category> Select(string sortField, string sortOrder,int pageNumber,int pageSize,ref int recordCount)
        {
            List<Category> categoryList;
            using (SqlDB db = new SqlDB(NorthWindDB))
            {
                string orderBy= string.IsNullOrEmpty(sortField)?"":string.Format("{0} {1}", sortField, sortOrder);
                List<SqlParameter> whereList = new List<SqlParameter>() { new SqlParameter("CategoryID>0", "") };
                if (orderBy == "") orderBy = "CategoryID";
                DataTable dt = db.Select("Categories", "*", whereList, orderBy, pageNumber, pageSize);
                recordCount = db.SelectCount("Categories", whereList);                     
                categoryList = ObjectHelper.DataTableToList<Category>(dt);
            }
            return (categoryList != null) ? categoryList.ToList() : null;           
        }

        public Category Select(int categoryID)
        {
            List<Category> categoryList;
            using (SqlDB db = new SqlDB(NorthWindDB))
            {
                List<SqlParameter> paramList = new List<SqlParameter>();
                paramList.Add(new SqlParameter("CategoryID", categoryID));
                DataTable dt = db.Select("Categories", "*", paramList, "CategoryID");
                categoryList = ObjectHelper.DataTableToList<Category>(dt);
            }
            Category category=categoryList.First();
            return category;
        }

        public bool Insert(Category category)
        {
            int ra = 0;
            using (SqlDB db = new SqlDB(NorthWindDB))
            {
                List<SqlParameter> keyValues = new List<SqlParameter>();
                keyValues.Add(new SqlParameter("CategoryName", category.CategoryName));
                keyValues.Add(new SqlParameter("Description", category.Description));
   
                ra = db.Insert("Categories", keyValues);
                if (ra < 0) LastError = db.LastError;
            }
            return (ra > 0);

        }

        public bool Update(Category category)
        {
            int ra = 0;
            using (SqlDB db = new SqlDB(NorthWindDB))
            {
                List<SqlParameter> keyValues = new List<SqlParameter>();
                keyValues.Add(new SqlParameter("CategoryName", category.CategoryName));
                keyValues.Add(new SqlParameter("Description", category.Description));
                List<SqlParameter> whereClause = new List<SqlParameter>();
                whereClause.Add(new SqlParameter("CategoryID", category.CategoryID));

                ra = db.Update("Categories", keyValues, whereClause);
                if (ra<0) LastError=db.LastError;
            }
            return (ra > 0);
        }

        public bool Delete(int categoryID)
        {
            int ra = 0;
            using (SqlDB db = new SqlDB(NorthWindDB))
            {               
                List<SqlParameter> whereClause = new List<SqlParameter>();
                whereClause.Add(new SqlParameter("CategoryID", categoryID));
                ra = db.Delete("Categories",  whereClause);
                if (ra <= 0) LastError = db.LastError;
            }
            return (ra > 0);
        }
    }
}