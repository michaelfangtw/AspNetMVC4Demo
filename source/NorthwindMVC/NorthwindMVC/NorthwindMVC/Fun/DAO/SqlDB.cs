using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Fun.Log;
using System.Configuration;

namespace Fun.DAO
{
    class SqlDB : AbstractDB<SqlConnection, SqlCommand, SqlDataReader, SqlParameter>
    {
        public SqlDB()
        {
            if (!string.IsNullOrEmpty(ConnectionString))
            {
                Open(ConnectionString);
            }
           
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connstr"></param>
        public SqlDB(string connstr)
        {
            Open(connstr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="Columns"></param>
        /// <param name="whereClause"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        override
        public DataTable Select(string tableName, string Columns, List<SqlParameter> whereClause, string orderBy,int pageNumber,int pageSize)
        {
            int firstRow = 0;
            int lastRow = 0;
            firstRow = (pageNumber-1) * pageSize + 1;
            lastRow = (pageNumber-1) * pageSize + pageSize;
            DataTable dt = new DataTable();        
            SqlCommand cmd = new SqlCommand();
            string sql = "";
            sql += " with cte  \r\n";
            sql += " as \r\n";
            sql += " ( \r\n";
            sql += string.Format("    select ROW_NUMBER() OVER (ORDER BY {0} )  as rownum, \r\n",orderBy);
            sql += string.Format("     {0}  \r\n", Columns);
            sql += string.Format("    from  {0} \r\n", tableName);
            sql += commandBuilder.CreateWhereClause(ref cmd, whereClause);
            sql += "     )  \r\n";
            sql += "     select * from cte \r\n";
            sql += string.Format("     where rownum between {0} and {1} \r\n", firstRow, lastRow);
            sql += string.Format("     order by {0}  \r\n", orderBy);
            cmd.CommandText = sql;         
         
            try
            {
                dt = Select(cmd);              
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.logtype.Error, ex.ToString());
            }           
            return dt;
        }
    }
}
