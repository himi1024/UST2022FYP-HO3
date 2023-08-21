using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DisallowMultipleComponent]
public class AimEventHandler : MonoBehaviour
{
    private PlayerControl playerControl;
    [SerializeField] private Transform weaponRotation;
    [SerializeField] private bool isPlayer = true;


    // Start is called before the first frame update
    public void Awake()
    {
        if(isPlayer)
            playerControl = GetComponent<PlayerControl>();
    }

    public void OnEnable()
    {
        if (isPlayer)
            playerControl.AimWithMouse += AimWithMouse;
    }

    public void OnDisable()
    {
        if (isPlayer)
            playerControl.AimWithMouse -= AimWithMouse; 
    }


    private void AimWithMouse(MonoBehaviour control, AimArgs aimArgs) {
        float characterAngle =  aimArgs.characterAngle;
        weaponRotation.eulerAngles = new Vector3(0f, 0f, characterAngle);

        switch (aimArgs.faceDirection) {
            case FaceDirection.Left:
                weaponRotation.localScale = new Vector3(1f, -1f, 0f);
                break;
            default:
                weaponRotation.localScale = new Vector3(1f, 1f, 0f);
                break;
        }    
    }
}
