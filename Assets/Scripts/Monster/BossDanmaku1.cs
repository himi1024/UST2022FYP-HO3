using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDanmaku1 : MonoBehaviour
{
    public BulletSO bulletData;

    private bool aimPlayer;

    public Health healthObject;
    private float maxHealth;
    private float health;

    public float coolDown = 10;

    private bool patternEmitting = false;

    private EnemyMove enemyMove;

    private Vector2 currentDirection = new Vector2(0, 1);

    private float[,] SineWaves = new float[,] {
                                            { 90f, 0.02f, 0.14f, 50 },
                                            { 90f, 0.02f, 3f, 600 },
                                            { 90f, 0.02f, 5.5f, 800 },
                                            { 90f, 0.02f, 5.25f, 1000 }
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

            int pattern = Mathf.RoundToInt(Random.Range(0f, 4f));

            Debug.Log(pattern);

            if (pattern == 4) StartCoroutine(RoundPattern());
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

    // public float angle = 50;
    // public float frequency = 0.05f;
    // public float speed = 1f;

    private IEnumerator SineWave(float angle, float frequency, float speed, float number)
    {
        patternEmitting = true;
        enemyMove.movable = false;
        float PlayerPosition = GetPlayerDegreeDirection();
        for (float i = 0; i < number; i+=speed)
        {
            GameObject bullet = ObjectPooling.Instance.GetGameObject(bulletData.bullet);

            bullet.transform.position = gameObject.transform.position;

            TurnTo(Mathf.Sin(i) * angle + PlayerPosition);

            bullet.GetComponent<Bullet>().Shoot(currentDirection.normalized);

            yield return new WaitForSeconds(frequency);
        }
        enemyMove.movable = true;
        patternEmitting = false;
    }

    public float lines;
    public float frequency;
    private IEnumerator RoundPattern()
    {
        patternEmitting = true;
        enemyMove.movable = false;

        List<GameObject> bullets = new List<GameObject>();

        for (int j = 0; j < 5; j++)
        {
            bullets.Clear();

            for (float i = 0; i < 360; i += 360 / lines)
            {
                GameObject bullet = ObjectPooling.Instance.GetGameObject(bulletData.bullet);

                bullet.transform.position = gameObject.transform.position + new Vector3(Degree2Vector2(i).x, Degree2Vector2(i).y, 0f) * 2;

                bullets.Add(bullet);

                yield return new WaitForSeconds(frequency);
            }

            yield return new WaitForSeconds(0.5f);

            for (float i = 0; i < 360; i += 360 / lines)
            {
                bullets[(int)(i / (360 / lines))].GetComponent<Bullet>().Shoot(new Vector2(Degree2Vector2(i).x, Degree2Vector2(i).y));
            }

            StartCoroutine(RoundPatternRoutine(bullets));
        }
        enemyMove.movable = true;
        patternEmitting = false;
    }

    private IEnumerator RoundPatternRoutine(List<GameObject> bullets)
    {
        for (int i = 0; i < 4; i++)
        {
            foreach (GameObject bullet in bullets)
            {
                Rigidbody2D rigidbody2D = bullet.GetComponent<Rigidbody2D>();
                float x = rigidbody2D.velocity.x;
                float y = rigidbody2D.velocity.y;
                rigidbody2D.velocity = new Vector2(x * 1.4f, y * 1.4f);
            }
            yield return new WaitForSeconds(0.1f);
        }
        
    }

    private Vector3 GetPlayerTarget()
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
}
