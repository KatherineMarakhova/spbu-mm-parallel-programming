using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace task3
{
    class SharingThread : IMyThread
    {
        private readonly Dictionary<int, IDEQueue<IMyTask>> Queue;
        private readonly Random Random;
        private const int THRESHOLD = 2;
        public SharingThread(Dictionary<int, IDEQueue<IMyTask>> queue)
        {
            Queue = queue ?? throw new ArgumentNullException(nameof(queue));
            Random = new Random();
        }

        public void Run(CancellationToken token)
        {
            int currentThreadId = Thread.CurrentThread.ManagedThreadId;
            while (!token.IsCancellationRequested)
            {
                IMyTask task = null;
                lock (Queue[currentThreadId])
                {
                    task = Queue[currentThreadId].PopTop();
                }
                
                task?.Start();

                int size = Queue[currentThreadId].Count();

                if (Random.Next(size + 1) == size && size > 1)
                {
                    int victim = GetRandomThreadId(currentThreadId);
                    if (victim == currentThreadId)
                        continue;
                    int min = (victim <= currentThreadId) ? victim : currentThreadId;
                    int max = (victim <= currentThreadId) ? currentThreadId : victim;
        
                    lock (Queue[min])
                    {
                        lock (Queue[max])
                        {
                            Balance(Queue[min], Queue[max]);
                        }
                    }
                }

                Thread.Sleep(60);
            }
        }

        private int GetRandomThreadId(int excludeThreadId)
        {
            int victim;
            do
            {
                victim = Queue.Keys.ElementAt(Random.Next(Queue.Count));
            }while (victim == excludeThreadId);

            return victim;
        }

        private void Balance(IDEQueue<IMyTask> q0, IDEQueue<IMyTask> q1)
        {
            var qMin = (q0.Count() < q1.Count()) ? q0 : q1;
            var qMax = (q0.Count() < q1.Count()) ? q1 : q0;
            int diff = qMax.Count() - qMin.Count();
            if (diff > THRESHOLD)
                while (qMax.Count() > qMin.Count())
                    qMin.Enqueue(qMax.PopTop());
        }
    }
}
