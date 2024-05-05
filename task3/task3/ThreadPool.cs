using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace task3
{
    public class ThreadPool : IDisposable
    {
        private volatile int ThreadCount;
        public List<Thread> Pool;
        public Dictionary<int, IDEQueue<IMyTask>> TaskQueues;
        private CancellationTokenSource TokenSource;
        Random Random = new Random();
        public ThreadPool(int numThreads, bool sharing)
        {
            if (numThreads <= 0)
                throw new ArgumentOutOfRangeException(nameof(numThreads), "Number of threads must be greater than 0.");
            
            ThreadCount = numThreads;
            TokenSource = new CancellationTokenSource();
            TaskQueues = new Dictionary<int, IDEQueue<IMyTask>>();
            Pool = new List<Thread>();
            
            for (int i = 0; i < ThreadCount; i++)
            {
                IMyThread thr;
                if (sharing)
                    thr = new SharingThread(TaskQueues);
                else
                    thr = new StealingThread(TaskQueues);
                Thread poolMember = new Thread(() => thr.Run(TokenSource.Token));
                TaskQueues.Add(poolMember.ManagedThreadId, new BDEQueue());
                Pool.Add(poolMember);
            }
        }

        public void Start()
        {
            Pool.ForEach(thread => thread.Start());
        }

        public void Enqueue<T>(MyTask<T> task)
        {
            int index = Random.Next(TaskQueues.Keys.ToList().Count);

            task.SetPool(this);

            lock (TaskQueues[TaskQueues.Keys.ToList()[index]])
            {
                TaskQueues[TaskQueues.Keys.ToList()[index]].Enqueue(task);
            }
        }

        public void Stop()
        {
            TokenSource.Cancel();
            Pool.ForEach(thread =>
             {
                if (thread.IsAlive)
                    thread.Join();
             });
        }

        public void Dispose()
        {
            Stop();
            TokenSource.Dispose();
        }
    }
}
