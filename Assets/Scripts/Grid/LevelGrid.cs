using UnityEngine;
using System.Collections.Generic;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance { get; private set; }

    //the type is transform because in the gridsystem class we call it as transfrom. does the same thing as an object, we just care about where the numbers show up
    [SerializeField] private Transform gridDebugObjectPrefab;
    GridSystem gridSystem;




    private void Awake()
    {
        //if the 3.5 changes, remember to change the grid visual prefab scale too
        gridSystem = new GridSystem(10, 10, 4f);
        gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

        SetInstanceAndDebug();
    }




    //to track the mech and know its in a cell
    public void AddMechAtGridPosition (GridPosition gridPosition, PCMech pCMech)
    {
        //go into gridsystem and grab the function to get a grid object at a position. now get the grid object that's there
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        
        //take that gridobject we got before and run the SetPcMech function and set it to pcMech (the function is on the GridObject class)
        gridObject.AddPcMech (pCMech);
    }

    //same as above we just wanna get the pcmech that is there. function for getting the mech on GridObject
    public List <PCMech> GetMechListAtGridPosition (GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetPcMechList();
    }

    //exactly same as set but we wanna delete the info when we call this function
    public void RemoveMechAtGridPosition (GridPosition gridPosition, PCMech pCMech)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.RemovePcMech(pCMech);
    }

    public void MechMovedGridPosition (PCMech pcMech, GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        RemoveMechAtGridPosition(fromGridPosition, pcMech);
        AddMechAtGridPosition(toGridPosition, pcMech);
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

    //the => is basically {return...} 
    public GridPosition GetGridPosition (Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);
    
    public Vector3 GetWorldPosition (GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);

    public bool IsValidPosition(GridPosition gridPosition) => gridSystem.IsValidPosition(gridPosition);

    //the following 2 take the width and height from gridsystem and pass it to gridsystemvisuals. need them to cycle through the grid from outside
    public int GetWidth () => gridSystem.GetWidth();

    public int GetHeight() => gridSystem.GetHeight();

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
}
