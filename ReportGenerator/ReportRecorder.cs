using System;
using System.Collections.Generic;

namespace NIASReport
{
    public class ReportRecorder : IDisposable
    {
        public event ErrorEventHandler? ErrorHandler;
        private readonly Queue<KeyValuePair<Type, object>> buffer;
        private readonly Task task;
        private readonly CancellationTokenSource cancellation;

        public ReportRecorder() {
            buffer = new Queue<KeyValuePair<Type, object>>();
            cancellation = new CancellationTokenSource();
            task = new Task(Procedure, cancellation.Token);
            task.Start();
        }

        private void Procedure()
        {
            while (!cancellation.Token.IsCancellationRequested)
            {
                if (buffer.Count > 0)
                {
                    try
                    {
                        KeyValuePair<Type, object> data = buffer.Dequeue();
                        DatabaseHelper.SaveData((IEnumerable<Type>)data.Value);
                    }
                    catch (Exception e)
                    {
                        ErrorHandler?.Invoke(this, new ErrorEventArgs(e));
                    }
                }

                Thread.Sleep(100);
            }
        }

        public void AddData<T>(IEnumerable<T> list)
        {
            try
            {
                buffer.Enqueue(new KeyValuePair<Type, object>(typeof(T), list));
            }
            catch (Exception e)
            {
                ErrorHandler?.Invoke(this, new ErrorEventArgs(e));
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                cancellation.Cancel();
            }
        }
    }
}