using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WeaponSO", menuName = "Scriptable Objects/Weapon/Weapon")]
public class WeaponSO : ScriptableObject
{
    public string weaponName; // Weapon Name?
    /*Sprite weaponSprite; // Sprite of the weapon?
    GameObject weapon // Prefab of the weapon*/

    //public float magazine;
    public float maxAmmo;   // max Ammo
    public float shootCD;      // Cool Down per shoot
    public Vector2 weaponShootPosition; // record shoot position
    public BulletSO bulletDetails; //store Bullet Details}
    public bool continueShoot;
    public float reloadTime; // The time needed to reload the ammo

    public bool isPlayer = true;

    public GameObject smokeParticle;
    public SoundEffectSO firingSoundEffect;
    public SoundEffectSO reloadSoundEffect;
    public SoundEffectSO noAmmoEffect;
}
