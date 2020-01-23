using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Convoy : MonoBehaviour
{
    // All units in this convoy
    public List<Unit> m_units = new List<Unit>();
    // Which faction this convoy belongs to
    public int m_faction = -1;

    public NavPathManager m_navPathManager;
    
    // Use this for initialization
    void Start()
    {
        //m_navPathManager = GetComponent<NavPathManager>();
    }

    private void Awake()
    {
        m_navPathManager = GetComponent<NavPathManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 convoyCenter = new Vector3();
        foreach(Unit unit in m_units)
        {
            convoyCenter += unit.transform.position+unit.m_relativePosInConvoy;
        }
        convoyCenter = convoyCenter * (1.0f / m_units.Count);
        transform.position = convoyCenter;
        // Point the convoy to its destination
        Vector3 toNextWaypoint = M_GetNextCorner() - transform.position;
        if(!m_navPathManager.M_DestinationReached())
        {
            transform.forward = toNextWaypoint.normalized;
        }
    }

    // It would be better to have a public member, right?
    public Vector3 M_GetNextCorner()
    {
        return m_navPathManager.M_GetNextCorner();
    }

    public void M_MoveTo(Vector3 destination)
    {
        m_navPathManager.M_SetDestination(destination);
        //foreach(Unit unit in m_units)
        //{
        //    unit.M_MoveTo(destination);
        //}
    }
    
    public void M_AttackOrder(List<GameObject> targets)
    {
        foreach (Unit unit in m_units)
        {
            unit.M_AttackOrder(targets);
        }
    }

    public void M_AttackOrder(List<Convoy> targets)
    {
        List<GameObject> targetObjs = new List<GameObject>();
        foreach(Convoy targetConvoy in targets)
        {
            foreach(Unit targetUnit in targetConvoy.m_units)
            {
                targetObjs.Add(targetUnit.gameObject);
            }
        }
        M_AttackOrder(targetObjs);
    }
}
