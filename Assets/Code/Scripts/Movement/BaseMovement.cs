using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMovement : MonoBehaviour
{
    public Vector3 m_destination;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Sets the destination for this unit to move to
    public virtual void M_MoveTo(Vector3 destination)
    {
        m_destination = destination;
    }

    // Clears destination and causes the unit to stop
    public virtual void M_StopOrder()
    {
        m_destination = transform.position;
    }
}
