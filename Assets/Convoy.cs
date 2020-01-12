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

    }

    public void M_MoveTo(Vector3 destination)
    {
        foreach(Unit unit in m_units)
        {
            unit.GetComponent<BaseMovement>().M_MoveTo(destination);
        }
    }
    
    public void M_AttackOrder(List<GameObject> targets)
    {
        foreach (Unit unit in m_units)
        {

        }
    }

    public void M_AttackOrder(List<Convoy> targets)
    {
        foreach (Unit unit in m_units)
        {

        }
    }
}
