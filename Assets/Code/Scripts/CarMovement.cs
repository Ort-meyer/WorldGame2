using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarMovement : BaseMovement
{

    public float m_speed = 2;
    public float m_turnSpeed = 3;
    public float m_maxWheelAngle = 30;
    public List<GameObject> m_frontWheels;

    public GameObject m_DEBUG;

    private NavMeshAgent m_agent;
    private Vector3 m_desVelocity;
    private CharacterController m_charControl;

    private Vector3 m_destinationPostion;

    private float m_currentWheelAngle = 0;
    private Vector3 m_wheelForward = new Vector3(0, 0, 1);

    void Start()
    {
        m_agent = gameObject.GetComponent<NavMeshAgent>();
        m_charControl = gameObject.GetComponent<CharacterController>();
        m_agent.isStopped = true;
        m_wheelForward = transform.forward;
    }

    void Update()
    {
        Quaternion targetRot;

        m_agent.destination = m_destinationPostion;
        m_desVelocity = m_agent.desiredVelocity;

        m_agent.updatePosition = false;
        m_agent.updateRotation = false;

        if (!m_agent.isStopped)
        {
            // Move wheels to face towards next waypoint
            NavMeshPath path = m_agent.path;
            Vector3 toNextWaypoint = m_agent.path.corners[1] - transform.position;
            toNextWaypoint.y = 0;
            //targetRot = Quaternion.LookRotation(toNextWaypoint);
            //transform.rotation = targetRot;
            m_DEBUG.transform.position = m_agent.path.corners[1];

            M_TurnWheels(toNextWaypoint);
            
            //m_charControl.SimpleMove(m_desVelocity.normalized * m_speed);
        }

        m_agent.velocity = m_charControl.velocity;
    }

    // Sets the destination for this unit to move to
    public override void M_MoveTo(Vector3 destination)
    {
        m_destinationPostion = destination;
        m_agent.isStopped = false;
    }

    // Clears destination and causes the unit to stop
    public override void M_StopOrder()
    {
        m_agent.isStopped = true;
    }

    private void M_TurnWheels(Vector3 direction)
    {
        float targetAngle = Helpers.GetDiffAngle2D(m_wheelForward, direction);
        float sign = Mathf.Sign(targetAngle);
        m_currentWheelAngle += sign * m_turnSpeed * Time.deltaTime;
        // Limit wheel angle (wheels are instant atm)
        if (Mathf.Abs(m_currentWheelAngle) > m_maxWheelAngle)
        {
            m_currentWheelAngle = Mathf.Sign(m_currentWheelAngle) * m_maxWheelAngle;
        }

        Quaternion wheelRot = Quaternion.Euler(0, m_currentWheelAngle, 0);

        // Face the wheels correctly
        foreach (GameObject obj in m_frontWheels)
        {
            obj.transform.localRotation = wheelRot;
            m_wheelForward = obj.transform.forward;
        }
    }


    //public float m_maxMoveSpeed;
    //public float m_acceleration;
    //public float m_turnTime;

    //public float m_breakDistance;
    //public float m_breakPower;

    //public GameObject[] frontWheels;
    //public GameObject[] rearWheels;

    //// The maximum angle of the wheels
    //public float m_maxWheelAngle = 45;

    //private NavPathManager m_pathManager;

    ////private GameObject m_parent;

    //private float m_currentMoveSpeed = 0;
    //private float m_currentAngle = 0;
    //// Use this for initialization
    //void Start()
    //{
    //    m_pathManager = GetComponent<NavPathManager>();
    //    //m_parent = GetComponentInParent<BaseUnit>().gameObject;
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //    if (!m_pathManager.M_DestinationReached())
    //    {
    //        // Get next corner
    //        Vector3 nextCorner = m_pathManager.M_GetNextCorner();
    //        Vector3 nextToCurrent = nextCorner - transform.position;

    //        // Start breaking if we're approaching the destination
    //        if (m_pathManager.M_GetDistanceToDestination() <= m_breakDistance)
    //        {
    //            Break();
    //        }
    //        else
    //        {
    //            TurnWheels(nextToCurrent);
    //            Accelerate();
    //            // Rotate car depending on wheel angle
    //            Vector3 pivotPoint = transform.position; // Should be rear wheels, I reckon
    //            transform.RotateAround(pivotPoint, transform.up, m_currentAngle * m_turnTime * Time.deltaTime);
    //        }
    //        // Move car
    //        Vector3 movement = transform.forward * m_currentMoveSpeed * Time.deltaTime;
    //        GetComponent<CharacterController>().Move(movement);
    //    }
    //    else // Hard stop
    //    {
    //        m_currentMoveSpeed = 0;
    //    }
    //}


    //private void TurnWheels(Vector3 direction)
    //{
    //    m_currentAngle = Helpers.GetDiffAngle2D(transform.forward, direction);
    //    // Limit wheel angle (wheels are instant atm)
    //    if (Mathf.Abs(m_currentAngle) > m_maxWheelAngle)
    //    {
    //        m_currentAngle = Mathf.Sign(m_currentAngle) * m_maxWheelAngle;
    //    }

    //    // Face the wheels correctly
    //    Vector3 wheelForward = new Vector3(0, 0, 0);
    //    foreach (GameObject obj in frontWheels)
    //    {
    //        obj.transform.localRotation = Quaternion.Euler(0, m_currentAngle, 0);
    //        wheelForward = obj.transform.forward;
    //    }
    //}

    //private void Accelerate()
    //{
    //    // Todo: accelerate according to animation curve (quick in start, slow at end)
    //    m_currentMoveSpeed += m_acceleration * Time.deltaTime;
    //    if (m_currentMoveSpeed > m_maxMoveSpeed)
    //    {
    //        m_currentMoveSpeed = m_maxMoveSpeed;
    //    }
    //}

    //private void Break()
    //{
    //    m_currentMoveSpeed -= m_breakPower * Time.deltaTime;
    //    if (m_currentMoveSpeed < 0)
    //    {
    //        m_currentMoveSpeed = 0;
    //    }
    //}

    //public override void M_MoveTo(Vector3 destination)
    //{
    //    m_pathManager.M_SetDestination(destination);
    //}

    //public override void M_StopOrder()
    //{
    //    m_pathManager.M_ClearDestination();
    //}
}
