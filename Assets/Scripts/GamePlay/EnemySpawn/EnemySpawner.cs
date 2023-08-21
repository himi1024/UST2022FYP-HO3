using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner
{

    public List<EnemySpawnList> enemySpawnLists;
    private float totalWeight = 0;
    private List<float> enemySpawnWeightList;
    private List<EnemySpawnInfo> enemySpawnInfoList;



    public EnemySpawner(List<EnemySpawnList> enemySpawnLists) {
        this.enemySpawnLists = enemySpawnLists;
        this.enemySpawnWeightList = new List<float>();
        foreach (EnemySpawnList enemySpawnList in enemySpawnLists) {
            if (enemySpawnList.levelDetails == GameManager.Instance.levels[GameManager.Instance.dungeonLevelIndex]) {
                enemySpawnInfoList = enemySpawnList.enemySpawnInfoList;
                foreach (EnemySpawnInfo enemySpawnInfo in enemySpawnList.enemySpawnInfoList)
                    totalWeight += enemySpawnInfo.spawnWeight;
                foreach (EnemySpawnInfo enemySpawnInfo in enemySpawnList.enemySpawnInfoList)
                    enemySpawnWeightList.Add(enemySpawnInfo.spawnWeight / totalWeight);
            }
        }
    }

    public EnemySpawnInfo getRandomEnemy() {
        float randomValue = Random.Range(0f, 1f);
        int spawnIndex = 0;
        foreach (float weight in enemySpawnWeightList)
        {
            if (randomValue <= weight) return enemySpawnInfoList[spawnIndex];
            randomValue -= weight;
            spawnIndex++;
        }
        
        return null;
    }
    
}
