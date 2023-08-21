using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
public class MoveEventHandler : MonoBehaviour
{
    private PlayerControl playerControl;

    private new Rigidbody2D rigidbody2D;
    [SerializeField] private bool isPlayer = true;


    // Start is called before the first frame update
    public void Awake()
    {
        if (isPlayer)
            playerControl = GetComponent<PlayerControl>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void OnEnable()
    {
        if (isPlayer)
            playerControl.MoveWithKeyboard += Move;
    }

    // Update is called once per frame
    public void OnDisable()
    {
        if (isPlayer)
            playerControl.MoveWithKeyboard -= Move;
    }


    private void Move(MonoBehaviour control, MoveArgs moveArgs)
    {
        rigidbody2D.velocity = moveArgs.moveDireciton * moveArgs.moveSpeed;
    }
}
