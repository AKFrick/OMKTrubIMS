using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasker.ModelView
{
    public class ErrorScroller : BindableBase
    {
        public ErrorScroller()
        {
            errorList = new List<ErrorItem>();
            CurrentError = new ErrorItem("Пусто!");
        }
        public ErrorItem CurrentError { get; set; }
        public void AddError(ErrorItem error)
        {
            errorList.Add(error);
            CurrentError = errorList.ElementAt(0);
            RaisePropertyChanged();
        }
        List<ErrorItem> errorList;
    }
    public class ErrorItem
    {
        public ErrorItem(string msg)
        {
            Message = msg;
        }
        public string Message { get; private set; }
    }
}
