using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInformation_", menuName = "Scriptable Objects/Player/Player Information")]
public class PlayerInformationSO : ScriptableObject
{
    public string playerCharacterName;
    public GameObject playerPrefab;
    public RuntimeAnimatorController runtimeAnimatorController;
    public int playerInitialHealth;
    public Sprite playerIcon;
    public float noHurtTime = 2;
}
