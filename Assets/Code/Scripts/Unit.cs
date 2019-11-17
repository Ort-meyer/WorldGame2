using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{


    // All turrets directly attached to this unit
    public List<GameObject> m_turrets;
    // What faction this unit belongs to
    public int m_faction;

    void Start()
    {
        foreach (GameObject turret in m_turrets)
        {
            //turret.GetComponent<BaseTurret>().M_SetTargets(new List<GameObject> { DEBUGTarget });
        }
    }

    // Update is called once per frame
    void Update()
    {



    }

    public void M_AttackOrder(List<GameObject> targets)
    {
        foreach (GameObject turret in m_turrets)
        {
            turret.GetComponent<BaseTurret>().M_SetTargets(targets);
        }
    }
}
