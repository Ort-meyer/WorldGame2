using System.Collections;   
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : BaseMovement
{
    // Max speed
    public float m_maxSpeed = 2;
    // Speed units increase per second
    //public float m_acceleration = 1;
    // Speed units decrease per second during break
    //public float m_breakForce = 5;
    private float m_currentSpeed = 0;

    public float m_turnSpeed = 3;
    // The angle before the tank starts to move forward
    public float m_angleToMove = 2;

    //private NavPathManager m_navPathManager;
    private CharacterController m_charControl;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        m_navPathManager = gameObject.GetComponent<NavPathManager>();
        m_charControl = gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_active)
        {
            // The angle we want to turn
            float turnAngle = 0;

            // Rotate relative pos to be relative convoy's direction (Old implementation. Reuse?)
            //Vector3 unitNextWaypoint = m_navPathManager.M_GetNextCorner() + m_unit.m_convoy.transform.rotation * m_unit.m_relativePosInConvoy;

            Vector3 unitNextWaypoint = m_navPathManager.M_GetNextCorner();

            Vector3 toNextWaypoint = unitNextWaypoint - transform.position;
            toNextWaypoint.y = 0;
            float diffDestAngle = Helpers.GetDiffAngle2D(transform.forward, toNextWaypoint);

            // Should we remove this? Could just set leashfactor to something enormous
            // Rotate correct waypoint pos depending on convoy direction (which is towards next waypoint)
            Vector3 correctConvoyPos = m_unit.m_convoy.transform.position + m_unit.m_convoy.transform.rotation * m_unit.m_relativePosInConvoy;
            Vector3 toCorrectConvoyPos = correctConvoyPos - transform.position;
            toCorrectConvoyPos.y = 0;
            float diffConAngle = Helpers.GetDiffAngle2D(transform.forward, toCorrectConvoyPos);

            Debug.DrawLine(transform.position, correctConvoyPos, Color.red);
            Debug.DrawLine(transform.position, unitNextWaypoint, Color.black);


            // TODO add some "avoid your buddies" logic??


            // Calculate how much of each we use to determine direction
            float leashFactor = Mathf.Clamp(toCorrectConvoyPos.magnitude / m_maxLeashDistance, 0, 1);
            //float diffAngle = leashFactor * diffConAngle + (1 - leashFactor) * diffDestAngle;
            float diffAngle = diffDestAngle; // Replace with above line for convoy logic (broken atm)

            //// Ugly way to keep unit from spinning
            if (toNextWaypoint.magnitude < 0.05)
            {
                diffAngle = 0;
            }

            turnAngle = diffAngle;
            if (Mathf.Abs(diffAngle) > m_turnSpeed * Time.deltaTime)
            {
                turnAngle = Helpers.Sign(diffAngle) * m_turnSpeed * Time.deltaTime;
            }
            transform.Rotate(new Vector3(0, turnAngle, 0));

            // Calculate whether to slow down or speed up
            // The dot product used to calculate increase or decrease of speed
            float speedDot = Vector3.Dot(transform.forward, toCorrectConvoyPos.normalized);
            // Should be between 0 and 1
            m_currentSpeed = m_maxSpeed * ((1 + speedDot) / 2);
            m_currentSpeed = m_maxSpeed;

            // This is ugly, to get the unit to stop. TODO replace with engine component?
            if (toNextWaypoint.magnitude < 0.15)
            {
                m_currentSpeed = 0;
            }


            // TODO proper acceleration and breaking. Separate engine and steering component?
            // Move forward
            if (Mathf.Abs(diffAngle) < m_angleToMove)
            {
                m_charControl.SimpleMove(transform.forward * m_currentSpeed);
            }
        }
        // Just to trigger graphics. TODO improve this
        else
        {
            m_charControl.SimpleMove(new Vector3());
        }
    }

    private float M_GetAngleFromTo(Vector3 from, Vector3 to)
    {
        Vector3 fromTo = from - to;
        fromTo.y = 0;
        float diffAngle = Helpers.GetDiffAngle2D(transform.forward, fromTo);
        if (fromTo.magnitude < 0.05)
        {
            diffAngle = 0;
        }
        return diffAngle;
    }

    
}
