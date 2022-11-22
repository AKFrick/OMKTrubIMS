using System;
using Tasker.ModelView;
using System.Configuration;

namespace Tasker.Model
{
    /// <summary> Обмен данными с PLC </summary>
    public class Plc
    {
        readonly string lineNumber = ConfigurationManager.AppSettings.Get("LineNumber");

        Opc opc;
        public Plc()
        {
            opc = new Opc();
        }
        //ErrorItem plcConnectionError = new ErrorItem("Ошибка подключения к ПЛК");
        /// <summary> Отправить задание в ПЛК </summary>
        public bool SendTask(ProductionTaskExtended task)
        {           
            opc.SendTask(task);
            //if (lineNumber == "189") opc.SendItemLenSet(task);
            return true;
        }
        
        public ProductionTask GetCurrentTaskResult()
        {
            return null; //opc.GetCurrentTaskResult();
        }
    }
}
