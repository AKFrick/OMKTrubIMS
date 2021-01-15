using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using Tasker.Model;
using Tasker.View;

namespace Tasker.ModelView
{
    class Main : BindableBase
    {
        public ErrorScroller errorScroller { get; }
        public ObservableCollection<ProductionTask> TaskList { get; private set; }
        public Main()
        {
            errorScroller = new ErrorScroller();
            errorScroller.RaiseErrorChanged += () => RaisePropertyChanged(nameof(CurrentError));
            // Работа с SQL
            currentTasks = new CurrentTasks(errorScroller);
            checkForNewTasks = new AsutpServer(errorScroller);

            TaskList = new ObservableCollection<ProductionTask>(currentTasks.TaskList);
            ((INotifyCollectionChanged)currentTasks.TaskList).CollectionChanged += (s, a) =>
            {
                if (a.NewItems?.Count >= 1)
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        foreach (ProductionTask task in a.NewItems)
                            TaskList.Add(task);
                    }));
                if (a.OldItems?.Count >= 1)
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        foreach (ProductionTask task in a.OldItems)
                            TaskList.Remove(task);
                    }));
            };
            //Работа с ПЛК
            plc = new Plc();
            OpenNewTaskWindow = new DelegateCommand(() =>
            {
                NewTaskWindow newTaskWindow = new NewTaskWindow(new NewTask(currentTasks));
                newTaskWindow.ShowDialog();
            });
            StartTask = new DelegateCommand(() =>
            {
                plc.SendTask(SelectedTask);
                //RaisePropertyChanged(nameof(SelectedTask));
            });
            FinishTask = new DelegateCommand(() =>
            {
                ProductionTask taskResult = plc.GetCurrentTaskResult();
                taskResult.FinishDate = DateTime.Now;
                currentTasks.LoadTaskResult(taskResult);                
            });
        }
        public ProductionTask SelectedTask { get; set; }
        public ErrorItem CurrentError => errorScroller.CurrentError;
        CurrentTasks currentTasks;
        AsutpServer checkForNewTasks;
        Plc plc;
        public DelegateCommand OpenNewTaskWindow { get; private set; }
        public DelegateCommand StartTask { get; private set; }
        public DelegateCommand FinishTask { get; private set; }
    }  
}