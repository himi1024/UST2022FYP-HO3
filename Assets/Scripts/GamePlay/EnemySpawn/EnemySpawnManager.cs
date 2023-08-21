using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : SingletonMonoBehaviour<EnemySpawnManager>
{
    private int maxEnemyAmount;
    private int maxEnemyAtATime;
    private float enemySpawnInterval;
    private int curEnemyAmount;
    private int totalEnemySpawned;
    private Coroutine enemySpawnCor;
    private EnemySpawnConstraint enemySpawnConstraint;
    private BlockInfo curRoom;

    private void OnEnable()
    {
        if (GameResources.Instance.LevelIndex >= 2) return;
        LevelManager.visitRoom += EnemySpawn;
    }

    private void OnDisable()
    {
        if (GameResources.Instance.LevelIndex >= 2) return;
        LevelManager.visitRoom -= EnemySpawn;
    }

    public void ManualSpawnEnemy(BlockInfo blockInfo)
    {
        Debug.Log("spawn");
        EnemySpawn(blockInfo);
    }

    private void EnemySpawn(BlockInfo blockInfo){
        Debug.Log("Start");
        Debug.Log("You are entering " + blockInfo.blockType);
        curRoom = blockInfo;
        enemySpawnConstraint = blockInfo.enemySpawnConstraint;
        curEnemyAmount = 0;
        totalEnemySpawned = 0;
        if (blockInfo.allEnemiesDefeated || blockInfo.enemySpawnLists.Count == 0) { GameManager.Instance.CheckNormalRoomsDefeated(); curRoom.allEnemiesDefeated = true;  return; }
            
        maxEnemyAmount = Random.Range(enemySpawnConstraint.minEnemy, enemySpawnConstraint.maxEnemy+1);
        maxEnemyAtATime = Random.Range(enemySpawnConstraint.minEnemyAvailable, enemySpawnConstraint.maxEnemyAvailable+1);
        //curRoom.instantiatedBlock.Battling();

        // Play battle music here
        MusicManager.Instance.Play(curRoom.musicBattle);

        enemySpawnCor = StartCoroutine(EnemySpawning());
        

    }

    private IEnumerator EnemySpawning()
    {
        yield return new WaitForSeconds(0.2f);
        curRoom.instantiatedBlock.Battling();
        Vector2Int[] spawnArr = curRoom.enemyPositionArr;
        EnemySpawner enemySpawner = new EnemySpawner(curRoom.enemySpawnLists);
        for (int i = 0; i < maxEnemyAmount; i++)
        {
            while (curEnemyAmount >= maxEnemyAtATime) { yield return null; }

            if (GameManager.Instance.gameState == GameState.playerLost)
                break;

            EnemySpawnInfo enemy = enemySpawner.getRandomEnemy();
            Vector2Int spawnPos = spawnArr[Random.Range(0, spawnArr.Length)];
            GameObject enemyGameObject = Instantiate(enemy.monsterDetailSO.monsterPrefab, curRoom.instantiatedBlock.grid.CellToWorld((Vector3Int)spawnPos), Quaternion.identity, transform);
            //GameObject enemyGameObject = ObjectPooling.Instance.GetGameObject(enemy.monsterDetailSO.monsterPrefab);
            enemyGameObject.transform.position = curRoom.instantiatedBlock.grid.CellToWorld((Vector3Int)spawnPos);
            //GameObject enemyGameObject = Instantiate(enemy.monsterDetailSO.monsterPrefab, curRoom.instantiatedBlock.grid.CellToWorld((Vector3Int)spawnPos), Quaternion.identity, transform);
            enemyGameObject.GetComponent<Health>()?.SetInitialHealth(enemy.monsterDetailSO.health);
            enemyGameObject.GetComponent<Health>().Defeated += EnemyDefeated;
            curEnemyAmount++;
            totalEnemySpawned++;
            yield return new WaitForSeconds(Random.Range(enemySpawnConstraint.minEnemySpawnInterval, enemySpawnConstraint.maxEnemySpawnInterval));

        }
    }

    private void EnemyDefeated(Health health) {
        health.Defeated -= EnemyDefeated;
        curEnemyAmount--;
        if (totalEnemySpawned == maxEnemyAmount && curEnemyAmount == 0) {
            Debug.Log("Room Clear");
            curRoom.allEnemiesDefeated = true;
            curRoom.instantiatedBlock.EndBattle();
            // play non battle music here
            MusicManager.Instance.Play(curRoom.musicNobattle);
            GameManager.Instance.CheckNormalRoomsDefeated();
        }


    }
}
