using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BulletSO", menuName = "Scriptable Objects/Weapon/Bullet")]
public class BulletSO : ScriptableObject
{
    [Header("Basic")]
    public GameObject bullet; // prefab of the bullet
    public float fireSpeed; // bullet move speed
    public int minDamage = 5;
    public int maxDamage = 10;

    [Header("Debuff")]
    public bool hasDebuff = false;
    public DebuffSO debuffDetails;

    [Header ("Bullet Hell")]
    public Vector3 pOffset = Vector3.zero;  // Initial position offset
    public float rOffset = 0;   // Initial rotation offset
    public int count = 1;   // num of lines
    public float cdTime = 0.1f; // frequency of bullet
    public float angle = 0; // angle between lines
    public float distance = 0;  // distance between lines
    public float centerDistance = 0;    // distance to center point
    public float selfRotation = 0;  // speed of rotation of bullet line
    public float addRotation = 0;   // change of rotation line speed

    public float tempShootTime; // Record last bullet shoot time
    public Quaternion tempRotation; // Record last rotation
    public float tempSelfRotation;

    public GameObject bulletHitEffect;

    public SoundEffectSO hitWallEffect;


    [Header("Bullet Graphic Setting")]
    public bool useCustonSetting = false;
    public Color color;
    public float scale;
    public Sprite sprite;
    public void ResetTempData()
    {
        tempShootTime = 0;
        tempRotation = Quaternion.Euler(0f, 0f, 0f);
        tempSelfRotation = selfRotation;
    }
}
