using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : Building
{
    public float m_waterConsumed;
    public float m_foodProduced;
    // Use this for initialization
    void Start()
    {
        m_resourceProcesses.Add(
        new ResourceProcess(
        new Dictionary<Resource, float>
        {
                            { Resource.Water, m_waterConsumed }
        },
        new Dictionary<Resource, float>
        {
                            { Resource.Food, m_foodProduced }
        }));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
