using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Fun.DAO
{
    public class CommandBuilder<TCommand, TParameter>
         where TCommand : IDbCommand, new() //需提供無參數建構 
         where TParameter : IDataParameter
    {

        bool checkExpression(string paramName)
        {
            paramName = paramName.ToLower();
            bool result=false;
            string[] keyword={"=",">","<","in"};
            foreach (var str in keyword)
            {
                if (paramName.IndexOf(str)>=0)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public string CreateWhereClause(ref TCommand cmd,List<TParameter> where)
        {
            List<string> whereList = new List<string>();
            int count = 0;
            string sql = "";
            if (where != null)
            {
                foreach (var param in where)
                {
                    if (checkExpression(param.ParameterName) && (string.IsNullOrEmpty(param.Value.ToString())))
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
            return sql;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public string CreateUpdateValue(ref TCommand cmd, List<TParameter> keyValues)
        {
            int count = 0;
            List<string> valuesList = new List<string>();
            string sql="";
            foreach (var param in keyValues)
            {
                if ((param.ParameterName.IndexOf("=") >= 0) && (string.IsNullOrEmpty(param.Value.ToString())))
                {
                    valuesList.Add(string.Format("{0}", param.ParameterName));
                }
                else
                {
                    string parameterName = string.Format("column_{0}", param.ParameterName);
                    valuesList.Add(string.Format("{0}=@{1}", param.ParameterName, parameterName));
                    param.ParameterName = parameterName;
                    cmd.Parameters.Add(param);
                }
                count++;
            }
            sql += string.Join(",", valuesList);
            return sql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public string CreateInsertValue(ref TCommand cmd, List<TParameter> keyValues)
        {
            string sql = "";
            sql += "(";
            List<string> keyList = new List<string>();
            foreach (var param in keyValues)
            {
                keyList.Add(string.Format("{0}", param.ParameterName));
            }
            sql += string.Join(" , ", keyList);

            sql += ") values (";
            List<string> valueList = new List<string>();
            foreach (var param in keyValues)
            {
                valueList.Add(string.Format("@{0}", param.ParameterName));
                cmd.Parameters.Add(param);
            }
            sql += string.Join(" , ", valueList);
            sql += ")";
            return sql;
        }
    }
}