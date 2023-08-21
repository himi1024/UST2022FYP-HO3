using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class LevelManager
{
    public static Action<BlockInfo> visitRoom;
    public static void InvokeVisitRoom(BlockInfo blockInfo) {
        visitRoom?.Invoke(blockInfo);
    }
    
}
