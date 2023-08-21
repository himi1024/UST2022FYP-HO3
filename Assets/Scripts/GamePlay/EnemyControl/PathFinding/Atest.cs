using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Atest : MonoBehaviour
{
    private InstantiatedBlock instantiatedRoom;
    private Grid grid;
    private Tilemap frontTilemap;
    private Tilemap pathTilemap;
    private Vector3Int startGridPosition;
    private Vector3Int endGridPosition;
    private TileBase startPathTile;
    private TileBase finishPathTile;

    private Vector3Int noValue = new Vector3Int(9999, 9999, 9999);
    private List<Vector3> pathStack;

    private void OnEnable()
    {
        // Subscribe to the onRoomChanged event
        LevelManager.visitRoom += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        // Unsubscribe to the onRoomChanged event
        LevelManager.visitRoom -= StaticEventHandler_OnRoomChanged;
    }

    private void Start()
    {
        startPathTile = GameResources.Instance.preferredPath;
        finishPathTile = GameResources.Instance.collisionTiles[1];
    }


    private void StaticEventHandler_OnRoomChanged(BlockInfo roomChangedEventArgs)
    {
        pathStack = null;
        instantiatedRoom = roomChangedEventArgs.instantiatedBlock;
        frontTilemap = instantiatedRoom.transform.Find("Grid/FrontTilemap").GetComponent<Tilemap>();
        grid = instantiatedRoom.transform.GetComponentInChildren<Grid>();
        startGridPosition = noValue;
        endGridPosition = noValue;

        SetUpPathTilemap();
    }

    /// <summary>
    /// Use a clone of the front tilemap for the path tilemap.  If not created then create one, else use the exisitng one.
    /// </summary>
    private void SetUpPathTilemap()
    {
        Transform tilemapCloneTransform = instantiatedRoom.transform.Find("Grid/FrontTilemap");
        //Debug.Log(instantiatedRoom);

        // If the front tilemap hasn't been cloned then clone it
        if (tilemapCloneTransform != null)
        {
            pathTilemap = Instantiate(frontTilemap, grid.transform);
            pathTilemap.GetComponent<TilemapRenderer>().sortingOrder = 2;
            pathTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.dimmedMaterial;
            pathTilemap.gameObject.tag = "Untagged";
        }
        // else use it
        else
        {
            pathTilemap = instantiatedRoom.transform.Find("Grid/FrontTilemap(Clone)").GetComponent<Tilemap>();
            pathTilemap.ClearAllTiles();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        //Debug.Log(instantiatedRoom == null);
        if (instantiatedRoom == null || startPathTile == null || finishPathTile == null || grid == null || pathTilemap == null) return;

        if (Input.GetKeyDown(KeyCode.I))
        {
            ClearPath();
            SetStartPosition();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            ClearPath();
            SetEndPosition();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            DisplayPath();
        }
    }


    /// <summary>
    /// Set the start position and the start tile on the front tilemap
    /// </summary>
    private void SetStartPosition()
    {
        Debug.Log("Use");
        if (startGridPosition == noValue)
        {
            startGridPosition = grid.WorldToCell(HelperFunctions.GetMousePosition());

            if (!IsPositionWithinBounds(startGridPosition))
            {
                startGridPosition = noValue;
                return;
            }

            pathTilemap.SetTile(startGridPosition, startPathTile);
        }
        else
        {
            pathTilemap.SetTile(startGridPosition, null);
            startGridPosition = noValue;
        }
    }


    /// <summary>
    /// Set the end position and the end tile on the front tilemap
    /// </summary>
    private void SetEndPosition()
    {
        if (endGridPosition == noValue)
        {
            endGridPosition = grid.WorldToCell(HelperFunctions.GetMousePosition());

            if (!IsPositionWithinBounds(endGridPosition))
            {
                endGridPosition = noValue;
                return;
            }

            pathTilemap.SetTile(endGridPosition, finishPathTile);
        }
        else
        {
            pathTilemap.SetTile(endGridPosition, null);
            endGridPosition = noValue;
        }
    }


    /// <summary>
    /// Check if the position is within the lower and upper bounds of the room
    /// </summary>
    private bool IsPositionWithinBounds(Vector3Int position)
    {
        // If  position is beyond grid then return false
        if (position.x < instantiatedRoom.blockInfo.templateLowerBound.x || position.x > instantiatedRoom.blockInfo.templateUpperBound.x
            || position.y < instantiatedRoom.blockInfo.templateLowerBound.y || position.y > instantiatedRoom.blockInfo.templateUpperBound.y)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    /// <summary>
    /// Clear the path and reset the start and finish positions
    /// </summary>
    private void ClearPath()
    {
        // Clear Path
        if (pathStack == null) return;

        foreach (Vector3 worldPosition in pathStack)
        {
            pathTilemap.SetTile(grid.WorldToCell(worldPosition), null);
        }

        pathStack = null;

        //Clear Start and Finish Squares
        endGridPosition = noValue;
        startGridPosition = noValue;
    }

    /// <summary>
    /// Build and display the AStar path between the start and finish positions
    /// </summary>
    private void DisplayPath()
    {
        if (startGridPosition == noValue || endGridPosition == noValue) return;


        pathStack = PathFinding.PathFind(instantiatedRoom.blockInfo, startGridPosition.x, startGridPosition.y, endGridPosition.x, endGridPosition.y);
        //pathStack = AStar.BuildPath(instantiatedRoom.room, startGridPosition, endGridPosition);

        if (pathStack == null) return;

        Debug.Log(pathStack.Count);

        foreach (Vector3 worldPosition in pathStack)
        {
            Debug.Log(worldPosition);
            pathTilemap.SetTile(grid.WorldToCell(worldPosition), finishPathTile);
        }
    }

}