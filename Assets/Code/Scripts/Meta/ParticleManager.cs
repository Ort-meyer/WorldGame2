using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{

    private List<GameObject> m_activeParticleEffects = new List<GameObject>();
    //private List<GameObject> m_activeBulletHoles = new List<GameObject>(); // Do I ever need this?

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void M_AddParticle(GameObject newParticleEffect)
    {
        m_activeParticleEffects.Add(newParticleEffect);
    }
}
