using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Tilemaps;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }
            return instance;
        }
    }

    public BlockTypeListSO blockTypeList;

    public CurrentPlayerSO currentPlayer;

    public AudioMixerGroup masterSound;

    public AudioMixerSnapshot musicOnFullSnapshot;

    public Material dimmedMaterial;

    [SerializeField] public TileBase[] collisionTiles;
    public TileBase preferredPath;

    public GameObject hitEnemyBloodPt;

    public GameObject debuffPrefab;

    public GameObject lootPrefab;

    public GameObject damageNumPrefab;

    public int LevelIndex;
    public int currentLevelIndex;

    public bool bossLevelOneCompleted = false;
    public bool bossLeveltwoCompleted = false;
    public bool survivalLevelOneCompleted = false;
    public bool survivalLevelTwoCompleted = false;
}
