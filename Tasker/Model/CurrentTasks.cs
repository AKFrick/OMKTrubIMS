﻿using System;
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
                var result = db.ProductionTasks.SingleOrDefault(b => b.ID == taskResult.ID);

                result.PiceAmount = taskResult.PiceAmount;
                result.Operator = taskResult.Operator;
                result.FinishDate = taskResult.FinishDate;

                result.Status = "f";

                db.SaveChanges();
            }            
            RefreshTaskList();

        }               

    }
}
