using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Well : Building
{
    public int m_waterProduced;

    // Use this for initialization
    void Start()
    {
        m_resourceProcesses.Add(
            new ResourceProcess(
                new Dictionary<Resource, int>
                {
                    // No consumption
                },
                new Dictionary<Resource, int>
                {
                    { Resource.Water, m_waterProduced }
                },
                m_completionTime));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
