using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    public int lines = 3;
    public float angleBetween = 5;
    protected override void ShootBullet(AimArgs aimArgs)
    {
        float totalAngle = (lines - 1) * angleBetween;
        float startAngle = aimArgs.weaponAngle + totalAngle / 2;
        GameObject bullet;
        ShootEffect();
        for (int i = 0; i < lines; i++)
        {
            float angle = startAngle - i * angleBetween;

            bullet = ObjectPooling.Instance.GetGameObject(weaponDetails.bulletDetails.bullet);

            bullet.SetActive(true);

            bullet.transform.position = shootPosition.position;

            bullet.transform.eulerAngles = new Vector3(0f, 0f, angle);

            bullet.GetComponent<Bullet>().Shoot(Angle2Vector2(angle).normalized);

            //Debug.Log("Shoot bullet " + i);
        }
        
        shootCDTimer = weaponDetails.shootCD;
    }

    private Vector2 Angle2Vector2(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;

        float x = Mathf.Cos(radian);
        float y = Mathf.Sin(radian);

        return new Vector2(x, y);
    }
}
