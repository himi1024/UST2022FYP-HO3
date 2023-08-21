using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockType_", menuName = "Scriptable Objects/Dungeon/Block Type")]
public class BlockTypeSO : ScriptableObject
{
    public string blockTypeName;

    #region Header
    [Header("Whether the block is visible in editor")]
    #endregion Header
    public bool displayable = true;

    #region Header
    [Header("Whether the block is a room connector")]
    #endregion Header
    public bool isConnector;

    #region Header
    [Header("Whether the block is vertical connector")]
    #endregion Header
    public bool isConnectorVertical;

    #region Header
    [Header("Whether the block is horizontal connector")]
    #endregion Header
    public bool isConnectorHorizontal;

    #region Header
    [Header("Whether the block is Entrance")]
    #endregion Header
    public bool isEntrance;

    #region Header
    [Header("Whether the block is a Boss Room")]
    #endregion Header
    public bool isBossRoom;

    #region Header
    [Header("Whether the block is Unassigned")]
    #endregion Header
    public bool isUnassigned;

    //TODO: add validation
}
