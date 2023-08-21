using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour { 


    public BulletSO bulletDetails;
    [HideInInspector] public new Rigidbody2D rigidbody;  //rigidbody of the bullet
    [HideInInspector] public float moveSpeed; //Speed of bullet movement
    private TrailRenderer trail; //trail renderer
    private bool hasTrail = false;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        moveSpeed = bulletDetails.fireSpeed;
        trail = transform.GetComponentInChildren<TrailRenderer>();
        if (trail != null)
            hasTrail = true;


    }


    public virtual void Shoot(Vector2 direction) {
        SetBullet();
        if (trail)
            trail.gameObject.SetActive(true);
        rigidbody.velocity = direction * moveSpeed;
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Collide with " + collision);
        if (!collision.isTrigger && collision.transform.tag == "Player")
            return;
        Health healthComp = collision.GetComponent<Health>();
        bool tookDamage = false;
        if(healthComp!= null)
            tookDamage = healthComp.takeDamage(Random.Range(bulletDetails.minDamage, bulletDetails.maxDamage));
        //gameObject.SetActive(false);
        if(tookDamage)
            GenerateDebuffEffect(collision);
        HitEffect(transform.position);
        if(hasTrail) trail.gameObject.SetActive(false);
        ObjectPooling.Instance.PushGameObject(gameObject);
        Vector2 knockBack = (collision.transform.position - transform.position).normalized*0.75f;
        if (collision.tag == "Enemy")
        {   //Knockback enemy
            if (collision.GetComponent<Monster>().monsterDetail.knockAble)
                collision.transform.position = new Vector2(collision.transform.position.x + knockBack.x, collision.transform.position.y + knockBack.y);
                //collision.transform.GetComponent<Rigidbody2D>().AddForce(knockBack * 400f, ForceMode2D.Impulse);
            // change color
            collision.GetComponent<Monster>().getHit();
            // blood particle
            GameObject bloodPT = ObjectPooling.Instance.GetGameObject(GameResources.Instance.hitEnemyBloodPt);
            bloodPT.transform.position = collision.transform.position;
            
        }
        //collision.transform.GetComponent<Rigidbody2D>().AddForce(2f * knockBack);

        // Sound Effect
        if (collision.tag == "collisionTilemaps")
            SoundEffectManager.Instance.Play(bulletDetails.hitWallEffect, gameObject);
    }

    protected void HitEffect(Vector3 position)
    {
        if (bulletDetails.bulletHitEffect != null)
        {
            GameObject hitPT = ObjectPooling.Instance.GetGameObject(bulletDetails.bulletHitEffect);
            hitPT.transform.position = position;

        }
    }

    protected void GenerateDebuffEffect(Collider2D collision) {
        //Debug.Log(bulletDetails);
        if (!bulletDetails.hasDebuff)
            return;
        float chance = Random.Range(0.0f, 1.0f);
        //Debug.Log(chance +" "+ bulletDetails.debuffDetails.chance);
        if (chance <= bulletDetails.debuffDetails.chance)
        {
            //Debug.Log("Create Debuff effect");
            if(bulletDetails.debuffDetails.isBurn)
                collision.GetComponent<BurnDebuffHandler>()?.CreateDebuffEffect(bulletDetails.debuffDetails);
            else if(bulletDetails.debuffDetails.isBlood)
                collision.GetComponent<BloodDebuffHandler>()?.CreateDebuffEffect(bulletDetails.debuffDetails);
            else if (bulletDetails.debuffDetails.isFreezen)
                collision.GetComponent<FreezeDebuffHandler>()?.CreateDebuffEffect(bulletDetails.debuffDetails);
        }
    }

    protected virtual void SetBullet() {
        if (!bulletDetails.useCustonSetting)
            return;
        //Debug.Log(bulletDetails.name+" "+bulletDetails.color);
        if (bulletDetails.color != null)
            GetComponent<SpriteRenderer>().color = bulletDetails.color;
        if (bulletDetails.scale != 0)
            GetComponent<Transform>().localScale = new Vector3(bulletDetails.scale, bulletDetails.scale, bulletDetails.scale);
        if (bulletDetails.sprite != null)
            GetComponent<SpriteRenderer>().sprite = bulletDetails.sprite;


    }

    public void SetBulletData(BulletSO newBulletData) {
        bulletDetails = newBulletData;
    }

}
