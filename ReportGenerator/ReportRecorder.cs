namespace NIASReport
{
    public class ReportRecorder : IDisposable
    {
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
                    var result = buffer.TryDequeue(out KeyValuePair<Type, object> data);
                }

                Thread.Sleep(100);
            }
        }

        public void AddData<T>(IEnumerable<T> list)
        {
            buffer.Enqueue(new KeyValuePair<Type, object>(typeof(T), list));
        }

        public void Dispose()
        {
            cancellation.Cancel();
        }
    }
}