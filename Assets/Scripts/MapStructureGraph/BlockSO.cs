using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BlockSO : ScriptableObject
{
    #region Header BLOCK TYPE LIST
    [Space(10)]
    [Header("These attributes are shown for debug, please do not modify them!!!")]
    #endregion
    public string id;
    public List<string> parentBlockIDList = new List<string>();
    public List<string> childBlockIDList = new List<string>();
    public MapStructureGraphSO mapStructureGraph;
    public BlockTypeListSO blockTypeList;

    #region Header BLOCK TYPE LIST
    [Space(10)]
    [Header("Only this field is allow to modified!")]
    #endregion
    public BlockTypeSO blockType;

    #region Editor Code

#if UNITY_EDITOR

    // Block 
    public Rect rect;
    public bool isLeftClickDragging = false;
    public bool isSelected = false;

    public void Initialise(Rect rect, MapStructureGraphSO mapStructure, BlockTypeSO blockType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "Block";
        this.mapStructureGraph = mapStructure;
        this.blockType = blockType;

        // Load block type list
        blockTypeList = GameResources.Instance.blockTypeList;
    }

    public void ProcessEvents(Event action)
    {
        switch (action.type)
        {
            // Process Mouse Down Events
            case EventType.MouseDown:
                // left click down
                if (action.button == 0)
                {
                    Selection.activeObject = this;

                    // Toggle blcok selection
                    isSelected = !isSelected;
                }
                // right click down
                else if (action.button == 1)
                {
                    mapStructureGraph.blockToDrawLineFrom = this;
                    mapStructureGraph.linePosition = action.mousePosition;
                }
                break;

            // Process Mouse Up Events
            case EventType.MouseUp:
                // left click
                if (action.button == 0 && isLeftClickDragging)
                {
                    isLeftClickDragging = false;
                }
                break;

            // Process Mouse Drag Events
            case EventType.MouseDrag:
                // Left click
                if (action.button == 0)
                {
                    isLeftClickDragging = true;
                    DragBlock(action.delta);
                }
                break;

            default:
                break;
        }
    }

    public void Draw(GUIStyle blockStyle)
    {
        //  Draw Block Using Begin Area
        GUILayout.BeginArea(rect, blockStyle);

        // Start Region To Detect Popup Selection Changes
        EditorGUI.BeginChangeCheck();

        // if the room blcok has a parent or is of type entrance then display a label else display a popup
        if (parentBlockIDList.Count > 0 || blockType.isEntrance)
        {
            // Display a label that can't be changed
            EditorGUILayout.LabelField(blockType.blockTypeName);
        }
        else
        {
            // Display a popup using the blockType name values that can be selected from (default to the currently set blockType)
            int selected = blockTypeList.blocks.FindIndex(x => x == blockType);

            string[] roomArray = new string[blockTypeList.blocks.Count];

            for (int i = 0; i < blockTypeList.blocks.Count; i++)
                if (blockTypeList.blocks[i].displayable) roomArray[i] = blockTypeList.blocks[i].blockTypeName;

            int selection = EditorGUILayout.Popup("", selected, roomArray);

            blockType = blockTypeList.blocks[selection];

            // If the block type selection has changed making child connections potentially invalid
            if (blockTypeList.blocks[selected].isConnector && !blockTypeList.blocks[selection].isConnector || !blockTypeList.blocks[selected].isConnector && blockTypeList.blocks[selection].isConnector || !blockTypeList.blocks[selected].isBossRoom && blockTypeList.blocks[selection].isBossRoom)
            {
                // If a block type has been changed and it already has children then delete the parent child links since we need to revalidate any
                if (childBlockIDList.Count > 0)
                {
                    for (int i = childBlockIDList.Count - 1; i >= 0; i--)
                    {
                        // Get child
                        BlockSO childBlock = mapStructureGraph.GetBlock(childBlockIDList[i]);

                        // If the child is not null
                        if (childBlock != null)
                        {
                            // Remove childID from parent block
                            RemoveChildBlockIDFromBlock(childBlock.id);

                            // Remove parentID from child block
                            childBlock.RemoveParentBlockIDFromBlock(id);
                        }
                    }
                }
            }
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);

        GUILayout.EndArea();
    }

    public bool AddChildBlockIDToBlock(string childID)
    {

        bool isConnectedBossBlockAlready = false;
        bool isChildBlockValid = true;

        #region Validation Check

        // Check if there is there already a connected boss room in the graph
        foreach (BlockSO block in mapStructureGraph.blockList)
            if (block.blockType.isBossRoom && block.parentBlockIDList.Count > 0) isConnectedBossBlockAlready = true;

        // if the child has a type of boss room and there is already a connected boss then return false
        if (mapStructureGraph.GetBlock(childID).blockType.isBossRoom && isConnectedBossBlockAlready) isChildBlockValid = false;

        // If the child has a type of none then return false
        if (mapStructureGraph.GetBlock(childID).blockType.isUnassigned) isChildBlockValid = false;

        // If the block already has a child with this child ID return false
        if (childBlockIDList.Contains(childID)) isChildBlockValid = false;

        // If this block ID and the child ID are the same return false
        if (id == childID) isChildBlockValid = false;

        // If this childID is already in the parentID list return false
        if (parentBlockIDList.Contains(childID)) isChildBlockValid = false;

        // If the child block already has a parent return false
        if (mapStructureGraph.GetBlock(childID).parentBlockIDList.Count > 0) isChildBlockValid = false;

        // If child is a connector and this block is a connector return false
        if (mapStructureGraph.GetBlock(childID).blockType.isConnector && blockType.isConnector) isChildBlockValid = false;

        // If child is not a connector and this block is not a connector return false
        if (!mapStructureGraph.GetBlock(childID).blockType.isConnector && !blockType.isConnector) isChildBlockValid = false;

        // If adding a corridor check that this block has < the maximum permitted child corridors
        if (mapStructureGraph.GetBlock(childID).blockType.isConnector && childBlockIDList.Count >= Settings.maxChildCorridors) isChildBlockValid = false;

        // if the child room is an entrance return false
        if (mapStructureGraph.GetBlock(childID).blockType.isEntrance) isChildBlockValid = false;

        // If adding a room to a connector check that this connector doesn't already have a room added
        if (!mapStructureGraph.GetBlock(childID).blockType.isConnector && childBlockIDList.Count > 0) isChildBlockValid =  false;

        #endregion

        // Check child can be added validly to parent
        if (isChildBlockValid)
        {
            childBlockIDList.Add(childID);
            return true;
        }
        return false;
    }

    public bool AddParentBlockIDToBlock(string parentID)
    {
        parentBlockIDList.Add(parentID);
        return true;
    }

    public bool RemoveChildBlockIDFromBlock(string childID)
    {
        // if the block contains the child ID then remove it
        if (childBlockIDList.Contains(childID))
        {
            childBlockIDList.Remove(childID);
            return true;
        }
        return false;
    }

    public bool RemoveParentBlockIDFromBlock(string parentID)
    {
        // if the block contains the parent ID then remove it
        if (parentBlockIDList.Contains(parentID))
        {
            parentBlockIDList.Remove(parentID);
            return true;
        }
        return false;
    }

    public void DragBlock(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }

#endif

    #endregion Editor Code
}
