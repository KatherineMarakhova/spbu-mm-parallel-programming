using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using task4.Models;

namespace task4.Models
{
    public class FineGrainedSet<T>:IExamSystem<T>
    {
        private Node<T> head;

        public FineGrainedSet()
        {
            head = new Node<T>(int.MinValue);
            head.Next = new Node<T>(Int32.MaxValue);
        }

        public bool Add(T item)
        {
            int key = item.GetHashCode();
            head.Lock.WaitOne();
            Node<T> pred = head;
            try
            {
                Node<T> curr = pred.Next;
                curr.Lock.WaitOne();
                try
                {
                    while (curr.Key < key)
                    {
                        pred.Lock.ReleaseMutex();
                        pred = curr;
                        curr = curr.Next;
                        curr.Lock.WaitOne();
                    }
                    if (curr.Key == key)
                    {
                        return false;
                    }
                    Node<T> newNode = new Node<T>(item);
                    newNode.Next = curr;
                    pred.Next = newNode;
                    return true;
                }
                finally
                {
                    curr.Lock.ReleaseMutex();
                }
            }
            finally
            {
                pred.Lock.ReleaseMutex();
            }
        }

        public bool Remove(T item)
        {
            Node<T> pred = null, curr = null;
            int key = item.GetHashCode();
            head.Lock.WaitOne();
            try
            {
                pred = head;
                curr = pred.Next;
                curr.Lock.WaitOne();
                try
                {
                    while (curr.Key < key)
                    {
                        pred.Lock.ReleaseMutex();
                        pred = curr;
                        curr = curr.Next;
                        curr.Lock.WaitOne();
                    }
                    if (curr.Key == key)
                    {
                        pred.Next = curr.Next;
                        return true;
                    }
                    return false;
                }
                finally
                {
                    curr.Lock.ReleaseMutex();
                }
            }
            finally
            {
                pred.Lock.ReleaseMutex();
            }
        }

        public bool Contains(T item)
        {
            Node<T> pred = null, curr = null;
            int key = item.GetHashCode();
            head.Lock.WaitOne();
            try
            {
                pred = head;
                curr = pred.Next;
                curr.Lock.WaitOne();
                try
                {
                    while (curr.Key < key)
                    {
                        pred.Lock.ReleaseMutex();
                        pred = curr;
                        curr = curr.Next;
                        curr.Lock.WaitOne();
                    }
                    return curr.Key == key;
                }
                finally
                {
                    curr.Lock.ReleaseMutex();
                }
            }
            finally
            {
                pred.Lock.ReleaseMutex();
            }
        }

        public int Count()
        {
            int size = 0;
            Node<T> cur = head;
            while (cur.Next != null)
            {
                size++;
                cur = cur.Next;
            }
            return size - 1;
        }
    }
}