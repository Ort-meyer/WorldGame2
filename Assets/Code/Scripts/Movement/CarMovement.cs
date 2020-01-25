using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarMovement : BaseMovement
{

    public float m_speed = 2;
    public float m_turnSpeed = 3;
    public float m_maxWheelAngle = 30;
    // How many degrees the car turns towards the wheels per second
    public float m_steering = 30;
    public List<GameObject> m_frontWheels;

    //public GameObject m_DEBUG;
    private Vector3 m_desVelocity;
    private CharacterController m_charControl;

    private float m_currentWheelAngle = 0;

    protected override void Start()
    {
        base.Start();
        m_navPathManager = gameObject.GetComponent<NavPathManager>();
        m_charControl = gameObject.GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector3 toNextWaypoint = m_navPathManager.M_GetNextCorner() - transform.position;
        // Poor way to ensure we stop moving when we're close to destination
        if (toNextWaypoint.magnitude > 0.05)
        {
            return;
        }
        toNextWaypoint.y = 0;

        // Direction vector with magnitude to where this unit should be in the convoy
        Vector3 toWhereWeShoudldBe = (m_unit.m_convoy.transform.position + m_unit.m_convoy.transform.position) - transform.position;

        M_TurnWheels(toNextWaypoint);
        M_TurnVehicle();
        m_charControl.SimpleMove(m_frontWheels[0].transform.forward.normalized * m_speed);

    }

    private void M_TurnWheels(Vector3 direction)
    {
        float diffToTarget = Helpers.GetDiffAngle2D(m_frontWheels[0].transform.forward, direction);
        // Set the diff angle if we risk overshooting out target (abvoids near 0 diff stutter)
        if (Mathf.Abs(diffToTarget) < 1) // TODO improve this? Magic number is kinda bad
        {
            m_currentWheelAngle += diffToTarget;
        }
        else
        {
            m_currentWheelAngle += Mathf.Sign(diffToTarget) * m_turnSpeed * Time.deltaTime;
        }
        m_currentWheelAngle = Helpers.LimitWithSign(m_currentWheelAngle, m_maxWheelAngle);

        foreach (GameObject obj in m_frontWheels)
        {
            obj.transform.localRotation = Quaternion.Euler(0, m_currentWheelAngle, 0);
        }
    }

    private void M_TurnVehicle()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, m_frontWheels[0].transform.rotation, m_steering * Time.deltaTime);
    }
}
