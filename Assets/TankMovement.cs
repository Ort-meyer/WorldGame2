using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : BaseMovement
{
    public float m_speed = 2;
    public float m_turnSpeed = 3;

    private NavPathManager m_navPathManager;
    private CharacterController m_charControl;

    // Use this for initialization
    public void Start()
    {
        m_navPathManager = gameObject.GetComponent<NavPathManager>();
        m_charControl = gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_navPathManager.M_DestinationReached())
        {
            Vector3 toNextWaypoint = m_navPathManager.M_GetNextCorner() - transform.position;
            toNextWaypoint.y = 0;

            // Rotate to face towards target (within a certain angle maybe?)
            
            // Move forward
        }
    }
}
