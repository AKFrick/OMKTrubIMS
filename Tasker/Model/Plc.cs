using System;
using Tasker.ModelView;

namespace Tasker.Model
{
    /// <summary> Обмен данными с PLC </summary>
    public class Plc
    {
        
        Opc opc;
        public Plc()
        {
            opc = new Opc();
        }
        ErrorScroller errorScroller;
        public Plc(ErrorScroller errorScroller) : this()
        {
            this.errorScroller = errorScroller;
        }
        ErrorItem plcConnectionError = new ErrorItem("Ошибка подключения к ПЛК");
        /// <summary> Отправить задание в ПЛК </summary>
        public bool SendTask(ProductionTaskExtended task)
        {
            //opc.SendTask(task);
            opc.SendItemLenSet(task);
            return true;
        }
        
        public ProductionTask GetCurrentTaskResult()
        {
            return opc.GetCurrentTaskResult();
        }
    }
}
