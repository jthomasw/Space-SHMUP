using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Audio;

public enum eWeaponType
{
    none,
    blaster,
    spread,
    phaser,
    missle,
    laser,
    shield
}

[System.Serializable]
public class WeaponDefinition
{
    public eWeaponType type = eWeaponType.none;
    [Tooltip("Letter to show on the PowerUp Cube")]
    public string Letter;
    [Tooltip("Color of the PowerUp Cube")]
    public Color powerUpColor = Color.white;
    [Tooltip("prefab of Weapon model that is attached to the Player Ship")]
    public GameObject weaponModelPrefab;
    [Tooltip("Prefab of the projectile that is fired")]
    public GameObject projectilePrefab;
    [Tooltip("Color of the Projectile that is fired")]
    public Color projectileColor = Color.white;
    [Tooltip("Damage caused when a single projectile hits and enemy")]
    public float damageOnHit = 0;
    [Tooltip("Damage caused per second by the Laser [not implemented]")]
    public float damagePerSec = 0;
    [Tooltip("Seconds to delay between shots")]
    public float delayBetweenShots = 0;
    [Tooltip("Velocity of individual Projectiles")]
    public float velocity = 50;
}

public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;

    [Header("Dynamic")]
    [SerializeField]
    [Tooltip("Setting this manually while playing does not work properly")]
    private eWeaponType  _type = eWeaponType.none;
    public WeaponDefinition def;
    public float nextShotTime;
    public AudioSource audioSource;
    public AudioClip audioClip;

    private GameObject weaponModel;
    private Transform shotPointTrans;

    void Start()
    {
        if(PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
            Debug.Log("Projectile Anchor Made");
        }

        shotPointTrans = transform.GetChild(0);

        SetType(_type);
        Debug.Log("Type Set To: " + _type );

        Hero hero = GetComponentInParent<Hero>();

        if (hero != null)
        {
            Debug.Log("Hero component found: " + hero.gameObject.name);
            hero.fireEvent += Fire;
        }
        else
        {
            Debug.LogError("Weapon could not find a Hero component in parent!");
        }

       
            audioSource = GetComponent<AudioSource>();
      


        
            audioSource.clip = audioClip;
       

    }

    public eWeaponType type
    {
        get { return _type; }
        set { SetType(value); }
    }

    public void SetType(eWeaponType wt)
    {
        _type = wt;
        if (type == eWeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        } else
        {
            this.gameObject.SetActive(true);
        }

        def = Main.GET_WEAPON_DEFINITION(_type);
        if (weaponModel != null) Destroy(weaponModel);
        
        weaponModel = Instantiate<GameObject>(def.weaponModelPrefab, transform);
        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localScale = Vector3.one;

        if (weaponModel == null) Debug.LogError("No WeaponModel loaded");

        nextShotTime = 0;
    }

    private void Fire()
    {
        if (!gameObject.activeInHierarchy) return;
        if (Time.time < nextShotTime) return;

        ProjectileHero p;
        Debug.Log( Vector3.up + " * " + def.velocity);
        Vector3 vel = Vector3.up * def.velocity;
        Debug.Log("Vel: " + vel);

        switch (type)
        {
            case eWeaponType.blaster:
                p = MakeProjectile();
                Debug.Log("Vel: " + vel);
                p.vel = vel;
                Debug.Log("picked blaster and blaster vel: " + p.rigid.linearVelocity);
                PlayAudio();
                break;

            case eWeaponType.spread:
                p = MakeProjectile();
                p.vel = vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.vel = p.transform.rotation * vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.vel = p.transform.rotation * vel;
                Debug.Log("picked spread");
                PlayAudio();
                break;
        }
    }

    private ProjectileHero MakeProjectile()
    {
        GameObject go;
        go = Instantiate<GameObject>(def.projectilePrefab, PROJECTILE_ANCHOR);
        Debug.Log("Instatiated Projectile");
        ProjectileHero p = go.GetComponent<ProjectileHero>();

        Vector3 pos = shotPointTrans.position;
        pos.z = 0;
        p.transform.position = pos;

        p.type = type;
        nextShotTime = Time.time + def.delayBetweenShots;
        return (p);
    }

    public void PlayAudio()
    {
        
            audioSource.PlayOneShot(audioClip);
        
    }

}