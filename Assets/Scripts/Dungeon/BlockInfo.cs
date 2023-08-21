using System.Collections.Generic;
using UnityEngine;



public class BlockInfo
{
    public string id;
    public string templateID;
    public GameObject blockPrefab;
    public BlockTypeSO blockType;
    public Vector2Int realWorldLowerBound;
    public Vector2Int realWorldUpperBound;
    public Vector2Int templateLowerBound;
    public Vector2Int templateUpperBound;
    public Vector2Int[] enemyPositionArr;
    public List<string> childBlockIDList;
    public string parentBlockInfoID;
    public List<GatePosition> gatePositionList;
    public bool isPlaced = false;
    public InstantiatedBlock instantiatedBlock;
    public List<EnemySpawnList> enemySpawnLists;
    public EnemySpawnConstraint enemySpawnConstraint;

    public bool isLit = false;
    public bool allEnemiesDefeated = false;
    public bool isVisited = false;

    public MusicSO musicBattle;
    public MusicSO musicNobattle;

    public BlockInfo()
    {
        childBlockIDList = new List<string>();
        gatePositionList = new List<GatePosition>();
    }

}



public enum Orientation
{

    top,
    right,
    left,
    down,
    none

}


[System.Serializable]
public class GatePosition
{
    public Vector2Int position; // the position of the gate in a block
    public Orientation gatePosition; // the orientation of the gate
    public GameObject gatePrefab;   // prefab of the gate


    public Vector2Int gateDuplicateStartPosition;      // starting location to close the gate
    public int gateWidth;                               // width of the gate
    public int gateHeight;                              // height of the gate

    [HideInInspector]
    public bool isLinked = false;                       // gate is connnected 

    [HideInInspector]
    public bool isUnavailable = false;                  // gate is available 
}