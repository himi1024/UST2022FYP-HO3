using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[DisallowMultipleComponent]
public class EnemyWeapon : MonoBehaviour
{
    MonsterDetailSO monsterDetailSO;
    public event Action<MonoBehaviour, AimArgs> Shoot;
    public event Action<MonoBehaviour, AimArgs> AimWithMouse; //the event of mouse input
    private float shootCDTimer;
    private float shootContinousTimer;
    public Transform shootPos;
    public LayerMask layerMask;
    //public WeaponSO weaponSO;


    // Start is called before the first frame update
    void Start()
    {
        //setWeapon();
        //shootPos = transform.Find("WeaponShootPosition");
        //shootPos = transform.Find("WeaponAnchorPosition").Find("WeaponShootPosition");
        shootCDTimer = 0;
        shootContinousTimer = monsterDetailSO.minShootContinue;
    }

    private void Awake()
    {
        monsterDetailSO = GetComponent<Monster>().monsterDetail;
        //weaponSO = transform.GetComponentInChildren<Weapon>().weaponDetails;
    }

    // Update is called once per frame
    void Update()
    {
        if (monsterDetailSO.hasWeapon)
        {
            Vector3 playerPos ;
            AimPlayer(out Vector3 weaponDirection, out float aimAngle, out float playerAngle, out FaceDirection faceDirection);
            if (shootCDTimer > 0)
                shootCDTimer -= Time.deltaTime;
            if (shootCDTimer <= 0)
                if (shootContinousTimer >0)
                {
                    shootContinousTimer -= Time.deltaTime;
                    StartShoot(weaponDirection,  aimAngle,  playerAngle,  faceDirection);
                }
                else {
                    shootContinousTimer = monsterDetailSO.minShootContinue;
                    shootCDTimer = UnityEngine.Random.Range(monsterDetailSO.minShootInterval, monsterDetailSO.maxShootInterval);
                }
        } 
    }

    private void AimPlayer(out Vector3 weaponDirection, out float aimAngle, out float playerAngle, out FaceDirection faceDirection) {
        Vector3 playerPos = GameManager.Instance.GetPlayer().playerPositionOffset();
        weaponDirection = (playerPos - shootPos.position);
        aimAngle = HelperFunctions.Vector2Angle(weaponDirection);
        playerAngle = HelperFunctions.Vector2Angle(playerPos - transform.position);
        faceDirection = new FaceDirection();
        if (playerPos.x < transform.position.x) faceDirection = FaceDirection.Left;
        else faceDirection = FaceDirection.Right;
        AimWithMouse?.Invoke(this, new AimArgs() { characterAngle = playerAngle, weaponAngle = aimAngle, weaponDirection = weaponDirection, faceDirection = faceDirection });
    }

    private void StartShoot(Vector3 weaponDirection,  float aimAngle,  float playerAngle,  FaceDirection faceDirection) {
        //Debug.Log("startShooting");
        //Vector3 playerPos = GameManager.Instance.GetPlayer().playerPosition();
        //Vector3 weaponDirection = (playerPos - shootPos.position);

        if (monsterDetailSO.seePlayer)
        {
            RaycastHit2D hitTarget = Physics2D.Raycast(shootPos.position, weaponDirection.normalized, 100, 1 << 10 | 1 << 13);
            //Debug.Log(hitTarget.transform.name);
            //Debug.DrawLine(shootPos.position, hitTarget.transform.position);
            //Debug.DrawLine(shootPos.position, shootPos.position+weaponDirection1*100f, Color.blue);
            //Debug.DrawLine(shootPos.position, GameManager.Instance.GetPlayer().playerPosition(), Color.red);
            if (hitTarget && hitTarget.transform.CompareTag("Player"))
                Shoot?.Invoke(this, new AimArgs() { characterAngle = playerAngle, weaponAngle = aimAngle, weaponDirection = weaponDirection, faceDirection = faceDirection });
        }

        else {
                Shoot?.Invoke(this, new AimArgs() { characterAngle = playerAngle, weaponAngle = aimAngle, weaponDirection = weaponDirection, faceDirection = faceDirection });
        }

    }

    public bool DetectPlayer(Vector3 transformPos, Vector3 weaponDirection) {
        RaycastHit2D hitTarget = Physics2D.Raycast(transformPos, weaponDirection.normalized, 100, 1 << 10 | 1 << 13);
        //Debug.DrawLine(transform.position, transform.position + weaponDirection * 100f, Color.blue);
        //Debug.DrawLine(transform.position, transformPos, Color.red);
        if (hitTarget == null)
            return false;
        //Debug.Log(hitTarget.transform.tag);
        if (!hitTarget.transform.CompareTag("Player"))
            return false;
        return true;
    }
}
