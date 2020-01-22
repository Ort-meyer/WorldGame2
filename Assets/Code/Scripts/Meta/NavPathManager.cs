using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavPathManager : MonoBehaviour
{
    private Vector3 m_destination;
    private NavMeshPath m_path;

    private bool m_active = false;
    public float m_pathUpdateFrequency;

    public float m_cornerIncrementDistance;

    private int m_nextCornerIndex = 0;
    private bool m_destinationReached = true;
    
    // Use this for initialization
    void Start()
    {
        m_path = new NavMeshPath();
    }

    // Update is called once per frame
    void Update()
    {
        // Automatically increment when close to next corner (still not sure that this is best way to do it)
        if (m_active && m_path != null) // Shouldn't need second part of this if-case
        {
            if ((transform.position - m_path.corners[m_nextCornerIndex]).magnitude < m_cornerIncrementDistance)
            {
                m_nextCornerIndex++;
                // If we've reached the end, deactivate the component
                if (m_nextCornerIndex == m_path.corners.Length)
                {
                    m_active = false;
                    m_destinationReached = true;
                }
            }
        }
        // Don't do anything unless we have a destination
        if (!m_active)
        {
            CancelInvoke();
            return;
        }

        InvokeRepeating("UpdatePath", m_pathUpdateFrequency, m_pathUpdateFrequency);
    }

    private void UpdatePath()
    {
        NavMesh.CalculatePath(transform.position, m_destination, NavMesh.AllAreas, m_path);
        m_nextCornerIndex = 1;
        // Visualize path
        //foreach (Vector3 nodePos in m_path.corners)
        //{
        //    GameObject.Instantiate(DEBUGPathNodePrefab, nodePos, new Quaternion());
        //}
    }

    public void M_SetDestination(Vector3 destination)
    {
        // TODO what happens when I want to go somewhere I can't? Add feature to move to closes viable position
        m_active = true;
        m_destinationReached = false;
        m_destination = destination;
        UpdatePath();
    }

    public void M_ClearDestination()
    {
        m_active = false;
        m_destinationReached = true;
    }

    public Vector3 M_GetNextCorner()
    {
        Vector3 nextCorner = transform.position;
        if (!m_active) // Just to make sure we have a path at all
        {
            return nextCorner;
        }
        // If we haven't reached the end. Should need this but just to make sure
        if (m_nextCornerIndex < m_path.corners.Length)
        {
            nextCorner = m_path.corners[m_nextCornerIndex];
        }

        return nextCorner;
    }

    public float M_GetDistanceToDestination()
    {
        float distance = 0;
        if (m_path.corners.Length > 0)
        {
            distance = (m_path.corners[m_path.corners.Length - 1] - transform.position).magnitude;
        }
        return distance;
    }

    public bool M_DestinationReached()
    {
        return m_destinationReached;
    }
}
