using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Distance between each unit in a formation. TODO have this be dynamic somehow
    public int m_formationSpread;
    public int m_faction;
    public Dictionary<int, GameObject> m_selectedUnits = new Dictionary<int, GameObject>();
    // Dictionary of every unit owned by this player
    public Dictionary<int, GameObject> m_ownedUnits = new Dictionary<int, GameObject>();
    // Use this for initialization
    void Start()
    {
        // Hack to get all debug units into the list of owned units for AI TODO clean this up
        if (m_faction == 0)
        {
            Object[] objs = FindObjectsOfType<Unit>();
            for (int i = 0; i < objs.Length; i++)
            {
                Unit unit = (objs[i] as Unit);
                if (unit.m_faction == m_faction)
                {
                    m_ownedUnits.Add(unit.gameObject.GetInstanceID(), unit.gameObject);
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void M_SelectUnits(List<GameObject> units)
    {
        foreach (GameObject unit in units)
        {
            int objId = unit.GetInstanceID();
            // Avoid adding the same unit twice
            if (m_selectedUnits.ContainsKey(objId))
            {
                continue;
            }
            m_selectedUnits.Add(unit.GetInstanceID(), unit);
            //// If this is the human, also draw selection circle (not sure if this is the right place...)
            //if (GetComponent<Human>())
            //{
            //    DrawSelected(unit, true);
            //}
        }
    }

    public void M_ClearSelectedUnits()
    {
        m_selectedUnits.Clear();
    }

    public void M_MoveSelectedUnits(Vector3 destination)
    {
        int squareSize = (int)System.Math.Ceiling(System.Math.Sqrt(m_selectedUnits.Count));
        int unitIndex = 0;

        foreach (KeyValuePair<int, GameObject> pair in m_selectedUnits)
        {
            if (pair.Value == null)
                continue;
            GameObject obj = pair.Value;

            // Place the units in a square centered on the destination point.
            int row = unitIndex % squareSize;
            int col = unitIndex / squareSize;
            float rowOffset = ((float)row - squareSize / 2) * 5.0f;  // TODO: Should be scaled according to unit size
            float colOffset = ((float)col - squareSize / 2) * 5.0f;  // TODO: Should be scaled according to unit size

            Vector3 unitDestination = destination;
            unitDestination.x += rowOffset;
            unitDestination.z += colOffset;

            obj.GetComponent<BaseMovement>().M_MoveTo(unitDestination);
            unitIndex++;
        }
    }

    public void M_EngageWithSelectedUnits(List<GameObject> targets)
    {
        foreach (GameObject obj in m_selectedUnits.Values)
        {
            if (obj == null) // Cannot assume that dictionary is clean TODO clean it up somewhere? 
                continue;

            Unit thisUnit = obj.GetComponent<Unit>(); // This shouldn't be necessary. Can I ever select something that's not a unit?
            if (thisUnit)
            {
                thisUnit.M_AttackOrder(targets);
            }
        }
    }

    public void M_EngageWithSelectedUnits(GameObject target)
    {
        M_EngageWithSelectedUnits(new List<GameObject> { target });
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
