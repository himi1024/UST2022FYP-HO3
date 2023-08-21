using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PauseUI : MonoBehaviour
{
    private void Start()
    {
            gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    public void Continue()
    {
        GameManager.Instance.PauseGameMenu();
    }
    public void BackToStartMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
