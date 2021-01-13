using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data.Entity.Core;
using Tasker.ModelView;
using System.Threading;
using System.Data.SqlClient;
using System.Configuration;

namespace Tasker.Model
{
    public class CheckForNewTasks
    {
        Thread trackingThread;
        ErrorScroller errorScroller;
        ErrorItem connectionError = new ErrorItem("Ошибка подключения к серверу АСУТП");

        public CheckForNewTasks()
        {
            trackingThread = new Thread(threadTask) { IsBackground = true };
            trackingThread.Start();

        }
        public CheckForNewTasks(ErrorScroller errorScroller) : this()
        {
            this.errorScroller = errorScroller;
        }
        void threadTask()
        {
            try
            {
                Check();
                UpdateTaskResult();
                errorScroller?.RemoveError(connectionError);
                Thread.Sleep(20000);
                threadTask();
            }
            catch (EntityException ex)
            {
                errorScroller?.AddError(connectionError);
                Log.logThis(ex.Message);
                Thread.Sleep(10000);
                threadTask();
            }
        }

        public void Check()
        {
            using (Trubodetal189Entities local = new Trubodetal189Entities())
            {
                int LastID = local.ProductionTasks.Max(p => p.ID);

                using (ASUTPEntities asutp = new ASUTPEntities() )
                {
                    IQueryable<Tasker.Task> query = from b in asutp.Tasks
                                                       where b.ID > LastID
                                                       select b;

                    foreach (Task task in query)
                    {
                        ProductionTask productionTask = new ProductionTask()
                        {
                            ID = task.ID,
                            TaskNumber = task.TaskNumber,
                            TaskPosition = task.TaskPosition,
                            Product = task.Product,
                            ProductBatchNumber = task.ProductBatchNumber,
                            StartSerialNumber = task.StartSerialNumber,
                            ProductsAmount = task.ProductsAmount,
                            PipeBatchNumber = task.PipeBatchNumber,
                            PipeNumber = task.PipeNumber,
                            Heat = task.Heat,
                            SteelType = task.SteelType,
                            Diameter = task.Diameter,
                            Thickness = task.Thickness,
                            PieceLength = task.PieceLength,
                            PieceQuantity = task.PieceQuantity,
                            CreationDate = task.CreationDate,
                            StartDate = task.StartDate,
                            FinishDate = task.FinishDate,
                            Source = task.Source,
                            PiceAmount = task.PiceAmount,
                            Operator = task.Operator,
                            Status = task.Status
                        };
                        local.ProductionTasks.Add(productionTask);
                        local.SaveChanges();
                    }
                }
                
            }
        }
        public void UpdateTaskResult()
        {
            using (ASUTPEntities asutp = new ASUTPEntities())
            {
                using (Trubodetal189Entities local = new Trubodetal189Entities())
                {
                    IQueryable<Tasker.ProductionTask> query = from b in local.ProductionTasks
                                                    where b.Status == "f"
                                                    select b;
                    foreach(ProductionTask task in query)
                    {
                        try
                        {
                            Task targetTask = asutp.Tasks.Single(e => e.ID == task.ID);
                            targetTask.PiceAmount = task.PiceAmount;
                            targetTask.Status = "s";
                            task.Status = "s";
                        }
                        catch (InvalidOperationException)
                        {

                        }
                    }

                    local.SaveChanges();
                }
                asutp.SaveChanges();
            }

        }
    }
}
