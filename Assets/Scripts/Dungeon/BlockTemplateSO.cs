using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu(fileName = "Block_", menuName = "Scriptable Objects/Dungeon/Block")]
public class BlockTemplateSO : ScriptableObject
{
    [HideInInspector] public string guid; // guid for the block

    public GameObject blockPrefab;     // prefab for the block

    [HideInInspector] public GameObject previousBlockPrefab; //  regenerate guid if prefab changed


    public BlockTypeSO blockType;   //block type

    public Vector2Int lowerBound;  //lower bound of the block

    public Vector2Int upperBound; //upper bound of the block


    [SerializeField] public List<GatePosition> gateList; //lists of the gate


    public Vector2Int[] enemyPositionArr; //spawning loactions list

    [SerializeField] public List<EnemySpawnList> enemySpawnLists; // list of enemy spawn infomation

    [SerializeField] public EnemySpawnConstraint enemySpawnConstraint; // list of enemy spawn infomation

    public MusicSO musicBattle;
    public MusicSO musicNobattle;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (guid != "" && previousBlockPrefab == blockPrefab) return;

        guid = GUID.Generate().ToString();
        previousBlockPrefab = blockPrefab;
        EditorUtility.SetDirty(this);
        
    }
#endif
}


[System.Serializable]
public class EnemySpawnInfo{
    public MonsterDetailSO monsterDetailSO;
    public float spawnWeight;

}

[System.Serializable]
public class EnemySpawnList {
    public LevelSO levelDetails;
    public List<EnemySpawnInfo> enemySpawnInfoList;

}


[System.Serializable]

public class EnemySpawnConstraint {
    
    public int maxEnemy;
    public int minEnemy;
    public int maxEnemyAvailable;
    public int minEnemyAvailable;
    public float minEnemySpawnInterval;
    public float maxEnemySpawnInterval;



}

