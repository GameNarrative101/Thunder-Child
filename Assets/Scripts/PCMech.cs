using Unity.VisualScripting;
using UnityEngine;

public class PCMech : MonoBehaviour
{
    //all grid position stuff are here for the sake of forced movement handling
    GridPosition gridPosition;
    //will be storing all actions in baseaction later
    MoveAction moveAction;
    SpinAction spinAction;
    BaseAction[] baseActionArray;

    private void Awake()
    {
        moveAction = GetComponent<MoveAction>();
        spinAction = GetComponent<SpinAction>();
        baseActionArray = GetComponents<BaseAction>();
    }

    private void Start()
    {
        //so on start, this script calculates its own grid position then calls the setmechatgridposition function in levelgrid
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddMechAtGridPosition(gridPosition, this);
    }

    private void Update()
    {
        GetNewGridPosition();
    }

    private void GetNewGridPosition()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            LevelGrid.Instance.MechMovedGridPosition(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;
        }
    }

    public MoveAction GetMoveAction()
    {
        return moveAction;
    }
    
    public SpinAction GetSpinAction()
    {
        return spinAction;
    }


    //lets other scripts know where this pcmech is
    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public BaseAction[] GetBaseActionArray()
    {
        return baseActionArray;
    }
}
