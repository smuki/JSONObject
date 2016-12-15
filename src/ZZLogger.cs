using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace Volte.Data.Json
{

    public class ZZLogger {
        const string ZFILE_NAME = "ZZLogger";

        private readonly StringBuilder sb = new StringBuilder();
        private static ZZLogger _Logger;
        private static Queue<JSONObject> _que;
        private static object _PENDING       = new object();
        public static bool DebugEnabled      = true;
        public static bool SQLEnabled        = true;
        public static bool MethodInfoEnabled = false;

        private bool _Running                = false;
        private Thread _worker;
        private string _loggerType;

        private string rootPath  = "";
        private string separator = "";

        public static ZZLogger getInstance()
        {
            if (_Logger == null) {
                _que = new Queue<JSONObject>();

                _Logger = new ZZLogger();
            }

            return _Logger;
        }

        internal ZZLogger()
        {
            _worker = new Thread(new ThreadStart(_Do_Worker));
            _worker.IsBackground = true;
            _worker.Start();

            _loggerType = "";
        }

        internal void _Do_Worker()
        {
            if (_Running) {
                return;
            }

            _Running  = true;
            rootPath  = AppDomain.CurrentDomain.BaseDirectory;
            separator = Path.DirectorySeparatorChar.ToString();
            rootPath  = rootPath.Replace("/", separator);

            if (rootPath.Substring(rootPath.Length - 1) != separator) {
                rootPath = rootPath + separator;
            }

            while (true) {
                Queue<JSONObject> ToBeSend;

                lock (_PENDING) {
                    ToBeSend = _que;
                    _que     = new Queue<JSONObject>();
                }

                try {
                    if (ToBeSend.Count > 0) {
                        string sPathName = rootPath + "temp" + separator + "log" + separator;

                        if (!Directory.Exists(sPathName)) {
                            Directory.CreateDirectory(sPathName);
                        }

                        string _ff        = "";
                        string _File_Path = sPathName + DateTime.Now.ToString("yyyyMMdd") + separator;
                        bool Opening      = false;

                        if (!Directory.Exists(_File_Path)) {
                            Directory.CreateDirectory(_File_Path);
                        }

                        StreamWriter sw = null;

                        foreach (JSONObject _JSONObject in ToBeSend) {
                            string cFile_Name = _JSONObject.GetValue("ZZFILE_NAME");

                            if (_ff != cFile_Name) {
                                if (Opening) {
                                    sw.Flush();
                                    sw.Close();
                                    Opening = false;
                                }

                                sw      = new StreamWriter(_File_Path + cFile_Name + ".log", true);
                                Opening = true;
                            }

                            sw.WriteLine(_JSONObject.GetValue("MSG"));

                            _ff = cFile_Name;
                        }

                        if (Opening) {
                            sw.Flush();
                        }

                        sw.Close();
                    }
                } catch (Exception e) {

                }

                Thread.Sleep(1000);
            }
        }

        public static void WriteToFile(string FileName, object Data)
        {
            string rootPath  = AppDomain.CurrentDomain.BaseDirectory;
            string separator = Path.DirectorySeparatorChar.ToString();
            rootPath         = rootPath.Replace("/", separator);
            string sPathName = rootPath + "temp" + separator + "log" + separator + FileName;

            ZZLogger.Debug(ZFILE_NAME, sPathName);

            StreamWriter swer = new StreamWriter(sPathName, false);
            swer.Write(Data);
            swer.Flush();
            swer.Close();
        }

        public static void Sql(string cModules_Name, object msg, params object[] objs)
        {
            if (!SQLEnabled) {
                return;
            }

            if (_Logger == null) {
                ZZLogger.getInstance();
            }

            _Logger.WriteLog("SQL", cModules_Name, "" + msg, objs);
        }

        public static void Info(string cModules_Name, object msg, params object[] objs)
        {
            if (!DebugEnabled) {
                return;
            }

            if (_Logger == null) {
                ZZLogger.getInstance();
            }

            _Logger.WriteLog("INFO", cModules_Name, "" + msg, objs);
        }

        public static void Warn(string cModules_Name, object msg, params object[] objs)
        {
            if (_Logger == null) {
                ZZLogger.getInstance();
            }

            _Logger.WriteLog("WARN", cModules_Name, "" + msg, objs);
        }

        public static void Fatal(string cModules_Name, object msg, params object[] objs)
        {
            if (_Logger == null) {
                ZZLogger.getInstance();
            }

            _Logger.WriteLog("FATAL", cModules_Name, "" + msg, objs);
        }

        public static void Trace(string cModules_Name, object msg, params object[] objs)
        {
            if (_Logger == null) {
                ZZLogger.getInstance();
            }

            _Logger.WriteLog("TRACE", cModules_Name, "" + msg, objs);
        }

        public static void Debug(string cModules_Name, object msg, params object[] objs)
        {
            if (!DebugEnabled) {
                return;
            }

            if (_Logger == null) {
                ZZLogger.getInstance();
            }

            _Logger.WriteLog("DEBUG", cModules_Name, "" + msg, objs);
        }

        public static void Error(string sMsg)
        {
            if (_Logger == null) {
                ZZLogger.getInstance();
            }

            _Logger.WriteLog("ERROR", "", "" + sMsg);
        }
        public static void Error(string cModules_Name, Exception sErrMsg, params object[] objs)
        {
            if (_Logger == null) {
                ZZLogger.getInstance();
            }

            string cMessage = "Message=[" + sErrMsg.Message + "]" + "\nSource=[" + sErrMsg.Source + "]\nStackTrace=[" + sErrMsg.StackTrace + "]\nTargetSite=[" + sErrMsg.TargetSite + "]";
            _Logger.WriteLog("ERROR", cModules_Name, "" + cMessage, objs);

        }

        public static void Error(string cModules_Name, object msg, params object[] objs)
        {
            if (_Logger == null) {
                ZZLogger.getInstance();
            }

            _Logger.WriteLog("ERROR", cModules_Name, "" + msg, objs);
        }

        private void WriteLog(string logtype, string cModules_Name, string msg, params object[] objs)
        {
            string meth = "";

            if (MethodInfoEnabled) {
                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(2);
                System.Diagnostics.StackFrame sf = st.GetFrame(0);
                meth = sf.GetMethod().Name;
            }

            this.WriteLog(logtype, cModules_Name, _loggerType, meth, msg, objs);
        }

        private string FormatLog(string log, string cModules_Name, string type, string meth, string msg, object[] objs)
        {
            sb.Length    = 0;
            sb.AppendLine(
                    "" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") +
                    "|" + log +
                    "|" + AppDomain.GetCurrentThreadId() +
                    "|" + type +
                    "|" + cModules_Name +
                    "|" + meth +
                    "| " + msg);

            foreach (object o in objs) {
                sb.AppendLine("|" + o);
            }

            return sb.ToString();
        }

        private void WriteLog(string logtype, string cModules_Name, string type, string meth, string msg, params object[] objs)
        {
            lock (_PENDING) {
                JSONObject _JSONObject = new JSONObject();
                _JSONObject.SetValue("ZZFILE_NAME", cModules_Name);
                _JSONObject.SetValue("MSG", FormatLog(logtype, cModules_Name, type, meth, msg, objs));
                _que.Enqueue(_JSONObject);
            }
        }
    }
}
