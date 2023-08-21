using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DamageNum : MonoBehaviour
{
    public TextMeshProUGUI damageNumber;
    public float lifeTimer;
    public float upSpeed;

    private void Start()
    {
        //StartCoroutine(PushToPool(lifeTimer));
    }

    private void Update()
    {
        transform.position += new Vector3(0, upSpeed * Time.deltaTime, 0);
    }

    private void OnEnable()
    {
        StartCoroutine(PushToPool(lifeTimer));
    }

    public void setDamageNum(int damage) {
        damageNumber.text = damage.ToString();
    
    }

    private IEnumerator PushToPool(float lifeTimer) {
        yield return new WaitForSeconds(lifeTimer);
        ObjectPooling.Instance.PushGameObject(gameObject);
    }
}
