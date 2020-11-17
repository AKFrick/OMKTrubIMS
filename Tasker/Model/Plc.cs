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
        public bool SendTask(ProductionTask task)
        {
            opc.SendTask(task);
            return true;
            //try
            //{
            //    opc.SendTask(task);
            //    errorScroller?.RemoveError(plcConnectionError);
            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    errorScroller?.AddError(plcConnectionError);
            //    Log.logThis(ex.Message);
            //    return false;
            //}
        }
        public TaskResult GetCurrentTaskResult()
        {
            return opc.GetCurrentTaskResult();
        }
    }
}
