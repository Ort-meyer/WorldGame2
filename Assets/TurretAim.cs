using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAim : BaseTurret
{
    // Speed of projectile fired by weapon, used to calculate traverse
    public float m_weaponProjectileLaunchSpeed = 10;

    public float m_traverseSpeed = 200;
    public float m_elevationSpeed = 20;

    public float m_maxTraverse = 50;
    public float m_maxElevation = 30;

    private float m_currentTraverseAngle = 0;
    private float m_currentElevationAngle = 0;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (m_targets.Count > 0)
        {
            // TODO can't much of this be inherited from RotatingTurret?
            m_targetTrans = m_targets[0].transform;
            foreach (BaseWeapon weapon in m_weapons)
            {
                weapon.M_SetTargetPos(m_targetTrans);
            }

            // Traverse
            Vector3 toTarget = m_targetTrans.position - transform.position;
            float diffToTarget = Helpers.GetDiffAngle2D(transform.forward, toTarget);
            if (Mathf.Abs(diffToTarget) < 1) // TODO improve this? Magic number is kinda bad
            {
                m_currentTraverseAngle += diffToTarget;
            }
            else
            {
                m_currentTraverseAngle += Mathf.Sign(diffToTarget) * m_traverseSpeed * Time.deltaTime;
            }
            // Clamp to max traverse
            m_currentTraverseAngle = Helpers.LimitWithSign(m_currentTraverseAngle, m_maxTraverse);

            // Elevation
            float distanceToTarget = toTarget.magnitude;
            float heightDifference = toTarget.y;
            float elevationTarget = Helpers.GetAngleToHit(distanceToTarget, heightDifference, m_weaponProjectileLaunchSpeed) * Mathf.Rad2Deg;
            float elevationDiff = elevationTarget - m_currentElevationAngle;

            if (Mathf.Abs(elevationDiff) < 1) // TODO improve this? Magic number is kinda bad
            {
                m_currentElevationAngle += elevationDiff;
            }
            else
            {
                m_currentElevationAngle += Mathf.Sign(elevationDiff) * m_elevationSpeed * Time.deltaTime;
            }
            // Clamp to max traverse
            m_currentElevationAngle = Helpers.LimitWithSign(m_currentElevationAngle, m_maxElevation);

            transform.localRotation = Quaternion.Euler(-1 * m_currentElevationAngle, m_currentTraverseAngle, 0);
        }
    }
}
