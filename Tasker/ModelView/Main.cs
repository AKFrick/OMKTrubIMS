using System;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using Tasker.Model;

namespace Tasker.ModelView
{
    class Main : BindableBase
    {
        public Main()
        {
            StatusBarInit();
            FillTaskList();
            
        }
        public ObservableCollection<ProductionTask> TaskList { get; private set; }
        CurrentTasks currentTasks;
        
        void FillTaskList()
        {
            try { currentTasks = new CurrentTasks(); }
            catch (Exception ex) { Log.logThis(ex.Message); }


            //TaskList = new ObservableCollection<ProductionTask>(currentTasks.CurrentTasksCollection);
            TaskList = new ObservableCollection<ProductionTask>();
        }
        public ErrorScroller errorScroller { get; private set; }
        public ErrorItem CurrentError { get { return errorScroller.CurrentError; } }
        void StatusBarInit()
        {
            errorScroller = new ErrorScroller();
        }
    }  
}
