using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Heap<T> where T : IHeapItem<T>
{
    T[] items;
    int count;


    public int Count
    {
        get { return count; }
    }
    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = count;
        items[count] = item;
        SortUP(item);
        count++;
    }

    public T RemoveFirst() {
        T firstItem = items[0];
        count--;
        items[0] = items[count];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    
    }

   
    void SortDown(T item) {
        while (true) {
            int childLeftIndex = item.HeapIndex * 2 + 1;
            int childRightIndex = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if (childLeftIndex < count)
            {
                swapIndex = childLeftIndex;
                if (childRightIndex < count)
                {
                    if (items[childLeftIndex].CompareTo(items[childRightIndex]) < 0)
                        swapIndex = childRightIndex;
                }

                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else return;

            }
            else return;
        }

    }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);

    }

    public void UpdateItem(T item) {
        SortUP(item);
    
    }

    void SortUP(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;
        while (true)
        {
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);

            }
            else break;

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int tempIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = tempIndex;

    }
}

public interface IHeapItem<T> : IComparable<T> {
    int HeapIndex {
        get;
        set;
    
    }
}


