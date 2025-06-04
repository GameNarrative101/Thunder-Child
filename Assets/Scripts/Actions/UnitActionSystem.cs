using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }

    [SerializeField] PCMech selectedPcMech;
    [SerializeField] LayerMask unitLayerMask;
    BaseAction selectedAction;

    public event EventHandler OnSelectedUnitChange;
    public event EventHandler<bool> onBusyChanged; //<bool> replaces the eventargs in the eventhandler parameters
    public event EventHandler OnActionStarted;
    public event EventHandler OnSelectedActionChanged;

    bool isBusy;



    private void Awake()
    {
        SetInstanceAndDebug();
    }
    void Start()
    {
        SetSelectedPcMech(selectedPcMech);
    }
    private void Update()
    {
        UnitActionOperation();
    }






    /* 
                                                           SETUP AND UPDATE
    ==================================================================================================================================== 
    */
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
    void SetSelectedPcMech(PCMech pcMech)
    {
        selectedPcMech = pcMech;
        SetSelectedAction(pcMech.GetAction<MoveAction>()); //defaults to move action

        OnSelectedUnitChange?.Invoke(this, EventArgs.Empty);
    }
    private void UnitActionOperation()
    {
        if (isBusy) { return; }
        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty); //called here so visuals update when mouse is still on UI
        if (EventSystem.current.IsPointerOverGameObject()) { return; } //no action if the mouse is over a UI element
        if (TryHandleUnitSelection()) { return; } //no action if selecting a unit
        if (!TurnSystemScript.Instance.IsPlayerTurn()) { return; } //no action if it's not the player's turn

        HandleSelectedAction();
    }



    //==================================================================================================================================== 
    #region UNIT SELECTION

    bool TryHandleUnitSelection()
    {
        if (!Input.GetMouseButtonDown(0)) return false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask)) return false;

        if (!TrySelectPcMech(raycastHit)) return false;

        return true;
    }
    bool TrySelectPcMech(RaycastHit raycastHit)
    {
        if (!raycastHit.transform.TryGetComponent<PCMech>(out PCMech pcMech)) return false;

        if (pcMech == selectedPcMech) return false;
        if (pcMech.IsEnemy()) return false;

        SetSelectedPcMech(pcMech);
        return true;
    }

    #endregion



    //==================================================================================================================================== 
    #region ACTIONS

    void SetBusy()
    {
        isBusy = true;

        onBusyChanged?.Invoke(this, isBusy);
    }
    void ClearBusy()
    {
        isBusy = false;

        onBusyChanged?.Invoke(this, isBusy);
    }
    void HandleSelectedAction()
    {
        GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
        if (!Input.GetMouseButtonDown(0)) return;
        if (!selectedAction.IsValidActionGridPosition(mouseGridPosition)) return;
        if (!selectedPcMech.TrySpendCorePowerForAction(selectedAction)) return;

        SetBusy();
        selectedAction.TakeAction(mouseGridPosition, ClearBusy);
        OnActionStarted?.Invoke(this, EventArgs.Empty);
    }

    #endregion



    //==================================================================================================================================== 
    #region GETTERS AND SETTERS

    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;
    }
    public PCMech GetSelectedMech()
    {
        return selectedPcMech;
    }
    public BaseAction GetSelectedAction() //for use in the grid system visuals script
    {
        return selectedAction;
    }
    
    #endregion
}
