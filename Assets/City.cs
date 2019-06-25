using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 Procution of water (well)
 Produces: water
    
 Production of food (farm)
 Consumes: water
 Produces: food
 
 Production of population (house)
 Consumes: water and food
 Produces: people
 
 

 Production of building materials (WIP)
 Consumes: steel, concrete
 Produces: building materials*/



public enum Resource { Food, Water, People}
class ResourceStockpile
{
    // Current amount of resources currently in stockpile
    float m_amount;
    // Maximum amount of resources in stockpile
    float m_max;
};
public class ResourceProcess
{
    public ResourceProcess(Dictionary<Resource, float> resourcesConsumed, Dictionary<Resource, float> resourcesProduced)
    {
        m_resourcesConsumed = resourcesConsumed;
        m_resourcesProduced = resourcesProduced;
    }
    // Resource mapped to amount of resource consumed by this process to produce
    protected Dictionary<Resource, float> m_resourcesConsumed;
    // Resource mapped to amount of resource produced by this process
    protected Dictionary<Resource, float> m_resourcesProduced;
    // Performs the process, consuming goods and producing
    public void Execute(ref Dictionary<Resource, float> cityStockpiles)
    {
        // First make sure that the resources needed exist
        foreach(var item in m_resourcesConsumed)
        {
            if(cityStockpiles[item.Key] - item.Value * Time.deltaTime >= 0)
            {
                // Check passed
                continue;
            }
            else
            {
                // Check didn't pass, don't produce
                return;
            }
        }
        // Then consume the resources
        foreach(var item in m_resourcesConsumed)
        {
            cityStockpiles[item.Key] -= item.Value * Time.deltaTime;
        }
        // Finally add the produced resources
        foreach(var item in m_resourcesProduced)
        {
            cityStockpiles[item.Key] += item.Value * Time.deltaTime;
        }
    }
}
// Placeholder classes
public class Building
{
    // Workers (WIP)
    //public int m_maxWorkers;
    //public int m_currentWorkers;
    // List of resource processes in this building
    protected List<ResourceProcess> m_resourceProcesses = new List<ResourceProcess>();

    public virtual void Produce(ref Dictionary<Resource, float> cityStockpiles)
    {
        foreach(ResourceProcess process in m_resourceProcesses)
        {
            process.Execute(ref cityStockpiles);
        }
        //// Consume resources
        //foreach (KeyValuePair<Resource, float> kvp in m_resourceConsumption)
        //{
        //    cityStockpiles[kvp.Key] -= kvp.Value;
        //}
        //// Produce resources
        //foreach (KeyValuePair<Resource, float> kvp in m_resourceConsumption)
        //{
        //    cityStockpiles[kvp.Key] += kvp.Value;
        //}
    }
};
class Farm : Building
{
    public Farm()
    {
        // TODO there must be a prettier way of doing this...
        m_resourceProcesses.Add(
            new ResourceProcess(
                new Dictionary<Resource, float>
                {
                    { Resource.Water, 5 }
                },
                new Dictionary<Resource, float>
                {
                    { Resource.Food, 2 }
                }));
    }
};
public class Well : Building
{
    public Well()
    {
        m_resourceProcesses.Add(
            new ResourceProcess(
                new Dictionary<Resource, float>
                {
                    // No consumption
                },
                new Dictionary<Resource, float>
                {
                    {Resource.Water, 3 }
                }
                ));
    }
};
//class House: Building
//{
//    public House()
//    {
//        m_resourceConsumption[Resource.Water] = 1;
//        m_resourceConsumption[Resource.Food] = 2;
//    }
//    public override void Produce(ref Dictionary<Resource, float> cityStockpiles)
//    {

//    }
//};
public class City : MonoBehaviour
{
    // List of all buildings in this city
    private List<Building> m_buildings = new List<Building>();
    // Resource type mapped to amount of said resource currently present in city
    private Dictionary<Resource, float> m_resourceStockpiles = new Dictionary<Resource, float>();


    // DEBUG stuff
    public float food = 0;
    public float water = 0;
    public float people = 0;

    // Use this for initialization
    void Start()
    {
        // Create all stockpiles with 0 value
        foreach (Resource res in System.Enum.GetValues(typeof(Resource)))
        {
            m_resourceStockpiles[res] = 0;
        }

        // Create some buildings
        
        m_buildings.Add(new Well());
        m_buildings.Add(new Farm());
    }

    // Update is called once per frame
    void Update()
    {
        // Update city stockpile with building consumption and production
        foreach(Building building in m_buildings)
        {
            building.Produce(ref m_resourceStockpiles);
        }

        food = m_resourceStockpiles[Resource.Food];
        water = m_resourceStockpiles[Resource.Water];
        people = m_resourceStockpiles[Resource.People];
    }
}
