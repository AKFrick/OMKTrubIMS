using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasker.Model
{
    /// <summary> Обмен данными с PLC </summary>
    public class Plc
    {
        /// <summary> Отправить задание в ПЛК </summary>
        public bool SendTask(ProductionTask task)
        {
            return true;
        }
    }
}
