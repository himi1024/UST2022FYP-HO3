using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPG_Bullet : Bullet
{
    public float lerp;
    [HideInInspector] private Vector3 targetPosition;
    [HideInInspector] private Vector3 direction;
    [HideInInspector] private bool arrived = false;
    public override void Shoot(Vector2 direction)
    {
        this.direction = direction;
        targetPosition = HelperFunctions.GetMousePosition();
        rigidbody.velocity = direction * moveSpeed;
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        arrived = false;
        base.OnTriggerEnter2D(collision);
    }

    public void FixedUpdate()
    {
        direction = (targetPosition - transform.position).normalized;

        if (!arrived)
        {
            transform.right = Vector3.Slerp(transform.right, direction, lerp / Vector2.Distance(transform.position, targetPosition));
            rigidbody.velocity = transform.right * moveSpeed;
        }
        if (Vector2.Distance(transform.position, targetPosition) < 1f && !arrived)
            arrived = true;
    }
}
