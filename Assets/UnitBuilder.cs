using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HullType { Tank, Buggy };
public enum TurretType { Rotating, Traverse };
public enum WeaponType { MachineGun, Cannon };

[System.Serializable]
public struct HullTypePrefab
{
    public HullType hullType;
    public GameObject prefab;
}

[System.Serializable]
public struct TurretTypePrefab
{
    public TurretType turretType;
    public GameObject prefab;
}

[System.Serializable]
public struct WeaponTypePrefab
{
    public WeaponType weaponType;
    public GameObject prefab;
}

[System.Serializable]
public class MetaUnit
{
    public HullType m_hullType;
    public Dictionary<int, MetaTurret> m_turrets = new Dictionary<int, MetaTurret>();
    public MetaUnit(HullType hulltype)
    {
        m_hullType = hulltype;
    }
}

[System.Serializable]
public class MetaTurret
{
    public TurretType m_turretType;
    public Dictionary<int, MetaTurret> m_turrets = new Dictionary<int, MetaTurret>();
    public Dictionary<int, MetaWeapon> m_weapons = new Dictionary<int, MetaWeapon>();
    public MetaTurret(TurretType turretType)
    {
        m_turretType = turretType;
    }
}

[System.Serializable]
public class MetaWeapon
{
    public WeaponType m_weaponType;
    public MetaWeapon(WeaponType weaponType)
    {
        m_weaponType = weaponType;
    }
}

public class UnitBuilder : MonoBehaviour
{
    public HullTypePrefab[] hullPrefabsArray;
    public TurretTypePrefab[] turretPrefabsArray;
    public WeaponTypePrefab[] weaponPrefabsArray;

    public Dictionary<HullType, GameObject> m_hullPrefabs = new Dictionary<HullType, GameObject>();
    public Dictionary<TurretType, GameObject> m_turrentPrefabs = new Dictionary<TurretType, GameObject>();
    public Dictionary<WeaponType, GameObject> m_weaponPrefabs = new Dictionary<WeaponType, GameObject>();

    public GameObject m_spawnPosition;

    // Use this for initialization
    void Start()
    {
        // Convert arrays to dictionaries
        foreach(HullTypePrefab kvp in hullPrefabsArray)
        {
            m_hullPrefabs.Add(kvp.hullType, kvp.prefab);
        }
        foreach (TurretTypePrefab kvp in turretPrefabsArray)
        {
            m_turrentPrefabs.Add(kvp.turretType, kvp.prefab);
        }
        foreach (WeaponTypePrefab kvp in weaponPrefabsArray)
        {
            m_weaponPrefabs.Add(kvp.weaponType, kvp.prefab);
        }

        // Create a full meta unit here for development purposes
        MetaWeapon machineGun = new MetaWeapon(WeaponType.Cannon);

        MetaTurret machineGunMount = new MetaTurret(TurretType.Traverse);
        machineGunMount.m_weapons.Add(0, machineGun);

        MetaTurret turret = new MetaTurret(TurretType.Rotating);
        turret.m_turrets.Add(0, machineGunMount);

        MetaUnit devUnit = new MetaUnit(HullType.Buggy);
        devUnit.m_turrets.Add(0, turret);

        UnitSaveLoader saveLoadHandler = FindObjectOfType<UnitSaveLoader>();
        saveLoadHandler.M_SaveUnitToFile("buggy", devUnit);

        MetaUnit loadedUnit = saveLoadHandler.M_LoadFromFile("buggy");
        //M_BuildMetaUnit(devUnit, m_spawnPosition.transform.position, m_spawnPosition.transform.rotation);
        M_BuildMetaUnit(loadedUnit, m_spawnPosition.transform.position, m_spawnPosition.transform.rotation);


    }

    // Update is called once per frame
    void Update()
    {

    }
        
    public GameObject M_BuildMetaUnit(MetaUnit metaUnit, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        GameObject newUnitObj = Instantiate(m_hullPrefabs[metaUnit.m_hullType]);
        newUnitObj.transform.position = spawnPosition;
        newUnitObj.transform.rotation = spawnRotation;
        M_BuildTurrets(metaUnit.m_turrets, newUnitObj);
        newUnitObj.GetComponent<Unit>().M_Init();

        return newUnitObj;
    }

    private void M_BuildTurrets(Dictionary<int, MetaTurret> turrets, GameObject parentObj)
    {
        foreach (var kvp in turrets)
        {
            int hardpointIndex = kvp.Key;
            MetaTurret turret = kvp.Value;
            Transform hardpointTransform = parentObj.GetComponent<Hardpoints>().m_hardPoints[hardpointIndex].transform;
            Vector3 turretLocalPosition = hardpointTransform.localPosition;
            Quaternion turretLocalRotation = hardpointTransform.localRotation;
            M_BuildTurret(turret, parentObj, turretLocalPosition, turretLocalRotation);
        }
    }

    private GameObject M_BuildTurret(MetaTurret metaTurret, GameObject parentObj, Vector3 localPosition, Quaternion localRotation)
    {
        GameObject newTurretObj = Instantiate(m_turrentPrefabs[metaTurret.m_turretType],parentObj.transform);
        BaseTurret turret = newTurretObj.GetComponent<BaseTurret>();
        newTurretObj.transform.localPosition = localPosition;
        newTurretObj.transform.localRotation = localRotation;
        M_BuildTurrets(metaTurret.m_turrets, newTurretObj);
        M_BuildWeapons(metaTurret.m_weapons, newTurretObj);
        return newTurretObj; // Do we need this?
    }

    private void M_BuildWeapons(Dictionary<int, MetaWeapon> weapons, GameObject parentObj)
    {
        foreach (var kvp in weapons)
        {
            int hardpointIndex = kvp.Key;
            MetaWeapon weapon = kvp.Value;
            Transform hardpointTransform = parentObj.GetComponent<Hardpoints>().m_hardPoints[hardpointIndex].transform;
            Vector3 turretLocalPosition = hardpointTransform.localPosition;
            Quaternion turretLocalRotation = hardpointTransform.localRotation;
            M_BuildWeapon(weapon, parentObj, turretLocalPosition, turretLocalRotation);
        }
    }

    private GameObject M_BuildWeapon(MetaWeapon metaWeapon, GameObject parentObj, Vector3 localPosition, Quaternion localRotation)
    {
        GameObject newWeaponObj = Instantiate(m_weaponPrefabs[metaWeapon.m_weaponType], parentObj.transform);
        newWeaponObj.transform.localPosition = localPosition;
        newWeaponObj.transform.localRotation = localRotation;
        newWeaponObj.GetComponent<BaseWeapon>().m_parentTurretObj = parentObj;
        parentObj.GetComponent<BaseTurret>().m_weapons.Add(newWeaponObj);
        return newWeaponObj; // Do we need this?
    }


}
