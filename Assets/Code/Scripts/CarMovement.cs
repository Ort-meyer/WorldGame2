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

    //private float m_currentWheelAngle = 0;
    private Vector3 m_wheelForward = new Vector3(0, 0, 1);

    private float m_currentWheelAngle = 0;
    private float m_currentTransformAngle = 0;

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
            //// Move wheels to face towards next waypoint
            Vector3 toNextWaypoint = m_navPathManager.M_GetNextCorner() - transform.position;

            toNextWaypoint.y = 0;
            //targetRot = Quaternion.LookRotation(toNextWaypoint);
            //transform.rotation = targetRot;
            m_DEBUG.transform.position = m_navPathManager.M_GetNextCorner();

            //// Turn wheels
            //M_TurnWheels(toNextWaypoint);

            //// Rotate car towards wheels
            //float diffToTarget = Helpers.GetDiffAngle2D(transform.forward, m_wheelForward);
            ////float wheelAngle = 0; // TODO Remove this
            //float changeAngle = Helpers.Sign(diffToTarget) * m_steering * Time.deltaTime;
            //float newAngle = changeAngle;

            //float diff = Mathf.Abs(diffToTarget) - Mathf.Abs(newAngle);
            //if (diff < 0 && Mathf.Abs(diffToTarget) < 0.5)
            //{
            //    changeAngle = diffToTarget;
            //}

            ////// Limit to max wheel angle
            //if (Mathf.Abs(newAngle) > m_maxWheelAngle)  // TODO solve bug with waypoints right behind car
            //{
            //    changeAngle = 0;
            //}

            //transform.Rotate(0, changeAngle, 0);
            ////transform.Rotate(0, diffToTarget, 0);
            ////if(Mathf.Abs(steering) > Mathf.Abs(diffAngle))
            ////{
            ////    transform.rotation = Quaternion.LookRotation(toNextWaypoint);
            ////}
            ////else
            ////m_charControl.SimpleMove(transform.forward.normalized * m_speed);
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
        //float diffToTarget = Helpers.GetDiffAngle2D(m_wheelForward, direction);

        //// debug
        ////float changeAngle = diffToTarget;

        //float wheelAngle = Helpers.GetDiffAngle2D(transform.forward, m_wheelForward);
        //float changeAngle = Helpers.Sign(diffToTarget) * m_turnSpeed * Time.deltaTime;
        //float newAngle = wheelAngle + changeAngle;

        //float diff = Mathf.Abs(diffToTarget) - Mathf.Abs(newAngle);
        //if (diff < 0 && Mathf.Abs(diffToTarget) < 0.5)
        //{
        //    changeAngle = diffToTarget;
        //}

        ////// Limit to max wheel angle
        //if (Mathf.Abs(newAngle) > m_maxWheelAngle)  // TODO solve bug with waypoints right behind car
        //{
        //    changeAngle = 0;
        //}

        ////// Face the wheels correctly
        //foreach (GameObject obj in m_frontWheels)
        //{
        //    obj.transform.Rotate(0, changeAngle, 0);
        //}
        //m_wheelForward = Quaternion.Euler(0, changeAngle, 0) * m_wheelForward;
    }
}
