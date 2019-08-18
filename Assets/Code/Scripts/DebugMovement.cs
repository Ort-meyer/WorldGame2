using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMovement : BaseMovement
{
    // Movement speed of the unit
    public float m_moveSpeed;
    // How quickly the unit rotates towards forward
    public float m_rotationSpeed;
    // How far above the ground the unit is (this will likely be replaced)
    public float m_hoverheight;
    // When the distance between stopDistance and destination is this or below, the unit stops
    public float m_stopDistance;
    
    // Character controller of this unit
    private CharacterController m_controller;
    private NavPathManager m_pathManager;
    // Use this for initialization
    void Start()
    {
        m_pathManager = GetComponent<NavPathManager>();
        m_controller = GetComponent<CharacterController>();
    }
    
    // Update is called once per frame
    void Update()
    {
        // Movement: move in XY plane, then set height and up-vector (tilt)
        Vector3 vecToDest = m_pathManager.M_GetNextCorner() - transform.position;
        float currentDistanceToDest = vecToDest.magnitude;
        // If not there yet, keep moving
        if (!(currentDistanceToDest < m_stopDistance))
        {
            // Move in XZ plane
            Vector3 xyMoveVector = new Vector3(vecToDest.x, 0, vecToDest.z);
            m_controller.SimpleMove(m_moveSpeed * xyMoveVector.normalized);
            transform.forward = new Vector3(m_controller.velocity.x, 0, m_controller.velocity.z);
        }
    }

    public override void M_MoveTo(Vector3 destination)
    {
        m_pathManager.M_SetDestination(destination);
    }

    public override void M_StopOrder()
    {
        m_pathManager.M_ClearDestination();
    }
}
