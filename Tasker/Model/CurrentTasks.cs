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

        ObservableCollection<ProductionTask> HiddenTaskCollection;
        public ReadOnlyObservableCollection<ProductionTask> HiddenTaskList { get; private set; }

        Thread trackingThread;
        public CurrentTasks()
        {
            currentTaskCollection = new ObservableCollection<ProductionTask>();
            TaskList = new ReadOnlyObservableCollection<ProductionTask>(currentTaskCollection);

            finishedTaskCollection = new ObservableCollection<ProductionTask>();
            FinishedTaskList = new ReadOnlyObservableCollection<ProductionTask>(finishedTaskCollection);

            HiddenTaskCollection = new ObservableCollection<ProductionTask>();
            HiddenTaskList = new ReadOnlyObservableCollection<ProductionTask>(HiddenTaskCollection);

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
                                                       where b.Status != "s" && b.Status != "e" && b.Status != "f" && b.Status != "h"
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

                    //Получим скрытые задания
                    IQueryable<ProductionTask> query3 = from b in db.ProductionTasks
                                                        where (b.Status == "h")
                                                        select b;
                    foreach (ProductionTask task in query3)
                    {
                        if (!HiddenTaskCollection.Any(item => item.ID == task.ID))
                            HiddenTaskCollection.Add(task);
                    }
                    HiddenTaskCollection.ToList().ForEach(task =>
                    {
                        if (!query3.Any(item => item.ID == task.ID))
                            HiddenTaskCollection.Remove(task);
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

        public void HideTask (ProductionTask SelectedTask)
        {
            using (Trubodetal189Entities db = new Trubodetal189Entities())
            {
                try
                {
                    ProductionTask task = db.ProductionTasks.SingleOrDefault(b => b.ID == SelectedTask.ID);
                    task.Status = "h";
                    db.SaveChanges();
                    OutputLog.That($"Задание скрыто {task.ID}");
                }
                catch (Exception e)
                {
                    OutputLog.That($"Не уадлсоь скрыть задание: {e.Message}");
                }
                RefreshTaskList();
            }
        }
        public void UnhideTask(ProductionTask SelectedTask)
        {
            using (Trubodetal189Entities db = new Trubodetal189Entities())
            {
                try
                {
                    ProductionTask task = db.ProductionTasks.SingleOrDefault(b => b.ID == SelectedTask.ID);
                    task.Status = "1";
                    db.SaveChanges();
                    OutputLog.That($"Убрано сокрытие задания {task.ID}");
                }
                catch (Exception e)
                {
                    OutputLog.That($"Не уадлалось убрать сокрытие задание: {e.Message}");
                }
                RefreshTaskList();
            }
        }
        public ProductionTask InsertNewTask(ProductionTask task)
        {
            using (Trubodetal189Entities db = new Trubodetal189Entities())
            {
                try
                {
                    int MinID = db.ProductionTasks.Min(e => e.ID);
                    if (MinID < 0)
                    {
                        task.ID = --MinID;
                        db.ProductionTasks.Add(task);
                        db.SaveChanges();
                        OutputLog.That($"Задание {task.ID} создано");
                    }
                    else
                        throw new ArgumentNullException();

                    
                }
                catch (ArgumentNullException e)
                {                   
                    task.ID = -1;
                    db.ProductionTasks.Add(task);
                    db.SaveChanges();
                    OutputLog.That($"Задание {task.ID} создано");

                }
                catch (Exception e)
                {
                    OutputLog.That($"Не удалось создать задание {e.Message}");
                }

                RefreshTaskList();
                return task;
            }                        
        }
        public void LoadTaskResult(ProductionTask taskResult)
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
                catch(InvalidOperationException e)
                {
                    taskResult.Status = "f";
                    InsertNewTask(taskResult);
                    OutputLog.That("Задание с заданным ID не найдено. Создано новое задание");
                }
                catch (Exception e)
                {
                    OutputLog.That($"Не удалось загрузить результаты задания: {e.Message}");
                }

            }                     
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
