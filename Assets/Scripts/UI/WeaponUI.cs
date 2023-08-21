using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{

    public PlayerControl playerControl;
    public Player player;
    public Image weaponImage;
    public GameObject prevWeapon;
    public TextMeshProUGUI ammoText;
    public Image refillImage;


    private Coroutine reloadCoroutine;


    private void Awake()
    {
        player = GameManager.Instance.GetPlayer();
        playerControl = player.gameObject.GetComponent<PlayerControl>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        playerControl.SwitchWeapon += UpdateWeaponUI;

    }

    private void OnDisable()
    {
        playerControl.SwitchWeapon -= UpdateWeaponUI;
        if (prevWeapon != null)
        {
            prevWeapon.GetComponent<Weapon>().AmmoShoot -= UpdateAmmoUI;
            prevWeapon.GetComponent<Weapon>().Reloading -= ReloadAction;

        }
    }


    private void UpdateWeaponUI(MonoBehaviour control, GameObject weapon) {
        if (prevWeapon != weapon && prevWeapon != null) {
            prevWeapon.GetComponent<Weapon>().AmmoShoot -= UpdateAmmoUI;
            prevWeapon.GetComponent<Weapon>().Reloading -= ReloadAction;
        }
        prevWeapon = weapon;
        weaponImage.sprite = weapon.GetComponent<SpriteRenderer>().sprite;
        ammoText.text = weapon.GetComponent<Weapon>().curAmmo.ToString();
        if (refillImage.fillAmount != 0)
            refillImage.fillAmount = 0;
        weapon.GetComponent<Weapon>().AmmoShoot += UpdateAmmoUI;
        weapon.GetComponent<Weapon>().Reloading += ReloadAction;
    }

    private void ReloadAction(Weapon weaponClass, float fillAmount) {
        refillImage.fillAmount = fillAmount;
        ammoText.text = weaponClass.curAmmo.ToString();
    }

    private void UpdateAmmoUI(Weapon weaponClass, GameObject weapon)
    {
        ammoText.text = weaponClass.curAmmo.ToString();
    }

        // Update is called once per frame
        void Update()
    {
        
    }
}
