using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Tasker.Model
{
    public class OutputLog
    {
        private static OutputLog instance;        
        private static readonly Lazy<OutputLog> lazy = new Lazy<OutputLog>(() => new OutputLog());

        public static OutputLog GetInstance()
        {
            return lazy.Value;
            //if(instance == null)
            //    instance = new OutputLog();
            //return instance;
        }
        private OutputLog()
        {
            logItems = new ObservableCollection<LogItem>();
            LogItems = new ReadOnlyObservableCollection<LogItem>(logItems);

        }
        public class LogItem
        {
            public LogItem(string message)
            {
                TimeStamp = DateTime.Now;
                Message = message;
            }
            public DateTime TimeStamp { get; private set; }
            public string Message { get; private set; }
            public override string ToString()
            {
                return $"{TimeStamp}:{Message}";
            }
        }

        private ObservableCollection<LogItem> logItems;
        public ReadOnlyObservableCollection<LogItem> LogItems { get; private set; }
        public static void That(string message)
        {
            LogItem item = new LogItem(message);
            GetInstance().logItems.Add(item);
            try
            {
                Log.logThis(item);
            }
            catch (Exception ex)
            {
                GetInstance().logItems.Add(new LogItem($"Не удалась запись в лог файл: {ex.Message}"));
            }
        }  
        public static void DoNothing()
        {

        }
    }
}