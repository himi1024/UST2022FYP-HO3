using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(IdleEvent))]
[DisallowMultipleComponent]
#endregion REQUIRE COMPONENTS



public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerInformationSO playerInformation;
    [HideInInspector] public Health health;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Animator animator;
    [HideInInspector] public IdleEvent idleEvent;

    [HideInInspector] public bool isAttacked;

    public float moveSpeed;
    public float rollSpeed;
    public float rollDistance;
    public float rollCD;
    private void Awake()
    {
        health = GetComponent<Health>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        idleEvent = GetComponent<IdleEvent>();
    }

    public void Initialize(PlayerInformationSO playerInformation)
    {
        this.playerInformation = playerInformation;
        health.SetInitialHealth(playerInformation.playerInitialHealth);
    }

    public Vector3 playerPosition() {
        return transform.position;
    }

    public Vector3 playerPositionOffset() {
        Vector3 positionOffset = transform.position + new Vector3(0f, 0.5f, 0f);
        return positionOffset;
    }

}

