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

public enum Resource { Food, Water }


public class ResourceStockpile
{
    // Current amount of resources currently in stockpile
    public float m_amount;
    // Maximum amount of resources in stockpile
    public float m_max;

    public ResourceStockpile(float startAmount, float max)
    {
        m_amount = startAmount;
        m_max = max;
    }
    ///* Updates amount of a resource and returns whatever was left over
    // * if stockpile was e*/
    //public float M_ChangeAmount(float change)
    //{
    //    float leftOver = 0;
    //    float newAmount = m_amount + change;
    //    if (newAmount < 0)
    //    {
    //        leftOver = -1 * newAmount;
    //        newAmount = 0;
    //    }
    //    else if (newAmount > m_max)
    //    {
    //        leftOver = newAmount - m_max;
    //        newAmount = m_max;
    //    }
    //    m_amount = newAmount;
    //    return leftOver;
    //}
};

public class ResourceProcess
{
    enum ProcessState { BuildingLocalStockpiles, Working, DoneWorking }
    public ResourceProcess(Dictionary<Resource, int> resourcesConsumed, Dictionary<Resource, int> resourcesProduced, float completionTime)
    {
        m_currentState = ProcessState.BuildingLocalStockpiles;
        m_completionTime = completionTime;
        m_resourcesConsumed = resourcesConsumed;
        m_resourcesProduced = resourcesProduced;
        // Build local stockpile maximums
        foreach (var kvp in m_resourcesConsumed)
        {
            m_inputStockpiles[kvp.Key] = new ResourceStockpile(0, kvp.Value);
        }
        // TODO some test that asserts that the same resource isn't consume AND produce?
    }

    // The time it takes for the process to be completed
    protected float m_completionTime;
    // How long the current process has been working
    protected float m_currentTime;
    // The current state of the process
    ProcessState m_currentState;
    // Whether the process is currently active
    //protected bool m_currentlyActive;
    // Resource mapped to amount of resource consumed by this process to produce
    protected Dictionary<Resource, int> m_resourcesConsumed = new Dictionary<Resource, int>();
    // Resource mapped to amount of resource produced by this process
    protected Dictionary<Resource, int> m_resourcesProduced = new Dictionary<Resource, int>();
    // How much of each resource is currently stockpiled. This never exceed 1 for production and the amount needed for consumption
    protected Dictionary<Resource, ResourceStockpile> m_inputStockpiles = new Dictionary<Resource, ResourceStockpile>();
    //protected Dictionary<Resource, ResourceStockpile> m_outputStockpiles = new Dictionary<Resource, ResourceStockpile>();
    // Performs the process, consuming goods and producing
    public void Execute(ref Dictionary<Resource, ResourceStockpile> cityStockpiles)
    {
        if (m_currentState == ProcessState.BuildingLocalStockpiles)
        {
            // 1) Fill local input stockpiles if the city has the supplies we need
            foreach (var kvp in m_inputStockpiles)
            {
                if (kvp.Value.m_amount < kvp.Value.m_max)
                {
                    if (cityStockpiles[kvp.Key].m_amount >= 1)
                    {
                        cityStockpiles[kvp.Key].m_amount--;
                        kvp.Value.m_amount++;
                    }
                }
            }
            // 2) If ALL local stockpiles are filled, deplete them and start proce
            bool allStockpilesFull = true;
            foreach (var kvp in m_inputStockpiles)
            {
                if (kvp.Value.m_amount != kvp.Value.m_max)
                {
                    allStockpilesFull = false;
                    break;
                }
            }
            if (allStockpilesFull)
            {
                foreach (var kvp in m_inputStockpiles)
                {
                    kvp.Value.m_amount = 0;
                }
                m_currentState = ProcessState.Working;
            }
        }

        // 3) When complete time is exceeded, try to increase city stockpile by produced. If it isn't possible, try again next frame
        if (m_currentState == ProcessState.Working)
        {
            m_currentTime += Time.deltaTime;
            if (m_currentTime >= m_completionTime)
            {
                m_currentTime = 0;
                m_currentState = ProcessState.DoneWorking;
            }
        }
        if (m_currentState == ProcessState.DoneWorking)
        {
            /* This only updates city stockpiles if ALL stockpiles can be updated at once
             * This means that if a process produces two resources, then both need to fit or the process hangs
             * It also means that if for instance newAmount is 14 but max is 15, it also hangs even though parts
             * of the production can be stored*/
            bool roomInCityStockiles = true;
            foreach (var kvp in m_resourcesProduced)
            {
                float newAmount = cityStockpiles[kvp.Key].m_amount + kvp.Value;
                if (newAmount >= cityStockpiles[kvp.Key].m_max)
                {
                    roomInCityStockiles = false;
                }
            }
            if (roomInCityStockiles)
            {
                foreach (var kvp in m_resourcesProduced)
                {
                    float newAmount = cityStockpiles[kvp.Key].m_amount + kvp.Value;
                    cityStockpiles[kvp.Key].m_amount = newAmount;
                }
                // 4) When city stockpiles are updated, go to 1)
                m_currentState = ProcessState.BuildingLocalStockpiles;
            }
        }
    }
}

public class City : MonoBehaviour
{
    // This will have to be put in some sort of list or something
    // Production buildings
    public GameObject m_wellPrefab;
    public GameObject m_farmPrefab;
    // City buildings
    public GameObject m_dwellingPrefab;


    // List of all buildings in this city
    private List<GameObject> m_buildings = new List<GameObject>();
    private List<GameObject> m_dwellings = new List<GameObject>();
    // Resource type mapped to amount of said resource currently present in city
    private Dictionary<Resource, ResourceStockpile> m_resourceStockpiles = new Dictionary<Resource, ResourceStockpile>();

    // Population
    // Current population total
    private float m_population;
    // How much food each pop eats
    public float m_foodPerPop;
    // How much food before a new pop appears
    public float m_newPopThreshold;
    //// How manu pops live in each dwelling
    //public int m_popPerDwelling;
    //// How many 
    //private int m_currentPopDwellingCounter = 0;

    // DEBUG stuff
    public float food = 0;
    public float water = 0;
    public float people = 0;

    // Use this for initialization
    void Start()
    {
        m_resourceStockpiles[Resource.Food] = new ResourceStockpile(0, 15);
        m_resourceStockpiles[Resource.Water] = new ResourceStockpile(0, 15);
        m_population = 10;

        // Create some buildings
        m_buildings.Add(Instantiate(m_wellPrefab, transform.position + new Vector3(-1, 0, 1), transform.rotation, transform));
        m_buildings.Add(Instantiate(m_wellPrefab, transform.position + new Vector3(0, 0, 1), transform.rotation, transform));
        //m_buildings.Add(Instantiate(m_wellPrefab, transform.position + new Vector3(1, 0, 1), transform.rotation, transform));

        m_buildings.Add(Instantiate(m_farmPrefab, transform.position + new Vector3(1, 0, -1), transform.rotation, transform));
        //m_buildings.Add(Instantiate(m_farmPrefab, transform.position + new Vector3(0, 0, -2), transform.rotation, transform));
    }

    // Update is called once per frame
    void Update()
    {
        M_UpdateProduction();
        //M_UpdatePopulation();
        food = m_resourceStockpiles[Resource.Food].m_amount;
        water = m_resourceStockpiles[Resource.Water].m_amount;
        people = m_population;
    }

    void M_UpdateProduction()
    {
        // Update city stockpile with building consumption and production
        foreach (GameObject buildingObj in m_buildings)
        {
            Building building = buildingObj.GetComponent<Building>();
            if (building)
            {
                building.Produce(ref m_resourceStockpiles);
            }
            else
            {
                Debug.LogError("building object in city doesn't have building script");
            }
        }
    }

    void M_UpdatePopulation()
    {
        float totalMaintenance = m_foodPerPop * m_population * Time.deltaTime;
        float remainingFood = m_resourceStockpiles[Resource.Food].m_amount - totalMaintenance;
        // Surplus: consume all food and create new pop
        if (remainingFood > m_newPopThreshold)
        {
            m_population++;
            m_resourceStockpiles[Resource.Food].m_amount = 0;
        }
        // Maintain current pop
        else if (remainingFood > 0)
        {
            m_resourceStockpiles[Resource.Food].m_amount = remainingFood;
        }
        // Starvation: remove a pop
        else
        {
            m_resourceStockpiles[Resource.Food].m_amount = 0;
            m_population -= 1 * Time.deltaTime; ;
        }
    }

    void M_PlaceDwelling()
    {

    }
}



//public class ResourceCache
//{
//    Resource m_resource;
//    float m_currentAmount;
//    float m_maxAmount;

//    public ResourceCache(Resource resource, float m_maxAmount)
//    {
//        m_resource = resource;
//        m_currentAmount = 0;
//    }

//    /* Updates amount of a resource and returns whatever was left over
//     * if stockpile was e*/
//    public float M_changeAmount(float change)
//    {
//        float leftOver = 0;
//        float newAmount = m_currentAmount - change;
//        if(newAmount < 0)
//        {
//            leftOver = -1 * newAmount;
//            newAmount = 0;
//        }
//        else if (newAmount > m_maxAmount)
//        {
//            leftOver = newAmount - m_maxAmount;
//            newAmount = m_maxAmount;
//        }
//        return leftOver;
//    }
//};