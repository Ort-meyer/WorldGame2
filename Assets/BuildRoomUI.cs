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

    // Use this for initialization
    void Start()
    {
        // Set meta references
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
        Dropdown componentDropdown = m_componentDropdownObj.GetComponent<Dropdown>();
        componentDropdown.options.Clear();
        componentDropdown.options.Add(new Dropdown.OptionData("dummy"));


        foreach (HullType hulltype in m_unitBuilder.m_hullPrefabs.Keys)
        {
            componentDropdown.options.Add(new Dropdown.OptionData(hulltype.ToString()));
            
        }
    }

    void M_ComponentDropdownValueChange(Dropdown dropdown)
    {
        MetaUnit newUnit = new MetaUnit(HullType.Tank, 0);
        GameObject newUnitObj = m_unitBuilder.M_BuildMetaUnit(newUnit, m_unitSpawnPosition.transform.position, m_unitSpawnPosition.transform.rotation);
    }
}
