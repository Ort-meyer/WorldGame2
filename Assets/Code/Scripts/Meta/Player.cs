using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Distance between each unit in a formation. TODO have this be dynamic somehow
    public int m_formationSpread;
    public int m_faction;
    public Dictionary<int, Convoy> m_selectedConvoys = new Dictionary<int, Convoy>();
    // Dictionary of every convoy owned by this player (Is this risky? Cant we lose units?)
    public Dictionary<int, Convoy> m_ownedConvoys = new Dictionary<int, Convoy>();
    // Use this for initialization
    void Start()
    {
        // Hack to get all debug units into the list of owned units for AI TODO clean this up
        if (m_faction == 0)
        {
            Object[] objs = FindObjectsOfType<Convoy>();
            for (int i = 0; i < objs.Length; i++)
            {
                Convoy convoy = (objs[i] as Convoy);
                if (convoy.m_faction == m_faction)
                {
                    m_ownedConvoys.Add(convoy.gameObject.GetInstanceID(), convoy);
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void M_SelectConvoys(List<Convoy> convoys)
    {
        foreach (Convoy convoy in convoys)
        {
            int objId = convoy.GetInstanceID();
            // Avoid adding the same unit twice
            if (m_selectedConvoys.ContainsKey(objId))
            {
                continue;
            }
            m_selectedConvoys.Add(convoy.GetInstanceID(), convoy);
            //// If this is the human, also draw selection circle (not sure if this is the right place...)
            //if (GetComponent<Human>())
            //{
            //    DrawSelected(unit, true);
            //}
        }
    }

    public void M_ClearSelectedConvoys()
    {
        m_selectedConvoys.Clear();
    }

    public void M_MoveSelectedConvoys(Vector3 destination)
    {
        foreach(Convoy convoy in m_selectedConvoys.Values)
        {
            convoy.M_MoveTo(destination);
        }

        //int squareSize = (int)System.Math.Ceiling(System.Math.Sqrt(m_selectedConvoys.Count));
        //int unitIndex = 0;

        //foreach (KeyValuePair<int, Convoy> pair in m_selectedConvoys)
        //{
        //    if (pair.Value == null)
        //        continue;
        //    GameObject obj = pair.Value.gameObject;

        //    // Place the units in a square centered on the destination point.
        //    int row = unitIndex % squareSize;
        //    int col = unitIndex / squareSize;
        //    float rowOffset = ((float)row - squareSize / 2) * 5.0f;  // TODO: Should be scaled according to unit size
        //    float colOffset = ((float)col - squareSize / 2) * 5.0f;  // TODO: Should be scaled according to unit size

        //    Vector3 unitDestination = destination;
        //    unitDestination.x += rowOffset;
        //    unitDestination.z += colOffset;

        //    obj.GetComponent<BaseMovement>().M_MoveTo(unitDestination);
        //    unitIndex++;
        //}
    }

    public void M_EngageWithSelectedConvoys(List<GameObject> targets)
    {
        foreach (Convoy convoy in m_selectedConvoys.Values)
        {
            if (convoy == null) // Cannot assume that dictionary is clean TODO clean it up somewhere? 
                continue;
            if (convoy)
            {
                convoy.M_AttackOrder(targets);
            }
        }
    }

    public void M_EngageWithSelectedConvoys(List<Convoy> targets)
    {
        foreach (Convoy convoy in m_selectedConvoys.Values)
        {
            if (convoy == null) // Cannot assume that dictionary is clean TODO clean it up somewhere? 
                continue;
            if (convoy)
            {
                convoy.M_AttackOrder(targets);
            }
        }
    }

    public void M_EngageWithSelectedConvoys(Convoy target)
    {
        M_EngageWithSelectedConvoys(new List<Convoy> { target });
    }

    public void M_FormConvoy()
    {
        GameObject newConvoyObj = new GameObject();
        Convoy newConvoy = newConvoyObj.AddComponent<Convoy>();
        newConvoy.m_faction = m_faction;

        foreach(Convoy convoy in m_selectedConvoys.Values)
        {
            newConvoy.m_units.AddRange(convoy.m_units);
            m_ownedConvoys.Remove(convoy.GetInstanceID());
            Destroy(convoy.gameObject);
        }
        m_ownedConvoys.Add(newConvoy.GetInstanceID(), newConvoy);
        m_selectedConvoys.Clear();
        m_selectedConvoys.Add(newConvoy.GetInstanceID(), newConvoy);
        foreach(Unit unit in newConvoy.m_units)
        {
            unit.m_convoy = newConvoy;
        }

        //// Add all selected units together to the convoy
        //List<Unit> selectedUnits = new List<Unit>();
        //foreach (GameObject unitObj in m_selectedConvoys.Values)
        //{
        //    Unit unit = unitObj.GetComponent<Unit>();
        //    //// If a convoy is selected, merge it into a large convoy
        //    //Convoy convoy = unitObj.GetComponent<Convoy>();
        //    //if (convoy)
        //    //{
        //    //    foreach (Unit convoyUnit in convoy.m_units)
        //    //    {
        //    //        selectedUnits.Add(convoyUnit);
        //    //    }
        //    //    Destroy(convoy.gameObject);
        //    //}
        //    //Unit unit = controllable.GetComponent<Unit>();
        //    //if (unit)
        //    //{
        //    //    selectedUnits.Add(unit);
        //    //}
        //}
        //M_ClearSelectedUnits();

        //newConvoy.M_InitConvoy(selectedUnits);
    }

    //private void DrawSelected(GameObject obj, bool selected)
    //{
    //    if (selected)
    //    {
    //        obj.GetComponent<PlayerControlledEntity>().Select();
    //    }
    //    else
    //    {
    //        obj.GetComponent<PlayerControlledEntity>().DeSelect();
    //    }
    //}
}
