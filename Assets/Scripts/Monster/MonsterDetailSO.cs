using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MonsterDetails_", menuName = "Scriptable Objects/Monster/MonsterDetails")]
public class MonsterDetailSO : ScriptableObject
{
    #region Header BASE MONSTER DETAILS
    [Space(10)]
    [Header("BASE MONSTER DETAILS")]
    #endregion

    #region Tooltip
    [Tooltip("The name of the Monster")]
    #endregion
    public string monsterName;

    #region Tooltip
    [Tooltip("The prefab for the Monster")]
    #endregion
    public GameObject monsterPrefab;

    #region Tooltip
    [Tooltip("SpawnTime")]
    #endregion
    public float spawnTime;

    #region Tooltip
    [Tooltip("Health")]
    #endregion
    public int health = 100;

    /*#region Tooltip
    [Tooltip("Blood")]
    #endregion
    public GameObject bloodParticle;*/

    #region Tooltip
    [Tooltip("Type of Monster")]
    #endregion
    public bool isNormal;
    public bool isElite;
    public bool isBoss;

    [Header("Monster Move Settings")]
    public float maxMoveSpeed;
    public float minMoveSpeed;

    public float chaseDist;

    public float chaseCD;
    public bool knockAble = true;

    #region Tooltip
    [Tooltip("Monster Shoot Settings")]
    [Header("Monster Shoot Settings")]
    #endregion

    public float minShootInterval;
    public float maxShootInterval;
    public float minShootContinue;
    public float maxShootContinue;
    public bool hasWeapon = true;
    public bool seePlayer = true;
    public float attackRange = 50f;

    [Header("Monster Loot Drop Details")]
    public float lootDropChance = 0f;
    public List<LootSO> lootItems;



}
