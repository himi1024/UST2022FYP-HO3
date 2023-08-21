using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedBlock : MonoBehaviour
{
    public BlockInfo blockInfo;
    public Grid grid;
    public Tilemap groundTilemap;
    public Tilemap decorationTilemap;
    public Tilemap shadowTilemap;
    public Tilemap frontTilemap;
    public Tilemap collisionTilemap;
    public Tilemap minimapTilemap;
    public Bounds colliderBounds;


    private BoxCollider2D boxCollider2D;


    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();

        colliderBounds = boxCollider2D.bounds;
    }


    public void Initialise(GameObject blockObjectGameObject) {
        grid = blockObjectGameObject.GetComponentInChildren<Grid>();

        Tilemap[] tilemaps = blockObjectGameObject.GetComponentsInChildren<Tilemap>();

        foreach(Tilemap tilemap in tilemaps){
            //Debug.Log(tilemap.gameObject.tag);
            switch (tilemap.gameObject.tag) {
                case "groundTilemaps":
                    groundTilemap = tilemap;
                    break;
                case "decorationTilemaps":
                    decorationTilemap = tilemap;
                    break;
                case "shadowTilemaps":
                    shadowTilemap = tilemap;
                    break;
                case "frontTilemaps":
                    frontTilemap = tilemap;
                    break;
                case "collisionTilemaps":
                    collisionTilemap = tilemap;
                    break;
                case "minimapTilemaps":
                    minimapTilemap = tilemap;
                    break;
                
            }
        }

        foreach (GatePosition gate in blockInfo.gatePositionList)
        {
            if (!gate.isLinked)
            {
                if (collisionTilemap != null)
                    BlockOnTilemapLayer(collisionTilemap, gate);

                if (decorationTilemap != null)
                    BlockOnTilemapLayer(decorationTilemap, gate);

                if (minimapTilemap != null)
                    BlockOnTilemapLayer(minimapTilemap, gate);

                if (frontTilemap != null)
                    BlockOnTilemapLayer(frontTilemap, gate);

                if (shadowTilemap != null)
                    BlockOnTilemapLayer(shadowTilemap, gate);

                if (groundTilemap != null)
                    BlockOnTilemapLayer(groundTilemap, gate);
            }
        }

        AddDoorToBlock();
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }

    private void BlockOnTilemapLayer(Tilemap tilemap, GatePosition gate)
    {
        Vector2Int startPosition = gate.gateDuplicateStartPosition;

        switch (gate.gatePosition)
        {
            case Orientation.top:
            case Orientation.down:
                for (int xPos = 0; xPos < gate.gateWidth; xPos++)
                    for (int yPos = gate.gateHeight - 1; yPos >= 0; yPos--)
                    {
                        int transformMatrixX = startPosition.x + xPos;
                        int transformMatrixY = startPosition.y - yPos;
                        Vector3Int transformMatrixVector = new Vector3Int(transformMatrixX, transformMatrixY, 0);
                        Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(transformMatrixVector);

                        int setTileX = startPosition.x + 1 + xPos;
                        int setTileY = startPosition.y - yPos;
                        Vector3Int setTileVector = new Vector3Int(setTileX, setTileY, 0);
                        tilemap.SetTile(setTileVector, tilemap.GetTile(transformMatrixVector));

                        tilemap.SetTransformMatrix(setTileVector, transformMatrix);
                    }
                break;

            case Orientation.right:
            case Orientation.left:
                for (int yPos = 0; yPos < gate.gateHeight; yPos++)
                    for (int xPos = gate.gateWidth - 1; xPos >= 0; xPos--)
                    {
                        int transformMatrixX = startPosition.x + xPos;
                        int transformMatrixY = startPosition.y - yPos;
                        Vector3Int transformMatrixVector = new Vector3Int(transformMatrixX, transformMatrixY, 0);
                        Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(transformMatrixVector);

                        int setTileX = startPosition.x + xPos;
                        int setTileY = startPosition.y - 1 - yPos;
                        Vector3Int setTileVector = new Vector3Int(setTileX, setTileY, 0);
                        tilemap.SetTile(setTileVector, tilemap.GetTile(transformMatrixVector));

                        tilemap.SetTransformMatrix(setTileVector, transformMatrix);

                    }
                break;

            default:
                break;
        }

    }

    private void AddDoorToBlock()
    {
        if (blockInfo.blockType.isConnector)
            return;
        foreach(GatePosition gates in blockInfo.gatePositionList)
        {
            if(gates.gatePrefab != null && gates.isLinked)
            {
                float tileDistance = Settings.tileSizePixels / Settings.pixelsPerUnit;

                GameObject gate = null;

                switch (gates.gatePosition)
                {
                    case Orientation.top:
                        gate = Instantiate(gates.gatePrefab, gameObject.transform);
                        gate.transform.localPosition = new Vector3(gates.position.x + tileDistance / 2f, gates.position.y + tileDistance, 0f);
                        break;
                    case Orientation.down:
                        gate = Instantiate(gates.gatePrefab, gameObject.transform);
                        gate.transform.localPosition = new Vector3(gates.position.x + tileDistance / 2f, gates.position.y, 0f);
                        break;
                    case Orientation.left:
                        gate = Instantiate(gates.gatePrefab, gameObject.transform);
                        gate.transform.localPosition = new Vector3(gates.position.x + tileDistance, gates.position.y + tileDistance * 1.25f, 0f);
                        break;
                    case Orientation.right:
                        gate = Instantiate(gates.gatePrefab, gameObject.transform);
                        gate.transform.localPosition = new Vector3(gates.position.x, gates.position.y + tileDistance * 1.25f, 0f);
                        break;
                }

                Gate gateComponent = gate.GetComponent<Gate>();
                if (blockInfo.blockType.isBossRoom)
                {
                    gateComponent.isBossRoomGate = true;

                    gateComponent.GateLock();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Detect Player enter the room
        if (collision.tag == "Player" && GameManager.Instance.GetCurrentBlockInfo() != blockInfo)
        {
            GameManager.Instance.SetCurrentBlockInfo(blockInfo);
            Debug.Log("Enter New Room");
            LevelManager.InvokeVisitRoom(blockInfo);
            /*if (blockInfo.enemySpawnLists != null) {
                EnemySpawner enemySpawner = new EnemySpawner(blockInfo.enemySpawnLists);
                Debug.Log(enemySpawner.getRandomEnemy().monsterDetailSO.name);
            }*/
        }

    }

    public bool isObstacle(int x, int y) {
        if (collisionTilemap.GetTile(new Vector3Int(x + blockInfo.templateLowerBound.x, y + blockInfo.templateLowerBound.y, 0)) == GameResources.Instance.collisionTiles[0] || 
            collisionTilemap.GetTile(new Vector3Int(x + blockInfo.templateLowerBound.x, y + blockInfo.templateLowerBound.y, 0)) == GameResources.Instance.collisionTiles[1])
        {
            return true;
        }
        else return false;
    }

    public bool isPreferredPath(int x, int y)
    {
        if (collisionTilemap.GetTile(new Vector3Int(x + blockInfo.templateLowerBound.x, y + blockInfo.templateLowerBound.y, 0)) == GameResources.Instance.preferredPath)
        {
            return true;
        }
        else return false;
    }

    public void Battling() {
        if (!blockInfo.allEnemiesDefeated && !blockInfo.blockType.isConnectorHorizontal && !blockInfo.blockType.isConnectorVertical)
        {
            foreach (Gate gate in GetComponentsInChildren<Gate>())
                gate.GateLock();
            boxCollider2D.enabled = false;
        }
    }

    public void EndBattle()
    {
           foreach (Gate gate in GetComponentsInChildren<Gate>())
               gate.GateUnlock();
           boxCollider2D.enabled = true;
        
    }
}
