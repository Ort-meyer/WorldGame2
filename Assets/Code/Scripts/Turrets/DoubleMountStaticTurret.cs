using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleMountStaticTurret : BaseTurret
{
    // Duration between firing the two weapons
    public float m_durationBetweenGunFire = 0.3f;

    // Diff of angle in degrees before the weapons get to start firing
    public float m_diffAngleBeforeFiring = 5;

    // The target currently being fired on
    public GameObject m_target;

    // Use this for initialization
    protected override void Start()
    {

    }

    // Update is called once per frame
    protected override void Update()
    {
        if (m_target)
        {
            float diffToTarget = Helpers.GetDiffAngle2D(transform.forward, m_target.transform.position - transform.position);
            if (Mathf.Abs(diffToTarget) < m_diffAngleBeforeFiring)
            {
                m_turrets[0].M_SetFiring(true);
                Invoke("M_FireOtherGun", m_durationBetweenGunFire);
            }
        }
    }

    private void M_FireOtherGun()
    {
        m_turrets[1].M_SetFiring(true);
    }

    public override void M_SetTargets(List<GameObject> targets)
    {
        m_targets = targets;
        m_target = Helpers.GetClosestObject(this.gameObject, targets);
        
        foreach (BaseTurret turret in m_turrets)
        {
            turret.M_SetTargets(targets);
            turret.M_SetFiring(false);
        }
        foreach (BaseWeapon weapon in m_weapons)
        {
            weapon.M_SetTargets(targets);
        }
    }
}
