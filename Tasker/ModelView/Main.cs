using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using Tasker.Model;
using Tasker.View;
using System.Linq;

namespace Tasker.ModelView
{
    class Main : BindableBase
    {
        public ObservableCollection<ProductionTask> TaskList { get; private set; }
        public ObservableCollection<ProductionTask> FinishedTaskList { get; private set; }
        public ObservableCollection<Login> logins { get; private set; }
        public Login SelectedLogin { get; set; }
        public ObservableCollection<OutputLog.LogItem> LogItems { get; set; }
        public ProductionTask SelectedTask { get; set; }
        CurrentTasks currentTasks;
        AsutpServer checkForNewTasks;

        public Main()
        {

            #region LOG
            LogItems = new ObservableCollection<OutputLog.LogItem>(OutputLog.GetInstance().LogItems);
            ((INotifyCollectionChanged)OutputLog.GetInstance().LogItems).CollectionChanged += (s, a) =>
            {
                if (a.NewItems?.Count >= 1)
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        foreach (OutputLog.LogItem item in a.NewItems)
                            LogItems.Add(item);
                    }));
                if (a.OldItems?.Count >= 1)
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        foreach (OutputLog.LogItem item in a.OldItems)
                            LogItems.Remove(item);
                    }));
            };
            #endregion

            // Работа с SQL
            #region Задания локальной базы
            currentTasks = new CurrentTasks();
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
            #endregion

            checkForNewTasks = new AsutpServer();
            AsutpServerLogins AsutpLogins = new AsutpServerLogins();
            logins = new ObservableCollection<Login>(AsutpLogins.Logins);

            //Работа с ПЛК
            plc = new Plc();
            OpenNewTaskWindow = new DelegateCommand(() =>
            {
                NewTaskWindow newTaskWindow = new NewTaskWindow(new NewTask(currentTasks));
                newTaskWindow.ShowDialog();
            });

            StartTask = new DelegateCommand(() =>
            {
                if (SelectedLogin != null)
                {
                    try
                    {
                        plc.SendTask(new ProductionTaskExtended(SelectedTask));

                        SelectedTask.StartDate = DateTime.Now;
                        SelectedTask.Operator = SelectedLogin.FIO;
                        SelectedTask.IDOperatorNumber = SelectedLogin.IDNumber;
                        currentTasks.UpdateTask(SelectedTask);
                    }
                    catch (Exception e)
                    {
                        OutputLog.That(e.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Выберите пользователя");
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
            ShowCurrentTask = new DelegateCommand(() => { VisibleCurrentTask = true; VisibleFinishedTask = false; });
            ShowFinishedTask = new DelegateCommand(() => { VisibleCurrentTask = false; VisibleFinishedTask = true; });
        }

        Plc plc;

        public bool VisibleFinishedTask { get { return visibleFinishedTask; } set { visibleFinishedTask = value; RaisePropertyChanged(nameof(VisibleFinishedTask)); } }
        bool visibleFinishedTask;
        public bool VisibleCurrentTask { get { return visibleCurrentTask; } set { visibleCurrentTask = value; RaisePropertyChanged(nameof(VisibleCurrentTask)); } }
        bool visibleCurrentTask;
        public DelegateCommand OpenNewTaskWindow { get; private set; }
        public DelegateCommand StartTask { get; }
        public DelegateCommand FinishTask { get; }
        public DelegateCommand ShowFinishedTask { get; }
        public DelegateCommand ShowCurrentTask { get; }

    }
}