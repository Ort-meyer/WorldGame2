using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // Movement speed of the unit
    public float m_moveSpeed;
    // How quickly the unit rotates towards forward
    public float m_rotationSpeed;
    // How far above the ground the unit is (this will likely be replaced)
    public float m_hoverheight;
    // When the distance between stopDistance and destination is this or below, the unit stops
    public float m_stopDistance;

    // Where the unit is currently moving to
    private Vector3 m_destination;
    // Use this for initialization
    void Start()
    {
        m_destination = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        // Movement: move in XY plane, then set height and up-vector (tilt)

        Vector3 vecToDest = m_destination - transform.position;
        float currentDistanceToDest = vecToDest.magnitude;
        // If not there yet, keep moving

        Vector3 newForward = transform.forward;
        Vector3 newUp = transform.up;
        if(!(currentDistanceToDest < m_stopDistance))
        {
            // Move in XZ plane
            Vector3 xyMoveVector = new Vector3(vecToDest.x, 0, vecToDest.z);
            transform.position += m_moveSpeed * xyMoveVector.normalized * Time.deltaTime;
            newForward = xyMoveVector;
            //transform.rotation = Quaternion.FromToRotation(transform.forward, xyMoveVector);
            //transform.rotation = Quaternion.Euler(0, 45, 0);
        }

        // Adjust hover distance and tilt
        Ray ray = new Ray(transform.position, -1 * Vector3.up);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        // Find terrain
        foreach (RaycastHit hit in hits)
        {
            if(hit.transform.gameObject.GetComponent<Terrain>())
            {
                newUp = hit.normal;
                // Set position to ray hit on terrain plus an offset (hover)
                transform.position = hit.point + hit.normal.normalized * m_hoverheight;
                break;
            }
        }

        transform.rotation = Quaternion.LookRotation(newForward, newUp);
        
    }

    public void M_SetDestination(Vector3 destination)
    {
        m_destination = destination;
    }

}
