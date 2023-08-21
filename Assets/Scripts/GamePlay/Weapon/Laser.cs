using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Weapon
{

    protected LineRenderer lineRenderer;
    private float bulletTimer = 0f;
    private float damageTimer = 0.1f;
    private float damageInterval = 0f;

    private GameObject sound;

    private bool isPlaying = false;

    protected override void Start()
    {
        base.Start();
        sound = ObjectPooling.Instance.GetGameObject(weaponDetails.firingSoundEffect.sound);
        sound.GetComponent<AudioSource>().Stop();
        lineRenderer = GetComponentInChildren<LineRenderer>();
    }

    protected override void Update()
    {
        if (damageInterval > 0) damageInterval -= Time.deltaTime;
        base.Update();
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonDown(1) || curAmmo == 0)
        {
            lineRenderer.enabled = false;
            // SoundEffectManager.Instance.DisablePlayPressing(true);
            sound.GetComponent<AudioSource>().Stop();
            isPlaying = false;
        }
        
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        lineRenderer.enabled = false;
    }

    protected override void ShootBullet(AimArgs aimArg) {
        // Debug.Log("Shoot");
        // SoundEffectManager.Instance.PlayPressing(weaponDetails.firingSoundEffect);

        if (!isPlaying)
        {
            sound.GetComponent<AudioSource>().clip = weaponDetails.firingSoundEffect.audioClip;
            sound.GetComponent<AudioSource>().Play();
            isPlaying = true;
        }
        


        RaycastHit2D hitTarget = Physics2D.Raycast(shootPosition.position, aimArg.weaponDirection.normalized, 100, 1<<13 | 1<<15);
        lineRenderer.enabled = true;
        //Debug.DrawLine(shootPosition.position, hitTarget.point);
        Vector3 target = hitTarget.point;
        lineRenderer.SetPosition(0, shootPosition.position);
        lineRenderer.SetPosition(1, target);
        if (hitTarget.transform.GetComponent<Health>() != null && damageInterval<=0)
        {
            hitTarget.transform.GetComponent<Health>().takeDamage(Random.Range(base.weaponDetails.bulletDetails.minDamage, base.weaponDetails.bulletDetails.maxDamage));
            hitTarget.transform.GetComponent<Monster>()?.getHit();
            damageInterval = damageTimer;
        }


        bulletTimer += Time.deltaTime;
        //Debug.Log(bulletTimer);
        //Debug.Log(bulletTimer >= 1f);
        if (bulletTimer >= 1f)
        {
            curAmmo--;
            base.InvokeAmmoShoot();
            //Debug.Log(bulletTimer);
            bulletTimer = 0f;
        }
    }

}
