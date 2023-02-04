using System;
using UnityEngine;

namespace Game
{
    public class Counter : ScriptableObject
    {
        public uint Count { get; private set; }
        public event Action<uint> EventUpdateCount;
        public void Add()
        {
            Count += 1;
            EventUpdateCount.Invoke(Count);
        }
    }
}