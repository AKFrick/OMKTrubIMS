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
            FillTaskList();
            errorScroller = new ErrorScroller();
            errorScroller.RaiseErrorChanged += () => RaisePropertyChanged(nameof(CurrentError));

            GetAllTasks = new DelegateCommand(() => 
            {
                try { currentTasks = new CurrentTasks(); }
                catch (Exception ex)
                {
                    errorScroller.AddError(new ErrorItem(ex.Message));
                    
                    Log.logThis(ex.Message);
                }
            });
        }
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
