using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DebugMovement : BaseMovement
{
    public float m_speed = 2;
    public float m_turnSpeed = 3;
    NavPathManager m_pathManager;

    void Start()
    {
        m_pathManager = GetComponent<NavPathManager>();
    }

    void Update()
    {
        Vector3 vecToNextCorner = m_pathManager.M_GetNextCorner() - transform.position;
        vecToNextCorner.y = 0;
        transform.position += vecToNextCorner.normalized * m_speed * Time.deltaTime;
    }

    //// Sets the destination for this unit to move to
    //public override void M_MoveTo(Vector3 destination)
    //{
    //    m_pathManager.M_SetDestination(destination);
    //}

    // Clears destination and causes the unit to stop
    public override void M_StopOrder()
    {

    }
}
