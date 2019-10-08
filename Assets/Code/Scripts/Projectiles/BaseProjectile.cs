using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectile : MonoBehaviour
{
    // The unit that this was fired from
    protected GameObject m_firingUnitObject;
    // Use this for initialization
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    public void M_SetFiringUnit(GameObject unitObject)
    {
        m_firingUnitObject = unitObject;
    }
}
