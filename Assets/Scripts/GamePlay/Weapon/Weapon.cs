using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Weapon : MonoBehaviour
{
    [SerializeField] public WeaponSO weaponDetails;
    public float curAmmo;  // current Ammo of the bullet
    public float shootCDTimer; // Timer to count shoot CD time

    protected Transform shootPosition;



    protected PlayerControl playerControl;
    protected EnemyWeapon enemyWeapon;
    [SerializeField] protected bool isPlayer = true;
    protected bool usable = true;
    [HideInInspector] public bool continueShoot;
    [HideInInspector] public bool isReloading = false;
    [HideInInspector] public float reloadTimer = 0;
    [HideInInspector] public Coroutine reloadCoroutine;
    [HideInInspector] public event Action<Weapon, GameObject> AmmoShoot; // used to Update UI ammo value
    [HideInInspector] public event Action<Weapon, float> Reloading; // used to Update Reloading UI
    private float flipX;
    private float flipY;
    private float flipZ;




    private void Awake()
    {
        isPlayer = weaponDetails.isPlayer;
        curAmmo = weaponDetails.maxAmmo;
        shootCDTimer = 0;
        continueShoot = weaponDetails.continueShoot;
        if (isPlayer)
            playerControl = GetComponentInParent<PlayerControl>();
        else
            enemyWeapon = GetComponentInParent<EnemyWeapon>();

    }

    protected virtual void Start()
    {
        shootPosition = transform.Find("ShootPosition");
        flipX = transform.localScale.x;
        flipY = transform.localScale.y;
        flipZ = transform.localScale.z;
    }

    private void OnEnable()
    {
        if (isPlayer) {
            playerControl.AimWithMouse += SetAimAngle;
            playerControl.Shoot += Shoot;
            playerControl.Reload += Reload;
        }
        else {
            enemyWeapon.AimWithMouse += SetAimAngle;
            enemyWeapon.Shoot += Shoot;
        }



    }

    protected virtual void OnDisable()
    {
        if (isPlayer)
        {
            playerControl.AimWithMouse -= SetAimAngle;
            playerControl.Shoot -= Shoot;
            playerControl.Reload -= Reload;
        }
        else {
            enemyWeapon.AimWithMouse -= SetAimAngle;
            enemyWeapon.Shoot -= Shoot;
        }
        if (reloadCoroutine != null) {

            StopCoroutine(reloadCoroutine);
            isReloading = false;
        }
    }

    protected virtual void Update()
    {
        shootCDTimer -= Time.deltaTime;
        // No Limit to no ammo sound effect
        if (Input.GetMouseButtonUp(0) && weaponDetails.continueShoot && gameObject.name != "LaserGun" && curAmmo == 0)
        {
            SoundEffectManager.Instance.DisablePlayPressing();
        }
    }

    private void SetAimAngle(MonoBehaviour control, AimArgs aimArgs)
    {
        float characterAngle = aimArgs.characterAngle;
        //transform.right = aimArgs.weaponDirection;
        transform.eulerAngles = new Vector3(0f, 0f, characterAngle);

        if (isPlayer)
            switch (aimArgs.faceDirection)
            {
                case FaceDirection.Left:

                    transform.localScale = new Vector3(-flipX, -flipY, flipZ);
                    break;
                default:
                    transform.localScale = new Vector3(flipX, flipY, flipZ);
                    break;
            }
        else {
            switch (aimArgs.faceDirection)
            {
                case FaceDirection.Left:

                    transform.localScale = new Vector3(flipX, -flipY, flipZ);
                    break;
                default:
                    transform.localScale = new Vector3(flipX, flipY, flipZ);
                    break;
            }

        }
    }

    protected void Shoot(MonoBehaviour control, AimArgs aimArgs)
    {
        //Debug.Log("Shoot");
        if (shootCDTimer <= 0 && curAmmo > 0 && !isReloading)
        {
            ShootBullet(aimArgs);
            if (weaponDetails.firingSoundEffect != null && gameObject.name != "LaserGun")
                SoundEffectManager.Instance.Play(weaponDetails.firingSoundEffect);
            if (curAmmo != 999 && gameObject.name != "LaserGun")
            {
                curAmmo--;
                AmmoShoot?.Invoke(this, gameObject);
            }
            //Debug.Log(curAmmo);
        }
        else if (curAmmo == 0 && weaponDetails.continueShoot)
        {
            SoundEffectManager.Instance.PlayPressing(weaponDetails.noAmmoEffect);

            // GameObject sound = ObjectPooling.Instance.GetGameObject(weaponDetails.noAmmoEffect.sound);
            // sound.GetComponent<AudioSource>().clip = weaponDetails.noAmmoEffect.audioClip;
            // sound.GetComponent<AudioSource>().Play();
            //StartCoroutine(DisableSound(sound, sound.GetComponent<AudioSource>().clip.length));
        }
        else if (curAmmo == 0 && !weaponDetails.continueShoot)
        {
            SoundEffectManager.Instance.Play(weaponDetails.noAmmoEffect);
        }
        else
            return;
    }

    protected void Reload(MonoBehaviour control, int flag) {
        reloadCoroutine = StartCoroutine(AmmoReload());
        SoundEffectManager.Instance.Play(weaponDetails.reloadSoundEffect);
    }

    protected IEnumerator AmmoReload() {
        isReloading = true;
        if(isPlayer)
            Reloading?.Invoke(this, 1);
        while (reloadTimer <= weaponDetails.reloadTime) {
            reloadTimer += Time.deltaTime;
            if(isPlayer)
                Reloading?.Invoke(this, 1- reloadTimer / weaponDetails.reloadTime);
            yield return Time.deltaTime;
        }
        isReloading = false;
        reloadTimer = 0;
        curAmmo = weaponDetails.maxAmmo;
        Reloading?.Invoke(this, 0);
        //Debug.Log("Finish Reloading");

    }

    protected virtual void ShootBullet(AimArgs aimArgs) {
        //Debug.Log(weaponDetails.bulletDetails.bullet.GetInstanceID());
        GameObject bullet = ObjectPooling.Instance.GetGameObject(weaponDetails.bulletDetails.bullet);
        bullet.GetComponent<Bullet>().bulletDetails = weaponDetails.bulletDetails;
        bullet.transform.position = shootPosition.position;
        bullet.transform.eulerAngles = new Vector3(0f, 0f, aimArgs.weaponAngle);
        ShootEffect();
        //bullet.SetActive(true);
        //Debug.Log(shootPosition.position);
        float angle = UnityEngine.Random.Range(-5f,5f);
        bullet.GetComponent<Bullet>().Shoot(Quaternion.AngleAxis(angle, Vector3.forward) * aimArgs.weaponDirection.normalized);
        //Generate smoke particle
        
        shootCDTimer = weaponDetails.shootCD;
    }

    protected void ShootEffect() {

        if (weaponDetails.smokeParticle != null)
        {
            GameObject smokePT = ObjectPooling.Instance.GetGameObject(weaponDetails.smokeParticle);
            smokePT.transform.position = shootPosition.position;

        }
    }

    public bool IsUsable() {
        return usable;
    }

    public void InvokeAmmoShoot() {

        AmmoShoot?.Invoke(this, gameObject);
    }

    private IEnumerator DisableSound(GameObject soundObject, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (soundObject != null) ObjectPooling.Instance.PushGameObject(soundObject);
    }

}
