using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMovement : MonoBehaviour
{
    // How far until this unit leashes fully
    public float m_maxLeashDistance = 3;
    // The diff vector to where it should be in the convoy
    [HideInInspector]
    public Vector3 m_convoyPosDiff;
    public Unit m_unit;

    public NavPathManager m_navPathManager;

    // Whether this unit should move at all
    public bool m_active = false;

    // Use this for initialization
    protected virtual void Start()
    {
        m_unit = GetComponent<Unit>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Sets the destination for this unit to move to
    public virtual void M_MoveTo(Vector3 destination)
    {
        m_navPathManager.M_SetDestination(destination);
    }

    // Clears destination and causes the unit to stop
    public virtual void M_StopOrder()
    {
        m_navPathManager.M_ClearDestination();
    }

    public void M_SetConvoyPosDiff(Vector3 convoyPosDiff)
    {
        m_convoyPosDiff = convoyPosDiff;
    }
}
