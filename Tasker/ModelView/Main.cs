using System.Collections.ObjectModel;
using Prism.Mvvm;
using Tasker.Model;

namespace Tasker.ModelView
{
    class Main : BindableBase
    {
        public Main()
        {
            FillTaskList();
        }
        public ObservableCollection<ProductionTask> TaskList { get; private set; }
        CurrentTasks currentTasks;
        void FillTaskList()
        {
            currentTasks = new CurrentTasks();            
            
            //TaskList = new ObservableCollection<ProductionTask>(currentTasks.CurrentTasksCollection);
            TaskList = new ObservableCollection<ProductionTask>();
        }
    }  
}
