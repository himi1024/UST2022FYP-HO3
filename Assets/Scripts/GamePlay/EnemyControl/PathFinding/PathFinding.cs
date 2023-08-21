using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathFinding
{
    private const int HDist = 10;
    private const int DiagDist = 14;

    public static List<Vector3> PathFind(BlockInfo blockInfo, int startPositionX, int startPositionY, int endPositionX, int endPositionY) {
        PathGrid pathGrid;
        int tempStartX, tempStartY, tempEndX, tempEndY;
        PathInitialize(out pathGrid, out tempStartX, out tempStartY, out tempEndX, out tempEndY, blockInfo, startPositionX, startPositionY, endPositionX, endPositionY);

        //List<PathNode> openList = new List<PathNode>();
        //List<PathNode> closeList = new List<PathNode>();

        Heap<PathNode> openList = new Heap<PathNode>( pathGrid.MaxSize);
        HashSet<PathNode> closeList = new HashSet<PathNode>();
        List<Vector3> path = new List<Vector3>();


        PathNode startNode = pathGrid.GetGridNode(tempStartX, tempStartY);
        PathNode endNode = pathGrid.GetGridNode(tempEndX, tempEndY);



        openList.Add(startNode);

        while (openList.Count > 0) {
            PathNode curNode = GetLowestCost(openList);
            if (curNode == endNode) {
                while (curNode.parentNode != null){
                    path.Add(blockInfo.instantiatedBlock.grid.CellToWorld(new Vector3Int(curNode.x + blockInfo.templateLowerBound.x, curNode.y + blockInfo.templateLowerBound.y, 0)));
                    curNode = curNode.parentNode;
                }
                path.Reverse();
                return path;
                
            }
            //openList.Remove(curNode);
            closeList.Add(curNode);
            List<PathNode> neighbours = GetNeighbours(pathGrid, curNode);
            //Debug.Log(neighbours.Count);
            foreach (PathNode neighbour in neighbours) {
                //if (blockInfo.instantiatedBlock.isObstacle(neighbour.x, neighbour.y))
                if (closeList.Contains(neighbour) || blockInfo.instantiatedBlock.isObstacle(neighbour.x, neighbour.y)) continue;
                int extraCost = blockInfo.instantiatedBlock.isPreferredPath(neighbour.x, neighbour.y) ? 1 : 100;
                
                //int extraCost = 0;
                int newGcost = curNode.gCost + ManhattanDistCost(curNode.x, curNode.y, neighbour.x, neighbour.y) + extraCost;
                if (newGcost < neighbour.gCost) {
                    neighbour.gCost = newGcost;
                    neighbour.hCost = ManhattanDistCost(neighbour.x, neighbour.y, tempEndX, tempEndY);
                    neighbour.parentNode = curNode;

                    if (!openList.Contains(neighbour))
                        openList.Add(neighbour);
                }
            }


        }

        return null;

    }

    private static void PathInitialize(out PathGrid pathGrid, out int tempStartX, out int tempStartY, out int tempEndX, out int tempEndY, BlockInfo blockInfo, int startPositionX, int startPositionY, int endPositionX, int endPositionY) {
        pathGrid = new PathGrid(blockInfo.templateUpperBound.x - blockInfo.templateLowerBound.x + 1, blockInfo.templateUpperBound.y - blockInfo.templateLowerBound.y + 1);
        tempStartX = startPositionX - blockInfo.templateLowerBound.x;
        tempStartY = startPositionY - blockInfo.templateLowerBound.y;
        tempEndX = endPositionX - blockInfo.templateLowerBound.x;
        tempEndY = endPositionY - blockInfo.templateLowerBound.y;

    }

    private static int ManhattanDistCost(int startX, int startY, int endX, int endY) {
        int verticalDist = Mathf.Abs(startY - endY);
        int horizontalDist = Mathf.Abs(startX - endX);
        int diff = Mathf.Abs(verticalDist - horizontalDist);
        return DiagDist*Mathf.Min(verticalDist, horizontalDist) + HDist * diff;

    }

    private static PathNode GetLowestCost(List<PathNode> nodeList) {
        PathNode nodeWithLowestCost = nodeList[0];
        foreach (PathNode node in nodeList) {
            if (node.fCost < nodeWithLowestCost.fCost)
                nodeWithLowestCost = node;
        }
        return nodeWithLowestCost;

    }

    private static PathNode GetLowestCost(Heap<PathNode> nodeList)
    {
        return nodeList.RemoveFirst();

    }

    private static List<PathNode> GetNeighbours(PathGrid pathGrid, PathNode curNode){
        List<PathNode> neighbours = new List<PathNode>();
        if (curNode.x - 1 >= 0) neighbours.Add(pathGrid.GetGridNode(curNode.x - 1, curNode.y)); // Left
        if(curNode.x + 1 < pathGrid.width) neighbours.Add(pathGrid.GetGridNode(curNode.x + 1, curNode.y)); //right
        if (curNode.y + 1 < pathGrid.height) neighbours.Add(pathGrid.GetGridNode(curNode.x, curNode.y+1)); //down
        if (curNode.y - 1 >= 0) neighbours.Add(pathGrid.GetGridNode(curNode.x, curNode.y - 1));//top

        if(curNode.x - 1 >= 0 && curNode.y + 1 < pathGrid.height) neighbours.Add(pathGrid.GetGridNode(curNode.x-1, curNode.y + 1));//Top left
        if (curNode.x - 1 >= 0 && curNode.y - 1 >= 0) neighbours.Add(pathGrid.GetGridNode(curNode.x - 1, curNode.y - 1));//Down left
        if (curNode.x + 1 < pathGrid.width && curNode.y + 1 < pathGrid.height) neighbours.Add(pathGrid.GetGridNode(curNode.x + 1, curNode.y + 1));//Top right
        if (curNode.x + 1 < pathGrid.width && curNode.y - 1 >= 0) neighbours.Add(pathGrid.GetGridNode(curNode.x + 1, curNode.y - 1));//Down right

        return neighbours;



    }
}
