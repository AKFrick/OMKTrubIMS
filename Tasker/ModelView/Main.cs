using System;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using Tasker.Model;

namespace Tasker.ModelView
{
    class Main : BindableBase
    {
        public Main()
        {            
            errorScroller = new ErrorScroller();
            errorScroller.RaiseErrorChanged += () => RaisePropertyChanged(nameof(CurrentError));

            GetAllTasks = new DelegateCommand(() => 
            {
                try
                {
                    if (currentTaskError != null)
                    {
                        errorScroller.RemoveError(currentTaskError);
                        currentTaskError = null;
                    }                    
                    currentTasks = new CurrentTasks();
                    TaskList = new ObservableCollection<ProductionTask>(currentTasks.CurrentTasksCollection);
                    RaisePropertyChanged(nameof(TaskList));
                }
                catch (Exception ex)
                {
                    currentTaskError = new ErrorItem(ex.Message);
                    errorScroller.AddError(currentTaskError);                    
                    Log.logThis(ex.Message);
                    TaskList = new ObservableCollection<ProductionTask>();
                    RaisePropertyChanged(nameof(TaskList));
                }
            });
            GetAllTasks.Execute();
        }
        ErrorItem currentTaskError;
        public ObservableCollection<ProductionTask> TaskList { get; private set; }
        CurrentTasks currentTasks;
        
        void FillTaskList()
        {
            try { currentTasks = new CurrentTasks(); }
            catch (Exception ex) { Log.logThis(ex.Message); }
            TaskList = new ObservableCollection<ProductionTask>();
            //TaskList = new ObservableCollection<ProductionTask>(currentTasks.CurrentTasksCollection);            
        }
        public ErrorScroller errorScroller { get; }
        public ErrorItem CurrentError => errorScroller.CurrentError;
        public DelegateCommand GetAllTasks { get; }
    }  
}
