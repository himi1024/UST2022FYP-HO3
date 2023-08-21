using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[DisallowMultipleComponent]
public class EnemyMove : MonoBehaviour
{
    public Monster monster;

    private Vector3 playerPosition;

    private List<Vector3> path; // path by A* Algorithm

    private Coroutine moveAlongPath;

    private float chaseCD = 0.5f;   //CD to re generate path

    private float chaseTimer = 0; // Timer to conut the CD

    private float chaseDist = 0; // Chase Distance

    private float chaseTimeout = 0f;
    private float chaseTimeout1 = 0f;

    private bool isChasing = false;

    //private bool chased = false;

    public Action<MonoBehaviour, Move2PosArgs> move2Pos;

    public Animator animator;

    private Transform healthBarContainer;

    private MonsterDanmaku monsterDanmaku;

    public bool movable = true;
    

    #region Animator variable

        private int isLeft = Animator.StringToHash("isLeft");
        private int isRight = Animator.StringToHash("isRight");
        private int isWalk = Animator.StringToHash("isWalk");
        private int isIdle = Animator.StringToHash("isIdle");
    #endregion


    private void Awake()
    {
        monster = GetComponent<Monster>();
        animator = GetComponent<Animator>();
        healthBarContainer = transform.Find("HealthBarContainer");
        monsterDanmaku = GetComponent<MonsterDanmaku>();

       
    }

    private void Start()
    {
        //playerPosition = GameManager.Instance.GetPlayer().playerPosition();
        chaseCD = monster.monsterDetail.chaseCD;
        chaseDist = monster.monsterDetail.chaseDist;
        animator.SetBool(isRight, true);

    }


    private void Update()
    {
        chaseTimer -= Time.deltaTime;
        MoveEnemy();

        Vector3 playerCurPosition = GameManager.Instance.GetPlayer().playerPositionOffset();
        Debug.DrawLine(transform.position, playerCurPosition, Color.red);
    }

    // Move the enemy according to the A* Path result
    private void MoveEnemy() {
        if (GameManager.Instance.GetPlayer() == null)
            return;
        playerPosition = GameManager.Instance.GetPlayer().playerPosition();
        if (playerPosition.x < transform.position.x) { animator.SetBool(isLeft, true); animator.SetBool(isRight, false); healthBarContainer.localScale = new Vector3(-1, 1, 1); }
        else { animator.SetBool(isLeft, false); animator.SetBool(isRight, true); healthBarContainer.localScale = new Vector3(1, 1, 1); }
        //healthBarContainer.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        //if (healthBarContainer)
        //if (healthBarContainer.localScale.x < 0) healthBarContainer.localScale = new Vector3(-healthBarContainer.localScale.x, healthBarContainer.localScale.y, healthBarContainer.localScale.z);

        if (chaseTimer <=0 && Vector3.Distance(playerPosition, transform.position) <= chaseDist && !isChasing) {
            //Debug.Log("Start Chasing");
            BlockInfo curBlock = GameManager.Instance.GetCurrentBlockInfo();
            Vector3Int curPosition = curBlock.instantiatedBlock.grid.WorldToCell(transform.position);
            Vector3Int targetPos = curBlock.instantiatedBlock.grid.WorldToCell(playerPosition);
            path = PathFinding.PathFind(curBlock, curPosition.x, curPosition.y, targetPos.x, targetPos.y);
            isChasing = true;

            if (moveAlongPath != null) { StopCoroutine(moveAlongPath); animator.SetBool(isIdle, false); }
            if (path != null)
                moveAlongPath = StartCoroutine(MoveAlongPath());
            else isChasing = false;
        }


    }

   
    // Coroutine to move the enemy
    private IEnumerator MoveAlongPath() {

        animator.SetBool(isIdle, false);
        animator.SetBool(isWalk, true);
        chaseTimeout1 = 1.5f;
        Vector3 playerCurPosition = GameManager.Instance.GetPlayer().playerPosition();
        while (path.Count > 0) {
            chaseTimeout = 1.5f;
            while (Vector3.Distance(transform.position, path[0]) > 0.3f)
            {
                playerCurPosition = GameManager.Instance.GetPlayer().playerPositionOffset();

                //Debug.DrawLine(transform.position, playerCurPosition, Color.red);
                if (!movable)
                    break;

                if (!monster.monsterDetail.hasWeapon && Vector3.Distance(playerCurPosition, transform.position) <= monster.monsterDetail.attackRange && GetComponent<EnemyWeapon>().DetectPlayer(transform.position, playerCurPosition - transform.position))
                {
                    chaseTimeout1 = 0;
                    if (monsterDanmaku != null) monsterDanmaku.canShoot = true;
                    //Debug.Log("Stop");
                    break;
                }
                else
                {
                   if (monsterDanmaku != null) monsterDanmaku.canShoot = false;
                }


                chaseTimeout -= Time.deltaTime;
                move2Pos?.Invoke(this, new Move2PosArgs { moveSpeed = UnityEngine.Random.Range(monster.monsterDetail.minMoveSpeed, monster.monsterDetail.maxMoveSpeed), curPos = transform.position, targetPos = path[0] });
                yield return new WaitForFixedUpdate();
                if (chaseTimeout <= 0f)
                    break;
            }
            path.RemoveAt(0);
            chaseTimeout1 -= Time.deltaTime;
            if (chaseTimeout1 <= 0)
                break;
            yield return new WaitForFixedUpdate();
        }

        isChasing = false;
        chaseTimer = chaseCD;
        animator.SetBool(isIdle, true);
        animator.SetBool(isWalk, false);
    }



}
