using UnityEngine;
using System.Collections.Generic;
using System;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance { get; private set; }

    //the type is transform because in the gridsystem class we call it as transfrom. does the same thing as an object, we just care about where the numbers show up
    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] float cellSize;
    GridSystem<GridObject> gridSystem;
    public event EventHandler OnPCMechMovedGridPosition;



    private void Awake()
    {
        //if the 3.5 changes, remember to change the grid visual prefab scale too
        gridSystem = new GridSystem<GridObject>
                (width, height, cellSize, (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));
        // gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

        SetInstanceAndDebug();
    }
    private void Start()
    {
        Pathfinding.Instance.Setup(width, height, cellSize);
    }



    private void SetInstanceAndDebug()
    {
        if (Instance != null)
        {
            Debug.LogError("there's more than one LevelGrid" + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    //to track the mech and know its in a cell
    public void AddMechAtGridPosition(GridPosition gridPosition, PCMech pCMech)
    {
        //go into gridsystem and grab the function to get a grid object at a position. now get the grid object that's there
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);

        //take that gridobject we got before and run the SetPcMech function and set it to pcMech (the function is on the GridObject class)
        gridObject.AddPcMech(pCMech);
    }
    //exactly same as set but we wanna delete the info when we call this function
    public void RemoveMechAtGridPosition(GridPosition gridPosition, PCMech pCMech)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.RemovePcMech(pCMech);
    }
    public void MechMovedGridPosition(PCMech pcMech, GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        RemoveMechAtGridPosition(fromGridPosition, pcMech);
        AddMechAtGridPosition(toGridPosition, pcMech);

        OnPCMechMovedGridPosition?.Invoke(this, EventArgs.Empty);
    }
    public bool IsValidPosition(GridPosition gridPosition) => gridSystem.IsValidPosition(gridPosition);
    public bool HasAnyPcMechOnGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.HasAnyPcMech();
    }
    


    //==================================================================================================================================== 
    #region GETTERS
    public PCMech GetPcMechAtGridPosition(GridPosition gridPosition)
    {
        //returns the unit that is on a grid position, used in actions
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetPCMech();
    }
    public List<PCMech> GetMechListAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetPcMechList();
    }
    //the following 2 take the width and height from gridsystem and pass it to gridsystemvisuals. need them to cycle through the grid from outside
    public int GetWidth() => gridSystem.GetWidth();
    public int GetHeight() => gridSystem.GetHeight();
    //the => is basically {return...} 
    public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);
    public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);
    #endregion
}
