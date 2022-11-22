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
        readonly string lineNumber = ConfigurationManager.AppSettings.Get("LineNumber");
        //ErrorItem connectionError = new ErrorItem("Ошибка подключения к серверу АСУТП");

        public AsutpServer()
        {
            trackingThread = new Thread(threadTask) { IsBackground = true };
            trackingThread.Start();

        }
        void threadTask()
        {
            while (true)
            {
                Check();
                UpdateTaskResult();
                Thread.Sleep(20000);
            }
        }

        public event Action<bool> ConnectionEstablished;

        public void Check()
        {
            using (Trubodetal189Entities local = new Trubodetal189Entities())
            {
                int LastID;
                try
                {
                    LastID = local.ProductionTasks.Max(p => p.ID);
                }
                catch 
                {
                    LastID = 0;
                }

                using (ASUTPEntities asutp = new ASUTPEntities() )
                {
                    try
                    {
                        //Получим новые задания от сервера
                        IQueryable<Tasker.Task> query = from b in asutp.Tasks
                                                        where b.ID > LastID && (b.Status == "0" || b.Status == "1")
                                                        select b;

                        foreach (Task task in query)
                        {

                            ProductionTaskExtended productionTask = new ProductionTaskExtended(task);
                            try
                            {
                                task.Status = "1";
                                local.ProductionTasks.Add(productionTask.Task);
                                OutputLog.That($"Новое задание загружено {task.TaskNumber}");
                            }
                            catch (Exception e)
                            {
                                task.Status = "e";
                                OutputLog.That($"Не удалось загрузить задание {task.TaskNumber}, заданию присвоен статус 'e' {e.Message}");
                            }
                        }
                        asutp.SaveChanges();


                        //Обновим статус на локальной машине, если он был изменен на сервере
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
                        ConnectionEstablished?.Invoke(true);
                    }
                    catch (Exception e)
                    {
                        OutputLog.That($"Не удалось получить список заданий от сервера АСУТП: {e.Message}");
                        ConnectionEstablished?.Invoke(false);
                    }
                }
                
            }
        }
        public void UpdateTaskResult()
        {

            using (Trubodetal189Entities local = new Trubodetal189Entities())
            {
                try
                {     
                    IQueryable<Tasker.ProductionTask> query = from b in local.ProductionTasks
                                                    where b.Status == "f"
                                                    select b;

                    using (ASUTPEntities asutp = new ASUTPEntities())
                    {
                        foreach (ProductionTask task in query)
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

                                targetTask.BandBrand = task.BandBrand;
                                targetTask.BandType = task.BandType;
                                targetTask.BandSpeed = task.BandSpeed;
                                targetTask.SawDownSpeed = task.SawDownSpeed;
                                targetTask.IDNumberOperator = task.IDOperatorNumber;
                                targetTask.LineNumber = lineNumber;
                                OutputLog.That($"Данные о задании загружены на сервер АСУТП {task.ID} {task.TaskNumber}");
                            }
                            catch (InvalidOperationException)
                            {
                                task.Status = "s";
                                try
                                {
                                    asutp.Tasks.Add(new NewAsutpTask(task, lineNumber).Task);
                                    asutp.SaveChanges();
                                    OutputLog.That($"Новое задание создано на сервере АСУТП {task.ID} {task.TaskNumber}");
                                }
                                catch (Exception e)
                                {
                                    OutputLog.That($"Не удалось создать новое задание {task.TaskNumber} на сервере АСУТП: {e.Message} ");
                                }
                            }
                            catch (Exception e)
                            {
                                OutputLog.That($"Не удалось загрузить задание {task.TaskNumber} на сервер АСУТП: {e.Message}");
                                break;
                            }
                        }
                        asutp.SaveChanges();
                        local.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    OutputLog.That($"Не подключится к локальной базе данных: {e.Message}");
                }
            }
        }
    }
}
