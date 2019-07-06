using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : Building
{
    public int m_waterConsumed;
    public int m_foodProduced;
    // Use this for initialization
    void Start()
    {
        m_resourceProcesses.Add(
            new ResourceProcess(
                new Dictionary<Resource, int>
                {
                    { Resource.Water, m_waterConsumed }
                },
                new Dictionary<Resource, int>
                {
                    { Resource.Food, m_foodProduced }
                }, 
                m_completionTime));
        }

    // Update is called once per frame
    void Update()
    {

    }
}
