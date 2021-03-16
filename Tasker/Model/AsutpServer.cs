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
    public class AsutpServer
    {
        Thread trackingThread;
        ErrorScroller errorScroller;
        ErrorItem connectionError = new ErrorItem("Ошибка подключения к серверу АСУТП");

        public AsutpServer()
        {
            trackingThread = new Thread(threadTask) { IsBackground = true };
            trackingThread.Start();

        }
        public AsutpServer(ErrorScroller errorScroller) : this()
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
                                                       where b.ID > LastID && b.Status == "0"
                                                       select b;

                    foreach (Task task in query)
                    {
                        task.Status = "1";
                        ProductionTaskExtended productionTask = new ProductionTaskExtended(task);
                        Log.logThis($"Считали идентификатор: {task.ID}");
                        local.ProductionTasks.Add(productionTask.Task);
                    }
                    asutp.SaveChanges();

                    IQueryable<ProductionTask> currentTasks = from b in local.ProductionTasks
                                                              where b.Status == "1"
                                                              select b;
                    foreach (ProductionTask productionTask in currentTasks)
                    {
                        try
                        {
                            Task task = asutp.Tasks.Single(t => t.ID == productionTask.ID);
                            if (task.Status != "1")
                            {
                                productionTask.Status = task.Status;
                            }
                        }
                        catch { }
                    }
                    local.SaveChanges();                    
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
                            targetTask.StartDate = task.StartDate;
                            targetTask.FinishDate = task.FinishDate;
                            targetTask.Operator = task.Operator;
                        }
                        catch (InvalidOperationException)
                        {
                            task.Status = "s";
                            try
                            {
                                asutp.Tasks.Add(new NewAsutpTask(task).Task);
                            }   catch(Exception e)
                            {
                                Log.logThis(e.Message);
                            }
                        }
                    }

                    local.SaveChanges();
                }
                asutp.SaveChanges();
            }

        }
    }
}
