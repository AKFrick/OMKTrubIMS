using System.IO;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Tasker.Model
{
    public class Log
    {
        static string LogPath = "Logs";
        private static readonly Lazy<Log> lazy = new Lazy<Log>(() => new Log());
        private ObservableCollection<OutputLog.LogItem> logItems;
        object locker = new object();
        public static Log GetInstance()
        {
            return lazy.Value;
        }
        private Log()
        {
            logItems = new ObservableCollection<OutputLog.LogItem>();
            ((INotifyCollectionChanged)logItems).CollectionChanged += (s, a) =>
            {
                if (a.NewItems?.Count >= 1)
                {
                    foreach (OutputLog.LogItem item in a.NewItems)
                        appendLog(item);
                }
            };

        }
        void appendLog(OutputLog.LogItem item)
        {
            lock (locker)
            {
                if (!Directory.Exists(LogPath))
                    Directory.CreateDirectory(LogPath);

                string fileName = $"{LogPath}\\{DateTime.Now.ToString("yyyyMMdd")}.txt";
                if (!File.Exists(fileName))
                    File.Create(fileName);

                File.AppendAllText(fileName, $"{item.ToString()}{Environment.NewLine}");
            }
        }
        public static void logThis(OutputLog.LogItem item)
        {
            GetInstance().logItems.Add(item);
        }
        public static void logThis(string msg)
        {
            logThis(new OutputLog.LogItem(msg));
        }
    }
}
