using UnityEngine;
using System.Collections.Generic;
using System;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance { get; private set; }

    //the type is transform because in the gridsystem class we call it as transfrom. does the same thing as an object, we just care about where the numbers show up
    [SerializeField] private Transform gridDebugObjectPrefab;
    GridSystem gridSystem;
   
    public event EventHandler OnMechMovedGridPosition;






    private void Awake()
    {
        //if the 4 changes, remember to change the grid visual prefab scale too
        gridSystem = new GridSystem(10, 10, 4f);
        gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

        SetInstanceAndDebug();
    }
    void SetInstanceAndDebug()
    {
        if (Instance != null)
        {
            Debug.LogError("there's more than one UnitActionSystem" + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }






/* 
                                                    MECH POSITION
==================================================================================================================================== 
*/
    //to track the mech and know its in a cell
    public void AddMechAtGridPosition (GridPosition gridPosition, PCMech mech)
    {
        //go into gridsystem and grab the function to get a grid object at a position. now get the grid object that's there
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        //take that gridobject we got before and run the SetPcMech function and set it to pcMech (the function is on the GridObject class)
        gridObject.AddPcMech (mech);
    }
    //exactly same as set but we wanna delete the info when we call this function
    public void RemoveMechAtGridPosition (GridPosition gridPosition, PCMech mech)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.RemovePcMech(mech);
    }
    //same as above we just wanna get the pcmech that is there. function for getting the mech on GridObject
    public List <PCMech> GetMechListAtGridPosition (GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetPcMechList();
    }
    public void MechMovedGridPosition (PCMech pcMech, GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        RemoveMechAtGridPosition(fromGridPosition, pcMech);
        AddMechAtGridPosition(toGridPosition, pcMech);

        OnMechMovedGridPosition?.Invoke(this, EventArgs.Empty);
    }
    public bool HasAnyPcMechOnGridPosition (GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject (gridPosition);
        return gridObject.HasAnyPcMech();
    }
    public PCMech GetPcMechAtGridPosition (GridPosition gridPosition)
    {
        //returns the unit that is on a grid position, used in actions
        GridObject gridObject = gridSystem.GetGridObject (gridPosition);
        return gridObject.GetPCMech();
    }






/* 
                                                    CHECKING AND GETTING
==================================================================================================================================== 
*/
    public bool IsValidPosition(GridPosition gridPosition) => gridSystem.IsValidPosition(gridPosition);
    //the => is basically {return...} 
    public GridPosition GetGridPosition (Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);
    public Vector3 GetWorldPosition (GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);
    //the following 2 take the width and height from gridsystem and pass it to gridsystemvisuals. need them to cycle through the grid from outside
    public int GetWidth () => gridSystem.GetWidth();
    public int GetHeight() => gridSystem.GetHeight();
}
