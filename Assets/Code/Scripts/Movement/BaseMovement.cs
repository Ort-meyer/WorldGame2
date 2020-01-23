using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMovement : MonoBehaviour
{
    // How far until this unit leashes fully
    public float m_maxLeashDistance = 3;
    // Destination of the unit
    [HideInInspector]
    public Vector3 m_destination;
    // The diff vector to where it should be in the convoy
    [HideInInspector]
    public Vector3 m_convoyPosDiff;
    public Unit m_unit;

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
        m_destination = destination;
    }

    // Clears destination and causes the unit to stop
    public virtual void M_StopOrder()
    {
        m_destination = transform.position;
    }

    public void M_SetConvoyPosDiff(Vector3 convoyPosDiff)
    {
        m_convoyPosDiff = convoyPosDiff;
    }
}
