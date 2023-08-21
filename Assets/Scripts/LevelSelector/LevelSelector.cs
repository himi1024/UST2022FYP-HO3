using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;


public class LevelSelector : MonoBehaviour
{
    public int _level;
    public int _lock;
    [SerializeField] private bool unlocked = false; 
    public Image unlockImage;

    private void Start()
    {
        //Debug.Log(GameResources.Instance.LevelIndex);
        UpdateLevelImage();
    }
    private void UpdateLevelImage()
    {
        /*if (GameResources.Instance.LevelIndex >= _lock)
        {
            //Debug.Log("aaaa");
            unlocked = true;
        }*/
        checkLevel();
        if (!unlocked)
        {
            unlockImage.gameObject.SetActive(true);
        }
        else
        {
            unlockImage.gameObject.SetActive(false);
            
        }
    }
    public void PressSelection()
    {
        // Debug.Log(GameResources.Instance.LevelIndex + " " + _lock + " " + gameObject.name);
        /*if (unlocked)
            if (GameResources.Instance.LevelIndex >= _level)
            {
                int highestLevel = GameResources.Instance.LevelIndex;
                GameResources.Instance.currentLevelIndex = _level;
                GameResources.Instance.LevelIndex = _level;
                SceneManager.LoadScene("MainGameScene");
                GameResources.Instance.LevelIndex = highestLevel;
            }*/
        if (unlocked) {
            Destroy(GameObject.Find("BGM"));
            GameResources.Instance.LevelIndex = _lock;
            SceneManager.LoadScene("MainGameScene");
        }
    }

    private void checkLevel() {
        switch (_lock) {
            case 0:
                unlocked = true;
                break;
            case 1:
                if (GameResources.Instance.bossLevelOneCompleted)
                    unlocked = true;
                break;
            case 2:
                unlocked = true;
                break;
            case 3:
                if (GameResources.Instance.survivalLevelOneCompleted)
                    unlocked = true;
                break;
        }  
        
    }
    
}
