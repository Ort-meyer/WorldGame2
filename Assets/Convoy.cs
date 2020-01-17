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

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 convoyCenter = new Vector3();
        foreach(Unit unit in m_units)
        {
            convoyCenter += unit.transform.position;
        }
        convoyCenter = convoyCenter / m_units.Count;
        transform.position = convoyCenter;
    }

    public void M_MoveTo(Vector3 destination)
    {
        foreach(Unit unit in m_units)
        {
            unit.M_MoveTo(destination);
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
