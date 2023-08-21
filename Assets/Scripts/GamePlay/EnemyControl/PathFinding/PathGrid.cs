using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode: IHeapItem<PathNode> { 
    public int x;
    public int y;
    public int gCost = int.MaxValue;
    public int hCost = 0;
    public int fCost = 0;
    public PathNode parentNode;
    int heapIndex;

    public PathNode(int x, int y, PathNode parentNode = null) {
        this.x = x;
        this.y = y;
        this.parentNode = parentNode;
    }

    public float GetFost() { return fCost = hCost + gCost; }

    public int HeapIndex {
        get{ return heapIndex; }
        set{ heapIndex = value; }

    }

    public int CompareTo(PathNode nodeToCompare) {
        int compare = GetFost().CompareTo(nodeToCompare.GetFost());
        if (compare == 0) {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;

    }
    
}

public class PathGrid {

    public PathNode[,] NodeGrid;
    public int width;
    public int height;


    public PathGrid(int x, int y) {
        width = x;
        height = y;
        NodeGrid = new PathNode[x, y];
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++) {
                NodeGrid[i, j] = new PathNode(i,j);
            }
    }

    public PathNode GetGridNode(int x, int y) {
        return NodeGrid[x, y];
    }

    public int MaxSize {
        get { return width * height; }
    }



}
