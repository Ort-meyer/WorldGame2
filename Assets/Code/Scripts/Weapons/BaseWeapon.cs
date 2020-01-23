using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour
{
    // The turret object that this weapon is attached to
    public GameObject m_parentTurretObj;
    // The target that the weapon should be firing at
    protected Transform m_targetTrans = null;
    // The list of all targets that the turret is set to engage
    public List<GameObject> m_targets;
    // Whether this weapon fires on its own initiative. If not, M_Fire has to be called by a turret or something
    public bool m_automaticFiring = true;

    public bool m_readyToFire = false;

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

    virtual public void M_SetTargetPos(Transform target)
    {
        m_targetTrans = target;
    }

    virtual public void M_ClearTarget()
    {
        m_targetTrans= null;
        m_targets.Clear();
    }

    virtual public void M_SetFiring(bool isFiring)
    {
    }

    virtual public void M_Fire()
    {

    }
}
