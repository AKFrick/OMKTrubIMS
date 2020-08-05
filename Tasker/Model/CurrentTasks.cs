using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data.Entity.Core;
using Tasker.ModelView;
using System.Threading;
using System.Data.SqlClient;
using System.Configuration;

namespace Tasker.Model
{
    public class CurrentTasks
    {
        Thread trackingThread;
        public CurrentTasks()
        {
            currentTaskCollection = new ObservableCollection<ProductionTask>();
            TaskList = new ReadOnlyObservableCollection<ProductionTask>(currentTaskCollection);

            trackingThread = new Thread(threadTask);
            trackingThread.IsBackground = true;
            trackingThread.Start();
        }
        void threadTask()
        {
            try
            {
                getAllCurrentTasks();
            }
            catch (EntityException ex)
            {
                errorScroller?.AddError(connectionError);
                Log.logThis(ex.Message);
                Thread.Sleep(10000);
                threadTask();
            }
        }
        ErrorScroller errorScroller;
        public CurrentTasks(ErrorScroller errorScroller) : this()
        {
            this.errorScroller = errorScroller;
        }        
        ObservableCollection<ProductionTask> currentTaskCollection;
        public ReadOnlyObservableCollection<ProductionTask> TaskList { get; private set; }
        ErrorItem connectionError = new ErrorItem("Ошибка подключения к SQL");
        void getAllCurrentTasks()
        {
            using (var db = new Trubodetal189Entities())
            {
                IQueryable<ProductionTask> query = from b in db.ProductionTasks select b;
                foreach (ProductionTask task in query)                    
                    currentTaskCollection.Add(task);                    
                errorScroller?.RemoveError(connectionError);                
            }
        }
    }
}
