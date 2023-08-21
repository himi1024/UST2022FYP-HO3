using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartingMenu : MonoBehaviour
{

    [SerializeField] private ToggleGroup toggleGroup;


    // Current selected toggle
    private Toggle currentToggle => toggleGroup.GetFirstActiveToggle();
    // Save the current activate toggle
    private Toggle activatedToggle;

    private void Start()
    {
        var toggles = toggleGroup.GetComponentsInChildren<Toggle>();
        foreach(var toggle in toggles)
        {
            toggle.onValueChanged.AddListener(_ => OnToggleValueChanged(toggle));
        }

        currentToggle.onValueChanged.Invoke(true);
    }

    private void OnToggleValueChanged(Toggle toggle)
    {
        if(currentToggle == activatedToggle)
        {
            switch (toggle.name)
            {
                case "GameStart":
                    SceneManager.LoadScene("LevelSelect");
                    break;
                case "Settings":
                    SceneManager.LoadScene("SettingUI");
                    break;
                case "Quit":
                    {
                        Application.Quit();
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#endif

                    }
                        break;
            }
            return;
        }
        if (toggle.isOn)
        {
            activatedToggle = toggle;
            activatedToggle.transform.Find("Label").GetComponent<TMP_Text>().color = Color.red;
        }
        else
        {
            activatedToggle.transform.Find("Label").GetComponent<TMP_Text>().color = Color.white;
        }
    }
}
