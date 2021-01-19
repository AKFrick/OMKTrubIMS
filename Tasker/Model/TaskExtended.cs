using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasker.Model
{
    public class TaskExtended
    {
        public Task Task { get; set; }
        public TaskExtended(ProductionTask prodTask)
        {
            Task = new Task()
            {
                CreationDate = prodTask.CreationDate,
                Diameter = prodTask.Diameter,
                Status = prodTask.Status
            };
        }
    }
}
