using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Northwind.Fun.DAL
{

    public class SelectCommandBuilder<TCommand, TParameter>
        where TCommand : IDbCommand, new() //需提供無參數建構 
        where TParameter : IDataParameter
    {
        private string _select;
        private string _from;
        private string _where;
        private string _orderBy;
        private TCommand cmd;

        public SelectCommandBuilder()
        {
            cmd = new TCommand();
        }
       
        public TCommand CreateCommand()
        {
            TCommand cmd = new TCommand();
            string sql = string.Format("{0} {1} {2} {3}", _select , _from, _where, _orderBy);
            cmd.CommandText = sql;
            return cmd;
        }

        void Select(string column)
        {
            _select = column;
        }
        void From(string tableName){
            _from= string.Format("from {0}");
        }
       
        void where(List<TParameter> where)
        {
            List<string> whereList = new List<string>();
            int count = 0;
            string sql = "";
            if (where != null)
            {
                foreach (var param in where)
                {
                    if ((param.ParameterName.IndexOf("=") >= 0) && (string.IsNullOrEmpty(param.Value.ToString())))
                    {
                        whereList.Add(string.Format("{0}", param.ParameterName));
                    }
                    else
                    {
                        string parameterName = string.Format("where_{0}", param.ParameterName);
                        whereList.Add(string.Format("{0}=@{1}", param.ParameterName, parameterName));
                        param.ParameterName = parameterName;
                        cmd.Parameters.Add(param);
                    }
                    count++;
                }
                sql += string.Format(" where {0}", string.Join(" and", whereList));
            }
            _where= sql;
        }
    }
}