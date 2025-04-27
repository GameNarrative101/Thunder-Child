using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set;}
    
    [SerializeField] PCMech selectedPcMech;
    [SerializeField] LayerMask unitLayerMask;
    BaseAction selectedAction;
    
    bool isBusy;

    public event EventHandler OnSelectedUnitChange;
    public event EventHandler <bool> onBusyChanged; //<bool> replaces the eventargs in the eventhandler parameters
    public event EventHandler OnActionStarted;
    public event EventHandler OnSelectedActionChanged;






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






    private void UnitActionOperation()
    {
        if (isBusy) {return;}
        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty); //called here so visuals update when mouse is still on UI
        if (EventSystem.current.IsPointerOverGameObject()){return;} //no action if the mouse is over a UI element
        if (TryHandleUnitSelection()) {return;} //no action if selecting a unit
        if (!TurnSystemScript.Instance.IsPlayerTurn()) {return;} //no action if it's not the player's turn

        HandleSelectedAction();
    }

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

    //if you click on a valid grid position, and have enough core power to spend, take the action
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

    //clean this up!
    bool TryHandleUnitSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Input.GetMouseButtonDown(0)) return false;
        if (!Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask)) return false; //does the raycast system hit anything at the layer defined above?

        if (raycastHit.transform.TryGetComponent<PCMech>(out PCMech pcMech))
        {
            if (pcMech == selectedPcMech) return false;
            if (pcMech.IsEnemy())return false;
            
            SetSelectedPcMech (pcMech);
            return true;
        }
       return false;
    }

    void SetSelectedPcMech(PCMech pcMech)
    {
        selectedPcMech = pcMech;
        //defaults to move action on selecting a unit when we select the mech
        SetSelectedAction(pcMech.GetMoveAction());
        
        //a simpler way to do what is just an if statement checkign to see if seleced unit has changed or not
        OnSelectedUnitChange?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;

        /* 
            //for selected button visuals. Not needed with current prefabs

            OnSelectedUnitChange?.Invoke(this, EventArgs.Empty); 
        */
    }

   //important to select a unit without having to assign it on every script
    public PCMech GetSelectedMech()
    {
        return selectedPcMech;
    }

    //for use in the grid system visuals script
    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }



            
}
