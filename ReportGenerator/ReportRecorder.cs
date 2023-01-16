namespace NIASReport
{
    public class ReportRecorder : IDisposable
    {
        public event ErrorEventHandler? ErrorHandler;

        private static readonly TimeSpan ExpireTime = TimeSpan.FromDays(30);
        private static readonly TimeSpan CheckTime = TimeSpan.FromDays(1);

        private readonly Queue<KeyValuePair<Type, object>> insertBuffer;
        private readonly Queue<KeyValuePair<Type, object>> updateBuffer;
        private readonly Task task;
        private readonly CancellationTokenSource cancellation;

        public ReportRecorder()
        {
            insertBuffer = new Queue<KeyValuePair<Type, object>>();
            updateBuffer = new Queue<KeyValuePair<Type, object>>();
            cancellation = new CancellationTokenSource();
            task = new Task(Procedure, cancellation.Token);
        }

        public void Start()
        {
            task.Start();
        }

        private async void Procedure()
        {
            DateTimeOffset timestamp = DateTimeOffset.MinValue;

            while (!cancellation.Token.IsCancellationRequested)
            {
                if (insertBuffer.Count > 0)
                {
                    try
                    {
                        KeyValuePair<Type, object> data = insertBuffer.Dequeue();
                        if (DatabaseHelper.Instance != null && data.Key != null && data.Value != null)
                        {
                            await DatabaseHelper.Instance.SaveData(data.Key, data.Value);
                        }
                    }
                    catch (Exception e)
                    {
                        ErrorHandler?.Invoke(this, new ErrorEventArgs(e));
                    }
                }

                if (updateBuffer.Count > 0)
                {
                    try
                    {
                        KeyValuePair<Type, object> data = updateBuffer.Dequeue();
                        if (DatabaseHelper.Instance != null && data.Key != null && data.Value != null)
                        {
                            await DatabaseHelper.Instance.ClearTable(data.Key);
                            await DatabaseHelper.Instance.SaveData(data.Key, data.Value);
                        }
                    }
                    catch (Exception e)
                    {
                        ErrorHandler?.Invoke(this, new ErrorEventArgs(e));
                    }
                }

                if (DateTimeOffset.Now - timestamp > CheckTime)
                {
                    timestamp = DateTimeOffset.Now;
                    if (DatabaseHelper.Instance != null)
                    {
                        await DatabaseHelper.Instance.DeleteExpiredRecord(timestamp - ExpireTime);
                    }
                }

                Thread.Sleep(100);
            }
        }

        public void AddData<T>(IEnumerable<T> list)
        {
            try
            {
                insertBuffer.Enqueue(new KeyValuePair<Type, object>(typeof(T), list));
            }
            catch (Exception e)
            {
                ErrorHandler?.Invoke(this, new ErrorEventArgs(e));
            }
        }
        public void UpdateData<T>(IEnumerable<T> list)
        {
            try
            {
                updateBuffer.Enqueue(new KeyValuePair<Type, object>(typeof(T), list));
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