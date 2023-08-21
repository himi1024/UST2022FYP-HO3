using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CusorMovement : MonoBehaviour
{
    // Start is called before the first frame update

    private void Awake()
    {
        Cursor.visible = false;
    }
    void Start()
    {
        
    }

    private void OnDestroy()
    {
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
    }
}
