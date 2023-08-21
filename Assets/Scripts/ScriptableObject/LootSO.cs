using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[System.Serializable]
public enum GunPart
{
    isBody,
    isMuzzle,
    isGrip,
    None
}

[System.Serializable]
public enum LootType
{

    pistol,
    rifle,
    shotgun,
    rpg,
    laser,
    health,
    none
}


[CreateAssetMenu(fileName = "LootSO", menuName = "Scriptable Objects/Weapon System/LootSO")]
public class LootSO : ScriptableObject
{
    public float chance = 0.5f;


    [SerializeField]
    public List<RollItem> rollItems;
    public LootType lootType;
    public bool hasNameTag = true;
    public NameTagSO nameTagSO;

}

[System.Serializable]
public class RollItem{
    public Sprite sprite;
    public float chance;
    public GunPart gunpart;

}

