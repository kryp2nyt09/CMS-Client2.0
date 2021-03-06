﻿using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CMS2.Common
{
    // TODO BUG cannot process simultaneous write in the file. Try implement async
    public static class Logs
    {
        private static DateTime LogDate;
        private static string AppLogFile;
        private static string ErrorLogFile;        
        
        public static async Task AppLogs(string logPath, string functionName, string message = "")
        {
            LogDate = DateTime.Now;
            string _fileName = "\\AppLogs_" + LogFileDate(LogDate) + ".txt";
            AppLogFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\APCARGO\\Logs";
            System.IO.Directory.CreateDirectory(AppLogFile);
            File.AppendAllText(@AppLogFile +_fileName, LogDate.ToString("yyyy/MM/dd HH:mm:ss") + " :: " + functionName + " :: " + message + "\r\n");
        }

        public static async Task ErrorLogs(string logPath, string functionName, string errorMessage)
        {
            LogDate = DateTime.Now;
            string _fileName = "\\ErrorLogs_" + LogFileDate(LogDate) + ".txt";
            ErrorLogFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\APCARGO\\Logs";
            System.IO.Directory.CreateDirectory(ErrorLogFile);
            File.AppendAllText(@ErrorLogFile + _fileName, LogDate.ToString("yyyy/MM/dd HH:mm:ss") + " :: " + functionName + " :: " + errorMessage + "\r\n");
        }

        private static string LogFileDate(DateTime logDate)
        {
            StringBuilder date = new StringBuilder();
            date.AppendFormat("{0}{1}{2}", logDate.Year.ToString("0000"), logDate.Month.ToString("00"), logDate.Day.ToString("00"));
            return date.ToString();
        }
    }
}
