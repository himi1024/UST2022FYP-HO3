using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DebuffSO", menuName = "Scriptable Objects/Debuff/Debuff")]
public class DebuffSO : ScriptableObject
{
    public float continueTime = 1.5f; // continue time of the debuff status
    public float damagePerInterval = 0f; // damage deal to the game object per interval
    public float timeInterval = 0.5f; // time interval causing damage
    public float moveSpeedDecreaseValue = 0f;
    public bool rollingForbidden = false;
    public float chance = 0.2f;

    #region Burn
    public bool isBurn;
    #endregion

    #region Blooding
    public bool isBlood;
    #endregion

    #region Blooding
    public bool isFreezen;
    #endregion


    public Sprite debuffIcon;



}
