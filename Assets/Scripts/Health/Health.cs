using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    private int initialHealth;
    private int currentHealth;

    public SoundEffectSO hittedEffect;
    public SoundEffectSO killedEffect;

    public Image redBar; //red health bar
    public Image whiteBar; //white effect health bar
    private bool getBarUI = false; //whether the health bar UI is made or not

    private Transform debug;
    public Action <Health> Defeated;
    public Action<Health> OnHealthChanged;
    private bool isPlayer = false;
    private PlayerControl playerControl;
    public LootDrop lootDrop;
    private bool defeated;

    private float timer;

    private void Awake()
    {
        redBar = transform.Find("HealthBarContainer")?.Find("HealthBar")?.Find("RedBar")?.GetComponent<Image>();
        //Debug.Log(debug);
        whiteBar = transform.Find("HealthBarContainer")?.Find("HealthBar")?.Find("WhiteBar")?.GetComponent<Image>();
        playerControl = GetComponent<PlayerControl>();
        if (playerControl != null) isPlayer = true;
        initialHealth = 100;
        currentHealth = 100;
        lootDrop = GetComponent<LootDrop>();
        //Debug.Log(redBar);
        if (redBar && whiteBar)
            getBarUI = true;

    }
    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (isPlayer && gameObject.GetComponent<Player>().isAttacked)
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            if (Time.time - timer >= playerControl.player.playerInformation.noHurtTime) {
                gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                gameObject.GetComponent<Player>().isAttacked = false;
            }
        }
    }
    public void SetInitialHealth(int initialHealth)
    {
        this.initialHealth = initialHealth;
        currentHealth = initialHealth;
        OnHealthChanged?.Invoke(this);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetInitialHealth() {
        return initialHealth;
    }

    //cal the current health and update health bar 
    public bool takeDamage(float damage, bool isDebuff = false, string type = "normalAtk") {
        //Create Damage Text for enemy
        if (!isPlayer) {
            GameObject damageText = ObjectPooling.Instance.GetGameObject(GameResources.Instance.damageNumPrefab);
            damageText.GetComponent<DamageNum>().setDamageNum(Mathf.RoundToInt(damage));
            damageText.transform.position = transform.position;

        }
            
        if (isPlayer && playerControl.isRolling && !isDebuff)
            return false;
        // Damage from blood debuff
        if (type == "blood") currentHealth = Mathf.Clamp(Mathf.RoundToInt(currentHealth * (1 - damage)), 0, initialHealth);
        // Damage from burn debuff
        else if (type == "burn") currentHealth = Mathf.Clamp(currentHealth - (int)damage, 0, initialHealth);
        // Damage to monster
        else if (!isPlayer) currentHealth = Mathf.Clamp(currentHealth - (int)damage, 0, initialHealth);
        // Damage to player
        else if (!gameObject.GetComponent<Player>().isAttacked)
        {
            currentHealth = Mathf.Clamp(currentHealth - (int)damage, 0, initialHealth);
            if (currentHealth == 0)
            {
                GameManager.Instance.DestroyMonster();
                GameManager.Instance.PlayerLost();
                Destroy(gameObject);
                //gameObject.SetActive(false);
                //playerControl.DisablePlayer();
                
            }

            gameObject.GetComponent<Player>().isAttacked = true;
            timer = Time.time;
        }
        
        SoundEffectManager.Instance.Play(hittedEffect);
        //Debug.Log(currentHealth);
        OnHealthChanged?.Invoke(this);
        if (getBarUI) 
        {
            redBar.fillAmount = (float)currentHealth / initialHealth;
            StartCoroutine(UpdateBar());
            //whiteBar.fillAmount = Mathf.MoveTowards(whiteBar.fillAmount, redBar.fillAmount, 0.005f);
            //Debug.Log(redBar.fillAmount);
            if (currentHealth == 0)
                Defeat();
        }


        return true;

    }

    public void heal(int healAmount) {
        if (currentHealth + healAmount > initialHealth)
            currentHealth = initialHealth;
        else currentHealth += healAmount;

        OnHealthChanged?.Invoke(this);


    }

    

    private IEnumerator UpdateBar()
    {
      
        while (whiteBar.fillAmount >= redBar.fillAmount)
        {
            whiteBar.fillAmount = Mathf.MoveTowards(whiteBar.fillAmount, redBar.fillAmount, 0.005f);
            yield return new WaitForSeconds(0.005f);
        }
        if (whiteBar.fillAmount < redBar.fillAmount)
        {
            whiteBar.fillAmount = redBar.fillAmount;
        }
    }

    private void Defeat() {
        if (defeated) return;
        defeated = true;
        SoundEffectManager.Instance.Play(killedEffect);
        lootDrop?.Roll();
        Destroy(gameObject, 0.2f);
        Defeated?.Invoke(this);
    }

    
}
