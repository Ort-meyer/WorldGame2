using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HullType { Tank, Buggy };
public enum TurretType { Rotating, Traverse, TankTurret, Double};
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

public class MetaUnit
{
    public HullType m_hullType;
    public int m_faction; // This is also tracked in the unit component. That feels redundant
    public Dictionary<int, MetaTurret> m_turrets = new Dictionary<int, MetaTurret>();
    public MetaUnit(HullType hulltype, int faction)
    {
        m_hullType = hulltype;
        m_faction = faction;
    }
}

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
    // The prefab for each convoy. Should probably not be in this class though
    public GameObject m_convoyPrefab;

    public HullTypePrefab[] hullPrefabsArray;
    public TurretTypePrefab[] turretPrefabsArray;
    public WeaponTypePrefab[] weaponPrefabsArray;

    public Dictionary<HullType, GameObject> m_hullPrefabs = new Dictionary<HullType, GameObject>();
    public Dictionary<TurretType, GameObject> m_turrentPrefabs = new Dictionary<TurretType, GameObject>();
    public Dictionary<WeaponType, GameObject> m_weaponPrefabs = new Dictionary<WeaponType, GameObject>();

    public GameObject m_spawnPosition;
    public GameObject m_enemyConvoySpawnPosition;

    // TODO just move all this singleton meta thingies to the same game object?
    WorldManager m_worldManager;

    UnitSaveLoader m_saveLoadHandler;

    private void Awake()
    {
        m_worldManager = FindObjectOfType<WorldManager>();
        m_saveLoadHandler = FindObjectOfType<UnitSaveLoader>();
    }

    private void Start()
    {
        // Convert arrays to dictionaries
        foreach (HullTypePrefab kvp in hullPrefabsArray)
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

        // Build some units
        //BuildBuggy();
        //BuildTank(new Vector3(0,0,0));
        //BuildTank(new Vector3(4,0,0));
        
        //BuildEnemyTank(new Vector3(0,0,0));
        //BuildEnemyTank(new Vector3(4,0,0));
        //BuildEnemyTank(new Vector3(-4, 0, 0));
        //BuildDoubleTank();

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
        Unit newUnit = newUnitObj.GetComponent<Unit>();
        List<BaseTurret> builtTurrets = M_BuildTurrets(metaUnit.m_turrets, newUnit.m_gfxObject);
        newUnit.M_Init(metaUnit, builtTurrets);
    
        return newUnitObj;
    }

    private List<BaseTurret> M_BuildTurrets(Dictionary<int, MetaTurret> turrets, GameObject parentObj)
    {
        List<BaseTurret> builtTurrets = new List<BaseTurret>();
        foreach (var kvp in turrets)
        {
            int hardpointIndex = kvp.Key;
            MetaTurret turret = kvp.Value;
            Transform hardpointTransform = parentObj.GetComponent<Hardpoints>().m_hardPoints[hardpointIndex].transform;
            Vector3 turretLocalPosition = hardpointTransform.localPosition;
            Quaternion turretLocalRotation = hardpointTransform.localRotation;
            GameObject newTurretObj = M_BuildTurret(turret, parentObj, turretLocalPosition, turretLocalRotation);
            builtTurrets.Add(newTurretObj.GetComponent<BaseTurret>());
        }
        return builtTurrets;
    }

    private GameObject M_BuildTurret(MetaTurret metaTurret, GameObject parentObj, Vector3 localPosition, Quaternion localRotation)
    {
        GameObject newTurretObj = Instantiate(m_turrentPrefabs[metaTurret.m_turretType],parentObj.transform);
        BaseTurret turret = newTurretObj.GetComponent<BaseTurret>();
        Transform attachToTransform = newTurretObj.GetComponent<Hardpoints>().m_attachesToHardpoint;
        newTurretObj.transform.localPosition = localPosition - attachToTransform.localPosition;
        newTurretObj.transform.localRotation = localRotation;
        turret.m_turrets = M_BuildTurrets(metaTurret.m_turrets, newTurretObj);
        turret.m_weapons = M_BuildWeapons(metaTurret.m_weapons, newTurretObj);
        return newTurretObj; // Do we need this?
    }

    private List<BaseWeapon> M_BuildWeapons(Dictionary<int, MetaWeapon> weapons, GameObject parentObj)
    {
        List<BaseWeapon> builtWeapons = new List<BaseWeapon>();
        foreach (var kvp in weapons)
        {
            int hardpointIndex = kvp.Key;
            MetaWeapon weapon = kvp.Value;
            Transform hardpointTransform = parentObj.GetComponent<Hardpoints>().m_hardPoints[hardpointIndex].transform;
            Vector3 turretLocalPosition = hardpointTransform.localPosition;
            Quaternion turretLocalRotation = hardpointTransform.localRotation;
            GameObject newWeaponObj = M_BuildWeapon(weapon, parentObj, turretLocalPosition, turretLocalRotation);
            builtWeapons.Add(newWeaponObj.GetComponent<BaseWeapon>());
        }
        return builtWeapons;
    }

    private GameObject M_BuildWeapon(MetaWeapon metaWeapon, GameObject parentObj, Vector3 localPosition, Quaternion localRotation)
    {
        GameObject newWeaponObj = Instantiate(m_weaponPrefabs[metaWeapon.m_weaponType], parentObj.transform);
        Transform attachToTransform = newWeaponObj.GetComponent<Hardpoints>().m_attachesToHardpoint;
        newWeaponObj.transform.localPosition = localPosition - attachToTransform.localPosition;
        newWeaponObj.transform.localRotation = localRotation;
        newWeaponObj.GetComponent<BaseWeapon>().m_parentTurretObj = parentObj;
        return newWeaponObj; // Do we need this?
    }

    public Convoy M_BuildNewConvoy(int faction)
    {
        GameObject newConvoyObj = Instantiate(m_convoyPrefab);
        Convoy newConvoy = newConvoyObj.GetComponent<Convoy>();
        Debug.Assert(newConvoy);
        newConvoy.m_faction = faction;
        return newConvoy;
    }

    public void BuildBuggy()
    {
        ///////////////// Define and build buggy///////////////
        // Create a full meta unit here for development purposes
        MetaWeapon machineGun = new MetaWeapon(WeaponType.MachineGun);

        MetaTurret machineGunMount = new MetaTurret(TurretType.Traverse);
        machineGunMount.m_weapons.Add(0, machineGun);

        MetaTurret buggyTurret = new MetaTurret(TurretType.Rotating);
        buggyTurret.m_turrets.Add(0, machineGunMount);

        MetaUnit buggyUnit = new MetaUnit(HullType.Buggy, 1);
        buggyUnit.m_turrets.Add(0, buggyTurret);

        m_saveLoadHandler.M_SaveUnitToFile("buggy", buggyUnit);

        MetaUnit loadedBuggy = m_saveLoadHandler.M_LoadFromFile("buggy");
        M_BuildMetaUnit(loadedBuggy, m_spawnPosition.transform.position, m_spawnPosition.transform.rotation);
    }
    public void BuildTank(Vector3 offset)
    {
        UnitSaveLoader saveLoadHandler = FindObjectOfType<UnitSaveLoader>();
        // Create a full meta unit here for development purposes
        MetaWeapon mainGun = new MetaWeapon(WeaponType.Cannon);
        MetaTurret mainGunMount = new MetaTurret(TurretType.Traverse);
        mainGunMount.m_weapons.Add(0, mainGun);

        MetaWeapon auxGun = new MetaWeapon(WeaponType.MachineGun);
        MetaTurret auxGunMount = new MetaTurret(TurretType.Traverse);
        auxGunMount.m_weapons.Add(0, auxGun);

        MetaTurret tankTurret = new MetaTurret(TurretType.TankTurret);
        tankTurret.m_turrets.Add(0, mainGunMount);
        tankTurret.m_turrets.Add(1, auxGunMount);

        MetaUnit tankUnit = new MetaUnit(HullType.Tank, 1);
        tankUnit.m_turrets.Add(0, tankTurret);

        saveLoadHandler.M_SaveUnitToFile("tank", tankUnit);

        MetaUnit loadedTank = saveLoadHandler.M_LoadFromFile("tank");
        M_BuildMetaUnit(loadedTank, m_spawnPosition.transform.position + offset, m_spawnPosition.transform.rotation);
    }
    public void BuildDoubleTank()
    {
        UnitSaveLoader saveLoadHandler = FindObjectOfType<UnitSaveLoader>();
        /////////////// Define and build double cannon tank
        MetaWeapon rightGun = new MetaWeapon(WeaponType.Cannon);
        MetaTurret rightGunMount = new MetaTurret(TurretType.Traverse);
        rightGunMount.m_weapons.Add(0, rightGun);

        MetaWeapon leftGun = new MetaWeapon(WeaponType.Cannon);
        MetaTurret leftGunMount = new MetaTurret(TurretType.Traverse);
        leftGunMount.m_weapons.Add(0, leftGun);

        MetaTurret doubleGunMount = new MetaTurret(TurretType.Double);
        doubleGunMount.m_turrets.Add(0, rightGunMount);
        doubleGunMount.m_turrets.Add(1, leftGunMount);

        MetaTurret doubleTankTurret = new MetaTurret(TurretType.TankTurret);
        doubleTankTurret.m_turrets.Add(0, doubleGunMount);

        MetaUnit doubleTankUnit = new MetaUnit(HullType.Tank, 1);
        doubleTankUnit.m_turrets.Add(0, doubleTankTurret);

        saveLoadHandler.M_SaveUnitToFile("doubletank", doubleTankUnit);
        MetaUnit loadedDoubleTank = saveLoadHandler.M_LoadFromFile("doubletank");
        M_BuildMetaUnit(loadedDoubleTank, m_spawnPosition.transform.position + new Vector3(-6, 0, 0), m_spawnPosition.transform.rotation);


    }
    public void BuildEnemyTank(Vector3 offset)
    {
        UnitSaveLoader saveLoadHandler = FindObjectOfType<UnitSaveLoader>();
        MetaWeapon rightGun = new MetaWeapon(WeaponType.Cannon);
        MetaTurret rightGunMount = new MetaTurret(TurretType.Traverse);
        rightGunMount.m_weapons.Add(0, rightGun);

        MetaWeapon leftGun = new MetaWeapon(WeaponType.Cannon);
        MetaTurret leftGunMount = new MetaTurret(TurretType.Traverse);
        leftGunMount.m_weapons.Add(0, leftGun);

        MetaTurret doubleGunMount = new MetaTurret(TurretType.Double);
        doubleGunMount.m_turrets.Add(0, rightGunMount);
        doubleGunMount.m_turrets.Add(1, leftGunMount);

        MetaTurret doubleTankTurret = new MetaTurret(TurretType.TankTurret);
        doubleTankTurret.m_turrets.Add(0, doubleGunMount);

        MetaUnit doubleTankUnit = new MetaUnit(HullType.Tank, 1);
        doubleTankUnit.m_turrets.Add(0, doubleTankTurret);

        saveLoadHandler.M_SaveUnitToFile("doubletank", doubleTankUnit);
        MetaUnit loadedDoubleTank = saveLoadHandler.M_LoadFromFile("doubletank");
        // Debuggy way of creating an enemy tank
        loadedDoubleTank.m_faction = 0;
        M_BuildMetaUnit(loadedDoubleTank, m_enemyConvoySpawnPosition.transform.position + offset, m_spawnPosition.transform.rotation);
    }

}