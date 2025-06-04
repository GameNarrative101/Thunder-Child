using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

public class GridSystemVisuals : MonoBehaviour
{
    public static GridSystemVisuals Instance { get; private set; }

    [Serializable] public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }
    public enum GridVisualType { White, Blue, Red, Yellow, SoftBlue, SoftRed, SoftYellow }

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



    //==================================================================================================================================== 
    #region SETUP AND SUBSCRIPTIONS

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
    void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e) { UpdateGridVisual(); }
    void LevelGrid_OnPCMechMovedGridPosition(object sender, EventArgs e) { UpdateGridVisual(); }

    #endregion



    //==================================================================================================================================== 
    #region GRID THINGS  

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

        PCMech selectedMech = UnitActionSystem.Instance.GetSelectedMech();
        BaseAction selelctedAction = UnitActionSystem.Instance.GetSelectedAction(); //access selected action from the unitactionsystem script

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
                ShowGridPositionRange(selectedMech.GetGridPosition(), shootAction.GetMaxShootDistance(), GridVisualType.SoftRed);
                break;
            case GrenadeLauncherAction grenadeLauncherAction:
                gridVisualType = GridVisualType.Yellow;
                break;
            case MeleeAction meleeAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRange(selectedMech.GetGridPosition(), meleeAction.GetMaxMeleeDistance(), GridVisualType.SoftRed);
                break;
        }

        ShowGridPositionList(selelctedAction.GetValidActionGridPositionList(), gridVisualType);
    }
    Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType) { return gridVisualTypeMaterial.material; }
        }
        Debug.LogError("Grid visual type not found: " + gridVisualType);
        return null;
    }
    void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);
                if (!LevelGrid.Instance.IsValidPosition(testGridPosition)) continue;
                // if (x == 0 && z == 0) continue;

                gridPositionList.Add(testGridPosition);
            }
        }
        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    #endregion  



    //==================================================================================================================================== 
    #region PUBLIC FUNCTIONS

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
    
    #endregion
}
