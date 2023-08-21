using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Loot : MonoBehaviour
{
    public bool isWeapon;
    public LootType lootType;
    private bool received = false;
    public NameTagType nameTagType;
    public GunPart gunPart;
    public float speed = 0f;

    public int healthRecover = 100;

    public SoundEffectSO pickEffect;


    private 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Initialize(LootType lootType, bool isWeapon, NameTagType nametag, GunPart gunPart, Sprite sprtie, Vector3 position) {
        this.isWeapon = isWeapon;
        this.lootType = lootType;
        nameTagType = nametag;
        this.gunPart = gunPart;
        received = false;
        GetComponent<SpriteRenderer>().sprite = sprtie;
        transform.position = position;
    }
    
    public 

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, GameManager.Instance.GetPlayer().playerPosition()) <= 5)
            transform.position = Vector3.MoveTowards(transform.position, GameManager.Instance.GetPlayer().playerPosition(), speed*Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (received) return;
        if (!collision.CompareTag("Player"))
            return;
        received = true;
        Apply(collision);
        ObjectPooling.Instance.PushGameObject(gameObject);


    }

    private void Apply(Collider2D collision) {
        Debug.Log("Update " + lootType + " with name tag " + nameTagType);
        if (isWeapon) {
            switch (lootType) {
                case LootType.pistol:
                    UpdateInventory(Inventory.Instance.Pistol);
                    break;
                case LootType.shotgun:
                    UpdateInventory(Inventory.Instance.Shotgun);
                    break;
                case LootType.rifle:
                    UpdateInventory(Inventory.Instance.Rifle);
                    break;
                case LootType.rpg:
                    UpdateInventory(Inventory.Instance.RPG);
                    break;
                case LootType.laser:
                    UpdateInventory(Inventory.Instance.Laser);
                    break;
                case LootType.none:
                    break;

            }
        }

        if (lootType == LootType.health)
        {
            collision.transform.GetComponent<Health>().heal(healthRecover);
            SoundEffectManager.Instance.Play(pickEffect);
        }
            
    }

    private void UpdateInventory(WeaponSystem weapon) {
        switch (gunPart) {     //update gun component value
            case GunPart.isBody:
                weapon.Body++;
                break;
            case GunPart.isGrip:
                weapon.Grip++;
                break;
            case GunPart.isMuzzle:
                weapon.Muzzle++;
                break;
        }

        switch (nameTagType)   //update upgrade point
        {     //update gun component value
            case NameTagType.Power:
                weapon.Power.UpgradePoint ++;
                Debug.Log(weapon.Power.UpgradePoint);
                break;
            case NameTagType.Speed:
                weapon.Speed.UpgradePoint++;
                Debug.Log(weapon.Speed.UpgradePoint);
                break;
            case NameTagType.Capacity:
                weapon.Capacity.UpgradePoint++;
                Debug.Log(weapon.Capacity.UpgradePoint);
                break;
            case NameTagType.Holy:
                weapon.Power.UpgradePoint++;
                weapon.Speed.UpgradePoint++;
                weapon.Capacity.UpgradePoint++;
                Debug.Log(weapon.Power.UpgradePoint);
                break;
        }

        SoundEffectManager.Instance.Play(pickEffect);
    }

}

