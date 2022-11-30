using System;
using Tasker.ModelView;
using System.Configuration;
using System.Threading;


namespace Tasker.Model
{
    /// <summary> Обмен данными с PLC </summary>
    public class Plc
    {        
        Thread checkForResultJob;

        Opc opc;
        CurrentTasks currentTasks;
        bool connectionEstablished = false;
        bool firstAttempt = true;
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
                if (firstAttempt || !connectionEstablished)
                {
                    OutputLog.That("Связь с ПЛК установлена");
                    connectionEstablished = true;
                    firstAttempt = false;
                }

            }
            catch (TaskNotFoundException ex)
            {
                if (firstAttempt || !connectionEstablished)
                {
                    OutputLog.That("Связь с ПЛК установлена");
                    connectionEstablished = true;
                    firstAttempt = false;
                }
                return;
            }
            catch (Exception ex)
            {
                if(firstAttempt || connectionEstablished)
                {
                    OutputLog.That($"Не удалось подключиться к ПЛК: {ex.Message}");
                    connectionEstablished = false;
                    firstAttempt = false;
                }
            }


        }
    }
}
