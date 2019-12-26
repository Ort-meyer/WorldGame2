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
    // Maximum recoil (arbitrary number for now)
    public float m_maxRecoil = 10;

    public GameObject m_gfxObject;

    private Vector3 m_recoil;
    // Current timer of recoil in seconds
    private float m_currentRecoilTimer = 0;
    // Time in seconds that the recoil progresses
    public float m_recoilDuration = 1;
    private float m_currentRecoilProgress = 1;
    public AnimationCurve m_recoilCurve;

    Vector3 m_wobbleAxis = new Vector3(0, 0, 1);

    void Start()
    {
        // Kidna ugly hack to make sure the wobble doesnt occur upon spawning
        m_currentRecoilTimer = m_recoilDuration;
    }

    // Update is called once per frame
    void Update()
    {
        // DEBUG to add some recoil
        if(Input.GetKeyUp(KeyCode.X))
        {
            M_AddRecoil(new Vector3(0,0,0));
        }

        if (m_gfxObject)
        {
            //m_current
            float wobble = 0;
            if (m_currentRecoilTimer < m_recoilDuration)
            {
                m_currentRecoilTimer += Time.deltaTime;
                m_currentRecoilProgress = m_currentRecoilTimer / m_recoilDuration;
                wobble = m_recoilCurve.Evaluate(m_currentRecoilProgress) * m_maxRecoil;
            }
            m_gfxObject.transform.localEulerAngles = m_wobbleAxis.normalized * wobble;

        }
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


    public void M_AddRecoil(Vector3 direction)
    {
        if (direction.magnitude > 80)
        {
            m_wobbleAxis = Vector3.Cross(direction.normalized, transform.up);
            m_currentRecoilProgress = 0;
            m_currentRecoilTimer = 0;
        }
    }
}
