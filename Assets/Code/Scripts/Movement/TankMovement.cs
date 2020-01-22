﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : BaseMovement
{
    public float m_speed = 2;
    public float m_turnSpeed = 3;
    // The angle before the tank starts to move forward
    public float m_angleToMove = 2;

    //private NavPathManager m_navPathManager;
    private CharacterController m_charControl;

    // Use this for initialization
    public void Start()
    {
        //m_navPathManager = gameObject.GetComponent<NavPathManager>();
        m_charControl = gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 toNextWaypoint = m_destination - transform.position;
        // Poor way to ensure we stop moving when we're close to destination
        if (toNextWaypoint.magnitude > 0.05)
        {
            return;
        }
        toNextWaypoint.y = 0;

        // Rotate to face towards target (within a certain angle maybe?)
        float turnAngle = 0;
        float diffAngle = Helpers.GetDiffAngle2D(transform.forward, toNextWaypoint);

        turnAngle = diffAngle;
        if (Mathf.Abs(diffAngle) > m_turnSpeed * Time.deltaTime)
        {
            turnAngle = Helpers.Sign(diffAngle) * m_turnSpeed * Time.deltaTime;
        }
        transform.Rotate(new Vector3(0, turnAngle, 0));

        // Move forward
        if (Mathf.Abs(diffAngle) < m_angleToMove)
        {
            m_charControl.SimpleMove(transform.forward * m_speed);
        }
    }
}
