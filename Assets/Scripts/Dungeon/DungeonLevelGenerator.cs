using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonLevelGenerator : SingletonMonoBehaviour<DungeonLevelGenerator>
{
    public Dictionary<string, BlockInfo> dungeonBuilderblockInfoDictionary = new Dictionary<string, BlockInfo>();
    private Dictionary<string, BlockTemplateSO> blockTemplateDictionary = new Dictionary<string, BlockTemplateSO>();
    private List<BlockTemplateSO> blockTemplateList = null;
    private BlockTypeListSO blockTypeList;
    private bool dungeonBuildSuccessful;


    protected override void Awake()
    {
        base.Awake();

        // Load the block type list
        blockTypeList = GameResources.Instance.blockTypeList;

        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
    }

    //Generate the dungeon according to the map graph and block template
    //return: true when success, false when fail

    public bool GenerateDungeon(LevelSO level)
    {

        blockTemplateList = level.blockTemplateList;


        // Load blockTemplateDictionary
        blockTemplateDictionary.Clear();
        foreach (BlockTemplateSO blockTemplate in blockTemplateList)
            if (!blockTemplateDictionary.ContainsKey(blockTemplate.guid))
                blockTemplateDictionary.Add(blockTemplate.guid, blockTemplate);

        dungeonBuildSuccessful = false;
        int buildAttempts = 0;

        while (!dungeonBuildSuccessful && buildAttempts <= Settings.maxDungeonBuildAttempts)
        {
            buildAttempts++;

            // Randomly select a map structure graph
            MapStructureGraphSO mapStructureGraph = null;
            if (level.mapStructureGraphList.Count > 0) mapStructureGraph = level.mapStructureGraphList[UnityEngine.Random.Range(0, level.mapStructureGraphList.Count)];
            int GraphbuildAttempt = 0;
            dungeonBuildSuccessful = false;

            while (!dungeonBuildSuccessful && GraphbuildAttempt <= Settings.maxDungeonRebuildAttemptsForMapStructureGraph)
            {
                //Destory previous gameObject and Dictionary
                if (dungeonBuilderblockInfoDictionary.Count > 0)
                {
                    foreach (KeyValuePair<string, BlockInfo> keyPair in dungeonBuilderblockInfoDictionary)
                    {
                        BlockInfo blockInfo = keyPair.Value;
                        if (blockInfo.instantiatedBlock != null) Destroy(blockInfo.instantiatedBlock.gameObject);

                    }
                    dungeonBuilderblockInfoDictionary.Clear();
                }

                GraphbuildAttempt++;

                dungeonBuildSuccessful = CreateDungeon(mapStructureGraph);
            }

        }

        if (dungeonBuildSuccessful)
        {
            foreach (KeyValuePair<string, BlockInfo> keyPair in dungeonBuilderblockInfoDictionary)
            {
                BlockInfo blockInfo = keyPair.Value;

                float blockInfoX = blockInfo.realWorldLowerBound.x - blockInfo.templateLowerBound.x;
                float blockInfoY = blockInfo.realWorldLowerBound.y - blockInfo.templateLowerBound.y;
                Vector3 blockPosition = new Vector3(blockInfoX, blockInfoY, 0f);

                GameObject blockGameObject = Instantiate(blockInfo.blockPrefab, blockPosition, Quaternion.identity, transform);

                InstantiatedBlock instantiatedBlock = blockGameObject.GetComponent<InstantiatedBlock>();

                instantiatedBlock.blockInfo = blockInfo;

                instantiatedBlock.Initialise(blockGameObject);

                blockInfo.instantiatedBlock = instantiatedBlock;
            }
        }

        return dungeonBuildSuccessful;


    }

    public bool CreateDungeon(MapStructureGraphSO mapStructureGraph)
    {
        Queue<BlockSO> openBlockQueue = new Queue<BlockSO>();
        BlockSO entrance = mapStructureGraph.GetBlock(blockTypeList.blocks.Find(x => x.isEntrance));
        if (entrance != null) openBlockQueue.Enqueue(entrance);
        else
        {
            Debug.Log("No Entrance Node");
            return false;
        }
        bool noOverLaps = true;
        noOverLaps = ProcessOpenQueue(mapStructureGraph, openBlockQueue, noOverLaps);
        if (openBlockQueue.Count == 0 && noOverLaps) return true;

        return false;
    }

    public bool ProcessOpenQueue(MapStructureGraphSO mapStructureGraph, Queue<BlockSO> openBlockQueue, bool noOverLaps)
    {
        while (openBlockQueue.Count > 0 && noOverLaps)
        {
            BlockSO block = openBlockQueue.Dequeue();

            foreach (BlockSO childBlock in mapStructureGraph.GetChildBlocks(block))
                openBlockQueue.Enqueue(childBlock);

            if (block.blockType.isEntrance)
            {
                BlockTemplateSO blockTemplate = GetTemplate(block.blockType);
                BlockInfo blockInfo = CreateBlockInfo(blockTemplate, block);
                blockInfo.isPlaced = true;

                dungeonBuilderblockInfoDictionary.Add(blockInfo.id, blockInfo);
            }
            else
            {
                BlockInfo parentblockInfo = dungeonBuilderblockInfoDictionary[block.parentBlockIDList[0]];
                noOverLaps = PositionBlockWithoutOverlapping(block, parentblockInfo);

            }
        }

        return noOverLaps;
    }

    public bool PositionBlockWithoutOverlapping(BlockSO block, BlockInfo parentBlockInfo)
    {
        bool OverLapping = true;

        while (OverLapping)
        {
            List<GatePosition> unlinkedAvailableGateList = GetunlinkedAvailableGateList(parentBlockInfo.gatePositionList).ToList();

            if (unlinkedAvailableGateList.Count == 0) return false;

            GatePosition parentGate = unlinkedAvailableGateList[UnityEngine.Random.Range(0, unlinkedAvailableGateList.Count)];

            BlockTemplateSO blockTemplate = null;
            if (block.blockType.isConnector)
                switch (parentGate.gatePosition)
                {
                    case Orientation.top:
                    case Orientation.down:
                        blockTemplate = GetTemplate(blockTypeList.blocks.Find(x => x.isConnectorVertical));
                        break;

                    case Orientation.left:
                    case Orientation.right:
                        blockTemplate = GetTemplate(blockTypeList.blocks.Find(x => x.isConnectorHorizontal));
                        break;
                    default:
                        break;
                }
            else
                blockTemplate = GetTemplate(block.blockType);

            BlockInfo blockInfo = CreateBlockInfo(blockTemplate, block);


            if (PlaceblockInfoToRealWorld(parentBlockInfo, parentGate, blockInfo))
            {
                OverLapping = false;
                blockInfo.isPlaced = true;
                dungeonBuilderblockInfoDictionary.Add(blockInfo.id, blockInfo);
            }
            else OverLapping = true;
        }

        return true;
    }

    private IEnumerable<GatePosition> GetunlinkedAvailableGateList(List<GatePosition> gatePositionList)
    {
        foreach (GatePosition gate in gatePositionList)
        {
            if (!gate.isLinked && !gate.isUnavailable)
                yield return gate;

        }
    }

    private bool PlaceblockInfoToRealWorld(BlockInfo parentblockInfo, GatePosition parentGate, BlockInfo blockInfo)
    {
        GatePosition gate = null;

        foreach (GatePosition gatePosition in blockInfo.gatePositionList)
        {
            if (gatePosition.gatePosition == Orientation.top && parentGate.gatePosition == Orientation.down)
                gate = gatePosition;
            else if (gatePosition.gatePosition == Orientation.left && parentGate.gatePosition == Orientation.right)
                gate = gatePosition;
            else if (gatePosition.gatePosition == Orientation.right && parentGate.gatePosition == Orientation.left)
                gate = gatePosition;
            else if (gatePosition.gatePosition == Orientation.down && parentGate.gatePosition == Orientation.top)
                gate = gatePosition;
        }

        if (gate == null)
        {
            parentGate.isUnavailable = true;
            return false;
        }

        Vector2Int parentGateRealWorldPosition = parentblockInfo.realWorldLowerBound + parentGate.position - parentblockInfo.templateLowerBound;
        Vector2Int adjustment = Vector2Int.zero;

        switch (gate.gatePosition)
        {
            case Orientation.top:
                adjustment = new Vector2Int(0, -1);
                break;
            case Orientation.down:
                adjustment = new Vector2Int(0, +1);
                break;
            case Orientation.right:
                adjustment = new Vector2Int(-1, 0);
                break;
            case Orientation.left:
                adjustment = new Vector2Int(+1, 0);
                break;
            default:
                break;

        }

        blockInfo.realWorldLowerBound = parentGateRealWorldPosition + adjustment + blockInfo.templateLowerBound - gate.position;
        blockInfo.realWorldUpperBound = blockInfo.realWorldLowerBound + blockInfo.templateUpperBound - blockInfo.templateLowerBound;

        bool overlapWithBlock = CheckForOverlapping(blockInfo);

        if (!overlapWithBlock)
        {
            parentGate.isLinked = true;
            parentGate.isUnavailable = true;
            gate.isLinked = true;
            gate.isUnavailable = true;
            return true;
        }
        else
            parentGate.isUnavailable = true;
        return false;
    }

    // check the testing block is overplapping with other blocks in the dictionary
    // return true if overlapping detected
    private bool CheckForOverlapping(BlockInfo testBlock)
    {
        bool BlockOverlapping = false;
        foreach (KeyValuePair<string, BlockInfo> keyPair in dungeonBuilderblockInfoDictionary)
        {
            BlockInfo blockInfo = keyPair.Value;
            if (blockInfo.id == testBlock.id || !blockInfo.isPlaced) continue;

            BlockOverlapping = Mathf.Max(testBlock.realWorldLowerBound.x, blockInfo.realWorldLowerBound.x) <= Mathf.Min(testBlock.realWorldUpperBound.x, blockInfo.realWorldUpperBound.x)
                                    && Mathf.Max(testBlock.realWorldLowerBound.y, blockInfo.realWorldLowerBound.y) <= Mathf.Min(testBlock.realWorldUpperBound.y, blockInfo.realWorldUpperBound.y);

            if (BlockOverlapping) break;
        }
        return BlockOverlapping;
    }

    // randomly get a block template with the correct blocktype
    public BlockTemplateSO GetTemplate(BlockTypeSO blockType)
    {
        List<BlockTemplateSO> list = new List<BlockTemplateSO>();

        foreach (BlockTemplateSO blockTemplate in blockTemplateList)
            if (blockTemplate.blockType == blockType) list.Add(blockTemplate);

        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    // create blockInfo and prerform deep copy to lists
    public BlockInfo CreateBlockInfo(BlockTemplateSO blockTemplate, BlockSO block)
    {
        BlockInfo blockInfo = new BlockInfo
        {
            templateID = blockTemplate.guid,
            id = block.id,
            blockPrefab = blockTemplate.blockPrefab,
            blockType = blockTemplate.blockType,
            templateLowerBound = blockTemplate.lowerBound,
            templateUpperBound = blockTemplate.upperBound,
            realWorldLowerBound = blockTemplate.lowerBound,
            realWorldUpperBound = blockTemplate.upperBound,
            enemySpawnLists = blockTemplate.enemySpawnLists,
            enemyPositionArr = blockTemplate.enemyPositionArr,
            enemySpawnConstraint = blockTemplate.enemySpawnConstraint,

            musicBattle = blockTemplate.musicBattle,
            musicNobattle = blockTemplate.musicNobattle,

            childBlockIDList = new List<string>()
        };

        foreach (string value in block.childBlockIDList)
            blockInfo.childBlockIDList.Add(value);

        blockInfo.gatePositionList = CopyGateList(blockTemplate);

        if (blockInfo.blockType.isEntrance)
        {
            blockInfo.parentBlockInfoID = "";
            blockInfo.isVisited = true;
            blockInfo.allEnemiesDefeated = true;
            GameManager.Instance.SetCurrentBlockInfo(blockInfo);
        }
        else
        {
            blockInfo.parentBlockInfoID = block.parentBlockIDList[0];
            blockInfo.isVisited = false;
        }

        return blockInfo;

    }

    public List<GatePosition> CopyGateList(BlockTemplateSO blockTemplate)
    {
        List<GatePosition> gateList = new List<GatePosition>();
        foreach (GatePosition gate in blockTemplate.gateList)
        {
            GatePosition newGate = new GatePosition
            {
                position = gate.position,
                gatePosition = gate.gatePosition,
                gatePrefab = gate.gatePrefab,
                isLinked = gate.isLinked,
                isUnavailable = gate.isUnavailable,
                gateDuplicateStartPosition = gate.gateDuplicateStartPosition,
                gateHeight = gate.gateHeight,
                gateWidth = gate.gateWidth
            };

            gateList.Add(newGate);
        }
        return gateList;
    }
}