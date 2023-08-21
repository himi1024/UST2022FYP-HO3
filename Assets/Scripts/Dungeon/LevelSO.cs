using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="Level_", menuName ="Scriptable Objects/Dungeon/Dungeon level")]
public class LevelSO : ScriptableObject
{
    public string missionType;
    public string levelName;

    public List<BlockTemplateSO> blockTemplateList;

    public List<MapStructureGraphSO> mapStructureGraphList;

#if UNITY_EDITOR

    public void OnValidate()
    {
        CheckBlockTemplateList();
        CheckMapStructureGraphList();



    }

    public void CheckBlockTemplateList()
    {
        bool hasEntrance = false;
        bool hasLRCorridor = false;
        bool hasTDCorridor = false;

        foreach (BlockTemplateSO blockTemplate in blockTemplateList)
        {
            if (blockTemplate == null) continue;
            hasEntrance = (blockTemplate.blockType.isEntrance ) ? true : (hasEntrance)? true: false ;
            hasLRCorridor = (blockTemplate.blockType.isConnectorHorizontal && !hasLRCorridor) ? true : (hasLRCorridor)? true: false;
            hasTDCorridor = (blockTemplate.blockType.isConnectorVertical && !hasTDCorridor) ? true : (hasTDCorridor)? true: false;

        }

        if (!hasEntrance) Debug.Log("Please define Entrance Block in " + this.name.ToString());
        if (!hasLRCorridor) Debug.Log("Please define Horizontal Connector Block in " + this.name.ToString());
        if (!hasTDCorridor) Debug.Log("Please define Vertical Connector Block Block in " + this.name.ToString());
    }

    // check there are block template for each blocks in the graph
    public void CheckMapStructureGraphList() {
        foreach (MapStructureGraphSO mapGraph in mapStructureGraphList) {
            if (mapGraph == null) return;
            foreach (BlockSO block in mapGraph.blockList)
            {
                if (block == null) continue;
                if (block.blockType.isEntrance || block.blockType.isConnector) continue;
                CheckBlockTypeExist(block, mapGraph);
            }
        }

    }


    public void CheckBlockTypeExist(BlockSO block, MapStructureGraphSO mapGraph) {
        bool blockFound = false;
        foreach (BlockTemplateSO blockTemplate in blockTemplateList) {
            blockFound = (block.blockType == blockTemplate.blockType) ? true : false;
            if (blockFound) break;
        }

        if (!blockFound) Debug.Log("Please specfy a " + block.blockType.name.ToString() + " block template in " + mapGraph.name.ToString());
    }
#endif


}
