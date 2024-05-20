﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace task4.Models
{
    public class Node<T>
    {
        public T Value;
        public int Key;
        public Node<T> Next;
        public Mutex Lock = new Mutex();
        public bool Marked = false;

        public Node(int key)
        {
            Key = key;
        }

        public Node(T value)
        {
            Value = value;
            Key = Value.GetHashCode();
        }

        public override int GetHashCode()
        {
            return Key;
        }
    }
}