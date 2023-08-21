using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NameTagSO", menuName = "Scriptable Objects/Weapon System/NameTag")]
public class NameTagSO : ScriptableObject
{
    /*public bool isPistol = false;
    public bool isRifle = false;
    public bool isLaser = false;
    public bool isShotGun = false;
    public bool isRPG = false;


    public NameTagType nameTagType;
    public float chance = 0.1f;*/
    [Header("Chance to get a name tag")]
    public float nameTagChance = 0.5f;

    [Header("Chance of rolling different nameTag")]
    public float powerChance = 0.1f;
    public float speedChance = 0.1f;
    public float capacityChance = 0.1f;
    public float holyChance = 0.1f;

}

[System.Serializable]
public enum NameTagType { 

    Power,
    Speed,
    Capacity,
    Holy,
    None

}


