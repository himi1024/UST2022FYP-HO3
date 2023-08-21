using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class IdleEventHandler : MonoBehaviour
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
            playerControl.Idle += StopMove;
    }

    // Update is called once per frame
    public void OnDisable()
    {
        if (isPlayer)
            playerControl.Idle -= StopMove;
    }


    private void StopMove(MonoBehaviour control, IdleArgs moveArgs)
    {
        rigidbody2D.velocity = Vector2.zero;
    }
}
