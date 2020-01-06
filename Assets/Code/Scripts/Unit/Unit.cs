using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private class Recoil
    {
        public float currentTimer = 0;
        public Vector3 recoil;

        public Recoil(Vector3 recoil)
        {
            this.recoil = recoil;
        }
    }

    // All turrets directly attached to this unit
    private List<BaseTurret> m_turrets = new List<BaseTurret>();
    // What faction this unit belongs to
    public int m_faction;
    // Meta unit representation of this unit (won't consistency be a problem?)
    public MetaUnit m_metaUnit;
    // Maximum recoil (arbitrary number for now)
    public float m_maxRecoil = 10;

    public GameObject m_gfxObject;

    private Vector3 m_recoil;
    // Time in seconds that the recoil progresses
    public float m_recoilDuration = 1;
    public AnimationCurve m_recoilCurve;

    List<Recoil> m_recoilList = new List<Recoil>();

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // DEBUG to add some recoil
        if (Input.GetKeyUp(KeyCode.X))
        {
            M_AddRecoil(new Vector3(0, 0, 0));
        }

        if (m_gfxObject)
        {
            // Calculate recoil based on all currently active recoils
            Vector3 totalRecoil = new Vector3();
            
            foreach (Recoil recoil in m_recoilList)
            {
                if (recoil.currentTimer < m_recoilDuration)
                {
                    recoil.currentTimer += Time.deltaTime;
                    float progress = recoil.currentTimer / m_recoilDuration;
                    float factor = m_recoilCurve.Evaluate(progress) * m_maxRecoil;
                    totalRecoil += factor * recoil.recoil;
                }
            }
            // TODO maybe not do this so often?
            m_recoilList.RemoveAll(recoil => recoil.currentTimer > m_recoilDuration);

            Vector3 wobbleAxis = Vector3.Cross(totalRecoil.normalized, transform.up);

            m_gfxObject.transform.localRotation = Quaternion.AngleAxis(totalRecoil.magnitude, wobbleAxis);
        }
    }

    public void M_AttackOrder(List<GameObject> targets)
    {
        GetComponent<BaseMovement>().M_StopOrder(); // TODO this should probably not this universal

        foreach (BaseTurret turret in m_turrets)
        {
            turret.M_SetTargets(targets);
        }
    }

    public void M_Init(MetaUnit metaUnit, List<BaseTurret> turrets)
    {
        m_metaUnit = metaUnit;
        m_turrets = turrets;
    }


    public void M_AddRecoil(Vector3 direction)
    {
        if (direction.magnitude > 80)
        {
            // TODO have magnitude be relevant
            m_recoilList.Add(new Recoil(direction.normalized));
        }
    }
}
