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

            trackingThread = new Thread(threadTask) { IsBackground = true };
            trackingThread.Start();            
        }
        void threadTask()
        {
            try
            {
                RefreshTaskList();
                errorScroller?.RemoveError(connectionError);
                Thread.Sleep(20000);
                threadTask();
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
        ErrorItem connectionError = new ErrorItem("Ошибка подключения к SQL");
        public CurrentTasks(ErrorScroller errorScroller) : this()
        {
            this.errorScroller = errorScroller;
        }        
        ObservableCollection<ProductionTask> currentTaskCollection;
        public ReadOnlyObservableCollection<ProductionTask> TaskList { get; private set; }
        public void RefreshTaskList()
        {
            using (Trubodetal189Entities db = new Trubodetal189Entities())
            {
                IQueryable<ProductionTask> query = from b in db.ProductionTasks select b;
                foreach (ProductionTask task in query)
                {
                    if (!currentTaskCollection.Any(item => item.Id == task.Id))
                        currentTaskCollection.Add(task);
                }
                currentTaskCollection.ToList().ForEach(task =>
                {
                    if (!query.Any(item => item.Id == task.Id))
                        currentTaskCollection.Remove(task);
                });
            }
        }
        public ProductionTask InsertNewTask(ProductionTask task)
        {
            using (Trubodetal189Entities db = new Trubodetal189Entities())
            {
                db.ProductionTasks.Add(task);
                db.SaveChanges();
            }
            RefreshTaskList();
            return task;

        }
        public void LoadTaskResult(TaskResult taskResult)
        {
            using (Trubodetal189Entities db = new Trubodetal189Entities())
            {
                db.TaskResults.Add(taskResult);
                db.SaveChanges();
            }
        }

    }
}
