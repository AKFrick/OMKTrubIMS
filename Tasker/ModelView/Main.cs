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
        public ObservableCollection<ProductionTask> FinishedTaskList { get; private set; }
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

            FinishedTaskList = new ObservableCollection<ProductionTask>(currentTasks.FinishedTaskList);
            ((INotifyCollectionChanged)currentTasks.FinishedTaskList).CollectionChanged += (s, a) =>
            {
                if (a.NewItems?.Count >= 1)
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        foreach (ProductionTask task in a.NewItems)
                            FinishedTaskList.Add(task);
                    }));
                if (a.OldItems?.Count >= 1)
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        foreach (ProductionTask task in a.OldItems)
                            FinishedTaskList.Remove(task);
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
                    MessageBox.Show("Нет подключения к ПЛК");

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
                        MessageBox.Show("Задание не найдено. Введите параметры");
                        NewTaskWindow newTaskWindow = new NewTaskWindow(new NewTask(currentTasks, taskResult));
                        newTaskWindow.ShowDialog();
                    }
                }                
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });
            ShowCurrentTask = new DelegateCommand(() => { VisibleCurrentTask = true; VisibleFinishedTask = false; } );
            ShowFinishedTask = new DelegateCommand(() => { VisibleCurrentTask = false; VisibleFinishedTask = true; });
        }
        public ProductionTask SelectedTask { get; set; }
        public ErrorItem CurrentError => errorScroller.CurrentError;
        CurrentTasks currentTasks;
        AsutpServer checkForNewTasks;
        Plc plc;


        public bool VisibleFinishedTask { get { return visibleFinishedTask; } set { visibleFinishedTask = value; RaisePropertyChanged("VisibleFinishedTask"); } }
        bool visibleFinishedTask;
        public bool VisibleCurrentTask { get { return visibleCurrentTask; } set { visibleCurrentTask = value; RaisePropertyChanged("VisibleCurrentTask"); } }
        bool visibleCurrentTask;
        public DelegateCommand OpenNewTaskWindow { get; private set; }
        public DelegateCommand StartTask { get; }
        public DelegateCommand FinishTask { get; }
        public DelegateCommand ShowFinishedTask { get; }
        public DelegateCommand ShowCurrentTask { get; }

    }
}