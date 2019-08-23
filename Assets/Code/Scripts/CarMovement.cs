using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarMovement : BaseMovement
{

    public float m_speed = 2;
    public float m_turnSpeed = 3;
    public float m_maxWheelAngle = 30;
    // How much the car turns towards the wheels. From 0 to 1 (0 no turning at all, 1 instantaneous)
    //public float m_steeringFactor = 1;
    // How many degrees the car turns towards the wheels per second
    public float m_steering = 30;
    public List<GameObject> m_frontWheels;

    public GameObject m_DEBUG;

    private NavPathManager m_navPathManager;
    private Vector3 m_desVelocity;
    private CharacterController m_charControl;

    private Vector3 m_destinationPostion;

    private float m_currentWheelAngle = 0;
    private Vector3 m_wheelForward = new Vector3(0, 0, 1);

    void Start()
    {
        m_navPathManager = gameObject.GetComponent<NavPathManager>();
        m_charControl = gameObject.GetComponent<CharacterController>();

        m_wheelForward = transform.forward;

        // Debug stuff
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.U))
        {
            M_MoveTo(transform.position + transform.forward * 10);
        }
        if (Input.GetKeyUp(KeyCode.H))
        {
            M_MoveTo(transform.position + transform.right * -1 * 10);
        }
        if (Input.GetKeyUp(KeyCode.K))
        {
            M_MoveTo(transform.position + transform.right * 10);
        }
        Quaternion targetRot;
        
        if (!m_navPathManager.M_DestinationReached())
        {
            // Move wheels to face towards next waypoint
            Vector3 toNextWaypoint = m_navPathManager.M_GetNextCorner() - transform.position;
            if(toNextWaypoint.x != 0)
            {
                int derp = 2;
                derp++;
                //Debug.Break();
            }
            toNextWaypoint.y = 0;
            //targetRot = Quaternion.LookRotation(toNextWaypoint);
            //transform.rotation = targetRot;
            m_DEBUG.transform.position = m_navPathManager.M_GetNextCorner();

            // Turn wheels
            M_TurnWheels(toNextWaypoint);

            // Rotate car towards wheels
            float diffAngle = Helpers.GetDiffAngle2D(transform.forward, m_wheelForward);

            float steering = Helpers.Sign(diffAngle) * m_steering * Time.deltaTime;

            if(Mathf.Abs(steering) > Mathf.Abs(diffAngle))
            {
                transform.rotation = Quaternion.LookRotation(toNextWaypoint);
            }
            else
            {
                transform.Rotate(0, steering, 0);
            }
            m_charControl.SimpleMove(transform.forward.normalized * m_speed);
        }

    }

    // Sets the destination for this unit to move to
    public override void M_MoveTo(Vector3 destination)
    {
        m_navPathManager.M_SetDestination(destination);
    }

    // Clears destination and causes the unit to stop
    public override void M_StopOrder()
    {
        m_navPathManager.M_ClearDestination();
    }

    private void M_TurnWheels(Vector3 direction)
    {
        float diffAngle = Helpers.GetDiffAngle2D(m_wheelForward, direction);

        if (diffAngle > 0)
        {
            int derp = 2;
            derp++;
        }

        m_currentWheelAngle += Helpers.Sign(diffAngle) * m_turnSpeed * Time.deltaTime;

        //if(Helpers.Sign(diffAngle) != Helpers.Sign(m_currentWheelAngle))
        //{
        //    m_currentWheelAngle = 0;
        //    transform.rotation = Quaternion.FromToRotation(transform.forward, direction);
        //}


        // Limit to max wheel angle
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
}
