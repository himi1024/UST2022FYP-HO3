using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum FaceDirection
{
    Up,
    Right,
    Left,
    Down
}


#region REQUIRE COMPONENTS
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(AimEventHandler))]
[RequireComponent(typeof(PlayerAnimationEventHandler))]
[RequireComponent(typeof(MoveEventHandler))]
[RequireComponent(typeof(IdleEventHandler))]
[RequireComponent(typeof(MoveToPosEventHandler))]
[DisallowMultipleComponent]
#endregion REQUIRE COMPONENTS
public class PlayerControl : MonoBehaviour
{
    public Player player;
    public Transform weaponShootPos;

    public SoundEffectSO changeWeaponEffect;

    public event Action<MonoBehaviour, AimArgs> AimWithMouse; //the event of mouse input
    public event Action<MonoBehaviour, MoveArgs> MoveWithKeyboard;// the event of keyboard input
    public event Action<MonoBehaviour, IdleArgs> Idle;// the event of idle
    public event Action<MonoBehaviour, Move2PosArgs> Roll;
    public event Action<MonoBehaviour, AimArgs> Shoot;
    public event Action<MonoBehaviour, int> Reload;
    public event Action<MonoBehaviour, GameObject> SwitchWeapon; //the event of switching weapon

    private Coroutine rollCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    public bool isRolling = false;
    private float rollCDTimer = 0;
    public bool canRolling = true;

    public GameObject curWeapon;
    private int curWeaponIndex;
    public GameObject[] weaponList;
    private bool shiftPressed = false;
    private bool ctrlPressed = false;

    [HideInInspector] public List<WeaponSystem> weaponSystems = new List<WeaponSystem>();


    private bool isPlayerMovementDisabled = false;
    [HideInInspector] public bool isFreezing = false;


    private void Awake()
    {
        player = GetComponent<Player>();
        curWeapon = weaponList[0];
        //weaponShootPos = curWeapon.transform.Find("ShootPosition");

    }

    private void Start()
    {
        weaponSystems.Add(Inventory.Instance.Pistol);
        weaponSystems.Add(Inventory.Instance.Rifle);
        weaponSystems.Add(Inventory.Instance.Shotgun);
        weaponSystems.Add(Inventory.Instance.Laser);
        weaponSystems.Add(Inventory.Instance.RPG);

        waitForFixedUpdate = new WaitForFixedUpdate();
        SwitchWeapon?.Invoke(this, curWeapon);
    }


    // Update is called once per frame
    void Update()
    {
        if (isRolling) return;
        if (rollCDTimer >= 0) rollCDTimer -= Time.deltaTime;
        MouseInput(); //process the mosue input from the user
        KeyboardInput(); //process the keyboard input from the user

    }


    private void MouseInput() {
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

    private void KeyboardInput() {
        Vector2 moveDirection = new Vector2( Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool rightClick = Input.GetMouseButtonDown(1);
        if (moveDirection.x != 0f && moveDirection.y != 0f) { moveDirection.x *= 0.7f; moveDirection.y *= 0.7f; }

        if (moveDirection.x == 0f && moveDirection.y == 0f) Idle?.Invoke(player, new IdleArgs());
        else {
            if (rightClick)
            {
                if (rollCDTimer <= 0 && canRolling)
                {
                    rollCoroutine = StartCoroutine(Roll2Pos((Vector3)moveDirection));
                    /*isRolling = true;
                   
                    rollCDTimer = player.rollCD;*/

                }
            }
            else if(!isRolling) MoveWithKeyboard?.Invoke(player, new MoveArgs() { moveDireciton = moveDirection, moveSpeed = player.moveSpeed });
        }

        if (Input.GetKeyDown("q")) SetNextWeapon();
        if (Input.GetKeyDown("r") && !curWeapon.GetComponent<Weapon>().isReloading) Reload?.Invoke(player, 1); // when R key is pressed and the current weapon is not reloading

        DetectIncreseSpeed();
       
        
    }

    public void DetectIncreseSpeed() {

        if (Input.GetKeyDown(KeyCode.LeftShift) && !shiftPressed)
        {
            player.moveSpeed += 5;
            player.rollSpeed += 7;
            player.rollDistance += 6;
            shiftPressed = true;
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl) && !ctrlPressed)
        {
            player.moveSpeed -= 5;
            player.rollSpeed -= 2;
            player.rollDistance -= 2;
            ctrlPressed = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) && shiftPressed)
        {
            player.moveSpeed -= 5;
            player.rollSpeed -= 7;
            player.rollDistance -= 6;
            shiftPressed = false;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl) && ctrlPressed)
        {
            player.moveSpeed += 5;
            player.rollSpeed += 2;
            player.rollDistance += 2;
            ctrlPressed = false;
        }
    }

    private void ProcessMouseInput(out Vector3 weaponDirection, out float aimAngle, out float playerAngle, out FaceDirection faceDirection) {
        Vector3 mousePostion = HelperFunctions.GetMousePosition();
        weaponDirection = (mousePostion - weaponShootPos.position);
        aimAngle = HelperFunctions.Vector2Angle(weaponDirection);
        playerAngle = HelperFunctions.Vector2Angle(mousePostion - transform.position);
        if (aimAngle > 0f && aimAngle <= 45f || aimAngle < 0f && aimAngle >= -45f)
            faceDirection = FaceDirection.Right;
        else if (aimAngle >= 46f && aimAngle <= 135f)
        {
            //Debug.Log("FaceUp");
            //Debug.Log(aimAngle);
            faceDirection = FaceDirection.Up;
        }
        else if (aimAngle <= 180f && aimAngle >= 136f || aimAngle > -180f && aimAngle <= -136f)
        {
            //Debug.Log("FaceLeft");
            faceDirection = FaceDirection.Left;
        }
        else if (aimAngle >= -135f && aimAngle <= -46f)
            faceDirection = FaceDirection.Down;
        else
            faceDirection = FaceDirection.Down;
        AimWithMouse?.Invoke(player, new AimArgs(){characterAngle = playerAngle, weaponAngle = aimAngle, weaponDirection = weaponDirection, faceDirection = faceDirection});

    }

    private IEnumerator Roll2Pos(Vector3 moveDirection) {
        isRolling = true;
        Vector3 targetPos = transform.position + moveDirection * player.rollDistance;
        while (Vector3.Distance(transform.position, targetPos) > 0.3f)
        {
            
            Roll?.Invoke(player, new Move2PosArgs() { moveSpeed = player.rollSpeed, curPos = transform.position, targetPos = targetPos  });
            DetectIncreseSpeed();
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


    private void SetNextWeapon() {
        curWeapon.SetActive(false);
        
        int previousIndex = curWeaponIndex;

        curWeaponIndex = (curWeaponIndex + 1 > 4) ? 0 : curWeaponIndex + 1;
        while (!weaponSystems[curWeaponIndex].Crafted) {
            curWeaponIndex = (curWeaponIndex + 1 > 4) ? 0 : curWeaponIndex + 1;
        }

        curWeapon = weaponList[curWeaponIndex];
        curWeapon.SetActive(true);

        if (previousIndex != curWeaponIndex) SoundEffectManager.Instance.Play(changeWeaponEffect);
        SwitchWeapon?.Invoke(this, curWeapon);
    }

    public void EnablePlayer()
    {
        isPlayerMovementDisabled = false;
    }

    public void DisablePlayer()
    {
        isPlayerMovementDisabled = true;
        player.idleEvent.CallIdleEvent();
    }
}




public class AimArgs {
    public float characterAngle; //the angle from player pivot point to the aim position
    public float weaponAngle; //angle from shoot position to mouse position;
    public Vector3 weaponDirection; // the vector direction from weapon shoot position to the aim position
    public FaceDirection faceDirection;
}

public class MoveArgs {
    public Vector3 moveDireciton; 
    public float moveSpeed;
}



public class IdleArgs { 
   
    
}

public class Move2PosArgs {
    public float moveSpeed;
    public Vector3 curPos;
    public Vector3 targetPos;

    public Vector2 GetMoveDirection() {
        return  (targetPos - curPos).normalized;
    }
    
}


