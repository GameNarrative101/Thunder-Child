using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

public class GridSystemVisuals : MonoBehaviour
{
    public static GridSystemVisuals Instance {get; private set;}
    
    [Serializable] public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }
    public enum GridVisualType {White, Blue, Red, Yellow,}

    [SerializeField] Transform gridSystemVisualsPrefab;
    [SerializeField] List<GridVisualTypeMaterial> gridVisualTypeMaterialList;
    
    /*
        a 2D array of the prefab script, instantiate it, then generate it on start for the whole grid.
        we then decide what we wanna hide or show, and tell the other script to do that
    */
    GridSystemVisualSingle[,] gridSystemVisualSingleArray;






    void Awake()
    {
        SetInstanceAndDebug();
    }
    void Start()
    {
        InstantiateGridSystemVisualSingleArray();
        GenerateGridVisuals();
        UpdateGridVisual();

        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        LevelGrid.Instance.OnPCMechMovedGridPosition += LevelGrid_OnPCMechMovedGridPosition;
    }






/* 
                                                    SETUP AND SUBSCRIPTIONS
==================================================================================================================================== 
*/
    void SetInstanceAndDebug()
    {
        if (Instance != null)
        {
            Debug.LogError("there's more than one GridSystemVisuals" + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e) {UpdateGridVisual();}
    void LevelGrid_OnPCMechMovedGridPosition(object sender, EventArgs e) {UpdateGridVisual();}
    
    
    
    
    
    
/* 
                                                    GRID THINGS
==================================================================================================================================== 
*/
    void InstantiateGridSystemVisualSingleArray()
    {
        gridSystemVisualSingleArray = new GridSystemVisualSingle[LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight()];
    }
    //on start, cycle through the grid and make a prefab for each. simple, but not performant.
    void GenerateGridVisuals()
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualsPrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
                gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }
    }
    void UpdateGridVisual()
    {
        HideAllGridPosition();

        //access selected action from the unitactionsystem script
        BaseAction selelctedAction = UnitActionSystem.Instance.GetSelectedAction();

        GridVisualType gridVisualType;
        switch (selelctedAction)
        {
           default:
           case MoveAction moveAction:
                gridVisualType = GridVisualType.White;
                break; 
            case SpinAction spinAction:
                gridVisualType = GridVisualType.Yellow;
                break; 
            case ShootAction shootAction:
                gridVisualType = GridVisualType.Red;
                break; 
        }
        
        ShowGridPositionList (selelctedAction.GetValidActionGridPositionList(), gridVisualType);
    }
    Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType) {return gridVisualTypeMaterial.material;}
        }
        Debug.LogError("Grid visual type not found: " + gridVisualType);
        return null;
    }






/* 
                                                    PUBLIC FUNCTIONS
==================================================================================================================================== 
*/
    //hide everything by default, then decide what to show
    public void HideAllGridPosition()
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                gridSystemVisualSingleArray[x, z].Hide();
            }
        }
    }
    public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
    {
        foreach (GridPosition gridPosition in gridPositionList)
        {
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }
}
