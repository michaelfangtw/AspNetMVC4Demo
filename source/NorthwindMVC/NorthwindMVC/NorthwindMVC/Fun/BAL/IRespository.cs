using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fun.BAL
{
    public interface IRespository<T>
    {
        List<T> Select(string sortField, string sortOrder, int pageNumber, int pageSize, ref int recordCount);
        T Select(int PrimaryKey);
        bool Insert(T category);
        bool Update(T category);
        bool Delete(int PrimaryKey);
        string LastError { get; set; }
       
    }
}