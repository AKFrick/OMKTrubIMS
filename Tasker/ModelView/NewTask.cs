using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using Tasker.Model;


namespace Tasker.ModelView
{
    public class NewTask : BindableBase
    {        
        public ProductionTask task { get; } = new ProductionTask();
        public DelegateCommand Create { get; }
        public event Action TaskCreated;
        public NewTask(CurrentTasks currentTasks)
        {
            Create = new DelegateCommand(() =>
            {
                task.CreatedAt = DateTime.Now;
                task.State = "Создано";
                currentTasks.InsertNewTask(task);
                TaskCreated?.Invoke();
            });
        }


    }
}
