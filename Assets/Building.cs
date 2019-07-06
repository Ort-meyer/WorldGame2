using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public int m_completionTime;
    // List of resource processes in this building
    protected List<ResourceProcess> m_resourceProcesses = new List<ResourceProcess>();

    public virtual void Produce(ref Dictionary<Resource, ResourceStockpile> cityStockpiles)
    {
        foreach (ResourceProcess process in m_resourceProcesses)
        {
            process.Execute(ref cityStockpiles);
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
