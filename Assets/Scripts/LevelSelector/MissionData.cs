using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MissionData : MonoBehaviour
{
    private static MissionData instance;

    [HideInInspector]
    public static MissionData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<MissionData>("MissionData");
        
            }
            return instance;
        }
    }


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

    }


}
