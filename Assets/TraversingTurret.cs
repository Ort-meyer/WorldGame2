using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraversingTurret : BaseTurret
{
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
        if(m_targets.Count > 0)
        {
            // TODO can't much of this be inherited from RotatingTurret?
            m_targetTrans = m_targets[0].transform;
            foreach (GameObject weaponObj in m_weapons)
            {
                weaponObj.GetComponent<BaseWeapon>().M_SetTargetPos(m_targetTrans);
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
            transform.localRotation = Quaternion.Euler(m_currentElevationAngle, m_currentTraverseAngle, 0);
        }
    }
}
