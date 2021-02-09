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
                try
                {
                    plc.SendTask(new ProductionTaskExtended(SelectedTask));
                    currentTasks.UpdateStartDate(SelectedTask.ID);
                }
                catch (Exception e)
                {
                    Log.logThis(e.Message);
                }
                //RaisePropertyChanged(nameof(SelectedTask));
            });
            FinishTask = new DelegateCommand(() =>
            {
                try
                {
                    ProductionTask taskResult = plc.GetCurrentTaskResult();
                    taskResult.FinishDate = DateTime.Now;
                    try
                    {
                        currentTasks.LoadTaskResult(taskResult);
                    }
                    catch (TaskNotCreatedException)
                    {
                        NewTaskWindow newTaskWindow = new NewTaskWindow(new NewTask(currentTasks, taskResult));
                    }
                }                
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
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