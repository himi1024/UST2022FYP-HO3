using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FreezeDebuffHandler : MonoBehaviour
{
    public DebuffSO debuffSO;
    public float debuffTimer = 0f;
    public float debuffIntervalTimer = 0f;
    public Coroutine debuffCors;
    public Action<DebuffSO> onFreezeDebuffCreate; // Let the UI to generate the debuff icon
    public Action<DebuffSO> onFreezeDebuffDestroy; // Let the UI destroy the debuff icon
    public Action<DebuffSO, float> onFreezeDebuffChange;

    private Health health;
    private Player player;
    private PlayerControl playerControl;
    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();
        player = GetComponent<Player>();
        playerControl = GetComponent<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateDebuffEffect(DebuffSO debuffDetail)
    {
        debuffSO = debuffDetail;
        debuffTimer = 0;
        if (debuffCors == null)
        {
            debuffCors = StartCoroutine(HandleDebuffEffect(debuffDetail));
            onFreezeDebuffCreate?.Invoke(debuffSO);
        }


    }

    private IEnumerator HandleDebuffEffect(DebuffSO debuffDetail) {
        DebuffInitailEffect();
        Debug.Log("Freeze");
        while (debuffTimer <= debuffSO.continueTime) {
            debuffTimer += Time.deltaTime;
            debuffIntervalTimer += Time.deltaTime;
            onFreezeDebuffChange?.Invoke(debuffDetail, 1 - ((float)debuffTimer / debuffSO.continueTime));
            if (debuffIntervalTimer >= debuffSO.timeInterval) {
                DealDebuffDamage();
                debuffIntervalTimer = 0f;
            }

            yield return new WaitForFixedUpdate();
        
        }

        CancelDebuffEffect();
        onFreezeDebuffDestroy?.Invoke(debuffSO);
    }

    private void DealDebuffDamage() {
        //Debug.Log("Take damage");
        if (debuffSO.damagePerInterval > 0)
            health.takeDamage((int)debuffSO.damagePerInterval, true);
    }

    private void DebuffInitailEffect() {
        if (debuffSO.moveSpeedDecreaseValue > 0)
            player.moveSpeed -= debuffSO.moveSpeedDecreaseValue;
        if (debuffSO.rollingForbidden)
            if (playerControl)
                playerControl.canRolling = false;
        onFreezeDebuffChange?.Invoke(debuffSO,1);
    }

    private void CancelDebuffEffect() {
        //Debug.Log("End Debuff");
        if (debuffSO.moveSpeedDecreaseValue > 0)
           player.moveSpeed += debuffSO.moveSpeedDecreaseValue;
        if (debuffSO.rollingForbidden)
            if (playerControl)
                playerControl.canRolling = true;
        debuffTimer = 0;
        debuffIntervalTimer = 0;
        debuffCors = null; 
    }
}
