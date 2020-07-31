using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tasker.ModelView
{
    public class ErrorScroller
    {
        public ErrorScroller()
        {
            errorList = new List<ErrorItem>();
            AddError(new ErrorItem("Пусто!"));
            //ShowNextError();
        }
        public Action RaiseErrorChanged;
        public ErrorItem CurrentError { get; set; }
        public void AddError(ErrorItem error)
        {
            errorList.Add(error);
            CurrentError = errorList.ElementAt(0);
            RaiseErrorChanged?.Invoke();
        }
        List<ErrorItem> errorList;
        Thread scrollingThread;
        void StartScrolling()
        {
            if (scrollingThread == null)
            {
                scrollingThread = new Thread(() =>
                {
                    while (true)
                    {
                        ShowNextError();                
                        Thread.Sleep(3000);
                    }
                });
            }
        }
        int currentErrorIndex = 0;
        void ShowNextError()
        {            
            if (errorList.Count - 1 > currentErrorIndex)
                currentErrorIndex++;
            else currentErrorIndex = 0;
            CurrentError = errorList.ElementAt(currentErrorIndex);

            RaiseErrorChanged();
        }
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
