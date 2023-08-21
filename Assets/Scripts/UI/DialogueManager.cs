using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DialogueManager : MonoBehaviour
{
    public GameObject dialogueBox;
    public Text dialogueText;
    public Text nameText; // NPC name
    public int index = 0;
    [TextArea(1, 3)]
    public string[] dialogueLinesL1BS;
    [TextArea(1, 3)]
    public string[] dialogueLinesL1SM;
    [TextArea(1, 3)]
    public string[] dialogueLinesL2BS;
    [TextArea(1, 3)]
    public string[] dialogueLinesL2SM;

    [TextArea(1, 3)]
    public string[] dialogueLinesBossFightLoss;
    [TextArea(1, 3)]
    public string[] dialogueLinesSurvivalLoss;

    private string[] dialogueLines;
    [SerializeField] private int currentLine;

    private void Start()
    {
        dialogueBox.SetActive(false);
        getCurrentLevelIndex();
        switch (index) {
            case 0:
                dialogueLines = dialogueLinesL1BS;
                break;
            case 1:
                dialogueLines = dialogueLinesL2BS;
                break;
            case 2:
                dialogueLines = dialogueLinesL1SM;
                break;
            case 3:
                dialogueLines = dialogueLinesL2SM;
                break;
            case 4:
                dialogueLines = dialogueLinesBossFightLoss;
                break;
            case 5:
                dialogueLines = dialogueLinesSurvivalLoss;
                break;
        }
        dialogueText.text = dialogueLines[currentLine];
    }
    
    private void Update()
    {
        if (dialogueBox.activeInHierarchy)
        {
            if (Input.GetMouseButtonUp(0))
            {
                currentLine++;

                if (currentLine < dialogueLines.Length)
                    dialogueText.text = dialogueLines[currentLine];
                else
                {
                    dialogueBox.SetActive(false);
                    //GameManager.Instance.GetPlayer().GetComponent<PlayerControl>().DisablePlayer();
                    Time.timeScale = 1f;
                }
            }
        }

    }

    private void getCurrentLevelIndex()
    {
        index = GameResources.Instance.currentLevelIndex;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    public void ShowPanel(int index) {
        UpdateDialogueText(index);
        Time.timeScale = 0f;
        dialogueBox.SetActive(true);
    }

    public void UpdateDialogueText(int index) {
        switch (index)
        {
            case 0:
                dialogueLines = dialogueLinesL1BS;
                break;
            case 1:
                dialogueLines = dialogueLinesL2BS;
                break;
            case 2:
                dialogueLines = dialogueLinesL1SM;
                break;
            case 3:
                dialogueLines = dialogueLinesL2SM;
                break;
            case 4:
                dialogueLines = dialogueLinesBossFightLoss;
                break;
            case 5:
                dialogueLines = dialogueLinesSurvivalLoss;
                break;
        }
        dialogueText.text = dialogueLines[currentLine];

    }
    
}
