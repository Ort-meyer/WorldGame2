using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : BaseProjectile
{
    public GameObject m_impactEffectPrefab;

    private ParticleManager m_particleManager;

    // Use this for initialization
    protected override void Start()
    {
        m_particleManager = Object.FindObjectOfType<ParticleManager>();
    }

    protected override void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        Unit firingUnit = other.gameObject.GetComponentInParent<Unit>();
        if (firingUnit)
        {
            GameObject firingUnitObject = firingUnit.gameObject;
            if (firingUnitObject == m_firingUnitObject)
            {
                return;
            }
        }
        M_SpawnImpactEffect();
        Destroy(this.gameObject);
        // Possibly helpfull code from some example somewhere
        //foreach (ContactPoint contact in collision.contacts)
        //{
        //    Debug.DrawRay(contact.point, contact.normal, Color.white);
        //}

    }

    void M_SpawnImpactEffect()
    {
        GameObject newEffect = Instantiate(m_impactEffectPrefab);
        newEffect.transform.parent = m_particleManager.transform;
        newEffect.transform.position = transform.position;
        newEffect.SetActive(true);
        ParticleSystem ps = newEffect.GetComponent<ParticleSystem>();

        if (ps != null)
        {
            var main = ps.main;
            if (main.loop)
            {
                ps.gameObject.AddComponent<CFX_AutoStopLoopedEffect>();
                ps.gameObject.AddComponent<CFX_AutoDestructShuriken>();
            }
        }

        m_particleManager.M_AddParticle(newEffect);
    }
}
