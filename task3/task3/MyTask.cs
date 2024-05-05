﻿using System;
using System.Threading;

namespace task3
{
    public class MyTask<T> : IMyTask
    {
        private Func<T> Job;
        private ThreadPool MyPool;
        public bool IsCompleted;
        private T Result;

        public MyTask(Func<T> newJob)
        {
            IsCompleted = false;
            Result = default(T);
            Job = newJob;
        }

        public void SetPool(ThreadPool pool)
        {
            MyPool = pool;
        }

        public void Start()
        {
            IsCompleted = false;
            try
            {
                Result = Job();
                IsCompleted = true;
            }
            catch (Exception E)
            {
                IsCompleted = false;
                throw new AggregateException(E);
            }
        }

        public T GetResult()
        {
            lock (Result)
            {
                return Result;
            }
        }
        
        public MyTask<TNew> ContinueWith<TNew>(Func<T, TNew> func)
        {
            {
                if (!IsCompleted)
                {
                    int timer = 50;
                    while (!IsCompleted && timer < 5000)
                    {
                        Thread.Sleep(timer);
                        timer *= 10;
                    }
                    if (!IsCompleted)
                        throw new TimeoutException("Too much time after starting");
                }
                MyTask<TNew> newTask = new MyTask<TNew>(() => func(GetResult()));
                MyPool.Enqueue(newTask);
                return newTask;
            }
        }
    }
}