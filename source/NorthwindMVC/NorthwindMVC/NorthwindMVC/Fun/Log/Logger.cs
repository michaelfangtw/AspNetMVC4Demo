using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
namespace Fun.Log
{
    /// <summary>
    /// Logger 的摘要描述
    /// </summary>
    public class Logger
    {
        static log4net.ILog log_info;
        static log4net.ILog log_error;

        public enum logtype
        {
            Error, //logger_error
            //以下均寫到logger
            Info,
            Debug,
            Fatal,
            Warn
        }
        public static void WriteLog(logtype type, string msg)
        {
            string ConfigPath = string.Format(HttpContext.Current.Server.MapPath("~\\Log4Net.config"));
            FileInfo f = new System.IO.FileInfo(ConfigPath);
            if (!f.Exists)
            {
                throw new Exception("WriteLog,Log4Net.config not found!");
            }
            log4net.Config.XmlConfigurator.Configure(f);
            switch (type)
            {
                case logtype.Error:
                    log_error = log4net.LogManager.GetLogger("logger_error");
                    log_error.Error(msg);
                    break;
                case logtype.Debug:
                    log_info = log4net.LogManager.GetLogger("logger");
                    log_info.Debug(msg);
                    break;
                default:
                    log_info = log4net.LogManager.GetLogger("logger");
                    log_info.Info(msg);
                    break;
            }
            GC.Collect();
        }

    }
}