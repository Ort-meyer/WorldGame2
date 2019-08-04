using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trader : MonoBehaviour
{
    // The trader will not move to the city unless the demand is greater than this
    public float m_demandThreshold;
    // For now, can only transport one resource at a time
    public float m_maxTraderStorage;
    private Resource m_resource;
    private ResourceStockpile m_stockpile;
    private GameObject m_supplyCityObj;
    private GameObject m_demandCityObj;

    enum TraderStatus {Idle, MovingToSupplier, MovingToDemander, Pause };

    private TraderStatus m_status = TraderStatus.Idle;

    public World m_world;

    private Unit m_unit;
    // Use this for initialization
    void Start()
    {
        m_unit = GetComponent<Unit>();
        m_stockpile = new ResourceStockpile(0, m_maxTraderStorage);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_status == TraderStatus.Idle)
        {
            M_FindTrade();
        }
        else if (m_status == TraderStatus.MovingToSupplier)
        {
            if (m_unit.m_destinationReached)
            {
                m_supplyCityObj.GetComponent<City>().m_resourceStockpiles[m_resource].M_TransferAllResourcesTo(ref m_stockpile);
                m_unit.M_SetDestination(m_demandCityObj.transform.position);
                m_status = TraderStatus.MovingToDemander;
            }
        }
        else if (m_status == TraderStatus.MovingToDemander)
        {
            if (m_unit.m_destinationReached)
            {
                ResourceStockpile targetStockpile = m_demandCityObj.GetComponent<City>().m_resourceStockpiles[m_resource]; // I really wish I had pointers to make this explicit...
                m_stockpile.M_TransferAllResourcesTo(ref targetStockpile);
                m_supplyCityObj.GetComponent<City>().m_resourceStockpiles[m_resource].M_TransferAllResourcesTo(ref m_stockpile);
                m_status = TraderStatus.Idle;
            }
        }
    }

    // Calculates a trade route
    private void M_FindTrade()
    {
        float value = 0;
        foreach (GameObject demandCityObj in m_world.m_cities)
        {
            City demandCity = demandCityObj.GetComponent<City>();
            // Iterate for demand
            foreach (KeyValuePair<Resource, float> kvp in demandCity.m_resourceDemands)
            {
                Resource resource = kvp.Key;
                float demand = kvp.Value;
                foreach (GameObject supplyCityObj in m_world.m_cities)
                {
                    City supplyCity = supplyCityObj.GetComponent<City>();
                    if (supplyCityObj.GetInstanceID() == demandCityObj.GetInstanceID())
                    {
                        continue;
                    }
                    //foreach(KeyValuePair<Resource, ResourceStockpile> supplyStockpiles in supplyCity.m_resourceStockpiles)
                    //{
                    float supply = supplyCity.m_resourceStockpiles[kvp.Key].m_amount;
                    float distanceToSupply = (transform.position - supplyCity.transform.position).magnitude;
                    float distanceSupplyToDemand = (supplyCity.transform.position - demandCity.transform.position).magnitude;
                    float totalDistance = distanceToSupply + distanceSupplyToDemand;
                    float newValue = M_CalculateTradeValue(demand, supply, totalDistance);
                    if (newValue > m_demandThreshold)
                    {
                        if (newValue > value)
                        {
                            value = newValue;
                            m_resource = resource;
                            m_supplyCityObj = supplyCityObj;
                            m_demandCityObj = demandCityObj;
                            // Start moving to supply city
                            m_status = TraderStatus.MovingToSupplier;
                            m_unit.M_SetDestination(m_supplyCityObj.transform.position);
                        }
                    }
                }
            }
        }
    }
    // Calcualtes the value of a proposed trade
    private float M_CalculateTradeValue(float demand, float supply, float distance)
    {
        float value = (demand * supply) / distance; //TODO improve this
        return value;
    }
}
