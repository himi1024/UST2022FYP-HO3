using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockTypeListSO", menuName = "Scriptable Objects/Dungeon/Block Type List")]
public class BlockTypeListSO : ScriptableObject
{
    #region Header BLOCK TYPE LIST
    [Space(10)]
    [Header("BLOCK TYPE LIST")]
    #endregion
    public List<BlockTypeSO> blocks;
}
