using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDrop : MonoBehaviour
{
    private Monster monster;
    private MonsterDetailSO monsterDetailSO;
    // Start is called before the first frame update
    void Start()
    {
        monster = GetComponent<Monster>();
        monsterDetailSO = monster.monsterDetail;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Roll() {
        float randomValue = Random.Range(0f,1f);
        
        //Debug.Log(randomValue);
        if (randomValue > monsterDetailSO.lootDropChance) { Debug.Log("No loot dropping"); return; }
        else {
            LootRollHelper lootRollHelper = new LootRollHelper(monsterDetailSO.lootItems[Random.Range(0, monsterDetailSO.lootItems.Count)]);
            LootData lootData = lootRollHelper.Roll();
            //Debug.Log(lootData);
            if (lootData == null)
                return;
            GameObject lootObject = ObjectPooling.Instance.GetGameObject(GameResources.Instance.lootPrefab);
            lootObject.transform.GetComponent<Loot>()?.Initialize(lootData.lootType, lootData.isWeapon, lootData.nameTagType, lootData.gunPart, lootData.sprite, transform.position);
        }

    
    }

}
