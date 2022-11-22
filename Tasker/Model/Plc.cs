using System;
using Tasker.ModelView;
using System.Configuration;
using System.Threading;


namespace Tasker.Model
{
    /// <summary> Обмен данными с PLC </summary>
    public class Plc
    {
        readonly string lineNumber = ConfigurationManager.AppSettings.Get("LineNumber");

        Thread checkForResultJob;

        Opc opc;
        CurrentTasks currentTasks;
        public Plc(CurrentTasks currentTasks)
        {
            opc = new Opc();
            this.currentTasks = currentTasks;

            checkForResultJob = new Thread(threadTask) { IsBackground = true };
            checkForResultJob.Start();
        }
        //ErrorItem plcConnectionError = new ErrorItem("Ошибка подключения к ПЛК");
        /// <summary> Отправить задание в ПЛК </summary>
        public bool SendTask(ProductionTaskExtended task)
        {           
            opc.SendTask(task);
            //if (lineNumber == "189") opc.SendItemLenSet(task);
            return true;
        }

        void threadTask()
        {
            while (true)
            {
                GetCurrentTaskResult();                
                Thread.Sleep(20000);
            }
        }

        public async void GetCurrentTaskResult()
        {
            try
            {
                ProductionTask task = await opc.GetCurrentTaskResult();
                currentTasks.LoadTaskResult(task);
                
            }
            catch (TaskNotFoundException ex)
            {
                OutputLog.That("Нет завершенных заданий для считывания в ПЛК");
            }
            catch (Exception ex)
            {
                OutputLog.That($"Не удалось подключиться к ПЛК: {ex.Message}");
            }


        }
    }
}
