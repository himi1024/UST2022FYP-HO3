using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private static Inventory instance;

    [HideInInspector]
    public static Inventory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<Inventory>("Inventory");
                instance.Pistol = new WeaponSystem(Inventory.Instance.pistolSO, 1, true);
                instance.Rifle = new WeaponSystem(Inventory.Instance.rifleSO, 1, false, 1, 1, 1);
                instance.Shotgun = new WeaponSystem(Inventory.Instance.shotgunSO, 1, false, 1, 1, 1);
                instance.Laser = new WeaponSystem(Inventory.Instance.lasergunSO, 1, false, 1, 1, 1);
                instance.RPG = new WeaponSystem(Inventory.Instance.RPGSO, 1, false, 1, 0, 0);
            }
            return instance;
        }
    }

    public WeaponSystem Pistol;
    public WeaponSystem Rifle;
    public WeaponSystem Shotgun;
    public WeaponSystem Laser;
    public WeaponSystem RPG;

    private void Awake()
    {
        /*Debug.Log(Inventory.Instance.pistolSO);
        Debug.Log(Inventory.Instance.rifleSO);
        Debug.Log(Inventory.Instance.shotgunSO);
        Debug.Log(Inventory.Instance.lasergunSO);
        Debug.Log(Inventory.Instance.RPGSO);*/

        /*Pistol = new WeaponSystem(Inventory.Instance.pistolSO, 0, true);
        Rifle = new WeaponSystem(Inventory.Instance.rifleSO, 1, false, 0, 0, 0);
        Shotgun = new WeaponSystem(Inventory.Instance.shotgunSO, 0, false, 0, 0, 0);
        Laser = new WeaponSystem(Inventory.Instance.lasergunSO, 0, false, 0, 0, 0);
        RPG = new WeaponSystem(Inventory.Instance.RPGSO, 0, false, 0, 0, 0);*/

    }

    public WeaponSO pistolSO;
    public WeaponSO rifleSO;
    public WeaponSO shotgunSO;
    public WeaponSO lasergunSO;
    public WeaponSO RPGSO;
}
