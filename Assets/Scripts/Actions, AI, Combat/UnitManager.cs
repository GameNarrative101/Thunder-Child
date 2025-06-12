using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    List <PCMech> mechList;
    List <PCMech> friendlyMechList;
    List <PCMech> enemyMechList;



    void Awake()
    {
        SetInstanceAndDebug();

        mechList = new List<PCMech>();
        friendlyMechList = new List<PCMech>();
        enemyMechList = new List<PCMech>();
    }
    void Start()
    {
        PCMech.OnAnyUnitSpawned += PCMech_OnAnyUnitSpawned;
        PCMech.OnAnyUnitDead += PCMech_OnAnyUnitDead;
    }



    private void SetInstanceAndDebug()
    {
        if (Instance != null)
        {
            Debug.LogError("there's more than one UnitActionSystem" + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void PCMech_OnAnyUnitSpawned(object sender, EventArgs e)
    {
        PCMech unit = sender as PCMech;

        mechList.Add(unit);
        if (unit.IsEnemy()) {enemyMechList.Add(unit);}
        else {friendlyMechList.Add(unit);}
    }
    void PCMech_OnAnyUnitDead(object sender, EventArgs e)
    {
        PCMech unit = sender as PCMech;

        mechList.Remove(unit);
        if (unit.IsEnemy()) {enemyMechList.Remove(unit);}
        else {friendlyMechList.Remove(unit);}    
    }
    public List<PCMech> GetMechList() => mechList;
    public List<PCMech> GetFriendlyMechList() => friendlyMechList;
    public List<PCMech> GetEnemyMechList() => enemyMechList;
}
