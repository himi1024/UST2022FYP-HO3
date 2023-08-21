using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{

    public PlayerControl playerControl;
    public Player player;
    public Health health;
    public TextMeshProUGUI healthText;
    public Image redBarImage;
    [SerializeField] private Transform debuffContainer;
    private BurnDebuffHandler burnDebuffHandler;
    private FreezeDebuffHandler freezeDebuffHandler;
    private BloodDebuffHandler bloodDebuffHandler;
    private GameObject burnDebuffObj;
    private GameObject freezeDebuffObj;
    private GameObject bloodDebuffObj;

    private void Awake()
    {
        /*player = GameManager.Instance.GetPlayer();
        playerControl = player.gameObject.GetComponent<PlayerControl>();
        health = player.gameObject.GetComponent<Health>();*/

    }

    // Start is called before the first frame update
    void Start()
    {
        
        //healthText.text = health.GetCurrentHealth() + "/" + health.GetInitialHealth();
        
    }

    private void OnEnable()
    {
        player = GameManager.Instance.GetPlayer();
        playerControl = player.gameObject.GetComponent<PlayerControl>();
        health = player.gameObject.GetComponent<Health>();
        HealthUIUpdate(health);
        health.OnHealthChanged += HealthUIUpdate;

        burnDebuffHandler = player.gameObject.GetComponent<BurnDebuffHandler>();
        bloodDebuffHandler = player.gameObject.GetComponent<BloodDebuffHandler>();
        freezeDebuffHandler = player.gameObject.GetComponent<FreezeDebuffHandler>();


        burnDebuffHandler.onBurnDebuffCreate += CreateDebuff;
        bloodDebuffHandler.onBloodDebuffCreate += CreateDebuff;
        freezeDebuffHandler.onFreezeDebuffCreate += CreateDebuff;
        //debuffHandler.onDebuffChange += ChangeDebuff;
        //debuffHandler.onDebuffDestroy += DestroyDebuff;

    }

    private void OnDisable()
    {
        health.OnHealthChanged -= HealthUIUpdate;
        burnDebuffHandler.onBurnDebuffCreate -= CreateDebuff;
        //debuffHandler.onDebuffChange -= ChangeDebuff;
        //debuffHandler.onDebuffDestroy -= DestroyDebuff;
    }

    private void HealthUIUpdate(Health health) {
        healthText.text = health.GetCurrentHealth() + "/" + health.GetInitialHealth();
        redBarImage.fillAmount = (float)health.GetCurrentHealth() / health.GetInitialHealth();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateDebuff(DebuffSO debuffSO) {
        GameObject debuffPrefab = Instantiate(GameResources.Instance.debuffPrefab, debuffContainer);
        debuffPrefab.transform.Find("DebuffIcon").GetComponentInChildren<Image>().sprite = debuffSO.debuffIcon;
        if (debuffSO.isBurn)
        {
            burnDebuffObj = debuffPrefab;
            burnDebuffHandler.onBurnDebuffChange += ChangeDebuff;
            burnDebuffHandler.onBurnDebuffDestroy += DestroyDebuff;
        }
        else if (debuffSO.isBlood)
        {
            bloodDebuffObj = debuffPrefab;
            bloodDebuffHandler.onBloodDebuffChange += ChangeDebuff;
            bloodDebuffHandler.onBloodDebuffDestroy += DestroyDebuff;
        }

        else if (debuffSO.isFreezen)
        {
            freezeDebuffObj = debuffPrefab;
            freezeDebuffHandler.onFreezeDebuffChange += ChangeDebuff;
            freezeDebuffHandler.onFreezeDebuffDestroy += DestroyDebuff;
        }
        
    }

    private void DestroyDebuff(DebuffSO debuffSO) {
        if (debuffSO.isBurn)
        {
            Destroy(burnDebuffObj);
            burnDebuffHandler.onBurnDebuffChange += ChangeDebuff;
            burnDebuffHandler.onBurnDebuffDestroy += DestroyDebuff;
            burnDebuffObj = null;
        }
        else if (debuffSO.isBlood)
        {
            Destroy(bloodDebuffObj);
            bloodDebuffHandler.onBloodDebuffChange += ChangeDebuff;
            bloodDebuffHandler.onBloodDebuffDestroy += DestroyDebuff;
            bloodDebuffObj = null;
        }

        else if (debuffSO.isFreezen)
        {
            Destroy(freezeDebuffObj);
            freezeDebuffHandler.onFreezeDebuffChange += ChangeDebuff;
            freezeDebuffHandler.onFreezeDebuffDestroy += DestroyDebuff;
            freezeDebuffObj = null;
        }

    }

    private void ChangeDebuff(DebuffSO debuffSO, float amount) {
        //Debug.Log(debuffObjs.Count);
        //if (debuffObjs.Count == 0)
        //return; 
        if (debuffSO.isBurn && burnDebuffObj != null)
        {
            burnDebuffObj.transform.Find("FillImage").GetComponent<Image>().fillAmount = Mathf.Clamp(amount, 0f, 1f);
        }
        else if (debuffSO.isBlood && bloodDebuffObj != null)
        {
           bloodDebuffObj.transform.Find("FillImage").GetComponent<Image>().fillAmount = Mathf.Clamp(amount, 0f, 1f);
        }

        else if (debuffSO.isFreezen && freezeDebuffObj != null)
        {
            freezeDebuffObj.transform.Find("FillImage").GetComponent<Image>().fillAmount = Mathf.Clamp(amount, 0f, 1f);
        }
    
    }

}
