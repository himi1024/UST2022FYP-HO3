using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDanmaku : MonoBehaviour
{

    public BulletSO bulletData;
    public bool aimPlayer;

    public bool canShoot = false;

    private void Start()
    {
        if (!GetComponent<Monster>().monsterDetail.hasWeapon)
            bulletData.ResetTempData();
    }

    private void FixedUpdate()
    {
        if (!GetComponent<Monster>().monsterDetail.hasWeapon)
             if (canShoot)
                Shoot();
    }

    void Shoot()
    {
        if (Time.time - bulletData.tempShootTime >= bulletData.cdTime)
        {
            bulletData.tempShootTime = Time.time;

            int num = bulletData.count / 2;

            Vector2 target = GetPlayerTarget();

            if (!aimPlayer) target = new Vector2(0, 1);
            //Debug.Log(target);

            bulletData.tempRotation *= Quaternion.Euler(0f, 0f, bulletData.tempSelfRotation);

            bulletData.tempSelfRotation += bulletData.addRotation;

            // Limit rotation value
            bulletData.tempSelfRotation = bulletData.tempSelfRotation >= 360 ? bulletData.tempSelfRotation - 360 : bulletData.tempSelfRotation;

            for (int i = 0; i < bulletData.count; i++)
            {

                GameObject bullet = ObjectPooling.Instance.GetGameObject(bulletData.bullet);

                bullet.GetComponent<Bullet>().SetBulletData(bulletData);

                bullet.transform.position = gameObject.transform.position + bulletData.pOffset;

                Quaternion rotate = bulletData.tempRotation * Quaternion.Euler(0, 0, -bulletData.rOffset);

                if (bulletData.count % 2 == 1)
                {
                    bullet.transform.position = bullet.transform.position + bulletData.distance * num * bullet.transform.right;

                    rotate *= Quaternion.Euler(0, 0, -bulletData.angle * num);

                    Vector3 direction = rotate * new Vector3(target.x, target.y, 0f);

                    float temp = Mathf.Atan2(direction.y, direction.x) * 180 / Mathf.PI;

                    bullet.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, temp));

                    bullet.GetComponent<Bullet>().Shoot(new Vector2(direction.x, direction.y).normalized);

                }
                else
                {
                    bullet.transform.position = bullet.transform.position + bullet.transform.right * ((num - 1) * bulletData.distance + bulletData.distance / 2);

                    rotate *= Quaternion.Euler(0, 0, -bulletData.angle / 2 - bulletData.angle * (num - 1));

                    Vector3 direction = rotate * new Vector3(target.x, target.y, 0f);

                    float temp = Mathf.Atan2(direction.y, direction.x) * 180 / Mathf.PI;

                    bullet.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, temp));

                    bullet.GetComponent<Bullet>().Shoot(new Vector2(direction.x, direction.y).normalized);
                }
                num--;
            }

            
        }

    }
    private Vector3 GetPlayerTarget()
    {
        Vector3 playerPos = GameManager.Instance.GetPlayer().playerPosition();
        Vector3 weaponDirection = (playerPos - gameObject.transform.position);
        return new Vector2(weaponDirection.x, weaponDirection.y);
    }


}
