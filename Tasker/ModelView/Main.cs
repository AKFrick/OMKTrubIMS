using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using Tasker.Model;

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
            RefreshTaskList = new DelegateCommand(currentTasks.RefreshTaskList);
            SendToPlc = new DelegateCommand(() => plc.SendTask(new ProductionTask()
            {
                Id = 123,
                Number = "Number1",
                Position = 33,
                Item = "Item1",
                ItemBatch = "ItemBatch1",
                ItemBeginSerial = "ItemBeginSerial1",
                RecipeNumber = "RecipeNumber1",
                PipeBatch = "PipeBatch1",
                PipeNumber = "PipeNumber1",
                PipeHeat = "PipeHeat1",
                PipeSteel = "PipeSteel1",
                PipeDiameter = 44,
                PipeThickness = 55
            }));
        }        
        public ErrorItem CurrentError => errorScroller.CurrentError;
        CurrentTasks currentTasks;
        Plc plc;
        public DelegateCommand RefreshTaskList { get; private set; }      
        public DelegateCommand SendToPlc { get; private set; }
    }  
}