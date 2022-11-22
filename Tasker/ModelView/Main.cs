using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using Tasker.Model;
using Tasker.View;
using System.Linq;
using System.Windows.Media;

namespace Tasker.ModelView
{
    class Main : BindableBase
    {
        public ObservableCollection<ProductionTask> TaskList { get; private set; }
        public ObservableCollection<ProductionTask> FinishedTaskList { get; private set; }
        public ObservableCollection<ProductionTask> HiddenTaskList { get; private set; }
        public ObservableCollection<Login> logins { get; private set; }
        public Login SelectedLogin { get; set; }
        public ObservableCollection<OutputLog.LogItem> LogItems { get; set; }
        public ProductionTask SelectedTask { get; set; }
        public ProductionTask SelectedHiddenTask { get; set; }
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
            HiddenTaskList = new ObservableCollection<ProductionTask>(currentTasks.HiddenTaskList);
            ((INotifyCollectionChanged)currentTasks.HiddenTaskList).CollectionChanged += (s, a) =>
            {
                if (a.NewItems?.Count >= 1)
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        foreach (ProductionTask task in a.NewItems)
                            HiddenTaskList.Add(task);
                    }));
                if (a.OldItems?.Count >= 1)
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        foreach (ProductionTask task in a.OldItems)
                            HiddenTaskList.Remove(task);
                    }));
            };
            #endregion

            checkForNewTasks = new AsutpServer();
            checkForNewTasks.ConnectionEstablished += (b) =>
             {
                 if (!b)
                     BackColor = Brushes.Red;
                 else
                     BackColor = Brushes.White;
                 OutputLog.That($"!! {b}");

             };
            AsutpServerLogins AsutpLogins = new AsutpServerLogins();
            logins = new ObservableCollection<Login>(AsutpLogins.Logins);

            //Работа с ПЛК
            plc = new Plc(currentTasks);
            OpenNewTaskWindow = new DelegateCommand(() =>
            {
                NewTaskWindow newTaskWindow = new NewTaskWindow(new NewTask(currentTasks));
                newTaskWindow.ShowDialog();
            });

            StartTask = new DelegateCommand(() =>
            {
            //    if (SelectedLogin != null)
            //    {
                    try
                    {
                        plc.SendTask(new ProductionTaskExtended(SelectedTask));                       
                        SelectedTask.StartDate = DateTime.Now;
                        //SelectedTask.Operator = SelectedLogin.FIO;
                        //SelectedTask.IDOperatorNumber = SelectedLogin.IDNumber;
                        currentTasks.UpdateTask(SelectedTask);
                    }
                    catch (Exception e)
                    {
                        OutputLog.That(e.Message);
                    }
                //}
                //else
                //{
                //    MessageBox.Show("Выберите пользователя");
                //}
                //RaisePropertyChanged(nameof(SelectedTask));
            });
            HideTask = new DelegateCommand(() =>
            {
                currentTasks.HideTask(SelectedTask);
            }) ;
            UnhideTask = new DelegateCommand(() =>
            {
                currentTasks.UnhideTask(SelectedHiddenTask);
            });
            ShowCurrentTask = new DelegateCommand(() => { VisibleCurrentTask = true; VisibleFinishedTask = false; VisibleHiddenTask = false; });
            ShowFinishedTask = new DelegateCommand(() => { VisibleCurrentTask = false; VisibleFinishedTask = true; VisibleHiddenTask = false; });
            ShowHiddenTask = new DelegateCommand(() => { VisibleCurrentTask = false; VisibleFinishedTask = false; VisibleHiddenTask = true; });
            ShowCurrentTask.Execute();
        }

        Plc plc;

        public bool VisibleFinishedTask { get { return visibleFinishedTask; } set { visibleFinishedTask = value; RaisePropertyChanged(nameof(VisibleFinishedTask)); } }
        bool visibleFinishedTask;
        public bool VisibleCurrentTask { get { return visibleCurrentTask; } set { visibleCurrentTask = value; RaisePropertyChanged(nameof(VisibleCurrentTask)); } }
        bool visibleCurrentTask;
        public bool VisibleHiddenTask { get { return visibleHiddenTask; } set { visibleHiddenTask = value; RaisePropertyChanged(nameof(VisibleHiddenTask)); } }
        bool visibleHiddenTask;
        public DelegateCommand OpenNewTaskWindow { get; private set; }
        public DelegateCommand StartTask { get; }
        public DelegateCommand FinishTask { get; }       
        public DelegateCommand HideTask { get; }
        public DelegateCommand UnhideTask { get; }
        public DelegateCommand ShowFinishedTask { get; }
        public DelegateCommand ShowCurrentTask { get; }
        public DelegateCommand ShowHiddenTask { get; }
        public Brush BackColor { get { return backColor; } set { backColor = value; RaisePropertyChanged(nameof(BackColor)); }  }
        Brush backColor;
    }
}