using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{


    // All turrets directly attached to this unit
    private List<GameObject> m_turrets = new List<GameObject>();
    // What faction this unit belongs to
    public int m_faction;
    // Meta unit representation of this unit (won't consistency be a problem?)
    public MetaUnit m_metaUnit;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void M_AttackOrder(List<GameObject> targets)
    {
        GetComponent<BaseMovement>().M_StopOrder(); // TODO this should probably not this universal
        
        foreach (GameObject turret in m_turrets)
        {

            turret.GetComponent<BaseTurret>().M_SetTargets(targets);
        }
    }

    public void M_Init(MetaUnit metaUnit)
    {
        m_metaUnit = metaUnit;
        BaseTurret[] turrets = GetComponentsInChildren<BaseTurret>();
        foreach (BaseTurret turret in turrets)
        {
            m_turrets.Add(turret.gameObject);
        }
    }
}
