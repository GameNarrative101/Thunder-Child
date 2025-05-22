using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance { get; private set; }

    [SerializeField] Transform gridDebugObjectPrefab;
    [SerializeField] LayerMask obstacleLayerMask;

    int width;
    int height;
    float cellSize;
    const int MOVE_STRAIGHT_COST = 10;
    const int MOVE_DIAGONAL_COST = 14;
    GridSystem<PathNode> gridSystem;



    void Awake()
    {
        SetInstanceAndDebug();
    }



    //==================================================================================================================================== 
    #region SETUP
    private void SetInstanceAndDebug()
    {
        if (Instance != null)
        {
            Debug.LogError("there's more than one Pathfinding" + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void Setup(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridSystem = new GridSystem<PathNode>(width, height, cellSize,
            (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));


        // gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                float raycastOffsetDistance = 5f;

                if (Physics.Raycast(
                    worldPosition + Vector3.down * raycastOffsetDistance,
                    Vector3.up,
                    raycastOffsetDistance * 2f,
                    obstacleLayerMask))
                {
                    GetNode(x, z).SetIsWalkable(false);
                }
            }
        }
    }
    #endregion



    //==================================================================================================================================== 
    #region FIND THAT PATH    
    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
    {
        List<PathNode> openList = new List<PathNode>();
        HashSet<PathNode> closedList = new HashSet<PathNode>();

        PathNode startNode = gridSystem.GetGridObject(startGridPosition);
        PathNode endNode = gridSystem.GetGridObject(endGridPosition);
        openList.Add(startNode);


        for (int x = 0; x < gridSystem.GetWidth(); x++)
        {
            for (int z = 0; z < gridSystem.GetHeight(); z++)
            {
                PathNode pathNode = gridSystem.GetGridObject(new GridPosition(x, z));
                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.CalculateFCost();
                pathNode.ResetCameFromPathNode();
            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();


        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostPathNode(openList);

            if (currentNode == endNode)
            {
                pathLength = endNode.GetFCost();
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);


            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode)) continue;

                if (!neighbourNode.GetIsWalkable())
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());

                if (tentativeGCost < neighbourNode.GetGCost() || !openList.Contains(neighbourNode))
                {
                    neighbourNode.SetGCost(tentativeGCost);
                    neighbourNode.SetHCost(CalculateDistance(neighbourNode.GetGridPosition(), endNode.GetGridPosition()));
                    neighbourNode.SetCameFromPathNode(currentNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        pathLength = 0;
        return null;
    }
    public int CalculateDistance(GridPosition a, GridPosition b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int zDistance = Mathf.Abs(a.z - b.z);
        int remaining = Mathf.Abs(xDistance - zDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
    }
    PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostPathNode = pathNodeList[0];

        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost())
            {
                lowestFCostPathNode = pathNodeList[i];
            }
        }

        return lowestFCostPathNode;
    }
    private PathNode GetNode(int x, int z)
    {
        return gridSystem.GetGridObject(new GridPosition(x, z));
    }
    List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        GridPosition gridPosition = currentNode.GetGridPosition();
        List<PathNode> neighbourList = new List<PathNode>();
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 && z == 0) continue; //Skip the current node

                GridPosition neighbourGridPosition = new GridPosition(
                    currentNode.gridPosition.x + x,
                    currentNode.gridPosition.z + z
                );

                if (!gridSystem.IsValidPosition(neighbourGridPosition)) continue;

                PathNode neighbourNode = gridSystem.GetGridObject(neighbourGridPosition);
                neighbourList.Add(neighbourNode);
            }
        }
        return neighbourList;
        /*                           MANUALLY CHECKING NEIGHBOURS        
        if (gridPosition.x -1 >= 0 ) //check for valid grid neighbours to the left
        {
            neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z)); //left
            
            if (gridPosition.z - 1 >= 0) //check for valid grid neighbours below
            {
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1)); //left down
            }
            if (gridPosition.z + 1 < gridSystem.GetHeight()) //check for valid grid neighbours above
            {
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1)); //left up
            }
        }
        if (gridPosition.x +1 < gridSystem.GetWidth()) //check for valid grid neighbours to the right
        {
            neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z)); //right
            
            if (gridPosition.z - 1 >= 0) //check for valid grid neighbours below
            {
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1)); //right down
            }
            if (gridPosition.z + 1 < gridSystem.GetHeight()) //check for valid grid neighbours above
            {
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1)); //right up
            }
        }
        if (gridPosition.z - 1 >= 0) //check for valid grid neighbours below
        {
            neighbourList.Add(GetNode(gridPosition.x, gridPosition.z - 1)); //down
        }
        if (gridPosition.z + 1 < gridSystem.GetHeight()) //check for valid grid neighbours above
        {
            neighbourList.Add(GetNode(gridPosition.x, gridPosition.z + 1)); //up
        } 
        */
    }
    List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new List<PathNode> { endNode };
        PathNode currentNode = endNode;

        while (currentNode.GetCameFromPathNode() != null)
        {
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }
        pathNodeList.Reverse();

        List<GridPosition> gridPositionList = new List<GridPosition>();
        foreach (PathNode pathNode in pathNodeList)
        {
            gridPositionList.Add(pathNode.GetGridPosition());
        }

        return gridPositionList;
    }
    #endregion



    //==================================================================================================================================== 
    #region GETTERS AND STUFF
    public bool GetIsWalkableGridPosInPath(GridPosition gridPosition)
    {
        return gridSystem.GetGridObject(gridPosition).GetIsWalkable();
    }
    public bool HasValidPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        return FindPath(startGridPosition, endGridPosition, out int pathLength) != null;
    }
    public int GetPathLength (GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }
    #endregion
}
