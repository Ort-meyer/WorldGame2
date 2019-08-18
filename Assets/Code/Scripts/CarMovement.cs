using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : BaseMovement
{
    public float m_maxMoveSpeed;
    public float m_acceleration;
    public float m_turnTime;

    public float m_breakDistance;
    public float m_breakPower;

    public GameObject[] frontWheels;
    public GameObject[] rearWheels;

    // The maximum angle of the wheels
    public float m_maxWheelAngle = 45;

    private NavPathManager m_pathManager;

    //private GameObject m_parent;

    private float m_currentMoveSpeed = 0;
    private float m_currentAngle = 0;
    // Use this for initialization
    void Start()
    {
        m_pathManager = GetComponent<NavPathManager>();
        //m_parent = GetComponentInParent<BaseUnit>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {

        if (!m_pathManager.M_DestinationReached())
        {
            // Get next corner
            Vector3 nextCorner = m_pathManager.M_GetNextCorner();
            Vector3 nextToCurrent = nextCorner - transform.position;

            // Start breaking if we're approaching the destination
            if (m_pathManager.M_GetDistanceToDestination() <= m_breakDistance)
            {
                Break();
            }
            else
            {
                TurnWheels(nextToCurrent);
                Accelerate();
                // Rotate car depending on wheel angle
                Vector3 pivotPoint = transform.position; // Should be rear wheels, I reckon
                transform.RotateAround(pivotPoint, transform.up, m_currentAngle * m_turnTime * Time.deltaTime);
            }
            // Move car
            Vector3 movement = transform.forward * m_currentMoveSpeed * Time.deltaTime;
            GetComponent<CharacterController>().Move(movement);
        }
        else // Hard stop
        {
            m_currentMoveSpeed = 0;
        }
    }


    private void TurnWheels(Vector3 direction)
    {
        m_currentAngle = Helpers.GetDiffAngle2D(transform.forward, direction);
        // Limit wheel angle (wheels are instant atm)
        if (Mathf.Abs(m_currentAngle) > m_maxWheelAngle)
        {
            m_currentAngle = Mathf.Sign(m_currentAngle) * m_maxWheelAngle;
        }

        // Face the wheels correctly
        Vector3 wheelForward = new Vector3(0, 0, 0);
        foreach (GameObject obj in frontWheels)
        {
            obj.transform.localRotation = Quaternion.Euler(0, m_currentAngle, 0);
            wheelForward = obj.transform.forward;
        }
    }

    private void Accelerate()
    {
        // Todo: accelerate according to animation curve (quick in start, slow at end)
        m_currentMoveSpeed += m_acceleration * Time.deltaTime;
        if (m_currentMoveSpeed > m_maxMoveSpeed)
        {
            m_currentMoveSpeed = m_maxMoveSpeed;
        }
    }

    private void Break()
    {
        m_currentMoveSpeed -= m_breakPower * Time.deltaTime;
        if (m_currentMoveSpeed < 0)
        {
            m_currentMoveSpeed = 0;
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
