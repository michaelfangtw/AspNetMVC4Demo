/*
 * Description:AbstractDB Class Handle common CRUD.
 * Modified History
 * Date         Author          Description
 * 2013.06.26   MichaelFang     1.0
 * 2014.10.11   MichaelFang     2.0
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Fun.Log;
using System.Reflection;

namespace Fun.DAO
{
    public abstract class AbstractDB<TConnection,TCommand,TDataReader,TParameter >:IDisposable
         //泛型可接受的的類別
        where TConnection : IDbConnection,new()  
        where TCommand : IDbCommand, new() //需提供無參數建構 
        where TDataReader : IDataReader
        where TParameter :IDataParameter
    {       

        public string ConnectionString { get; protected set; }
        public TConnection Connection { get; protected set; }
        public TDataReader Reader { get; private set; }
        public TCommand Command { get; private set; }
        public string LastError { get; protected set; }

        public CommandBuilder<TCommand, TParameter>  commandBuilder=new CommandBuilder<TCommand, TParameter>();
     
        //單一連線
        void AbstractDatabase(string connectionString)
        {
             Open(new string[] { connectionString });
        }
        //多資料庫連線
        void  AbstractDatabase(string[] connectionString)
        {
             Open(connectionString);
        }
    
        //開啟連線
        /// <summary>
        /// Open
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public void  Open(string[] connectionString){
            foreach(var connstr in connectionString){
                try{
                    Connection=new TConnection();
                    Connection.ConnectionString=connstr;
                    Connection.Open();
                    if (Connection.State==ConnectionState.Open)
                        break;      
                }
                catch(Exception ex){
                    LastError = ex.Message.ToString();
                    Logger.WriteLog(Logger.logtype.Error, ex.ToString());
                }
            }            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public void Open(string connectionString)
        {
            Open(new string[] { connectionString });           
        }

        /// <summary>
        /// Open
        /// </summary>
          public void Open()
        {
            if ((Reader != null) && (!Reader.IsClosed)) Reader.Close();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.ConnectionString = ConnectionString;
                try
                {
                    Connection.Open();

                }
                catch (Exception ex)
                {
                    LastError = ex.Message.ToString();
                    Logger.WriteLog(Logger.logtype.Error, ex.ToString());
                }
               
            }
        }
          /// <summary>
          /// Close
          /// </summary>
          public void Close()
          {
              if ((Reader != null) && (!Reader.IsClosed)) Reader.Close();
              if ((Connection != null) && (Connection.State != ConnectionState.Closed)) Connection.Close();
          }

          /// <summary>
          /// Dispose
          /// </summary>
          public void Dispose()
          {
              Dispose(true);
              GC.SuppressFinalize(this);
          }

          protected virtual void Dispose(bool disposing)
          {
              if (disposing)
              {
                  Close();
                  if (Command != null) Command.Dispose();
                  if (Reader != null) Reader.Dispose();
                  if (Connection != null) Connection.Dispose();
              }
          }

          public int ExecuteNonQuery(TCommand cmd)
          {
              cmd.Connection = Connection;       
              int result=0;
              try
              {
                  if (Connection.State != ConnectionState.Open) Connection.Open();
                  result = cmd.ExecuteNonQuery();
              }
              catch (Exception ex)
              {
                  LastError = ex.Message.ToString();
                  Logger.WriteLog(Logger.logtype.Error, ex.ToString());
                  result= -1;
              }
              finally
              {
                  cmd.Dispose();
                  Connection.Close();
              }
              return result;
          }

          public object ExecuteScalar(TCommand cmd)
          {
              
              cmd.Connection = Connection;
              object result ;
              try
              {
                  if (Connection.State != ConnectionState.Open) Connection.Open();
                  result = cmd.ExecuteScalar();
              }
              catch (Exception ex)
              {
                  LastError = ex.Message.ToString();
                  Logger.WriteLog(Logger.logtype.Error, ex.ToString());
                  result = null;
              }
              finally
              {
                  cmd.Dispose();
                  Connection.Close();
              }
              return result;
          }

          public TDataReader ExecuteReader(TCommand cmd)
          {
              if (Connection.State != ConnectionState.Open) Connection.Open();
              cmd.Connection = Connection;
              TDataReader result;
              try
              {
                  if (Connection.State != ConnectionState.Open) Connection.Open();
                  result = (TDataReader)cmd.ExecuteReader();
              }
              catch (Exception ex)
              {
                  LastError = ex.Message.ToString();
                  Logger.WriteLog(Logger.logtype.Error, ex.ToString());
                  result = default(TDataReader);
               }
              finally
              {
                  cmd.Dispose();
                  Connection.Close();
              }
              return result;
          }

           //CRUD
          //Active Record

         /// <summary>
         /// 
         /// </summary>
         /// <param name="tableName"></param>
         /// <param name="keyValues"></param>
         /// <returns></returns>
          public int Insert(string tableName, List<TParameter> keyValues)
          {  
              string sql=""; 
              TCommand cmd = new TCommand();
              sql=string.Format("insert into {0} {1} ",tableName, commandBuilder.CreateInsertValue(ref cmd, keyValues));
              cmd.CommandText = sql;
              cmd.Connection = Connection;
              try
              {
                  if (Connection.State != ConnectionState.Open) Connection.Open();
                  return cmd.ExecuteNonQuery();
              }
              catch (Exception ex)
              {
                  LastError = ex.Message.ToString();
                  Logger.WriteLog(Logger.logtype.Error, ex.ToString());
                  return -1;
              }
              finally
              {
                  cmd.Dispose();
                  Connection.Close();
              }
          }

          /// <summary>
          /// 
          /// </summary>
          /// <param name="tableName"></param>
          /// <param name="keyValues"></param>
          /// <param name="whereClause"></param>
          /// <returns></returns>
          public int Update(string tableName, List<TParameter> keyValues, List<TParameter> whereClause)
          {
              string sql = "";
              TCommand cmd = new TCommand();
              sql = string.Format("update  {0} set {1} {2} ", tableName,commandBuilder.CreateUpdateValue(ref cmd, keyValues),commandBuilder.CreateWhereClause(ref cmd, whereClause));
              cmd.CommandText = sql;
              cmd.Connection = Connection;
              try
              {
                  if (Connection.State != ConnectionState.Open) Connection.Open();
                  return cmd.ExecuteNonQuery();
              }
              catch (Exception ex)
              {
                  LastError = ex.Message.ToString();
                  Logger.WriteLog(Logger.logtype.Error, ex.ToString());
                  return -1;
              }
              finally
              {
                  cmd.Dispose();
                  Connection.Close();
              }

          }
          /// <summary>
          /// 
          /// </summary>
          /// <param name="tableName"></param>
          /// <param name="whereClause"></param>
          /// <returns></returns>
          public int Delete(string tableName, List<TParameter> whereClause)
          {
              string sql;
              TCommand cmd = new TCommand();
              sql = string.Format("delete from {0} {1}", tableName,commandBuilder.CreateWhereClause(ref cmd, whereClause));
              cmd.CommandText = sql;
              cmd.Connection = Connection;
              try
              {
                  if (Connection.State != ConnectionState.Open) Connection.Open();
                  return cmd.ExecuteNonQuery();
              }
              catch (Exception ex)
              {

                  LastError = ex.Message.ToString();
                  Logger.WriteLog(Logger.logtype.Error, ex.ToString());
                  return -1;
              }
              finally
              {
                  cmd.Dispose();
                  Connection.Close();
              }
          }

          /// <summary>
          /// 
          /// </summary>
          /// <param name="cmd"></param>
          /// <returns></returns>
          protected DataTable Select(TCommand cmd)
          {
              cmd.Connection = Connection;
              DataTable dt = new DataTable();
              try
              {
                  if (Connection.State != ConnectionState.Open) Connection.Open();
                  IDataReader dr = cmd.ExecuteReader();
                  dt.Load(dr);
                  return dt;
              }catch(Exception ex){
                  LastError = ex.Message.ToString();
                  Logger.WriteLog(Logger.logtype.Error, ex.ToString());
                  return dt;
              }
              finally
              {
                  cmd.Dispose();
                  Connection.Close();
              }
          }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fields"></param>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
          public DataTable Select(string tableName, string Columns, List<TParameter> whereClause, string orderBy)
          {
              string sql;
              TCommand cmd = new TCommand();
              sql = string.Format(" select  {0} ", Columns);
              sql += string.Format(" from {0} ", tableName);
              sql += commandBuilder.CreateWhereClause(ref cmd, whereClause);
              if (!string.IsNullOrEmpty(orderBy.Trim()))
                  sql += string.Format(" order by  {0}", orderBy);
          
              cmd.CommandText = sql;
              cmd.Connection = Connection;
              DataTable dt = new DataTable();
              try
              {
                  if (Connection.State != ConnectionState.Open) Connection.Open();
                  IDataReader dr = cmd.ExecuteReader();                
                  dt.Load(dr);
                  return dt;
              }
              catch (Exception ex)
              {                 
                  LastError = ex.Message.ToString();
                  Logger.WriteLog(Logger.logtype.Error, ex.ToString());
                  return null;
              }
              finally
              {
                  cmd.Dispose();
                  Connection.Close();
              }
             
          }
          /// <summary>
          /// 
          /// </summary>
          /// <param name="tableName"></param>
          /// <param name="whereClause"></param>
          /// <returns></returns>
          public int SelectCount(string tableName, List<TParameter> whereClause)
          {
              string sql;
              int recordCount;
              TCommand cmd = new TCommand();
              sql = "select  count(*) as c ";
              sql += string.Format(" from {0} ", tableName);
              sql += commandBuilder.CreateWhereClause(ref cmd, whereClause);              
              cmd.CommandText = sql;
              cmd.Connection = Connection;
              DataTable dt = new DataTable();
              try
              {
                  if (Connection.State != ConnectionState.Open) Connection.Open();
                  recordCount  = (int)cmd.ExecuteScalar();
                  return recordCount;
              }
              catch (Exception ex)
              {
                  LastError = ex.Message.ToString();
                  Logger.WriteLog(Logger.logtype.Error, ex.ToString());
                  return 0;
              }
              finally
              {
                  cmd.Dispose();
                  Connection.Close();
              }

          }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columns"></param>
        /// <param name="whereClause"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
          public abstract DataTable Select(string tableName, string columns, List<TParameter> whereClause, string orderBy, int pageSize, int pageNumber);          
    }
}