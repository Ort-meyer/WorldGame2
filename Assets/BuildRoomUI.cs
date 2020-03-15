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

    // Use this for initialization
    void Start()
    {
        Dropdown componentDropdown = m_componentDropdownObj.GetComponent<Dropdown>();
        componentDropdown.options.Clear();
        componentDropdown.onValueChanged.AddListener(delegate { M_ComponentDropdownValueChange(); });

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
        componentDropdown.options.Add(new Dropdown.OptionData("something cool"));
        componentDropdown.options.Add(new Dropdown.OptionData("something cool2"));
        componentDropdown.options.Add(new Dropdown.OptionData("something cool5"));

    }

    void M_ComponentDropdownValueChange()
    {
        Debug.Log("derp");
    }
}
