using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Well : Building
{
    public float m_waterProduced;

    // Use this for initialization
    void Start()
    {
        m_resourceProcesses.Add(
            new ResourceProcess(
                new Dictionary<Resource, float>
                {
                    // No consumption
                },
                new Dictionary<Resource, float>
                {
                    { Resource.Water, m_waterProduced }
                }
                ));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
