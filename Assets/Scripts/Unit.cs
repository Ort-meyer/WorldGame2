using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // Movement speed of the unit
    public float m_moveSpeed;
    // How quickly the unit rotates towards forward
    public float m_rotationSpeed;
    // How far above the ground the unit is (this will likely be replaced)
    public float m_hoverheight;
    // When the distance between stopDistance and destination is this or below, the unit stops
    public float m_stopDistance;
    // Whether the unit has reached its destination or not
    public bool m_destinationReached;

    // Where the unit is currently moving to
    private Vector3 m_destination;
    // Character controller of this unit
    private CharacterController m_controller;
    // Use this for initialization
    void Start()
    {
        m_destination = transform.position;
        m_controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        // Movement: move in XY plane, then set height and up-vector (tilt)
        Vector3 vecToDest = m_destination - transform.position;
        // Only check in xz plane
        vecToDest.y = 0;
        float currentDistanceToDest = vecToDest.magnitude;
        // If not there yet, keep moving
        if(!(currentDistanceToDest < m_stopDistance))
        {
            m_destinationReached = false;
            // Move in XZ plane
            Vector3 xyMoveVector = new Vector3(vecToDest.x, 0, vecToDest.z);
            m_controller.SimpleMove(m_moveSpeed * xyMoveVector.normalized);
            transform.forward = new Vector3(m_controller.velocity.x, 0, m_controller.velocity.z);
        }
        else
        {
            m_destinationReached = true;
        }
    }

    public void M_SetDestination(Vector3 destination)
    {
        m_destination = destination;
    }

}
