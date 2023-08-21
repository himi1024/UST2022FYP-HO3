using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalBlock : MonoBehaviour
{
    public float maxTime;
    private bool ended = false;

    [HideInInspector] public float currentTime;
    [HideInInspector] public float remainingTime;

    [HideInInspector] public float startTime;
    [HideInInspector] public float stopTime = 0;

    [HideInInspector] public TimerUI timerUI;


    public void Start()
    {
        timerUI = FindObjectOfType<TimerUI>();
        if (GameResources.Instance.LevelIndex == 2 || GameResources.Instance.LevelIndex == 3)
            timerUI.SetTimer((int)(maxTime));
    }
    public void StartTimer()
    {
        EnemySpawnManager enemySpawnManager = GameObject.Find("EnemySpawnManager").GetComponent<EnemySpawnManager>();
        enemySpawnManager.ManualSpawnEnemy(gameObject.GetComponent<InstantiatedBlock>().blockInfo);
        startTime = Time.time;
        stopTime = Time.time + maxTime;
        //timerUI.SetTimer((int)(remainingTime));
        InvokeRepeating("PrintTime", 1f, 1f);
    }

    public void FixedUpdate()
    { 
        currentTime = Time.time - startTime;
        remainingTime = stopTime - Time.time;
        if (stopTime <= Time.time && stopTime != 0)
        {
            StopTime();
        }
    }

    public void StopTime()
    {
        if (ended)
            return;
        Debug.Log("Time's up");
        ended = true;
        GameManager.Instance.AllRoomsDefeated();
    }

    public void PrintTime()
    {
        if (ended) return;
        timerUI.SetTimer((int)(remainingTime));
        Debug.Log("Time remaining: " + Mathf.RoundToInt(remainingTime) );
    }
}
