using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] public List<LevelSO> levels;   //the level scriptable object

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject craftingMenu;

    [SerializeField] public int dungeonLevelIndex = 0;

    private BlockInfo currentBlockInfo;
    private BlockInfo previousBlockInfo;
    private PlayerInformationSO playerInformation;
    private Player player;

    public MusicSO winMusic;
    public MusicSO loseMusic;

    private InstantiatedBlock bossRoom;
    private GameState previousGameState;



    private bool canBoss = false;


    public GameState gameState;

    protected override void Awake()
    {
        base.Awake();

        playerInformation = GameResources.Instance.currentPlayer.playerInformation;
        GameObject playerGameObject = Instantiate(playerInformation.playerPrefab);
        player = playerGameObject.GetComponent<Player>();
        player.Initialize(playerInformation);
        dungeonLevelIndex = GameResources.Instance.LevelIndex;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.gameStarted;
        Debug.Log("Start");
    }

    // Update is called once per frame
    void Update()
    {
        HandleGameState();

        /*if (Input.GetKeyDown(KeyCode.R))
        {
            gameState = GameState.gameStarted;
        }*/


    }

    public void CheckNormalRoomsDefeated()
    {
        if (GameResources.Instance.LevelIndex >= 2) return;
        Debug.Log("Call CNRD");
        foreach (KeyValuePair<string, BlockInfo> block in DungeonLevelGenerator.Instance.dungeonBuilderblockInfoDictionary)
        {
            if (block.Value.blockType.isConnector) continue;
            if (block.Value.blockType.isBossRoom) continue;
            if (!block.Value.allEnemiesDefeated) return;
        }
        if (bossRoom != null && bossRoom.blockInfo.allEnemiesDefeated)
            AllRoomsDefeated();
        else
            gameState = GameState.enemiesDefeated;
        
    }

    public void AllRoomsDefeated()
    {
        gameState = GameState.levelComplete;
    }

    private void EnemiesDefeated()
    {
        if (canBoss == false)
        {
            Debug.Log("open");
            bossRoom.gameObject.SetActive(true);
            bossRoom.EndBattle();
            canBoss = true;
        }
        
    }


    private void HandleGameState() {
        switch (gameState) {
            case GameState.gameStarted:
                PlayDungeonLevel(dungeonLevelIndex);

                foreach (KeyValuePair<string, BlockInfo> block in DungeonLevelGenerator.Instance.dungeonBuilderblockInfoDictionary)
                {
                    if (block.Value.blockType.isBossRoom)
                    {
                        bossRoom = block.Value.instantiatedBlock;
                        Debug.Log("found");
                    }
                }

                if (GameResources.Instance.LevelIndex == 2 || GameResources.Instance.LevelIndex == 3)
                    gameState = GameState.playingSurvival;
                else
                    gameState = GameState.playingLevel;

                previousGameState = GameState.gameStarted;
                break;

            case GameState.playingSurvival:
                // timer
                if (Input.GetKeyDown(KeyCode.F))
                {
                    StartSurvive();
                }
                CheckMenuUI();
                break;
            case GameState.playingLevel:
                if (Input.GetKeyDown(KeyCode.V))
                {
                    PauseGameMenu();
                }
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    //CraftingUI
                    CraftingMenu();
                }
                break;

            case GameState.Crafting:
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    CraftingMenu();
                }
                break;

            case GameState.Paused:
                if (Input.GetKeyDown(KeyCode.V))
                {
                    Debug.Log("pause1");
                    PauseGameMenu();
                }
                break;

            case GameState.enemiesDefeated:
                EnemiesDefeated();
                CheckMenuUI();
                break;

            case GameState.levelComplete:
                UpdateLevelData();
                ShowWinDiaglogue();
                DestroyMonster();
                break;

            case GameState.playerLost:
                ShowLostDiaglogue();
                DestroyMonster();
                break;

            case GameState.waitForReturn:
                if (Input.GetKeyDown(KeyCode.B))
                    SceneManager.LoadScene("Menu");
                break;

        }
    }

    private void CheckMenuUI() {
        if (Input.GetKeyDown(KeyCode.V))
        {
            PauseGameMenu();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //CraftingUI
            CraftingMenu();
        }
    }

    public void DestroyMonster() {
        Monster[] monsters = FindObjectsOfType<Monster>();
        foreach (Monster monster in monsters)
            monster.gameObject.SetActive(false);
    
    }

    private void ShowWinDiaglogue()
    {
        DialogueManager dialogueManager = (DialogueManager)FindObjectOfType<DialogueManager>();
        dialogueManager.ShowPanel(dungeonLevelIndex);
        MusicManager.Instance.GetComponent<MusicManager>().audioSource.clip = winMusic.music;
        MusicManager.Instance.GetComponent<MusicManager>().currentAudioClip = winMusic.music;
        MusicManager.Instance.GetComponent<MusicManager>().audioSource.Play();
        gameState = GameState.waitForReturn;

    }

    private void ShowLostDiaglogue() {
        DialogueManager dialogueManager =(DialogueManager) FindObjectOfType<DialogueManager>();
        if (dungeonLevelIndex == 0 || dungeonLevelIndex == 1)
            dialogueManager.ShowPanel(4);
        else
        {
            currentBlockInfo.instantiatedBlock.gameObject.GetComponent<SurvivalBlock>().StopTime();
            dialogueManager.ShowPanel(5);
        }
        gameState = GameState.waitForReturn;


    }

    private void UpdateLevelData() {
        switch (dungeonLevelIndex) {
            case 0:
                GameResources.Instance.bossLevelOneCompleted = true;
                break;
            case 1:
                GameResources.Instance.bossLeveltwoCompleted = true;
                break;
            case 2:
                GameResources.Instance.survivalLevelOneCompleted = true;
                break;
            case 3:
                GameResources.Instance.survivalLevelTwoCompleted = true;
                break;
        }
    
    }

    private void StartSurvive()
    {
        if (!currentBlockInfo.blockType.isBossRoom)
        {
            Debug.Log("not entered yet");
            return;
        }
        currentBlockInfo.instantiatedBlock.gameObject.GetComponent<SurvivalBlock>().StartTimer();
    }

    public void PlayerLost()
    {
        Debug.Log("You Lose");
        MusicManager.Instance.GetComponent<MusicManager>().audioSource.clip = loseMusic.music;
        MusicManager.Instance.GetComponent<MusicManager>().currentAudioClip = loseMusic.music;
        MusicManager.Instance.GetComponent<MusicManager>().audioSource.Play();
        gameState = GameState.playerLost;
    }


    private void PlayDungeonLevel(int dungeonLEvelIndex) {
        bool dungeonBuildSuccessful = DungeonLevelGenerator.Instance.GenerateDungeon(levels[dungeonLEvelIndex]);
        if (!dungeonBuildSuccessful)
            Debug.LogError("Can't Build dungeon succesfully");

        float x = currentBlockInfo.realWorldLowerBound.x + currentBlockInfo.realWorldUpperBound.x / 2f;
        float y = currentBlockInfo.realWorldUpperBound.y + currentBlockInfo.realWorldUpperBound.y / 2f;
        player.gameObject.transform.position = new Vector3(x, y, 0f);

        player.gameObject.transform.position = HelperFunctions.GetSpawnPositionNearestToPlayer(player.gameObject.transform.position);
        Debug.Log("abc");
        //player.GetComponent<Player>().Initialize();
    }

    public BlockInfo GetCurrentBlockInfo()
    {
        return currentBlockInfo;
    }

    public Player GetPlayer()
    {
        return player;
    }

    public Sprite GetMinimapIcon()
    {
        return playerInformation.playerIcon;
    }

    public void SetCurrentBlockInfo(BlockInfo blockInfo)
    {
        previousBlockInfo = currentBlockInfo;
        currentBlockInfo = blockInfo;
    }

    public void PauseGameMenu()
    {
        if (gameState != GameState.Paused)
        {
            pauseMenu.SetActive(true);
            GetPlayer().GetComponent<PlayerControl>().DisablePlayer();
            previousGameState = gameState;
            Debug.Log(previousGameState);
            gameState = GameState.Paused;

        }
        else if (gameState == GameState.Paused)
        {
            pauseMenu.SetActive(false);
            GetPlayer().GetComponent<PlayerControl>().EnablePlayer();
            gameState = previousGameState;
            previousGameState = GameState.Paused;
        }
    }

    public void CraftingMenu()
    {
        if (gameState != GameState.Crafting)
        {
            craftingMenu.SetActive(true);
            GetPlayer().GetComponent<PlayerControl>().DisablePlayer();
            previousGameState = gameState;
            Debug.Log(previousGameState);
            gameState = GameState.Crafting;

        }
        else if (gameState == GameState.Crafting)
        {
            craftingMenu.SetActive(false);
            GetPlayer().GetComponent<PlayerControl>().EnablePlayer();
            gameState = previousGameState;
            previousGameState = GameState.Crafting;
        }
    }
}

public enum GameState
{
    gameStarted,
    playingLevel,
    restartGame,
    enemiesDefeated,
    levelComplete,
    playingSurvival,
    missionCompleted,
    waitForReturn,
    Crafting,
    Paused,
    playerLost,

}