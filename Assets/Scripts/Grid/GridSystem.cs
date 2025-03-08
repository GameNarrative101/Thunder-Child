using UnityEngine;

//not mono because we are using the constructor and can't do that on mono. Mono makes it so unity makes the object for you, this way we do it.
//Constructor builds an object or an instance of a class.
//Syntax is accessor (private, etc.) but no return bc/ the return is the object. name of the constructor is the class name. can have parameters or not (). Then with a keyword we USE the constructor and tell it to run this code.
public class GridSystem
{
    private int width;
    private int height;
    //introduce cellsize, add it as a parameter in the constructor after width and height, then multiply by it in the grid to world conversion so our normal grid takes up more space.
    private float cellSize;
    //normal array but in 2D. you store 2 items in the array. this is just to store the grid objects we create
    private GridObject[,] gridObjectArray;



    
    public GridSystem(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridObjectArray = new GridObject [width, height];
        
        for(int x = 0; x < width; x++)
        {
            //calling it height just because it makes more sense to codemonkey on a 2D spread. no reason
            for(int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                //bc the gridobject class calls for a gridsystem and position as parameters, we make it here with them included.
                //We also need a new gridposition that we define right above this
                gridObjectArray[x, z] = new GridObject (this, gridPosition);
            }
        }
    }

    //Converting grid to world so the system can lay the grid on the actual world
    public Vector3 GetWorldPosition (GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.z) *cellSize;
    }

    //Converting the world to grid so we can tell the system when an object is here in the world it's here on the grid
    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return new GridPosition(Mathf.RoundToInt(worldPosition.x / cellSize), Mathf.RoundToInt(worldPosition.z / cellSize));
    }

    //This is just so we give a visual to the grid objects being created, otherwise it's just there in maths and we can't see it.
    //We assign a prefab to it and make it public so we can call it from the Testing class.
    //Note: the transform below works because all transforms are attached to an object, so getting the transform is getting the object
    public void CreateDebugObjects(Transform debugPrefab)
    {
        for (int x = 0; x < width; x++)
        {
            //calling it height just because it makes more sense to codemonkey on a 2D spread. no reason
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition (x, z);

                //we grab the transform reference at the beginning so we can get the object that's in that world position,
                //then, we get its component (the script griddebugobject),
                //then, we get the grid object at the place that component is in (aka world position to grid position)
                
                //Syntax stuff
                //GameObject.Instantiate bc/ this is not mono so unity can't do it for us. 
                //Quaternion.identity just means no rotation
                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity); 
                GridDebugObject gridDebugObject = debugTransform.GetComponent <GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(gridPosition));
            }
        }
    }

    //ORIGINAL CODEMONEKY CODE. one pc mech was in the negative in the world and it would send an error. added a check below. works now.
    //public GridObject GetGridObject(GridPosition gridPosition)
    //{
    //    return gridObjectArray[gridPosition.x, gridPosition.z];
    //}

    public GridObject GetGridObject(GridPosition gridPosition)
    {
        
        if (gridPosition.x >= 0 && gridPosition.x < gridObjectArray.GetLength(0) &&
            gridPosition.z >= 0 && gridPosition.z < gridObjectArray.GetLength(1))
        {
            return gridObjectArray[gridPosition.x, gridPosition.z];
        }
        else
        {
            Debug.LogError("Grid position out of bounds: " + gridPosition);
            return null;
        }
    }

    public bool IsValidPosition(GridPosition gridPosition)
    {
        return  gridPosition.x >= 0 && 
                gridPosition.z >= 0 && 
                gridPosition.x < width && 
                gridPosition.z < height;
    }

    //used in the level grid to pass through to gridsystemvisuals
    public int GetWidth () { return width; }
    public int GetHeight () { return height; }


}
