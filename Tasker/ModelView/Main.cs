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
        public ErrorScroller errorScroller { get; }
        public ObservableCollection<ProductionTask> TaskList { get; private set; }
        public ObservableCollection<ProductionTask> FinishedTaskList { get; private set; }
        public ObservableCollection<Login> logins { get; private set; }
        public Login SelectedLogin { get; set; }
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


            AsutpServerLogins AsutpLogins = new AsutpServerLogins();
            logins = new ObservableCollection<Login>(AsutpLogins.Logins);

            //CardReader cardReader = new CardReader();
            //cardReader.NewCardRead += ReadCard;

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
                        Log.logThis(e.Message);
                        MessageBox.Show("Нет подключения к ПЛК");

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
        public ProductionTask SelectedTask { get; set; }
        public ErrorItem CurrentError => errorScroller.CurrentError;
        CurrentTasks currentTasks;
        AsutpServer checkForNewTasks;
        Plc plc;

        void ReadCard(long ID)
        {
            if (ID != 0)
            {
                try
                {
                    SelectedLogin = logins.Where(w => w.IDNumber == ID).Single();
                    RaisePropertyChanged("SelectedLogin");
                }
                catch
                {
                    MessageBox.Show($"Пользователь {ID} не найден");
                }
            }
        }

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