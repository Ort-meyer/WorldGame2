using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTurret : MonoBehaviour
{
    // The target that the turret should be facing
    protected Transform m_targetTrans = null;
    // The list of all targets that the turret is set to engage
    public List<GameObject> m_targets = new List<GameObject>();

    public List<GameObject> m_weapons = new List<GameObject>();

    // Use this for initialization
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
    }

    virtual public void M_SetTargets(List<GameObject> targets)
    {
        m_targets = targets;
    }

    virtual public void M_SetTarget(GameObject target)
    {
        M_SetTargets(new List<GameObject> { target });
    }

    virtual public void M_SetTargetPos(Transform targetPos)
    {
        m_targetTrans = targetPos;
    }

    virtual public void M_ClearTarget()
    {
        m_targetTrans = null;
        m_targets.Clear();
    }
}

