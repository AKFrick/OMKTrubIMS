using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data.Entity.Core;
using Tasker.ModelView;
using System.Threading;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows;

namespace Tasker.Model    
{
    public class TaskNotCreatedException : Exception { }
    public class CurrentTasks
    {
        Thread trackingThread;
        public CurrentTasks()
        {
            currentTaskCollection = new ObservableCollection<ProductionTask>();
            TaskList = new ReadOnlyObservableCollection<ProductionTask>(currentTaskCollection);

            finishedTaskCollection = new ObservableCollection<ProductionTask>();
            FinishedTaskList = new ReadOnlyObservableCollection<ProductionTask>(finishedTaskCollection);

            trackingThread = new Thread(threadTask) { IsBackground = true };
            trackingThread.Start();            
        }
        public CurrentTasks(ErrorScroller errorScroller) : this()
        {
            this.errorScroller = errorScroller;
        }
        ErrorScroller errorScroller;
        ErrorItem connectionError = new ErrorItem("Ошибка подключения к SQL");
        void threadTask()
        {
            try
            {
                RefreshTaskList();
                RefreshFinishedTaskList();
                errorScroller?.RemoveError(connectionError);
                Thread.Sleep(20000);
                threadTask();
            }
            catch (EntityException ex)
            {
                errorScroller?.AddError(connectionError);                
                Thread.Sleep(10000);
                threadTask();
                MessageBox.Show("Unsuccessful connect to SQL");
            }
        }

        ObservableCollection<ProductionTask> currentTaskCollection;
        public ReadOnlyObservableCollection<ProductionTask> TaskList { get; private set; }
        public void RefreshTaskList()
        {
            using (Trubodetal189Entities db = new Trubodetal189Entities())
            {
                IQueryable<ProductionTask> query = from b in db.ProductionTasks
                                                   where b.Status != "s" && b.Status != "e" && b.Status != "f"
                                                   select b;
                foreach (ProductionTask task in query)
                {
                    if (!currentTaskCollection.Any(item => item.ID == task.ID))
                        currentTaskCollection.Add(task);
                }
                currentTaskCollection.ToList().ForEach(task =>
                {
                    if (!query.Any(item => item.ID == task.ID))
                        currentTaskCollection.Remove(task);
                });
            }
        }

        public void RefreshFinishedTaskList()
        {
            using (Trubodetal189Entities db = new Trubodetal189Entities())
            {
                IQueryable<ProductionTask> query = from b in db.ProductionTasks
                                                   where (b.Status == "s" || b.Status == "f")
                                                   select b;
                foreach (ProductionTask task in query)
                {
                    if (!finishedTaskCollection.Any(item => item.ID == task.ID))
                        finishedTaskCollection.Add(task);
                }
                finishedTaskCollection.ToList().ForEach(task =>
                {
                    if (!query.Any(item => item.ID == task.ID))
                        finishedTaskCollection.Remove(task);
                });
            }
        }

        public void UpdateStartDate (int taskID)
        {
            using (Trubodetal189Entities db = new Trubodetal189Entities())
            {
                ProductionTask task = db.ProductionTasks.SingleOrDefault(b => b.ID == taskID);
                task.StartDate = DateTime.Now;
                db.SaveChanges();
            }
        }

        public ProductionTask InsertNewTask(ProductionTask task)
        {
            using (Trubodetal189Entities db = new Trubodetal189Entities())
            {
                int MinID = db.ProductionTasks.Min(e => e.ID);                
                task.ID = --MinID;
                db.ProductionTasks.Add(task);
                db.SaveChanges();                
            }
            RefreshTaskList();
            return task;

        }
        public void LoadTaskResult(ProductionTask taskResult)
        {
            using (Trubodetal189Entities db = new Trubodetal189Entities())
            {                
                try
                {
                    var result = db.ProductionTasks.Single(b => b.ID == taskResult.ID);

                    result.PiceAmount = taskResult.PiceAmount;
                    result.Operator = taskResult.Operator;
                    result.FinishDate = taskResult.FinishDate;
                    result.Status = "f";
                    db.SaveChanges();
                }
                catch (InvalidOperationException)
                {
                    throw new TaskNotCreatedException();
                }

            }            
            RefreshTaskList();

        }

        ObservableCollection<ProductionTask> finishedTaskCollection;
        public ReadOnlyObservableCollection<ProductionTask> FinishedTaskList { get; private set; }

    }
}
