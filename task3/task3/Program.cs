using System;
using System.Threading;

namespace task3
{
    class Program
    {
        static Random Random;
        private int numThreads = 4;
        static void Main(string[] args)
        {
            Program prog = new Program();
            Random = new Random();
            Console.WriteLine("This program is implementing ThreadPool and putting it through some tests");
            Console.WriteLine("Thread pool is working with " + prog.numThreads + " threads.");
            bool mode = true;
            prog.TestTasks(mode, 1);
            prog.TestTasks(mode, 500);
            mode = false;
            prog.TestTasks(mode, 1);
            prog.TestTasks(mode, 500);
            prog.TestAmountOfThreads(mode);
            prog.TestOnceContinuation(mode);
            prog.TestTriceContinuation(mode);
            Console.WriteLine("Processing finished. Press any key to exit.");
            Console.ReadKey();
        }

        public void TestTasks(bool mode, int numTasks)
        {
            ThreadPool pool = new ThreadPool(numThreads, mode);
            for (int i = 0; i < numTasks; i++)
            {
                MyTask<int> newTask = new MyTask<int>(()=>0);
                pool.Enqueue(newTask);
            }
            int TaskCount = 0;

            foreach (IDEQueue<IMyTask> TaskQueue in pool.TaskQueues.Values)
                TaskCount += TaskQueue.Count();

            pool.Dispose();
            Console.WriteLine("Test Tasks with mode " + mode + " and num of tasks " + numTasks + " : " + (TaskCount == numTasks ? "Passed" : "Failed"));
        }

        public void TestAmountOfThreads(bool mode)
        {
            ThreadPool pool = new ThreadPool(numThreads, mode);
            Console.WriteLine("Test Amount Of Threads (" + numThreads + ") : " + (pool.Pool.Count == numThreads ? "Passed" : "Failed"));
        }

        public void TestOnceContinuation(bool mode)
        {
            const int currentValue = 1;
            ThreadPool pool = new ThreadPool(numThreads, mode);
            pool.Start();
            MyTask<int> task = new MyTask<int>(() => currentValue);
            pool.Enqueue(task);
            MyTask<int> newTask = task.ContinueWith(result => result - (int)(result / 2) * 2);
            Thread.Sleep(100);
            pool.Dispose();
            Console.WriteLine("Test Once Continuation " + (currentValue % 2 == newTask.GetResult() ? "Passed" : "Failed"));
        }
        public void TestTriceContinuation(bool mode)
        {
            ThreadPool pool = new ThreadPool(numThreads, mode);
            int currentValue = 4;
            pool.Start();
            MyTask<int> task = new MyTask<int>(() => currentValue);
            pool.Enqueue(task);
            MyTask<int> newTask = task.ContinueWith(result => result * 2).ContinueWith(result => result + 1).ContinueWith(result => result + 7);
            Thread.Sleep(100);
            pool.Dispose();
            Console.WriteLine("Test Trice Continuation " + (16 == newTask.GetResult() ? "Passed" : "Failed"));
        }
    }
}
