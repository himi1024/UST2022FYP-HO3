using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootRollHelper
{

    public LootSO lootDetails;
    private float totalWeight = 0;
    private List<float> lootSpawnWeightList;
    private List<RollItem> lootItemsList;



    public LootRollHelper(LootSO lootDetails)
    {
        this.lootDetails = lootDetails;
        this.lootSpawnWeightList = new List<float>();
        lootItemsList = lootDetails.rollItems;
        foreach (RollItem rollItem in lootItemsList)
            totalWeight += rollItem.chance;
        foreach (RollItem rollItem in lootItemsList)
            lootSpawnWeightList.Add(rollItem.chance / totalWeight);
    }

    public LootData Roll() {
        LootData lootData = new LootData();
        float randomValue = Random.Range(0f, 1f);
        //Debug.Log(randomValue + " " + lootDetails.chance);
        //Debug.Log(randomValue + " " + lootDetails.chance);
        if (randomValue > lootDetails.chance) //drop this item?
            return null;
        randomValue = Random.Range(0f, 1f);
        int rollIndex = 0;
        foreach (float weight in lootSpawnWeightList) // drop which component
        {
            if (randomValue <= weight) {
                RollItem rollItem = lootItemsList[rollIndex];
                if (lootDetails.hasNameTag)
                {
                    NameTagRollHelper nametagRoll = new NameTagRollHelper(lootDetails.nameTagSO);
                    NameTagType nameTag = nametagRoll.Roll();
                    lootData.lootType = lootDetails.lootType;
                    lootData.nameTagType = nameTag;
                    lootData.isWeapon = true;
                    lootData.gunPart = rollItem.gunpart;
                    lootData.sprite = rollItem.sprite;
                    Debug.Log(nameTag + " " + rollItem.sprite.name);
                    return lootData;
                   
                }
                else {
                    lootData.lootType = LootType.health;
                    lootData.isWeapon = false;
                    lootData.nameTagType = NameTagType.None;
                    lootData.gunPart = GunPart.None;
                    lootData.sprite = rollItem.sprite;
                    Debug.Log("Health");
                    return lootData;

                }

            }
            randomValue -= weight;
            rollIndex++;
        }
        
        return null;

        //return null;
    }
}

public class NameTagRollHelper {
    public NameTagSO nameTagDetails;
    private float totalWeight = 0;
    private List<float> ntSpawnWeightList;
    //private List<RollItem> ntList;



    public NameTagRollHelper(NameTagSO nameTag)
    {
        this.nameTagDetails = nameTag;
        this.ntSpawnWeightList = new List<float>();

        totalWeight += nameTagDetails.powerChance;
        totalWeight += nameTagDetails.speedChance;
        totalWeight += nameTagDetails.capacityChance;
        totalWeight += nameTagDetails.holyChance;

        ntSpawnWeightList.Add(nameTagDetails.powerChance / totalWeight);
        ntSpawnWeightList.Add(nameTagDetails.speedChance / totalWeight);
        ntSpawnWeightList.Add(nameTagDetails.capacityChance / totalWeight);
        ntSpawnWeightList.Add(nameTagDetails.holyChance / totalWeight);

    }

    public NameTagType Roll()
    {
        float randomValue = Random.Range(0f, 1f);
        //Debug.Log(randomValue + " " + lootDetails.chance);
        if (randomValue > nameTagDetails.nameTagChance) //create this tag?
            return NameTagType.None;
        randomValue = Random.Range(0f, 1f);
        int rollIndex = 0;
        //string returnString = "";
        foreach (float weight in ntSpawnWeightList) // drop which component
        {
            if (randomValue <= weight)
            {
                switch (rollIndex) {
                    case 0:
                        return NameTagType.Power;
                    case 1:
                        return NameTagType.Speed;
                    case 2:
                        return NameTagType.Capacity;
                    case 3:
                        return NameTagType.Holy;
                }
                break;
            }
            randomValue -= weight;
            rollIndex++;
        }

        return NameTagType.None;
    }



}

public class LootData
{
    public bool isWeapon = false;
    public LootType lootType = LootType.health;
    public NameTagType nameTagType = NameTagType.None;
    public GunPart gunPart = GunPart.None;
    public Sprite sprite;


}


