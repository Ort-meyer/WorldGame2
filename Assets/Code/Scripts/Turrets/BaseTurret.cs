using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTurret : MonoBehaviour
{
    // The target that the turret should be facing
    protected Transform m_targetTrans = null;
    // The list of all targets that the turret is set to engage
    public List<GameObject> m_targets = new List<GameObject>();

    public List<BaseTurret> m_turrets = new List<BaseTurret>();
    public List<BaseWeapon> m_weapons = new List<BaseWeapon>();

    public bool m_isFiring = false;

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
        foreach (BaseTurret turret in m_turrets)
        {
            turret.M_SetTargets(targets);
            //turret.M_SetFiring(true);
        }
        foreach(BaseWeapon weapon in m_weapons)
        {
            weapon.M_SetTargets(targets);
            weapon.M_SetFiring(true);
        }
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

    virtual public void M_SetFiring(bool isFiring)
    {
        m_isFiring = isFiring;
        foreach (BaseTurret turret in m_turrets)
        {
            turret.M_SetFiring(isFiring);
        }
        foreach (BaseWeapon weapon in m_weapons)
        {
            weapon.M_SetFiring(isFiring);
        }
    }
}

