using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingTurret : BaseTurret
{
    public float m_rotationSpeed = 30;
    private float m_currentAngle = 0;
    private GameObject m_target;
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    public override void M_SetTargets(List<GameObject> targets)
    {
        base.M_SetTargets(targets);
        // Engage closest target TODO this shouldn't be this universal
        GameObject closestObject = targets[0]; // TODO secure for empty list argument?
        float closestDistance = 100000; // Should be far 
        foreach (GameObject targetObj in targets)
        {
            float distToObject = (transform.position - targetObj.transform.position).magnitude;
            if(distToObject < closestDistance)
            {
                closestObject = targetObj;
                closestDistance = distToObject;
            }
        }
        m_target = closestObject;
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
