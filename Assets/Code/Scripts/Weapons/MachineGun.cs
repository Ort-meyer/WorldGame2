using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : BaseWeapon
{
    // The gun will fire when the diff to the target is lower than this
    public float m_angleDiffToFire= 5;

    // Prefab for the projectile fired
    public GameObject m_projectilePrefab;
    // The speed at which the projectile is fired
    public float m_projectileLaunchSpeed;
    // The time between each shot
    public float m_maxCooldown;
    // How many degrees the weapon fire can diff
    public float m_weaponSpread = 0;
    // How strong the recoil is (arbitrary number for now)
    public float m_recoil = 0;

    private float m_currentCooldown;
    // Use this for initialization
    protected override void Start()
    {
        TraversingTurret parentTraversingTurret =  m_parentTurretObj.GetComponent<TraversingTurret>();
        if(parentTraversingTurret)
        {
            parentTraversingTurret.m_weaponProjectileLaunchSpeed = m_projectileLaunchSpeed;
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (m_currentCooldown <= 0)
        {
            if (m_targetTrans != null)
            {
                Vector3 toTarget = m_targetTrans.position - transform.position;
                toTarget.y = 0;
                Vector3 forward = transform.forward;
                forward.y = 0;
                float diff = Helpers.GetDiffAngle2D(toTarget, forward);

                if (diff <= m_angleDiffToFire)
                {
                    M_FireWeapon();
                    m_currentCooldown = m_maxCooldown;
                }
            }
        }
        else
        {
            m_currentCooldown -= Time.deltaTime;
        }
    }

    private void M_FireWeapon()
    {
        GameObject newProjectile = Instantiate(m_projectilePrefab, transform.position, transform.rotation);
        // Spread it some
        float spreadx = Random.Range(-m_weaponSpread, m_weaponSpread);
        float spready = Random.Range(-m_weaponSpread, m_weaponSpread);
        newProjectile.transform.Rotate(spreadx, spready, 0, Space.Self);
        newProjectile.GetComponent<Rigidbody>().velocity = newProjectile.transform.forward.normalized * m_projectileLaunchSpeed;
        // Add firing unit to the projectile so it doesnt hit itself
        Unit firingUnit = GetComponentInParent<Unit>();
        GameObject firingUnitObj = firingUnit.gameObject;
        newProjectile.GetComponent<BaseProjectile>().M_SetFiringUnit(firingUnitObj); // TODO safeguard this?
        firingUnit.M_AddRecoil(newProjectile.transform.forward * m_recoil);
    }
}
