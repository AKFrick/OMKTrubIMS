﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data.Entity.Core;

namespace Tasker.Model
{
    public class CurrentTasks
    {
        public CurrentTasks()
        {
            currentTaskCollection = new ObservableCollection<ProductionTask>(getAllCurrentTasks());
            CurrentTasksCollection = new ReadOnlyObservableCollection<ProductionTask>(currentTaskCollection);
        }
        ObservableCollection<ProductionTask> currentTaskCollection;
        public ReadOnlyObservableCollection<ProductionTask> CurrentTasksCollection { get; private set; }
        List<ProductionTask> getAllCurrentTasks()
        {
            List<ProductionTask> tasks = new List<ProductionTask>();
            using (var db = new Trubodetal189Entities())
            {
                try
                {
                    var query = from b in db.ProductionTasks select b;
                    foreach (ProductionTask task in query)
                    {
                        tasks.Add(task);
                    }
                }
                catch(EntityException ex)
                {
                    Log.logThis(ex.Message);
                    throw new Exception("Ошибка подклюения к SQL");
                }
                
            }
            return tasks;
        }



    }
}
