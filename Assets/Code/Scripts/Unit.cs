using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    public GameObject DEBUGTarget;

    // All turrets directly attached to this unit
    public List<GameObject> m_turrets;

    void Start()
    {
        foreach(GameObject turret in m_turrets)
        {
            turret.GetComponent<BaseTurret>().M_SetTargets(new List<GameObject> { DEBUGTarget });
        }
    }

    // Update is called once per frame
    void Update()
    {



    }
}
