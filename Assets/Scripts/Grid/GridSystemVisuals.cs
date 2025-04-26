using UnityEngine;
using System.Collections.Generic;

public class GridSystemVisuals : MonoBehaviour
{
    public static GridSystemVisuals Instance { get; private set; }

    [SerializeField] Transform gridSystemVisualsPrefab;

    /*
        a 2D array of the prefab script, instantiate it, then generate it on start for the whole grid.
        we then decide what we wanna hide or show, and tell the other script to do that
    */
    GridSystemVisualSingle[,] gridSystemVisualSingleArray;


    private void Awake()
    {
        SetInstanceAndDebug();
    }

    private void Start()
    {
        InstantiateGridSystemVisualSingleArray();

        GenerateGridVisuals();
    }

    private void Update()
    {
        UpdateGridVisual();
    }

    private void SetInstanceAndDebug()
    {
        if (Instance != null)
        {
            Debug.LogError("there's more than one GridSystemVisuals" + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void InstantiateGridSystemVisualSingleArray()
    {
        gridSystemVisualSingleArray = new GridSystemVisualSingle[LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight()];
    }

    //on start, cycle through the grid and make a prefab for each. simple, but not performant.
    private void GenerateGridVisuals()
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

    public void ShowGridPositionList(List<GridPosition> gridPositionList)
    {
        foreach (GridPosition gridPosition in gridPositionList)
        {
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show();
        }
    }

    /*
        updating every frame is temp. the going through selected pcmech to get the move action will change too
        once we have a whole actions script
    */
    void UpdateGridVisual()
    {
        HideAllGridPosition();

        //access selected action from the unitactionsystem script
        BaseAction selelctedAction = UnitActionSystem.Instance.GetSelectedAction();
        ShowGridPositionList (selelctedAction.GetValidActionGridPositionList());
    }

}
