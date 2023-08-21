using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapStructureGraph", menuName = "Scriptable Objects/Dungeon/Map Structure Graph")]
public class MapStructureGraphSO : ScriptableObject
{
    #region Header BLOCK TYPE LIST
    [Space(10)]
    [Header("These attributes are shown for debug, please do not modify them!!!")]
    #endregion
    public BlockTypeListSO blockTypeList;
    public List<BlockSO> blockList = new List<BlockSO>();
    public Dictionary<string, BlockSO> blockDictionary = new Dictionary<string, BlockSO>();

    private void Awake()
    {
        GetBlockDictionary();
    }

    private void GetBlockDictionary(){
        blockDictionary.Clear();

        foreach (BlockSO block in blockList) {
            blockDictionary[block.id] = block;
        }

    }


    public BlockSO GetBlock(BlockTypeSO blockType) {
        foreach (BlockSO block in blockList) {
            if (block.blockType == blockType) return block;
                   
        }

        return null;
    }

    public BlockSO GetBlock(string blockID)
    {
        if (blockDictionary.TryGetValue(blockID, out BlockSO block)) return block;
        else return null;
    }


    public IEnumerable<BlockSO> GetChildBlocks(BlockSO parentBlock) {
        foreach (string childBlockID in parentBlock.childBlockIDList) { yield return GetBlock(childBlockID); }
    }

    #region Editor Code

#if UNITY_EDITOR

    // Store the block that is the line starting point
    public BlockSO blockToDrawLineFrom = null;

    // Store the line position
    public Vector2 linePosition;

    public void OnValidate()
    {
        blockDictionary.Clear();

        // Populate dictionary
        foreach (BlockSO block in blockList)
        {
            blockDictionary[block.id] = block;
        }
    }

    public IEnumerable<BlockSO> GetChildRoomNodes(BlockSO parentRoomNode)
    {
        foreach (string childNodeID in parentRoomNode.childBlockIDList)
        {
            yield return GetBlock(childNodeID);
        }
    }

    // TODO: add validation

#endif

    #endregion Editor Code
}
