using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Human : MonoBehaviour
{
    public Texture2D m_selectionTexture;
    public Texture2D m_selectionEdgeTexture;
    public float m_edgeThickness;

    private static Vector3 m_mouseDownPoint;

    private bool m_isDragging = false;

    private RaycastHit[] m_hits;

    private Player m_player;

    private WorldManager m_worldManager;


    // Use this for initialization
    void Start()
    {
        m_player = GetComponent<Player>();
        m_worldManager = FindObjectOfType<WorldManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Do raycast every frame, and let other methods use the results as they want
        DoRaycast();
        RightClick();
        LeftClick();

        // Form convoy
        if(Input.GetKeyDown(KeyCode.Q))
        {
            m_player.M_FormConvoy();
        }
    }

    // Simply does a raycast and stores the hit data in private member
    private void DoRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        m_hits = Physics.RaycastAll(ray);
    }

    private void RightClick()
    {
        // Right mouse button - move order (really bad idea to have it here)
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Dictionary<int, Convoy> targetsToEngage = new Dictionary<int, Convoy>();
            foreach (RaycastHit hit in m_hits)
            {
                if (hit.transform != null)
                {
                    Unit hitUnit = hit.transform.GetComponent<Unit>();
                    if (hitUnit)
                    {
                        // If we hit a unit that't not our own
                        if (hitUnit.m_convoy.m_faction != m_player.m_faction)
                        {
                            m_player.M_EngageWithSelectedConvoys(hitUnit.m_convoy); // This feels risky
                            break; ; // Probably a bad way to only avoid multiple commands
                        }
                    }
                    // Move if this hit was on a terrain
                    if (hit.transform.GetComponent<Terrain>())
                    {
                        m_player.M_MoveSelectedConvoys(hit.point);
                    }
                }
            }
        }
    }

    private void LeftClick()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            m_isDragging = true;
            m_mouseDownPoint = Input.mousePosition;
        }
        // Temporary to select just the one unit
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Dictionary<int, Convoy> selected = new Dictionary<int, Convoy>();
            m_isDragging = false;
            // Check if selection was just a click (kinda ugly but should be stable)
            // Click (both where we started dragging and where we release are basically the same points)
            if ((Input.mousePosition - m_mouseDownPoint).magnitude < 4) // Value is virtually pixels in screenspace
            {
                m_player.M_ClearSelectedConvoys();
                // If we hit something, and if that is a player unit, select it 
                if (m_hits.Length > 0)
                {
                    foreach (RaycastHit hit in m_hits)
                    {
                        // See if we select something new
                        Unit hitUnit = hit.transform.GetComponent<Unit>();
                        if (hitUnit)
                        {
                            if (hitUnit.m_convoy.m_faction == m_player.m_faction)
                            {
                                selected[hitUnit.m_convoy.GetInstanceID()] = hitUnit.m_convoy;
                                //selected.Add(hit.transform.gameObject);
                                if (!Input.GetKey(KeyCode.LeftShift))
                                {
                                    m_player.M_ClearSelectedConvoys();
                                }
                                m_player.M_SelectConvoys(selected.Values.ToList());
                                break; // TODO I think that the list is sorted by distance. If not, ensure we only take the closes unit
                            }
                        }
                    }
                }
            }
            // Drag (selection box)
            else
            {
                // Group engage
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    List<Convoy> targets = new List<Convoy>();
                    // Engage all other players' units (TODO change when there's allied players)
                    foreach (Player player in m_worldManager.m_players.Values)
                    {
                        // Only hit things that aren't our own
                        if(player.m_faction != m_player.m_faction)
                        {
                            targets.AddRange(GetConvoysInSelectionBox(player.m_ownedConvoys.Values.ToList()));
                        }
                    }
                    m_player.M_EngageWithSelectedConvoys(targets);
                }
                // Group select
                else
                {
                    
                    //foreach (GameObject obj in m_worldManager.m_players[m_player.m_faction].m_ownedUnits.Values)
                    //{
                    //    if (IsWithinSelectionBounds(obj))
                    //    {
                    //        selected.Add(obj);
                    //    }
                    //}
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        m_player.M_ClearSelectedConvoys();
                    }
                    m_player.M_SelectConvoys(GetConvoysInSelectionBox(m_player.m_ownedConvoys.Values.ToList()));
                }
            }
        }
    }


    private void OnGUI()
    {
        if (m_isDragging)
        {
            Rect rect = GetScreenRect(m_mouseDownPoint, Input.mousePosition);
            DrawSelectionBox(rect);
        }
    }

    private Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        // Move origin from bottom left to top left
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        // Calculate corners
        var topLeft = Vector3.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
        // Create Rect
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    private void DrawSelectionBox(Rect rect)
    {
        // Draw the inner box
        GUI.DrawTexture(rect, m_selectionTexture);
        // Draw the edges
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, m_edgeThickness), m_selectionEdgeTexture);
        // Left
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, m_edgeThickness, rect.height), m_selectionEdgeTexture);
        // Right
        GUI.DrawTexture(new Rect(rect.xMax - m_edgeThickness, rect.yMin, m_edgeThickness, rect.height), m_selectionEdgeTexture);
        // Bottom
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMax - m_edgeThickness, rect.width, m_edgeThickness), m_selectionEdgeTexture);
    }

    private Bounds GetViewportBounds(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        var v1 = Camera.main.ScreenToViewportPoint(screenPosition1);
        var v2 = Camera.main.ScreenToViewportPoint(screenPosition2);
        var min = Vector3.Min(v1, v2);
        var max = Vector3.Max(v1, v2);
        min.z = Camera.main.nearClipPlane;
        max.z = Camera.main.farClipPlane;

        var bounds = new Bounds();
        bounds.SetMinMax(min, max);
        return bounds;
    }
    
    public List<Convoy> GetConvoysInSelectionBox(List<Convoy> convoys)
    {
        List<Convoy> convoysInBox = new List<Convoy>();
        foreach(Convoy convoy in convoys)
        {
            foreach(Unit unit in convoy.m_units)
            {
                if(IsWithinSelectionBounds(unit.gameObject))
                {
                    convoysInBox.Add(convoy);
                    break; // This takes me one level up, right?
                }
            }
        }
        return convoysInBox;
    }

    public bool IsWithinSelectionBounds(GameObject gameObject)
    {
        var viewportBounds = GetViewportBounds(m_mouseDownPoint, Input.mousePosition);
        return viewportBounds.Contains(Camera.main.WorldToViewportPoint(gameObject.transform.position));
    }


}
