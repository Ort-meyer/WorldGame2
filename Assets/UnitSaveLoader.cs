using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;


// TODO butcher this class and make it part of unit builder
public class UnitSaveLoader : MonoBehaviour
{
    private UnitBuilder m_unitBuilder;
    const string m_unitSaveFolder = @"SavedUnits\";
    const string m_fileFormat = ".unit";

    private void Start()
    {

    }

    //List<string> M_GetSavedUnitNames()
    //{
    //    m_unitBuilder = GetComponent<UnitBuilder>();
    //    string fullSaveDirectory = Directory.GetCurrentDirectory() + m_unitSaveFolder;
    //    return new List<string>(Directory.GetFiles(fullSaveDirectory));
    //}

    public void M_SaveUnitToFile(string unitName, MetaUnit metaUnit)
    {
        // See if unit name already exists, and if we should overwrite. Make check separate method?
        string fullFileName = m_unitSaveFolder + unitName + m_fileFormat;
        string jsonString = JsonConvert.SerializeObject(metaUnit);
        File.WriteAllText(fullFileName, jsonString);

    }

    public MetaUnit M_LoadFromFile(string unitName)
    {
        string fullFileName = m_unitSaveFolder + unitName + m_fileFormat;
        var fileStream = new FileStream(fullFileName, FileMode.Open, FileAccess.Read);
        using (var streamReader = new StreamReader(fileStream))
        {
            fullFileName = streamReader.ReadToEnd(); // Should I really reuse this string?
        }

        MetaUnit loadedMetaUnit = JsonConvert.DeserializeObject<MetaUnit>(fullFileName);
        return loadedMetaUnit;
    }

    //public void M_SaveUnitToFile(string unitName, UnitModule unitModuleToSave)
    //{
    //    SavedModule unitToSave =  SaveSubModules(unitModuleToSave);
    //    string jsonString = JsonUtility.ToJson(unitToSave);
    //    SaveToFile(unitName, jsonString);
    //}

    //public SavedModule M_LoadUnitFromFile(string unitName)
    //{
    //    string jsonString = LoadFromFile(m_unitSaveFolder + unitName + m_fileFormat);

    //    SavedModule unitToLoad = JsonUtility.FromJson<SavedModule>(jsonString);
    //    return unitToLoad;

    //    //////// This stuff should be in showroom manager, or buildroom
    //    //GameObject newUnit = m_unitBuilder.M_BuildUnit((ModuleType)Enum.Parse(typeof(ModuleType), unitToLoad.moduleType), m_spawnPosition);
    //    //Destroy(m_currentVehicle);
    //    //m_currentVehicle = newUnit;
    //    //m_currentHardpoint = null;
    //}

    //SavedModule SaveSubModules(UnitModule currentModule)
    //{
    //    SavedModule moduleToSave = new SavedModule();
    //    moduleToSave.moduleType = currentModule.m_moduleType.ToString();
    //    foreach (UnitSubModule module in currentModule.m_modules.Values)
    //    {
    //        SavedModule subModule = SaveSubModules(module);
    //        subModule.attachedToIndex = module.m_attachedToIndex;
    //        moduleToSave.modules.Add(subModule);
    //    }
    //    return moduleToSave;
    //}
}
