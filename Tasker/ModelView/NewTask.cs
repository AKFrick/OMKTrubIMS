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
        public ProductionTask task { get; }
        public DelegateCommand Create { get; }
        public event Action TaskCreated;
        public NewTask(CurrentTasks currentTasks)
        {            
            task = new ProductionTask();
            Create = new DelegateCommand(() =>
            {
                task.CreationDate = DateTime.Now;
                task.Status = "1";
                currentTasks.InsertNewTask(task);
                TaskCreated?.Invoke();
            });
        }
        public NewTask(CurrentTasks currentTasks, ProductionTask task)
        {
            this.task = task;
            Create = new DelegateCommand(() =>
            {
                this.task.Status = "f";
                this.task.CreationDate = DateTime.Now;
                this.task.FinishDate = DateTime.Now;
                currentTasks.InsertNewTask(this.task);
                TaskCreated?.Invoke();
            });
        }            


    }
}
