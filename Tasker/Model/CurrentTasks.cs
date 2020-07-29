using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Tasker.Model
{
    public class CurrentTasks
    {
        public CurrentTasks()
        {

        }
        public ReadOnlyObservableCollection<ProductionTask> CurrentTasksCollection { get; private set; }
        List<ProductionTask> getAllCurrentTasks()
        {
            List<ProductionTask> tasks = new List<ProductionTask>();
            using (var db = new Trubodetal189Entities())
            {
                var query = from b in db.ProductionTasks select b;
                foreach (ProductionTask task in query)
                {
                    tasks.Add(task);
                }
            }
            return tasks;
        }



    }
}
