using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



[DisallowMultipleComponent]
public class EnemyControl : MonoBehaviour
{
    public Player player;
    public Transform weaponShootPos;

    public event Action<MonoBehaviour, AimArgs> AimWithMouse; //the event of mouse input
    public event Action<MonoBehaviour, MoveArgs> MoveWithKeyboard;// the event of keyboard input
    public event Action<MonoBehaviour, IdleArgs> Idle;// the event of idle
    public event Action<MonoBehaviour, Move2PosArgs> Roll;
    public event Action<MonoBehaviour, AimArgs> Shoot;
    public event Action<MonoBehaviour, int> Reload;
    public event Action<MonoBehaviour, GameObject> SwitchWeapon;

    private Coroutine rollCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    private bool isRolling = false;
    private float rollCDTimer = 0;

    private GameObject curWeapon;
    private int curWeaponIndex;
    public GameObject[] weaponList;

    private void Awake()
    {
        

    }

    private void Start()
    {
        
    }


    // Update is called once per frame
    /*void Update()
    {
        if (isRolling) return;
        if (rollCDTimer >= 0) rollCDTimer -= Time.deltaTime;
        MouseInput(); //process the mosue input from the user
        KeyboardInput(); //process the keyboard input from the user

    }*/


    private void MouseInput()
    {
        Vector3 weaponDirection; // direction vector from shoot position to mouse position;
        float aimAngle; //angle from shoot position to mouse position;
        float playerAngle; // angle from character pivot point to mouse position;
        FaceDirection faceDirection;
        ProcessMouseInput(out weaponDirection, out aimAngle, out playerAngle, out faceDirection);
        if (curWeapon != null)
        { //Shoot the bullet
            if (!curWeapon.GetComponent<Weapon>().continueShoot && Input.GetMouseButtonDown(0))
                Shoot?.Invoke(player, new AimArgs() { characterAngle = playerAngle, weaponAngle = aimAngle, weaponDirection = weaponDirection, faceDirection = faceDirection });
            if (curWeapon.GetComponent<Weapon>().continueShoot && Input.GetMouseButton(0))
                Shoot?.Invoke(player, new AimArgs() { characterAngle = playerAngle, weaponAngle = aimAngle, weaponDirection = weaponDirection, faceDirection = faceDirection });
        }

    }

    private void KeyboardInput()
    {
        Vector2 moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool rightClick = Input.GetMouseButtonDown(1);
        if (moveDirection.x != 0f && moveDirection.y != 0f) { moveDirection.x *= 0.7f; moveDirection.y *= 0.7f; }

        if (moveDirection.x == 0f && moveDirection.y == 0f) Idle?.Invoke(player, new IdleArgs());
        else
        {
            if (rightClick)
            {
                if (rollCDTimer <= 0)
                {
                    rollCoroutine = StartCoroutine(Roll2Pos((Vector3)moveDirection));
                    /*isRolling = true;
                   
                    rollCDTimer = player.rollCD;*/

                }
            }
            else MoveWithKeyboard?.Invoke(player, new MoveArgs() { moveDireciton = moveDirection, moveSpeed = player.moveSpeed });
        }

        if (Input.GetKeyDown("q")) SetNextWeapon();
        if (Input.GetKeyDown("r") && !curWeapon.GetComponent<Weapon>().isReloading) Reload?.Invoke(player, 1); // when R key is pressed and the current weapon is not reloading

    }


    private void ProcessMouseInput(out Vector3 weaponDirection, out float aimAngle, out float playerAngle, out FaceDirection faceDirection)
    {
        Vector3 mousePostion = HelperFunctions.GetMousePosition();
        weaponDirection = (mousePostion - weaponShootPos.position);
        aimAngle = HelperFunctions.Vector2Angle(weaponDirection);
        playerAngle = HelperFunctions.Vector2Angle(mousePostion - transform.position);
        if (aimAngle > 0f && aimAngle <= 45f || aimAngle < 0f && aimAngle >= -45f)
            faceDirection = FaceDirection.Right;
        else if (aimAngle >= 46f && aimAngle <= 135f)
            faceDirection = FaceDirection.Up;
        else if (aimAngle <= 180f && aimAngle >= 136f || aimAngle > -180f && aimAngle <= -136f)
            faceDirection = FaceDirection.Left;
        else if (aimAngle >= -135f && aimAngle <= -46f)
            faceDirection = FaceDirection.Down;
        else
            faceDirection = FaceDirection.Down;
        AimWithMouse?.Invoke(player, new AimArgs() { characterAngle = playerAngle, weaponAngle = aimAngle, weaponDirection = weaponDirection, faceDirection = faceDirection });

    }

    private IEnumerator Roll2Pos(Vector3 moveDirection)
    {
        isRolling = true;
        Vector3 targetPos = transform.position + moveDirection * player.rollDistance;
        while (Vector3.Distance(transform.position, targetPos) > 0.3f)
        {

            Roll?.Invoke(player, new Move2PosArgs() { moveSpeed = player.rollSpeed, curPos = transform.position, targetPos = targetPos });
            yield return waitForFixedUpdate;
        }
        //Debug.Log(Vector3.Distance(transform.position, targetPos) > 0.3f);
        isRolling = false;
        rollCDTimer = player.rollCD;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isRolling)
            return;
        StopCoroutine(rollCoroutine);
        isRolling = false;
        rollCDTimer = player.rollCD;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isRolling)
            return;
        StopCoroutine(rollCoroutine);
        isRolling = false;
        rollCDTimer = player.rollCD;
    }


    private void SetNextWeapon()
    {
        curWeapon.SetActive(false);
        curWeaponIndex = (curWeaponIndex + 1 >= weaponList.Length) ? 0 : curWeaponIndex + 1;
        while (!weaponList[curWeaponIndex].GetComponent<Weapon>().IsUsable())
        {
            curWeaponIndex = (curWeaponIndex + 1 > weaponList.Length) ? 0 : curWeaponIndex + 1;
        }
        curWeapon = weaponList[curWeaponIndex];
        curWeapon.SetActive(true);
        SwitchWeapon?.Invoke(this, curWeapon);
    }

}






