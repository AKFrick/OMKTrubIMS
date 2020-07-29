using System.IO;
using System;
using System.Threading;

namespace TaskManager.Model
{
    public static class Log
    {
        private static object syncLock = new object();
        public static void logThis(string msg)
        {
            File.AppendAllText("Log.txt", $"{DateTime.Now.ToString()}:{msg}{Environment.NewLine}");
        }
    }
}
