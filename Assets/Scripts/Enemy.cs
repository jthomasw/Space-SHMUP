using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Scripting.APIUpdating;

[RequireComponent(typeof(BoundsCheck))]
public class Enemy : MonoBehaviour
{
    [Header("Inscribed")]
    public float speed = 10f;
    public float fireRate = 0.3f;
    public float health = 10;
    public int score = 100;
    public float powerUpChance = 1f;

    protected bool calledShipDestoryed;
    protected BoundsCheck bndCheck;

    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
    }
    public Vector3 pos
    {
        get
        {
            return this.transform.position;
        }
        set
        {
            this.transform.position = value;
        }
    }

    void Update()
    {
        Move();

        if (bndCheck.LocIs(BoundsCheck.eScreenlocs.offDown))
        {
            Destroy(gameObject);
        }
    }

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= Time.deltaTime;
        pos = tempPos;
    }

    void OnCollisionEnter(Collision coll)
    {
        Debug.Log("Hit");
        GameObject otherGO = coll.gameObject;
        ProjectileHero p = otherGO.GetComponent<ProjectileHero>();
        if (p != null)
        {
            if ( bndCheck.isOnScreen)
            {
                health -= Main.GET_WEAPON_DEFINITION(p.type).damageOnHit;
                if(health <= 0)
                {
                    if (!calledShipDestoryed)
                    {
                        calledShipDestoryed = true;
                        Main.SHIP_DESTROYED(this);
                    }
                    Destroy(this.gameObject);
                }
            }
            Destroy(otherGO);
        } else
        {
            Debug.Log("Enemy hit by non-ProjecitleHero: " + otherGO.name);
        }
    }

}
