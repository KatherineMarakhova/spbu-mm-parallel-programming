using System;
using System.Collections.Generic;
using System.Threading;

namespace Semaphore_Task2_1
{
    public class ProducerConsumer
    {
        private List<int> buffer = new List<int>();
        private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public void Producer(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                semaphore.Wait();
                buffer.Add(new Random().Next(0, 100));
                semaphore.Release();
                Console.WriteLine("Thread " + Environment.CurrentManagedThreadId + " produced value.");
                Thread.Sleep(1000);
            }
        }

        public void Consumer(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                semaphore.Wait();
                if (buffer.Count > 0)
                {
                    buffer.RemoveAt(0);
                }
                semaphore.Release();
                Console.WriteLine("Thread " + Environment.CurrentManagedThreadId + " consumed value.");
                Thread.Sleep(1000);
            }
        }
    }
}
