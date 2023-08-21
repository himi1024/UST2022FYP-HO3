using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperFunctions
{
    public static Camera mainCamera;


    //get the spawning position for placing player
    public static Vector3 GetSpawnPositionNearestToPlayer(Vector3 playerPosition)            
    {
        BlockInfo currentBlockObject = GameManager.Instance.GetCurrentBlockInfo();
        Grid grid = currentBlockObject.instantiatedBlock.grid;
        Vector3 nearestSpawnPosition = new Vector3(20000f, 20000f, 0f);

        foreach (Vector2Int spawnPositionGrid in currentBlockObject.enemyPositionArr)
        {
            Vector3 spawnPositionWorld = grid.CellToWorld((Vector3Int)spawnPositionGrid);

            if (Vector3.Distance(spawnPositionWorld, playerPosition) < Vector3.Distance(nearestSpawnPosition, playerPosition))
                nearestSpawnPosition = spawnPositionWorld;
        }

        return nearestSpawnPosition;

    }

    //Convert the mouse position to screen position
    public static Vector3 GetMousePosition()  
    {
        if (mainCamera == null) 
            mainCamera = Camera.main;

        Vector3 mouseScreenPosition = Input.mousePosition;

        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.height);

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        worldPosition.z = 0f;

        return worldPosition;
    }

    public static float Vector2Angle(Vector3 vector3) {
        return Mathf.Atan2(vector3.y, vector3.x) * Mathf.Rad2Deg;
    }

    // convert an angle to Vetor2 vector
    public static Vector2 Angle2Vector2(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;

        float x = Mathf.Cos(radian);
        float y = Mathf.Sin(radian);

        return new Vector2(x, y);
    }
}
