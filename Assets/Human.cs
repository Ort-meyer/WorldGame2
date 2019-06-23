using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{
    public Texture2D m_selectionTexture;
    public Texture2D m_selectionEdgeTexture;
    public float m_edgeThickness;

    private static Vector3 m_mouseDownPoint;

    private bool m_isDragging = false;

    private RaycastHit m_hit;

    private Player m_player;



    // Use this for initialization
    void Start()
    {
        m_player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        // Do raycast every frame, and let other methods use the results as they want
        DoRaycast();
        RightClick();
        LeftClick();
        Debug.DrawRay(m_hit.point, m_hit.normal);
    }

    // Simply does a raycast and stores the hit data in private member
    private void DoRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out m_hit);
    }

    private void RightClick()
    {
        // Right mouse button - move order (really bad idea to have it here)
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (m_hit.transform != null)
            {
                m_player.M_MoveSelectedUnits(m_hit.point);
                //// See if we clicked an enemy
                //BaseUnit unit = m_hit.transform.GetComponent<BaseUnit>();
                //if (unit)
                //{
                //    if (unit.m_alignment != m_player.m_alignment)
                //    {
                //        m_player.M_EngageWithSelectedUnits(new List<GameObject> { m_hit.transform.gameObject });
                //    }
                //}
                //else
                //{
                //    m_player.M_MoveSelectedUnits(m_hit.point);
                //}
            }
        }
    }

    private void LeftClick()
    {
        // Temporary to select just the one unit
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if(m_hit.transform.gameObject.GetComponent<Unit>())
                m_player.M_SelectUnits(new List<GameObject> { m_hit.transform.gameObject });
        }

        //if (Input.GetKeyDown(KeyCode.Mouse0))
        //{
        //    m_isDragging = true;
        //    m_mouseDownPoint = Input.mousePosition;
        //}

        //if (Input.GetKeyUp(KeyCode.Mouse0))
        //{
        //    if (!Input.GetKey(KeyCode.LeftShift))
        //    {
        //        m_player.M_ClearSelectedUnits();
        //    }
        //    List<GameObject> selected = new List<GameObject>();

        //    m_isDragging = false;
        //    // Check if selection was just a click (kinda ugly but should be stable)
        //    // Click (both where we started dragging and where we release are basically the same points)
        //    if ((Input.mousePosition - m_mouseDownPoint).magnitude < 4) // Value is virtually pixels in screenspace
        //    {
        //        // If we hit something, and if that is a player unit, select it 
        //        if (m_hit.transform != null)
        //        {
        //            // See if we select something new
        //            BaseUnit hitUnit = m_hit.transform.GetComponent<BaseUnit>();
        //            if (hitUnit)
        //            {
        //                if (hitUnit.m_alignment == m_player.m_alignment)
        //                {
        //                    selected.Add(m_hit.transform.gameObject);
        //                }
        //            }
        //        }
        //    }
        //    // Drag (selection box)
        //    else
        //    {
        //        // Group engage
        //        if (Input.GetKey(KeyCode.LeftControl))
        //        {
        //            Object[] objs = FindObjectsOfType(typeof(EnemyEntity));
        //            List<GameObject> targets = new List<GameObject>();
        //            for (int i = 0; i < objs.Length; i++)
        //            {
        //                GameObject obj = (objs[i] as EnemyEntity).gameObject;
        //                if (IsWithinSelectionBounds(obj))
        //                {
        //                    targets.Add(obj);
        //                }
        //            }
        //            m_player.M_EngageWithSelectedUnits(targets);
        //        }
        //        // Group select
        //        else
        //        {
        //            Object[] objs = FindObjectsOfType(typeof(BaseUnit));
        //            for (int i = 0; i < objs.Length; i++)
        //            {
        //                GameObject obj = (objs[i] as BaseUnit).gameObject;
        //                BaseUnit unit = obj.GetComponent<BaseUnit>();
        //                if (unit.m_alignment == m_player.m_alignment)
        //                {
        //                    if (IsWithinSelectionBounds(obj))
        //                    {
        //                        selected.Add(obj);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    m_player.M_SelectUnits(selected);
        //}
    }


    //private void OnGUI()
    //{
    //    if (m_isDragging)
    //    {
    //        Rect rect = GetScreenRect(m_mouseDownPoint, Input.mousePosition);
    //        DrawSelectionBox(rect);
    //    }
    //}

    //private Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    //{
    //    // Move origin from bottom left to top left
    //    screenPosition1.y = Screen.height - screenPosition1.y;
    //    screenPosition2.y = Screen.height - screenPosition2.y;
    //    // Calculate corners
    //    var topLeft = Vector3.Min(screenPosition1, screenPosition2);
    //    var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
    //    // Create Rect
    //    return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    //}

    //private void DrawSelectionBox(Rect rect)
    //{
    //    // Draw the inner box
    //    GUI.DrawTexture(rect, m_selectionTexture);
    //    // Draw the edges
    //    GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, m_edgeThickness), m_selectionEdgeTexture);
    //    // Left
    //    GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, m_edgeThickness, rect.height), m_selectionEdgeTexture);
    //    // Right
    //    GUI.DrawTexture(new Rect(rect.xMax - m_edgeThickness, rect.yMin, m_edgeThickness, rect.height), m_selectionEdgeTexture);
    //    // Bottom
    //    GUI.DrawTexture(new Rect(rect.xMin, rect.yMax - m_edgeThickness, rect.width, m_edgeThickness), m_selectionEdgeTexture);
    //}

    //private Bounds GetViewportBounds(Vector3 screenPosition1, Vector3 screenPosition2)
    //{
    //    var v1 = Camera.main.ScreenToViewportPoint(screenPosition1);
    //    var v2 = Camera.main.ScreenToViewportPoint(screenPosition2);
    //    var min = Vector3.Min(v1, v2);
    //    var max = Vector3.Max(v1, v2);
    //    min.z = Camera.main.nearClipPlane;
    //    max.z = Camera.main.farClipPlane;

    //    var bounds = new Bounds();
    //    bounds.SetMinMax(min, max);
    //    return bounds;
    //}

    //public bool IsWithinSelectionBounds(GameObject gameObject)
    //{
    //    var viewportBounds = GetViewportBounds(m_mouseDownPoint, Input.mousePosition);
    //    return viewportBounds.Contains(Camera.main.WorldToViewportPoint(gameObject.transform.position));
    //}
}
