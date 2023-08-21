using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStatus
{
    public int offset = 0;
    public int Ability = 0; // 1=Power, 2=Speed, 3=Capacity
    [HideInInspector] public int UpgradedTime = 0;
    [HideInInspector] public int UpgradePoint = 0;
    public float status = 0;
    public float maxDamage = 0;
    public float minDamage = 0;
    public float maxAmmo = 0;
    public float shootCD = 0;

    public WeaponStatus(int offset = 1, int Ability = 0, float maxDamage = 0, float minDamage = 0, float maxAmmo = 0, float shootCD = 0) 
    {
        this.Ability = Ability;
        this.offset = offset;
        this.maxDamage = maxDamage;
        this.minDamage = minDamage;
        this.maxAmmo = maxAmmo;
        this.shootCD = shootCD;

        if (Ability == 1) status = minDamage;
        else if (Ability == 2) status = 1 / shootCD;
        else if (Ability == 3) status = maxAmmo;
    }



    public int RequiredUpgradePoint()
    {
        return (1 + UpgradedTime) * offset;
    }

    public bool Upgrade()
    {
        // Upgrade Successful
        if (UpgradePoint >= RequiredUpgradePoint() && Ability == 1)
        {
            status *= Random.Range(1.25f, 1.5f);
            status = Mathf.RoundToInt(status);
            UpgradePoint -= RequiredUpgradePoint();
            UpgradedTime++;
            return true;
        }
        else if(UpgradePoint >= RequiredUpgradePoint() && Ability == 2)
        {
            status *= Random.Range(1.25f, 1.5f);
            status = Mathf.Round(status * 10f) / 10;
            Debug.Log(status);
            UpgradePoint -= RequiredUpgradePoint();
            UpgradedTime++;
            return true;
        }
        else if (UpgradePoint >= RequiredUpgradePoint() && Ability == 3)
        {
            status *= Random.Range(1.25f, 1.5f);
            status = Mathf.RoundToInt(status);
            UpgradePoint -= RequiredUpgradePoint();
            UpgradedTime++;
            return true;
        }

        return false;
    }


}
