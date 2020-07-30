using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasker.ModelView
{
    public class ErrorScroller
    {
        public ErrorScroller()
        {
            
        }
        public ErrorItem CurrentError { get; set; }
        public void AddError() { }
    }
    public class ErrorItem
    {
        public ErrorItem()
        {

        }
        public string Message { get; set; }
    }
}
