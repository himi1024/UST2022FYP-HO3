using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]

public class MoveToPosEventHandler : MonoBehaviour
{
    private PlayerControl playerControl;
    private EnemyMove enemyMove;

    private new Rigidbody2D rigidbody2D;
    [SerializeField] private bool isPlayer = true;


    // Start is called before the first frame update
    public void Awake()
    {
        playerControl = GetComponent<PlayerControl>();
        if (playerControl == null)
            isPlayer = false;
        if (isPlayer)
            playerControl = GetComponent<PlayerControl>();
        else enemyMove = GetComponent<EnemyMove>();

        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void OnEnable()
    {
        if (isPlayer)
            playerControl.Roll += Move2Pos;
        else enemyMove.move2Pos += Move2Pos;
    }

    // Update is called once per frame
    public void OnDisable()
    {
        if (isPlayer)
            playerControl.Roll -= Move2Pos;
        else enemyMove.move2Pos -= Move2Pos;
    }


    private void Move2Pos(MonoBehaviour control, Move2PosArgs move2Pos)
    {
        rigidbody2D.MovePosition(rigidbody2D.position + (move2Pos.moveSpeed * Time.fixedDeltaTime * move2Pos.GetMoveDirection()));
    }
}
