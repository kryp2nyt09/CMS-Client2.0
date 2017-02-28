using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CMS2.Common
{
    // TODO BUG cannot process simultaneous write in the file. Try implement async
    public class Logs
    {
        private DateTime LogDate;
        private string AppLogFile;
        private string ErrorLogFile;

        public Logs()
        {

        }

        public async Task AppLogs(string logPath, string functionName, string message="")
        {
            LogDate = DateTime.Now;
            AppLogFile = logPath + "AppLogs_" + LogFileDate(LogDate) + ".txt";
            File.AppendAllText(@AppLogFile, LogDate.ToString("yyyy/MM/dd HH:mm:ss") + " :: " + functionName + " :: " + message + "\r\n");
        }

        public async Task ErrorLogs(string logPath, string functionName, string errorMessage)
        {
            LogDate = DateTime.Now;
            ErrorLogFile = logPath + "ErrorLogs_" + LogFileDate(LogDate) + ".txt";
            File.AppendAllText(@ErrorLogFile, LogDate.ToString("yyyy/MM/dd HH:mm:ss") + " :: " + functionName + " :: " + errorMessage + "\r\n");
        }

        private string LogFileDate(DateTime logDate)
        {
            StringBuilder date = new StringBuilder();
            date.AppendFormat("{0}{1}{2}", logDate.Year.ToString("0000"), logDate.Month.ToString("00"), logDate.Day.ToString("00"));
            return date.ToString();
        }
    }
}
