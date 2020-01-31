using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Convoy : MonoBehaviour
{
    // All units in this convoy
    public List<Unit> m_units = new List<Unit>();
    // Which faction this convoy belongs to
    public int m_faction = -1;

    // Use this for initialization
    void Start()
    {
        //m_navPathManager = GetComponent<NavPathManager>();
    }

    private void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        M_CalculateCentersForConvoy();
    }

    public void M_MoveTo(Vector3 destination)
    {
        foreach (Unit unit in m_units)
        {
            unit.M_MoveTo(destination, destination - transform.position);
        }
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
        foreach (Convoy targetConvoy in targets)
        {
            foreach (Unit targetUnit in targetConvoy.m_units)
            {
                targetObjs.Add(targetUnit.gameObject);
            }
        }
        M_AttackOrder(targetObjs);
    }

    private void M_CalculateCentersForConvoy()
    {
        Vector3 convoyCenterPos = new Vector3();
        Vector3 convoyCenterForward = new Vector3();
        foreach (Unit unit in m_units)
        {
            convoyCenterPos += unit.transform.position;// + unit.m_relativePosInConvoy; // relative pos shouldn't be here right?
            convoyCenterForward += unit.m_movement.m_navPathManager.M_GetNextCorner() - unit.transform.position;
            convoyCenterForward += unit.transform.forward.normalized;
        }
        convoyCenterPos = convoyCenterPos * (1.0f / m_units.Count);
        convoyCenterForward = convoyCenterForward.normalized;
        transform.position = convoyCenterPos;
        transform.rotation = Quaternion.LookRotation(convoyCenterForward);
    }
}
