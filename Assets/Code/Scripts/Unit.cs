using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{


    // All turrets directly attached to this unit
    private List<GameObject> m_turrets = new List<GameObject>();
    // What faction this unit belongs to
    public int m_faction;

    void Start()
    {

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

    public void M_Init()
    {
        BaseTurret[] turrets = GetComponentsInChildren<BaseTurret>();
        foreach (BaseTurret turret in turrets)
        {
            m_turrets.Add(turret.gameObject);
        }
    }
}
