using System;
using UnityEngine;

/* 
    not mono because we are using the constructor and can't do that on mono. Mono makes it so unity makes the object for you, this way we do it.
    Constructor builds an object or an instance of a class.
    Syntax is accessor (private, etc.) but no return bc/ the return is the object. name of the constructor is the class name. 
    can have parameters or not (). Then with a keyword we USE the constructor and tell it to run this code. 
*/
public class GridSystem <TGridObject>
{
    private int width;
    private int height;
    private float cellSize;
    private TGridObject[,] gridObjectArray;


    
    public GridSystem(int width, int height, float cellSize, Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridObjectArray = new TGridObject [width, height];
        
        for(int x = 0; x < width; x++)
        {
            //calling it height just because it makes more sense to codemonkey on a 2D spread. no reason
            for(int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                gridObjectArray[x, z] = createGridObject (this, gridPosition);
            }
        }
    }
    public Vector3 GetWorldPosition (GridPosition gridPosition)
    {
        //Converting grid to world so the system can lay the grid on the actual world
        return new Vector3(gridPosition.x, 0, gridPosition.z) *cellSize;
    }
    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        //Converting the world to grid so we can tell the system when an object is here in the world it's here on the grid
        return new GridPosition(Mathf.RoundToInt(worldPosition.x / cellSize), Mathf.RoundToInt(worldPosition.z / cellSize));
    }

    public void CreateDebugObjects(Transform debugPrefab)
    {
        for (int x = 0; x < width; x++)
        {
            //calling it height just because it makes more sense to codemonkey on a 2D spread. no reason
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition (x, z);
                //GameObject.Instantiate bc/ this is not mono so unity can't do it for us. 
                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity); 
                GridDebugObject gridDebugObject = debugTransform.GetComponent <GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(gridPosition));
            }
        }
    }
    public TGridObject GetGridObject(GridPosition gridPosition)
    { 
        // if (gridPosition.x >= 0 && gridPosition.x < gridObjectArray.GetLength(0) &&
        //     gridPosition.z >= 0 && gridPosition.z < gridObjectArray.GetLength(1))
        // {
            return gridObjectArray[gridPosition.x, gridPosition.z];
        // }
        // else
        // {
        //     Debug.LogError("Grid position out of bounds: " + gridPosition);
        //     return null;
        // }
    }
    public bool IsValidPosition(GridPosition gridPosition)
    {
        return  gridPosition.x >= 0 && 
                gridPosition.z >= 0 && 
                gridPosition.x < width && 
                gridPosition.z < height;
    }
    public int GetWidth () { return width; } //used in the level grid to pass through to gridsystemvisuals
    public int GetHeight () { return height; } //used in the level grid to pass through to gridsystemvisuals
}
