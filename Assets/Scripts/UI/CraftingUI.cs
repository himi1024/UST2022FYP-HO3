using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingUI : MonoBehaviour
{
    public Image WeaponImage;
    public Image GripImage;
    public Image BodyImage;
    public Image MuzzleImage;
    public List<spriteList> sprites;
    public TextMeshProUGUI body;
    public TextMeshProUGUI grip;
    public TextMeshProUGUI muzzle;

    public TextMeshProUGUI Craft;
    public TextMeshProUGUI Power;
    public TextMeshProUGUI tPower;
    public TextMeshProUGUI pPower;

    public TextMeshProUGUI Speed;
    public TextMeshProUGUI tSpeed;
    public TextMeshProUGUI pSpeed;

    public TextMeshProUGUI Capacity;
    public TextMeshProUGUI tCapacity;
    public TextMeshProUGUI pCapacity;

    public TextMeshProUGUI UpgradePower;
    public TextMeshProUGUI UpgradeSpeed;
    public TextMeshProUGUI UpgradeCapacity;
    //public int UpgradeStatus;// 0=Power, 1=Speed, 2=Capacity
    public List<WeaponSystem> weapon = new List<WeaponSystem>();

    public SoundEffectSO craftedEffect;

    public int index; // 0-4

    public void Start()
    {
        Debug.Log(Inventory.Instance.Pistol);
        Debug.Log(Inventory.Instance.Rifle);
        Debug.Log(Inventory.Instance.Shotgun);
        Debug.Log(Inventory.Instance.Laser);
        Debug.Log(Inventory.Instance.RPG);

        gameObject.SetActive(false);
        index = 0;
        weapon.Add(Inventory.Instance.Pistol);
        weapon.Add(Inventory.Instance.Rifle);
        weapon.Add(Inventory.Instance.Shotgun);
        weapon.Add(Inventory.Instance.Laser);
        weapon.Add(Inventory.Instance.RPG);
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    public void Update()
    {
        ShowWeapon();
    }
    public void Leftbutton()
    {
        if (index == 0)
            index = 4;
        else
            index--;
        // Refresh the page to other weapon
        Update();
    }

    public void Rightbutton()
    {
        if (index == 4)
            index = 0;
        else
            index++;
        //Refresh the page to other weapon
        Update();
    }

    public void ShowWeapon() 
    {
        // Image
        WeaponImage.sprite = sprites[index].Sprite[0];
        BodyImage.sprite= sprites[index].Sprite[1];
        GripImage.sprite = sprites[index].Sprite[2];
        MuzzleImage.sprite = sprites[index].Sprite[3];

        // Amount && CraftButton
        if (!weapon[index].Crafted)
        {
            body.text = weapon[index].Body + " / " + weapon[index].ComponentRequiredB;
            grip.text = weapon[index].Grip + " / " + weapon[index].ComponentRequiredG;
            muzzle.text = weapon[index].Muzzle + " / " + weapon[index].ComponentRequiredM;

            Power.text = "X";
            Speed.text = "X";
            Capacity.text = "X";
            if (weapon[index].ComponentRequiredB <= weapon[index].Body
                && weapon[index].ComponentRequiredG <= weapon[index].Grip
                && weapon[index].ComponentRequiredM <= weapon[index].Muzzle)
            {
                Craft.text = "Craft";
            }
            else
            {
                Craft.text = "X";
            }
        }
        else
        {
            body.text = grip.text = muzzle.text = "Crafted!";
            Craft.text = "Already Crafted!";
        }
            Power.text = weapon[index].Power.status+ "";
            tPower.text = weapon[index].Power.UpgradedTime + "";
            pPower.text = weapon[index].Power.UpgradePoint + " / " + weapon[index].Power.RequiredUpgradePoint();

            Speed.text = weapon[index].Speed.status + "";
            tSpeed.text = weapon[index].Speed.UpgradedTime + "";
            pSpeed.text = weapon[index].Speed.UpgradePoint + " / " + weapon[index].Speed.RequiredUpgradePoint();

            Capacity.text = weapon[index].Capacity.status + "";
            tCapacity.text = weapon[index].Capacity.UpgradedTime + "";
            pCapacity.text = weapon[index].Capacity.UpgradePoint + " / " + weapon[index].Capacity.RequiredUpgradePoint();

            if (weapon[index].Power.RequiredUpgradePoint() <= weapon[index].Power.UpgradePoint)
                UpgradePower.text = "Upgrade";
            else
                UpgradePower.text = "X";
            if (weapon[index].Speed.RequiredUpgradePoint() <= weapon[index].Speed.UpgradePoint)
                UpgradeSpeed.text = "Upgrade";
            else
                UpgradeSpeed.text = "X";
            if (weapon[index].Capacity.RequiredUpgradePoint() <= weapon[index].Capacity.UpgradePoint)
                UpgradeCapacity.text = "Upgrade";
            else
                UpgradeCapacity.text = "X";

        
    }

    public void CraftButton()
    {
        if(Craft.text == "Craft")
        {
            switch (index)
            {
                case 0:
                    Inventory.Instance.Pistol.Crafted = true;
                    break;
                case 1:
                    Inventory.Instance.Rifle.Crafted = true;
                    break;
                case 2:
                    Inventory.Instance.Shotgun.Crafted = true;
                    break;
                case 3:
                    Inventory.Instance.Laser.Crafted = true;
                    break;
                case 4:
                    Inventory.Instance.RPG.Crafted = true;
                    break;
            }
            SoundEffectManager.Instance.Play(craftedEffect);
            Update();
        }
    }

    public void UpgradeStatus(int statusIndex)
    {
            switch (statusIndex)
            {
                case 0: // Power
                    if (UpgradePower.text == "Upgrade")
                    {
                        if (index == 0 && Inventory.Instance.Pistol.Crafted)
                        {
                            Inventory.Instance.Pistol.Power.Upgrade();
                            int diff = (int)Inventory.Instance.Pistol.Power.status - Inventory.Instance.pistolSO.bulletDetails.minDamage;
                            Inventory.Instance.pistolSO.bulletDetails.minDamage = Mathf.RoundToInt(Inventory.Instance.Pistol.Power.status);
                            Inventory.Instance.pistolSO.bulletDetails.maxDamage += diff;
                            SoundEffectManager.Instance.Play(craftedEffect);
                        }
                        else if (index == 1 && Inventory.Instance.Rifle.Crafted)
                        {
                            Inventory.Instance.Rifle.Power.Upgrade();
                            int diff = (int)Inventory.Instance.Rifle.Power.status - Inventory.Instance.rifleSO.bulletDetails.minDamage;
                            Inventory.Instance.rifleSO.bulletDetails.minDamage = Mathf.RoundToInt(Inventory.Instance.Rifle.Power.status);
                            Inventory.Instance.rifleSO.bulletDetails.maxDamage += diff;
                            SoundEffectManager.Instance.Play(craftedEffect);
                        }
                        else if (index == 2 && Inventory.Instance.Shotgun.Crafted)
                        {
                            Inventory.Instance.Shotgun.Power.Upgrade();
                            int diff = (int)Inventory.Instance.Shotgun.Power.status - Inventory.Instance.shotgunSO.bulletDetails.minDamage;
                            Inventory.Instance.shotgunSO.bulletDetails.minDamage = Mathf.RoundToInt(Inventory.Instance.Shotgun.Power.status);
                            Inventory.Instance.shotgunSO.bulletDetails.maxDamage += diff;
                            SoundEffectManager.Instance.Play(craftedEffect);
                        }
                        else if (index == 3 && Inventory.Instance.Laser.Crafted)
                        {
                            Inventory.Instance.Laser.Power.Upgrade();
                            int diff = (int)Inventory.Instance.Laser.Power.status - Inventory.Instance.lasergunSO.bulletDetails.minDamage;
                            Inventory.Instance.lasergunSO.bulletDetails.minDamage = Mathf.RoundToInt(Inventory.Instance.Laser.Power.status);
                            Inventory.Instance.lasergunSO.bulletDetails.maxDamage += diff;
                            SoundEffectManager.Instance.Play(craftedEffect);
                        }
                        else if (index == 4 && Inventory.Instance.RPG.Crafted)
                        {
                            Inventory.Instance.RPG.Power.Upgrade();
                            int diff = (int)Inventory.Instance.RPG.Power.status - Inventory.Instance.RPGSO.bulletDetails.minDamage;
                            Inventory.Instance.RPGSO.bulletDetails.minDamage = Mathf.RoundToInt(Inventory.Instance.RPG.Power.status);
                            Inventory.Instance.RPGSO.bulletDetails.maxDamage += diff;
                            SoundEffectManager.Instance.Play(craftedEffect);
                        }
                    }
                    break;
                
                case 1: // Speed
                if (UpgradeSpeed.text == "Upgrade")
                {
                    if (index == 0 && Inventory.Instance.Pistol.Crafted)
                    {
                        Inventory.Instance.Pistol.Speed.Upgrade();
                        Inventory.Instance.pistolSO.bulletDetails.cdTime = 1 / Inventory.Instance.Pistol.Speed.status;
                        SoundEffectManager.Instance.Play(craftedEffect);
                    }
                    else if (index == 1 && Inventory.Instance.Rifle.Crafted)
                    {
                        Inventory.Instance.Rifle.Speed.Upgrade();
                        Inventory.Instance.rifleSO.shootCD = 1 / Inventory.Instance.Rifle.Speed.status;
                        SoundEffectManager.Instance.Play(craftedEffect);
                    }
                    else if (index == 2 && Inventory.Instance.Shotgun.Crafted)
                    {
                        Inventory.Instance.Shotgun.Speed.Upgrade();
                        Inventory.Instance.shotgunSO.shootCD = 1 / Inventory.Instance.Shotgun.Speed.status;
                        SoundEffectManager.Instance.Play(craftedEffect);
                    }
                    else if (index == 3 && Inventory.Instance.Laser.Crafted)
                    {
                        Inventory.Instance.Laser.Speed.Upgrade();
                        Inventory.Instance.lasergunSO.shootCD = 1 / Inventory.Instance.Laser.Speed.status;
                        SoundEffectManager.Instance.Play(craftedEffect);
                    }
                    else if (index == 4 && Inventory.Instance.RPG.Crafted)
                    {
                        Inventory.Instance.RPG.Speed.Upgrade();
                        Inventory.Instance.RPGSO.shootCD = 1 / Inventory.Instance.RPG.Speed.status;
                        SoundEffectManager.Instance.Play(craftedEffect);
                    }
                }
                    break;

                case 2: // Capacity
                if (UpgradeCapacity.text == "Upgrade")
                {
                    if (index == 0 && Inventory.Instance.Pistol.Crafted)
                    {
                        Inventory.Instance.Pistol.Capacity.Upgrade();
                        Inventory.Instance.pistolSO.maxAmmo = Inventory.Instance.Pistol.Capacity.status;
                        SoundEffectManager.Instance.Play(craftedEffect);
                    }
                    else if (index == 1 && Inventory.Instance.Rifle.Crafted)
                    {
                        Inventory.Instance.Rifle.Capacity.Upgrade();
                        Inventory.Instance.rifleSO.maxAmmo = Inventory.Instance.Rifle.Capacity.status;
                        SoundEffectManager.Instance.Play(craftedEffect);
                    }
                    else if (index == 2 && Inventory.Instance.Shotgun.Crafted)
                    {
                        Inventory.Instance.Shotgun.Capacity.Upgrade();
                        Inventory.Instance.shotgunSO.maxAmmo = Inventory.Instance.Shotgun.Capacity.status;
                        SoundEffectManager.Instance.Play(craftedEffect);

                    }
                    else if (index == 3 && Inventory.Instance.Laser.Crafted)
                    {
                        Inventory.Instance.Laser.Capacity.Upgrade();
                        Inventory.Instance.lasergunSO.maxAmmo = Inventory.Instance.Laser.Capacity.status;
                        SoundEffectManager.Instance.Play(craftedEffect);

                    }
                    else if (index == 4 && Inventory.Instance.RPG.Crafted)
                    {
                        Inventory.Instance.RPG.Capacity.Upgrade();
                        Inventory.Instance.RPGSO.maxAmmo = Inventory.Instance.RPG.Capacity.status;
                        SoundEffectManager.Instance.Play(craftedEffect);
                    }
                }
                    break;
            
        }
        Update();
        //SoundEffectManager.Instance.Play(craftedEffect);
    }
}


[System.Serializable]public class spriteList
{
    public List<Sprite> Sprite;
}
