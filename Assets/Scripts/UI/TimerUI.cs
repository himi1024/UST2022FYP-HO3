using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerUI : MonoBehaviour
{
    public GameObject container;
    public TextMeshProUGUI timer;
    private int value;
    private float step = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTimer(int value)
    {
        container.SetActive(true);
        if (this.value != value) { 
            this.value = value;
            timer.text = this.value.ToString();
            StartCoroutine(FontSizeAni());
        }
    }

    public void UpdateTimer(int value) {

        timer.text = value.ToString();
        StartCoroutine(FontSizeAni());
    }

    private IEnumerator FontSizeAni() {
        timer.fontSize = 50;
        while (timer.fontSize > 40)
        {
            timer.fontSize = Mathf.Clamp(timer.fontSize - step * Time.deltaTime, 40, 50);
            yield return null;
        }
        if (timer.fontSize < 50)
            timer.fontSize = 50;
    }
}
