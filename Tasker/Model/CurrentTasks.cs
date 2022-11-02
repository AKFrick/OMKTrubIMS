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
        ObservableCollection<ProductionTask> finishedTaskCollection;
        public ReadOnlyObservableCollection<ProductionTask> FinishedTaskList { get; private set; }

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
        void threadTask()
        {
            while (true)
            {
                RefreshTaskList();
                Thread.Sleep(20000);
            }                        
        }

        ObservableCollection<ProductionTask> currentTaskCollection;
        public ReadOnlyObservableCollection<ProductionTask> TaskList { get; private set; }
        public void RefreshTaskList()
        {
            using (Trubodetal189Entities db = new Trubodetal189Entities())
            {
                try
                {
                    //Получим активные задания
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

                    //Получим завершенные задания
                    IQueryable<ProductionTask> query2 = from b in db.ProductionTasks
                                                       where (b.Status == "s" || b.Status == "f")
                                                       select b;
                    foreach (ProductionTask task in query2)
                    {
                        if (!finishedTaskCollection.Any(item => item.ID == task.ID))
                            finishedTaskCollection.Add(task);
                    }
                    finishedTaskCollection.ToList().ForEach(task =>
                    {
                        if (!query2.Any(item => item.ID == task.ID))
                            finishedTaskCollection.Remove(task);
                    });

                }
                catch (Exception e)
                {
                    OutputLog.That($"Не удалось обновить список заданий: {e.Message}");
                }
                                                    
            }
        }                

        public void UpdateStartDate (int taskID)
        {
            using (Trubodetal189Entities db = new Trubodetal189Entities())
            {
                try
                {
                    ProductionTask task = db.ProductionTasks.SingleOrDefault(b => b.ID == taskID);
                    task.StartDate = DateTime.Now;
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    OutputLog.That($"Не удалось установить дату начала задания: {e.Message}");
                }
            }
        }

        public ProductionTask InsertNewTask(ProductionTask task)
        {
            using (Trubodetal189Entities db = new Trubodetal189Entities())
            {
                try
                {
                    int MinID = db.ProductionTasks.Min(e => e.ID);                
                    task.ID = --MinID;
                    db.ProductionTasks.Add(task);
                    db.SaveChanges();
                    RefreshTaskList();
                    return task;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }                        
        }
        public void LoadTaskResult(ProductionTask taskResult)
        {
            if (taskResult.ID != 0)
            {
                using (Trubodetal189Entities db = new Trubodetal189Entities())
                {
                    try
                    {
                        var result = db.ProductionTasks.Single(b => b.ID == taskResult.ID);

                        result.PiceAmount = taskResult.PiceAmount;                        
                        result.FinishDate = taskResult.FinishDate;
                        result.BandBrand = taskResult.BandBrand;
                        result.BandSpeed = taskResult.BandSpeed;
                        result.BandType = taskResult.BandType;
                        result.SawDownSpeed = taskResult.SawDownSpeed;                        

                        result.Status = "f";
                        db.SaveChanges();
                        RefreshTaskList();
                    }
                    catch (Exception e)
                    {
                        OutputLog.That($"Не удалось загрузить результаты задания: {e.Message}");
                    }

                }
            }
            else throw new TaskNotCreatedException();
        }

        public void UpdateTask(ProductionTask task)
        {
            using (Trubodetal189Entities db = new Trubodetal189Entities())
            {
                try
                {
                    ProductionTask prodtask = db.ProductionTasks.SingleOrDefault(b => b.ID == task.ID);
                    prodtask.StartDate = task.StartDate;
                    prodtask.Operator = task.Operator;
                    prodtask.IDOperatorNumber = task.IDOperatorNumber;
                    db.SaveChanges();

                    currentTaskCollection.Remove(currentTaskCollection.Where(w => w.ID == task.ID).Single());
                    RefreshTaskList();
                }
                catch (Exception e)
                {
                    OutputLog.That($"Не удалось обновить информацию о задании: {e.Message}");
                }
            }

        }
    }
}
