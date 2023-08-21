using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class MapStructureGraphEditor : EditorWindow
{
    // Store attributes of map blocks
    private GUIStyle blockStyle;
    private GUIStyle blockSelectedStyle;
    private static MapStructureGraphSO currentMapStructureGraph;

    // Store the block that the mouse is pointing to
    private BlockSO currentBlock = null;

    // Store block type list during OnEnable
    private BlockTypeListSO blockTypeList;

    // Blocks layout attributes
    private const float blockWidth = 160f;
    private const float blockHeight = 75f;
    private const int blockPadding = 25;
    private const int blockBorder = 12;

    // Connecting line attributes
    private const float lineWidth = 3f;
    private const float lineArrowSize = 6f;

    // Register the Editor on Unity menu
    [MenuItem("Map Structure Editor", menuItem = "Window/Dungeon Editor/Map Structure Editor")]
    private static void OpenWindow()
    {
        GetWindow<MapStructureGraphEditor>("Map Structure Editor");
    }

    private void OnEnable()
    {
        // Add selected block to listener queue
        Selection.selectionChanged += InspectorSelectionChanged;

        // Define block layout style
        blockStyle = new GUIStyle();
        blockStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        blockStyle.normal.textColor = Color.white;
        blockStyle.padding = new RectOffset(blockPadding, blockPadding, blockPadding, blockPadding);
        blockStyle.border = new RectOffset(blockBorder, blockBorder, blockBorder, blockBorder);

        // Define selected block style
        blockSelectedStyle = new GUIStyle();
        blockSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
        blockSelectedStyle.normal.textColor = Color.white;
        blockSelectedStyle.padding = new RectOffset(blockPadding, blockPadding, blockPadding, blockPadding);
        blockSelectedStyle.border = new RectOffset(blockBorder, blockBorder, blockBorder, blockBorder);

        // Load block types
        blockTypeList = GameResources.Instance.blockTypeList;
    }

    private void OnDisable()
    {
        // Remove selected block from listener queue
        Selection.selectionChanged -= InspectorSelectionChanged;
    }

    // click GraphSO to open window
    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        MapStructureGraphSO MapStructureGraph = EditorUtility.InstanceIDToObject(instanceID) as MapStructureGraphSO;

        // Check that the editor window is MapStructure window
        if (MapStructureGraph != null)
        {
            OpenWindow();
            currentMapStructureGraph = MapStructureGraph;
            return true;
        }
        return false;
    }

    // This is the main UI output function
    private void OnGUI()
    {
        Event action = Event.current;

        if (currentMapStructureGraph != null)
        {
            // Draw dragged line
            if (currentMapStructureGraph.linePosition != Vector2.zero)
            {
                //Draw line from block to line position
                Handles.DrawBezier
                (
                    currentMapStructureGraph.blockToDrawLineFrom.rect.center,
                    currentMapStructureGraph.linePosition,
                    currentMapStructureGraph.blockToDrawLineFrom.rect.center,
                    currentMapStructureGraph.linePosition,
                    Color.white,
                    null,
                    lineWidth
                );
            }

            // Get block that mouse is over if it's null or not currently being dragged
            if (currentBlock == null || currentBlock.isLeftClickDragging == false)
            {
                currentBlock = IsMouseOverBlock(action);
            }

            // if mouse isn't over a block or we are currently dragging a line from the block then process graph events
            if (currentBlock == null || currentMapStructureGraph.blockToDrawLineFrom != null)
            {
                // Total 3 cases: Up, Down, Drag
                switch (action.type)
                {
                    // Process Mouse Down Events
                    case EventType.MouseDown:
                        // Right click (show menu)
                        if (action.button == 1)
                        {
                            GenericMenu menu = new GenericMenu();

                            // 1. Menu item that create block
                            menu.AddItem(
                                new GUIContent("Create Block"),
                                false,
                                // lambda function being processed after clicking the item
                                () => 
                                {
                                    // If current graph empty then add entrance block first
                                    if (currentMapStructureGraph.blockList.Count == 0)
                                    {
                                        Createblock(new Vector2(200f, 200f), blockTypeList.blocks.Find(x => x.isEntrance));
                                    }
                                    // create a block
                                    Createblock(action.mousePosition, blockTypeList.blocks.Find(x => x.isUnassigned));
                                }
                                );
                            menu.AddSeparator("");

                            // 2. Menu item that delete selected block links
                            menu.AddItem(
                                new GUIContent("Delete Selected Block Links"),
                                false,
                                // lambda function being processed after clicking the item
                                () => 
                                {
                                    // Iterate through all blocks
                                    foreach (BlockSO block in currentMapStructureGraph.blockList)
                                    {
                                        if (block.isSelected && block.childBlockIDList.Count > 0)
                                        {
                                            for (int i = block.childBlockIDList.Count - 1; i >= 0; i--)
                                            {
                                                // Get child
                                                BlockSO childBlock = currentMapStructureGraph.GetBlock(block.childBlockIDList[i]);

                                                // If the child is selected
                                                if (childBlock != null && childBlock.isSelected)
                                                {
                                                    // Remove childID from parent
                                                    block.RemoveChildBlockIDFromBlock(childBlock.id);

                                                    // Remove parentID from child
                                                    childBlock.RemoveParentBlockIDFromBlock(block.id);
                                                }
                                            }
                                        }
                                    }

                                    // Clear all selected blocks
                                    foreach (BlockSO block in currentMapStructureGraph.blockList)
                                        if (block.isSelected) block.isSelected = false;
                                }
                                );

                            // 3. Menu item that delete selected blocks
                            menu.AddItem(
                                new GUIContent("Delete Selected Blocks"),
                                false,
                                // lambda function being processed after clicking the item
                                () =>
                                {
                                    Queue<BlockSO> blockDeletionQueue = new Queue<BlockSO>();

                                    // Loop through all blocks
                                    foreach (BlockSO block in currentMapStructureGraph.blockList)
                                    {
                                        if (block.isSelected && !block.blockType.isEntrance)
                                        {
                                            blockDeletionQueue.Enqueue(block);

                                            // loop through children
                                            foreach (string childBlockID in block.childBlockIDList)
                                            {
                                                // get child block
                                                BlockSO childBlock = currentMapStructureGraph.GetBlock(childBlockID);

                                                // Remove parentID from child
                                                if (childBlock != null)
                                                    childBlock.RemoveParentBlockIDFromBlock(block.id);
                                            }

                                            // loop through parents
                                            foreach (string parentBlockID in block.parentBlockIDList)
                                            {
                                                // get parent block
                                                BlockSO parentBlock = currentMapStructureGraph.GetBlock(parentBlockID);

                                                if (parentBlock != null)
                                                    // Remove childID from parent block
                                                    parentBlock.RemoveChildBlockIDFromBlock(block.id);
                                            }
                                        }
                                    }

                                    // Delete queued blocks
                                    while (blockDeletionQueue.Count > 0)
                                    {
                                        // Get block from queue
                                        BlockSO blockToDelete = blockDeletionQueue.Dequeue();

                                        // Remove block from dictionary
                                        currentMapStructureGraph.blockDictionary.Remove(blockToDelete.id);

                                        // Remove block from list
                                        currentMapStructureGraph.blockList.Remove(blockToDelete);

                                        // Remove block from Asset database
                                        DestroyImmediate(blockToDelete, true);

                                        // Save asset database
                                        AssetDatabase.SaveAssets();

                                    }
                                }
                                );

                            menu.ShowAsContext();
                        }
                        break;

                    // Process Mouse Up Events
                    case EventType.MouseUp:
                        // if releasing the right mouse button and currently dragging a line
                        if (action.button == 1 && currentMapStructureGraph.blockToDrawLineFrom != null)
                        {
                            // Check if over a block
                            BlockSO block = IsMouseOverBlock(action);

                            // if pointing block then set it as a child of the parent if it can be added
                            if (block != null && currentMapStructureGraph.blockToDrawLineFrom.AddChildBlockIDToBlock(block.id))
                                block.AddParentBlockIDToBlock(currentMapStructureGraph.blockToDrawLineFrom.id);

                            // Clear line drag
                            currentMapStructureGraph.blockToDrawLineFrom = null;
                            currentMapStructureGraph.linePosition = Vector2.zero;
                        }
                        break;

                    // Process Mouse Drag Event
                    case EventType.MouseDrag:
                        // Right click (drag line)
                        if (action.button == 1 && currentMapStructureGraph.blockToDrawLineFrom != null)
                            currentMapStructureGraph.linePosition += action.delta;
                        // Left click (move graph)
                        // Noted that this action moves a whole graph, not a single block
                        else if (action.button == 0)
                            for (int i = 0; i < currentMapStructureGraph.blockList.Count; i++)
                                currentMapStructureGraph.blockList[i].DragBlock(action.delta);
                        break;

                    default:
                        break;
                }
            }
            // else process block events
            // e.g. move single block
            else
                currentBlock.ProcessEvents(action);

            // Draw Connections Between blocks
            // Loop through all blocks to draw connection lines
            foreach (BlockSO block in currentMapStructureGraph.blockList)
            {
                if (block.childBlockIDList.Count > 0)
                {
                    // Loop through child blocks
                    foreach (string childBlockID in block.childBlockIDList)
                    {
                        // get child block from dictionary
                        if (currentMapStructureGraph.blockDictionary.ContainsKey(childBlockID))
                        {
                            BlockSO parentBlock = block;
                            BlockSO childBlock = currentMapStructureGraph.blockDictionary[childBlockID];

                            // get line start and end position
                            Vector2 startPosition = parentBlock.rect.center;
                            Vector2 endPosition = childBlock.rect.center;

                            // calculate midway point
                            Vector2 midPosition = (endPosition + startPosition) / 2f;

                            // Vector from start to end position of line
                            Vector2 direction = endPosition - startPosition;

                            // Calulate normalised perpendicular positions from the mid point
                            Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * lineArrowSize;
                            Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * lineArrowSize;

                            // Calculate mid point offset position for arrow head
                            Vector2 arrowHeadPoint = midPosition + direction.normalized * lineArrowSize;

                            // Draw Arrow
                            Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, lineWidth);
                            Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, lineWidth);

                            // Draw line
                            Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.white, null, lineWidth);
                        }
                    }
                }
            }

            // Loop through all blocks and draw themm
            foreach (BlockSO block in currentMapStructureGraph.blockList)
            {
                if (block.isSelected) block.Draw(blockSelectedStyle);
                else block.Draw(blockStyle);
            }
        }
        
        Repaint();
    }

    // Create block 
    private void Createblock(object mousePositionObject, BlockTypeSO blockType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

        // create scriptable object
        BlockSO block = ScriptableObject.CreateInstance<BlockSO>();

        // add to current graph
        currentMapStructureGraph.blockList.Add(block);

        // set block values
        block.Initialise(new Rect(mousePosition, new Vector2(blockWidth, blockHeight)), currentMapStructureGraph, blockType);

        // add block to the graph scriptable object asset database
        AssetDatabase.AddObjectToAsset(block, currentMapStructureGraph);

        AssetDatabase.SaveAssets();

        // Refresh the dictionary
        currentMapStructureGraph.OnValidate();
    }

    // Return the block that the mouse is pointing to
    private BlockSO IsMouseOverBlock(Event currentEvent)
    {
        for (int i = currentMapStructureGraph.blockList.Count - 1; i >= 0; i--)
            if (currentMapStructureGraph.blockList[i].rect.Contains(currentEvent.mousePosition))
                return currentMapStructureGraph.blockList[i];
        return null;
    }

    private void InspectorSelectionChanged()
    {
        MapStructureGraphSO mapStructureGraph = Selection.activeObject as MapStructureGraphSO;
        if (mapStructureGraph != null) currentMapStructureGraph = mapStructureGraph;
    }
}
