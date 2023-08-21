using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem
{
    public int ComponentRequiredB;
    public int ComponentRequiredG;
    public int ComponentRequiredM;
    [HideInInspector] public bool Crafted;
    public WeaponStatus Power;
    public WeaponStatus Speed;
    public WeaponStatus Capacity;
    [HideInInspector] public int Body = 0;
    [HideInInspector] public int Grip = 0;
    [HideInInspector] public int Muzzle = 0;

    public WeaponSystem(WeaponSO weaponSO, int offset = 1, bool usable = false, int body = 1, int grip = 1, int muzzle = 1)
    {
        Crafted = usable;
        ComponentRequiredB = body;
        ComponentRequiredG = grip;
        ComponentRequiredM = muzzle;

        Debug.Log(weaponSO);

        Power = new WeaponStatus(offset, 1, weaponSO.bulletDetails.maxDamage, weaponSO.bulletDetails.minDamage, weaponSO.maxAmmo, weaponSO.shootCD);
        Speed = new WeaponStatus(offset, 2, weaponSO.bulletDetails.maxDamage, weaponSO.bulletDetails.minDamage, weaponSO.maxAmmo, weaponSO.shootCD);
        Capacity = new WeaponStatus(offset, 3, weaponSO.bulletDetails.maxDamage, weaponSO.bulletDetails.minDamage, weaponSO.maxAmmo, weaponSO.shootCD);
    }



    public bool Craftable()
    {
        if (Crafted == true) 
            return false;
        if(Body >= ComponentRequiredB && Grip >= ComponentRequiredG && Muzzle >= ComponentRequiredM)
        {
            return true;
        }
        return false;
    }
}
