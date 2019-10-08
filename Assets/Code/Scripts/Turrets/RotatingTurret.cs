using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingTurret : BaseTurret
{
    public float m_rotationSpeed = 30;
    private float m_currentAngle = 0;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    protected override void Update()
    {
        if (m_targets.Count > 0)
        {
            // Set target (this should obviously have some more solid logic in the future...)
            m_targetTrans = m_targets[0].transform;
            // Set the same target for all weapons on this turret
            foreach (GameObject weaponObj in m_weapons)
            {
                weaponObj.GetComponent<BaseWeapon>().M_SetTargetPos(m_targetTrans);
            }
            Vector3 toTarget = m_targetTrans.position - transform.position;
            float diffToTarget = Helpers.GetDiffAngle2D(transform.forward, toTarget);
            if (Mathf.Abs(diffToTarget) < 1) // TODO improve this? Magic number is kinda bad
            {
                m_currentAngle += diffToTarget;
            }
            else
            {
                m_currentAngle += Mathf.Sign(diffToTarget) * m_rotationSpeed * Time.deltaTime;
            }
            transform.localRotation = Quaternion.Euler(0, m_currentAngle, 0);
        }
    }
}
