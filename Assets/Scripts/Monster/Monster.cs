using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemyWeapon))]
[RequireComponent(typeof(EnemyMove))]
[RequireComponent(typeof(Health))]
#endregion REQUIRE COMPONENTS

[DisallowMultipleComponent]

public class Monster : MonoBehaviour
{
    public MonsterDetailSO monsterDetail;
    private CircleCollider2D cirCollider2D;
    private PolygonCollider2D polyCollider2D;
    [HideInInspector] public SpriteRenderer[] spriteRendererArr;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Animator animator;

    private float hitTimer;
    private float hitTime = 0.1f;

    private void Awake()
    {
        cirCollider2D = GetComponent<CircleCollider2D>();
        polyCollider2D = GetComponent<PolygonCollider2D>();
        spriteRendererArr = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    private void Update()
    {
        if (hitTimer > 0)
            hitTimer -= Time.deltaTime;
        else
            spriteRenderer.color = new Color(255, 255, 255, 255);

    }

    public void getHit() {
        spriteRenderer.color = new Color(255, 0, 0, 255);
        hitTimer = hitTime;
    }
}
