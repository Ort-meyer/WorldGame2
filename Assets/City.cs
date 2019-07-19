using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
// Different resource types
public enum Resource { Food, Water, Clothes, Drugs }
// All population types
public enum PopulationType { Uneducated, Educated, Rich }

public class Population
{
    // What type of pop this is
    public PopulationType m_popType;
    // How much in this population
    public float m_amount;
    // The needs of each resource for each pop in this group
    //public Dictionary<Resource, float> m_needs = new Dictionary<Resource, float>();
    // How much of each resource is consumed by each pop in this group
    public Dictionary<Resource, float> m_maintenance = new Dictionary<Resource, float>();
    // How much this pop desires each resource
    public Dictionary<Resource, float> m_importance = new Dictionary<Resource, float>();
};


public class ResourceStockpile
{
    // Current amount of resources currently in stockpile
    public float m_amount { get; private set; }
    // Maximum amount of resources in stockpile
    public float m_max { get; private set; }

    public ResourceStockpile(float startAmount, float max)
    {
        m_amount = startAmount;
        m_max = max;
    }
    // Updates value. Tries to increase the stockpile. Returns remaining if full
    public float M_AddResources(float change)
    {
        float newAmount = m_amount + change;
        float remaining = 0;
        if (newAmount > m_max)
        {
            remaining = newAmount - m_max;
            m_amount = m_max;
        }
        else
        {
            m_amount = newAmount;
        }
        return remaining;
    }

    // Gets resources. If the stockpile is empty, gives what's left
    public float M_FetchResources(float desiredAmount)
    {
        float newAmount = m_amount - desiredAmount;
        float resourcesFetched = desiredAmount;
        if (newAmount < 0)
        {
            resourcesFetched = m_amount;
            m_amount = 0;
        }
        else
        {
            m_amount = newAmount;
        }
        return resourcesFetched;
    }

    // Transfer resources from this stockpile to another stockpile
    public void M_TransferResourcesTo(ref ResourceStockpile otherStockpile, float amount)
    {
        if (amount < 0)
        {
            Debug.LogError("using ResourceStockpile.M_TransferResources wrong. Amount has to be >= 0");
        }
        // Prevent transfering more than what's possible
        if (amount >= m_amount)
        {
            amount = m_amount;
        }
        float leftOver = otherStockpile.M_AddResources(amount);
        m_amount = leftOver;
    }

    public void M_TransferAllResourcesTo(ref ResourceStockpile otherStockpile)
    {
        M_TransferResourcesTo(ref otherStockpile, m_amount);
    }

    // Sets amount to 0
    public void M_Empty()
    {
        m_amount = 0;
    }
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
        foreach (var kvp in m_resourcesProduced)
        {
            m_outputStockpiles[kvp.Key] = new ResourceStockpile(0, kvp.Value);
        }
        // TODO some test that asserts that the same resource isn't consume AND produce?
    }

    // The time it takes for the process to be completed
    protected float m_completionTime;
    // How long the current process has been working
    protected float m_currentTime;
    // The current state of the process
    ProcessState m_currentState;
    // Resource mapped to amount of resource consumed by this process to produce
    protected Dictionary<Resource, int> m_resourcesConsumed = new Dictionary<Resource, int>();
    // Resource mapped to amount of resource produced by this process
    protected Dictionary<Resource, int> m_resourcesProduced = new Dictionary<Resource, int>();
    // How much of each resource is currently stockpiled. This never exceed 1 for production and the amount needed for consumption
    protected Dictionary<Resource, ResourceStockpile> m_inputStockpiles = new Dictionary<Resource, ResourceStockpile>();
    // The stockpiles which containt he output of the process. This is emptied into city stockpiles once the production is complete
    protected Dictionary<Resource, ResourceStockpile> m_outputStockpiles = new Dictionary<Resource, ResourceStockpile>();
    // Performs the process, consuming goods and producing
    public void Execute(ref Dictionary<Resource, ResourceStockpile> cityStockpiles)
    {
        if (m_currentState == ProcessState.BuildingLocalStockpiles)
        {
            // 1) Fill local input stockpiles if the city has the supplies we need
            foreach (var kvp in m_inputStockpiles)
            {
                kvp.Value.M_AddResources(kvp.Value.m_amount + cityStockpiles[kvp.Key].M_FetchResources(1)); // TODO reconsider fetching 1 at a time. Maybe more, maybe max
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
                    kvp.Value.M_Empty();
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
                foreach (var kvp in m_resourcesProduced)
                {
                    m_outputStockpiles[kvp.Key].M_AddResources(kvp.Value);
                }
                m_currentState = ProcessState.DoneWorking;
            }
        }
        if (m_currentState == ProcessState.DoneWorking)
        {
            bool allTransfersDone = true;
            foreach (var kvp in m_outputStockpiles)
            {
                // This better behave like a pointer! (TODO be sure? I'm sure I'll notice)
                ResourceStockpile cityStockpile = cityStockpiles[kvp.Key];
                kvp.Value.M_TransferAllResourcesTo(ref cityStockpile);
                if(kvp.Value.m_amount >0)
                {
                    allTransfersDone = false;
                }
            }
            if(allTransfersDone)
            {
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
    private Dictionary<Resource, ResourceStockpile> m_prevResourceStockpiles = new Dictionary<Resource, ResourceStockpile>();
    // The demand for each resource in this city
    private Dictionary<Resource, float> m_resourceDemands = new Dictionary<Resource, float>();

    // Population
    private Dictionary<PopulationType, Population> m_population = new Dictionary<PopulationType, Population>();
    // Factor of how great reserves the city desires (between 0 and 1, where 1 is 100% stockpile filled) (TODO should be per-resource, not global)
    private float m_desiredResourceReserveFactor;
    
    // DEBUG stuff
    public float food = 0;
    public float water = 0;
    public float people = 0;
    public float drugs = 0;
    public float clothes = 0;

    public float m_foodDemand = 0;
    public float m_clothesDemand = 0;
    public float m_drugsDemand = 0;

    public float m_maxDemand = 20;
    public float m_demandGrowthFactor = 1;

    //public float foodNeed = 0;
    //public float Need = 0;
    //public float clothesNeed = 0;
    //public float drugsNee = 0;

    public float m_needChange;

    public float m_startPopulation;
    // How much food each pop eats
    public float m_foodPerPop;
    // How much food before a new pop appears
    public float m_newPopThreshold;


    // Use this for initialization
    void Start()
    {
        // Initialize all resource thingies
        foreach (var resource in (Resource[])Enum.GetValues(typeof(Resource)))
        {
            m_resourceStockpiles[resource] = new ResourceStockpile(0, 15); // TOOO not have hardcoded max stockpile for each resource
            m_resourceDemands[resource] = 0;
        }

        // Setup some workers
        Population uneducated = new Population();
        uneducated.m_amount = m_startPopulation;

        uneducated.m_maintenance[Resource.Food] = m_foodPerPop;
        uneducated.m_maintenance[Resource.Clothes] = m_foodPerPop;
        uneducated.m_maintenance[Resource.Drugs] = m_foodPerPop;
        uneducated.m_popType = PopulationType.Uneducated;
        //// Create a need for every maintenance
        //foreach (var kvp in uneducated.m_maintenance)
        //{
        //    uneducated.m_needs[kvp.Key] = 0;
        //}
        m_population[PopulationType.Uneducated] = uneducated;

        // Debug stuff (kinda)
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
        if(Input.GetKeyUp(KeyCode.K))
        {
            m_resourceStockpiles[Resource.Drugs].M_AddResources(100);
        }

        m_prevResourceStockpiles = m_resourceStockpiles;

        M_UpdateProduction();
        M_UpdatePopulation();


        // Visualization debug stuff
        food = m_resourceStockpiles[Resource.Food].m_amount;
        water = m_resourceStockpiles[Resource.Water].m_amount;
        drugs = m_resourceStockpiles[Resource.Drugs].m_amount;
        clothes = m_resourceStockpiles[Resource.Clothes].m_amount;


        m_foodDemand = m_resourceDemands[Resource.Food];
        m_drugsDemand = m_resourceDemands[Resource.Drugs];
        m_clothesDemand = m_resourceDemands[Resource.Clothes];
        

        //foodNeed = m_population[PopulationType.Uneducated].m_needs[Resource.Food];
        //clothesNeed = m_population[PopulationType.Uneducated].m_needs[Resource.Clothes];
        //drugsNeed = m_population[PopulationType.Uneducated].m_needs[Resource.Drugs];

        
        // Other debug stuff
        m_population[PopulationType.Uneducated].m_maintenance[Resource.Food] = m_foodPerPop;
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
        // Update maintenance
        foreach (var kvp in m_population)
        {
            Population pop = kvp.Value;
            foreach (var kvp1 in pop.m_maintenance)
            {
                Resource resource = kvp1.Key;
                float maintenance = kvp1.Value;
                float totalMaintenance = pop.m_amount * maintenance * Time.deltaTime;
                ResourceStockpile stockpile = m_resourceStockpiles[resource];
                float unfulfilledMaintenance = totalMaintenance - stockpile.M_FetchResources(totalMaintenance);

                // TODO improve this somehow?
                /* 1) add base demand. Too much supply decreases, too little supple increases
                 * 2) fix decay down to base demand
                 * 3) exponential growth?*/
                if (unfulfilledMaintenance > 0)
                {
                    m_resourceDemands[resource] += unfulfilledMaintenance * m_demandGrowthFactor;
                    if (m_resourceDemands[resource] > m_maxDemand)
                    {
                        m_resourceDemands[resource] = m_maxDemand;
                    }
                }
                else // Maintenance fulfilled
                {
                    m_resourceDemands[resource] = 0; // TODO not just clear maintenance
                }
            }
        }

        // Update needs
        //foreach (var kvp in m_resourceStockpiles)
        //{
        //    Resource res = kvp.Key;
        //    ResourceStockpile stockpile = kvp.Value;

        //    // Update demand depending on delta
        //    float delta = stockpile.m_amount - m_prevResourceStockpiles[res].m_amount;
        //    float deltaFactor = delta / stockpile.m_max;
        //    stockpile.m_demand += -1 * deltaFactor;

        //    // Update demand depending on reserves
        //    float stockpileFactor = stockpile.m_amount / stockpile.m_max;
        //    stockpile.m_demand += m_desiredResourceReserveFactor - stockpileFactor;
        //}



        // Update population counts
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