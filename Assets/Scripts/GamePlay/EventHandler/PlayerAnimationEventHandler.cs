using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DisallowMultipleComponent]
public class PlayerAnimationEventHandler: MonoBehaviour
{
    
    private int isAimUp = Animator.StringToHash("isAimUp");
    private int isAimDown = Animator.StringToHash("isAimDown");
    private int isAimRight = Animator.StringToHash("isAimRight");
    private int isAimLeft = Animator.StringToHash("isAimLeft");
    private int isMoving = Animator.StringToHash("isMoving");
    private int isIdling = Animator.StringToHash("isIdling");
    private int isRollUp = Animator.StringToHash("isRollUp");
    private int isRollDown = Animator.StringToHash("isRollDown");
    private int isRollRight = Animator.StringToHash("isRollRight");
    private int isRollLeft = Animator.StringToHash("isRollLeft");

    private PlayerControl playerControl;


    // Start is called before the first frame update
    public void Awake()
    {
        playerControl = GetComponent<PlayerControl>();
    }

    public void OnEnable()
    {
        playerControl.AimWithMouse += aimAnimation;
        playerControl.Idle += idleAnimation;
        playerControl.MoveWithKeyboard += moveAnimation;
        playerControl.Roll += rollAnimation;
    }

    // Update is called once per frame
    public void onDisable()
    {
        playerControl.AimWithMouse -= aimAnimation;
        playerControl.Idle -= idleAnimation;
        playerControl.MoveWithKeyboard -= moveAnimation;
        playerControl.Roll -= rollAnimation;
    }


    public void idleAnimation(MonoBehaviour player, IdleArgs idleArgs) {
        ResetRollAnimation();
        playerControl.player.animator.SetBool(isMoving, false);
        playerControl.player.animator.SetBool(isIdling, true);

    }

    public void moveAnimation(MonoBehaviour player, MoveArgs idleArgs)
    {
        ResetRollAnimation();
        playerControl.player.animator.SetBool(isMoving, true);
        playerControl.player.animator.SetBool(isIdling, false);
        
    }

    public void aimAnimation(MonoBehaviour player, AimArgs aimArgs) {
        ResetRollAnimation();
        playerControl.player.animator.SetBool(isAimUp, false);
        playerControl.player.animator.SetBool(isAimDown, false);
        playerControl.player.animator.SetBool(isAimRight, false);
        playerControl.player.animator.SetBool(isAimLeft, false);
        switch (aimArgs.faceDirection) {
            case FaceDirection.Right:
                playerControl.player.animator.SetBool(isAimRight, true);
                break;
            case FaceDirection.Up:
                playerControl.player.animator.SetBool(isAimUp, true);
                break;
            case FaceDirection.Left:
                playerControl.player.animator.SetBool(isAimLeft, true);
                break;
            case FaceDirection.Down:
                playerControl.player.animator.SetBool(isAimDown, true);
                break;
        }
    }

    public void rollAnimation(MonoBehaviour player, Move2PosArgs move2Pos) {
        ResetRollAnimation();
        playerControl.player.animator.SetBool(isMoving, false);
        playerControl.player.animator.SetBool(isIdling, false);
        if(move2Pos.GetMoveDirection().x > 0f)
            playerControl.player.animator.SetBool(isRollRight, true);
        else if (move2Pos.GetMoveDirection().x  <0f)
            playerControl.player.animator.SetBool(isRollLeft, true);
        else if (move2Pos.GetMoveDirection().y > 0f)
            playerControl.player.animator.SetBool(isRollUp, true);
        else if (move2Pos.GetMoveDirection().y < 0f)
            playerControl.player.animator.SetBool(isRollDown, true);
    }

    private void ResetRollAnimation() {
        playerControl.player.animator.SetBool(isRollUp, false);
        playerControl.player.animator.SetBool(isRollDown, false);
        playerControl.player.animator.SetBool(isRollRight, false);
        playerControl.player.animator.SetBool(isRollLeft, false);
    }
}
