using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildRoomUI : MonoBehaviour
{
    public GameObject m_newUnitButtonObj;
    public GameObject m_saveUnitButtonObj;
    public GameObject m_loadUnitButtonObj;
    public GameObject m_componentDropdownObj;

    public GameObject m_unitSpawnPosition;

    private UnitBuilder m_unitBuilder;
    private WorldManager m_worldManager;

    private Dictionary<string, string> m_componentTypeMap = new Dictionary<string, string>();
    private GameObject m_unitConstructingObj;

    // Use this for initialization
    void Start()
    {
        // Set meta references
        m_unitBuilder = FindObjectOfType<UnitBuilder>();
        m_unitBuilder = FindObjectOfType<UnitBuilder>();

        // Init component dropdown
        Dropdown componentDropdown = m_componentDropdownObj.GetComponent<Dropdown>();
        componentDropdown.options.Clear();
        componentDropdown.onValueChanged.AddListener(delegate { M_ComponentDropdownValueChange(componentDropdown); });

        // Init new unit button
        m_newUnitButtonObj.GetComponent<Button>().onClick.AddListener(delegate {M_NewUnitButtonPress(); });
    }

    // Update is called once per frame
    void Update()
    {

    }

    void M_NewUnitButtonPress()
    {
        foreach (HullType hulltype in m_unitBuilder.m_hullPrefabs.Keys)
        {
            m_componentTypeMap.Add(hulltype.ToString(), "hull");
        }
        M_RefreshComponentList();
    }

    void M_ComponentDropdownValueChange(Dropdown dropdown)
    {
        string componentName = dropdown.captionText.text;
        string componentType = m_componentTypeMap[componentName];
        if(componentType == "hull")
        {
            if(m_unitConstructingObj)
            {
                Destroy(m_unitConstructingObj);
            }
            MetaUnit newUnit = new MetaUnit(Helpers.ToEnum<HullType>(componentName), 0);
            m_unitConstructingObj = m_unitBuilder.M_BuildMetaUnit(newUnit, m_unitSpawnPosition.transform.position, m_unitSpawnPosition.transform.rotation);
        }
        if (componentType == "turret")
        {

        }
        if (componentType == "weapon")
        {

        }

        
    }

    void M_RefreshComponentList()
    {
        Dropdown componentDropdown = m_componentDropdownObj.GetComponent<Dropdown>();
        componentDropdown.options.Clear();
        componentDropdown.options.Add(new Dropdown.OptionData("dummy"));


        foreach (string componentName in m_componentTypeMap.Keys)
        {
            componentDropdown.options.Add(new Dropdown.OptionData(componentName));
        }
    }

    //void M_AddComponentType(string componentType, )
}
