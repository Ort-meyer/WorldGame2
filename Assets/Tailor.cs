using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tailor : Building
{
    public int m_clothesProduced;

    // Use this for initialization
    void Start()
    {
        m_resourceProcesses.Add(
            new ResourceProcess(
                new Dictionary<Resource, int>
                {
                },
                new Dictionary<Resource, int>
                {
                    { Resource.Clothes, m_clothesProduced }
                },
                m_completionTime));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
