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
            AddError(new ErrorItem("ПУсто!"));
            startScrolling();
        }
        public Action RaiseErrorChanged;
        public ErrorItem CurrentError { get; set; }
        public void AddError(ErrorItem error)
        {
            errorList.Add(error);         
        }
        public void RemoveError(ErrorItem error)
        {
            errorList.RemoveAll(item => item.Message == error.Message);
        }
        List<ErrorItem> errorList;
        Thread scrollingThread;
        void startScrolling()
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
                scrollingThread.IsBackground = true;
                scrollingThread.Start();
            }
        }
        int currentErrorIndex = 0;
        void ShowNextError()
        {            
            if (errorList.Count - 1 > currentErrorIndex)
                currentErrorIndex++;
            else currentErrorIndex = 0;
            CurrentError = errorList.ElementAt(currentErrorIndex);

            RaiseErrorChanged?.Invoke();
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
