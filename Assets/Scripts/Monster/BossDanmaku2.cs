using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDanmaku2 : MonoBehaviour
{
    public BulletSO bulletData;

    public bool aimPlayer;

    public Health healthObject;
    private float maxHealth;
    private float health;

    public float coolDown = 10;

    private bool patternEmitting = false;

    private EnemyMove enemyMove;

    private Vector2 currentDirection = new Vector2(0, 1);

    private float[,] SineWaves = new float[,] {
                                            { 20f, 0.02f, 0.14f, 42 },
                                            { 70f, 0.02f, 0.247f, 74 },
                                              };

    public int test = 0;
    private void Start()
    {
        if (!GetComponent<Monster>().monsterDetail.hasWeapon)
            bulletData.ResetTempData();
        enemyMove = GetComponent<EnemyMove>();
    }

    private void FixedUpdate()
    {
        Shoot();

        health = healthObject.GetCurrentHealth();
        maxHealth = healthObject.GetInitialHealth();
    }

    void Shoot()
    {
        if (Time.time - bulletData.tempShootTime >= coolDown)
        {
            bulletData.tempShootTime = Time.time;
            if (patternEmitting) return;

            int pattern = Mathf.RoundToInt(Random.Range(0f, 2f));

            Debug.Log(pattern);

            if (pattern == 2) StartCoroutine(TracePlayer());
            else
            {
                float angle = SineWaves[pattern, 0];
                float frequency = SineWaves[pattern, 1];
                float speed = SineWaves[pattern, 2];
                float number = SineWaves[pattern, 3];

                StartCoroutine(SineWave(angle, frequency, speed, number));
            }

        }
    }

    public float lines;
    // public float frequency;

    private IEnumerator RevolvePlayer()
    {
        patternEmitting = true;
        enemyMove.movable = false;
        List<GameObject> bullets = new List<GameObject>();

        bullets.Clear();

        for (float i = 0; i < 360; i += 360 / lines)
        {
            GameObject bullet = ObjectPooling.Instance.GetGameObject(bulletData.bullet);

            bullet.transform.position = GameManager.Instance.GetPlayer().playerPosition() + new Vector3(Degree2Vector2(i).x, Degree2Vector2(i).y, 0f) * 2;

            bullets.Add(bullet);

            yield return new WaitForSeconds(0.05f);
        }

        for (float i = 0; i < 360; i+=360 / lines)
        {
            bullets[(int)(i / (360 / lines))].transform.position = GameManager.Instance.GetPlayer().playerPosition() + new Vector3(Degree2Vector2(i).x, Degree2Vector2(i).y, 0f) * 2;
            
            yield return new WaitForSeconds(0.05f);
        }

        enemyMove.movable = true;
        patternEmitting = false;
    }

    public float distance;
    private IEnumerator TracePlayer()
    {
        patternEmitting = true;
        enemyMove.movable = false;
        List<GameObject> bullets = new List<GameObject>();

        for (float i = 0; i < 360; i += 360 / lines)
        {
            GameObject bullet = ObjectPooling.Instance.GetGameObject(bulletData.bullet);

            bullet.transform.position = gameObject.transform.position + new Vector3(Degree2Vector2(i).x, Degree2Vector2(i).y, 0f) * 2;

            bullets.Add(bullet);

            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(0.5f);

        foreach (GameObject bullet in bullets)
        {
            bullet.GetComponent<Bullet>().Shoot(GetPlayerTarget().normalized);
            StartCoroutine(OnFlyingAccel(bullet, 1.1f));
            yield return new WaitForSeconds(0.2f);
        }
        enemyMove.movable = true;
        patternEmitting = false;
    }

    // public float angle;
    // public float frequency;
    // public float speed;
    // public float number = -1;
    private IEnumerator SineWave(float angle, float frequency, float speed, float number)
    {
        patternEmitting = true;
        enemyMove.movable = false;
        float PlayerPosition = GetPlayerDegreeDirection();
        for (float i = 0; i < number; i += speed)
        {
            if (aimPlayer) PlayerPosition = GetPlayerDegreeDirection();

            GameObject bullet = ObjectPooling.Instance.GetGameObject(bulletData.bullet);

            bullet.transform.position = gameObject.transform.position;

            TurnTo(Mathf.Sin(2*Mathf.PI*i) * angle + PlayerPosition);

            bullet.GetComponent<Bullet>().Shoot(currentDirection.normalized);

            yield return new WaitForSeconds(frequency);
        }
        enemyMove.movable = true;
        patternEmitting = false;
    }

    private IEnumerator OnFlyingAccel(GameObject bullet, float value)
    {
        while (bullet.activeInHierarchy)
        {
            Rigidbody2D rigidbody = bullet.GetComponent<Rigidbody2D>();

            rigidbody.velocity += new Vector2(rigidbody.velocity.x * value, rigidbody.velocity.y * value);

            yield return new WaitForSeconds(0.1f);
        }
    }

    private Vector2 GetPlayerTarget()
    {
        Vector3 playerPos = GameManager.Instance.GetPlayer().playerPosition();
        Vector3 weaponDirection = (playerPos - gameObject.transform.position);
        return new Vector2(weaponDirection.x, weaponDirection.y);
    }

    private float GetPlayerDegreeDirection()
    {
        Vector3 direction = GetPlayerTarget();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return angle;
    }

    private void TurnAntiClockwise(float degree)
    {
        Quaternion rotate = Quaternion.Euler(0f, 0f, degree);
        Vector3 vector = new Vector3(currentDirection.x, currentDirection.y, 0f);
        vector = rotate * vector;
        currentDirection.x = vector.x;
        currentDirection.y = vector.y;
    }
    
    private Vector2 TurnTo(Vector2 vector, float degree)
    {
        float degree1 = Vector2Degree(vector) + degree;
        if (degree1 > 360) degree1 -= 360;
        if (degree1 < 0) degree1 = 360 - degree;
        return Degree2Vector2(degree1);
    }

    private void TurnTo(float degree)
    {
        currentDirection = Degree2Vector2(degree);
    }

    private Vector2 Degree2Vector2(float degree)
    {
        float angleInRadians = degree * Mathf.Deg2Rad;

        float x = Mathf.Cos(angleInRadians);
        float y = Mathf.Sin(angleInRadians);

        return new Vector2(x, y).normalized;
    }

    private float Vector2Degree(Vector2 vector)
    {
        float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;
        return angle;
    }
}
