using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FPSDisplay : MonoBehaviour
{
    public TextMeshProUGUI fpsText;



    public float updateIntevral = 1f;
    private float _timer;
    private float _avgFrameRate;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float smoothDeltaTime = Time.smoothDeltaTime;
        _timer = _timer <= 0 ? updateIntevral : _timer -= smoothDeltaTime;

        if (_timer <= 0)
        {
            _avgFrameRate = 1 / smoothDeltaTime;
            fpsText.text = $"{_avgFrameRate:F2} FPS";
        }
    }

    private void OnGUI()
    {
        
    }
}
