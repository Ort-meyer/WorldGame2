using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class WorldManager : MonoBehaviour
{
    // Dictionary of all players. Int is faction id
    public Dictionary<int, Player> m_players = new Dictionary<int, Player>();

    const string m_worldSaveFolder = @"SavedWorlds\";
    const string m_fileFormat = ".world";

    UnitBuilder m_unitBuilder;

    class SaveUnitData
    {
        public MetaUnit m_metaUnit;
        public Vector3 m_position;
        public Quaternion m_rotation;
        public SaveUnitData(MetaUnit metaUnit, Vector3 position, Quaternion rotation)
        {
            m_metaUnit = metaUnit;
            m_position = position;
            m_rotation = rotation;
        }
    }


    // Use this for initialization
    void Start()
    {

    }

    void Awake()
    {
        m_unitBuilder = FindObjectOfType<UnitBuilder>();
        // TODO create players somehow. This relies on players already existing in the scene
        Player[] players = FindObjectsOfType<Player>();
        foreach (Player player in players)
        {
            m_players.Add(player.m_faction, player);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.N))
        {
            M_SaveWorldToFile("testworld");
        }

        if (Input.GetKeyUp(KeyCode.B))
        {
            List<SaveUnitData> allUnitsInWorld = M_LoadWorldFromFile("testworld");
            foreach(var saveData in allUnitsInWorld)
            {
                m_unitBuilder.M_BuildMetaUnit(saveData.m_metaUnit, saveData.m_position, saveData.m_rotation);
            }
        }
    }


    private void M_SaveWorldToFile(string worldName)
    {
        // TODO rewrite with respect to convoys (shouldn't be too hard). Also needs to not write entire meta units
        //string fullFileName = m_worldSaveFolder + worldName + m_fileFormat;
        //// Create list of every unit in the game and their transforms (positions and rotations)      
        //List<SaveUnitData> allUnitsInWorld = new List<SaveUnitData>();
        //foreach (var playersKvp in m_players)
        //{
        //    int faction = playersKvp.Key;
        //    Player player = playersKvp.Value;
        //    foreach (var unitsKvp in player.m_ownedUnits)
        //    {
        //        int instanceId = unitsKvp.Key;
        //        GameObject unitObj = unitsKvp.Value;
        //        Unit unit = unitObj.GetComponent<Unit>();
        //        allUnitsInWorld.Add(new SaveUnitData(unit.m_metaUnit, unitObj.transform.position, unitObj.transform.rotation));
        //    }
        //}

        //string jsonString = JsonConvert.SerializeObject(allUnitsInWorld);
        //File.WriteAllText(fullFileName, jsonString);

    }

    private List<SaveUnitData> M_LoadWorldFromFile(string unitName)
    {
        string fullFileName = m_worldSaveFolder + unitName + m_fileFormat;
        var fileStream = new FileStream(fullFileName, FileMode.Open, FileAccess.Read);
        using (var streamReader = new StreamReader(fileStream))
        {
            fullFileName = streamReader.ReadToEnd(); // Should I really reuse this string?
        }

        List<SaveUnitData> allUnitsInWorld = JsonConvert.DeserializeObject<List<SaveUnitData>>(fullFileName);
        return allUnitsInWorld;
    }

    private void M_FormConvoy(GameObject newUnitObj, int faction)
    {
        Convoy newUnitConvoy = m_unitBuilder.M_BuildNewConvoy(faction);
        m_players[faction].m_ownedConvoys.Add(newUnitConvoy.GetInstanceID(), newUnitConvoy);
        Unit newUnit = newUnitObj.GetComponent<Unit>();
        newUnitConvoy.m_units.Add(newUnit);
        newUnit.M_Activate(newUnitConvoy);
    }
}
