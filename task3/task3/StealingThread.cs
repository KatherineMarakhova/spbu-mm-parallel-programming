using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace task3
{
    class StealingThread : IMyThread
    {
        private readonly Dictionary<int, IDEQueue<IMyTask>> Queue;
        private readonly Random Random;
        public StealingThread(Dictionary<int, IDEQueue<IMyTask>> queue)
        {
            Queue = queue ?? throw new ArgumentNullException(nameof(queue));
            Random = new Random();
        }

        public void Run(CancellationToken token)
        {
            int currentThreadId = Thread.CurrentThread.ManagedThreadId;
            IMyTask task = Queue[currentThreadId].PopBottom();
            while (!token.IsCancellationRequested)
            {
                while (task != null)
                {
                    task.Start();
                    task = Queue[currentThreadId].PopBottom();
                }
                while (task == null)
                {
                    Thread.Yield();
                    int victim = Queue.Keys.ToList()[Random.Next(Queue.Keys.Count)];
                    if (!Queue[victim].IsEmpty())
                    {
                        task = Queue[victim].PopTop();
                    }
                    if (!Queue[currentThreadId].IsEmpty())
                    {
                        task = Queue[currentThreadId].PopTop();
                    }
                    Thread.Sleep(60);
                    break;
                }
            }
        }
        }
    }
