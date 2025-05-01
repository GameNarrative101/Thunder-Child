using UnityEngine;

public class ActionButtonsUI : MonoBehaviour
{
    [SerializeField] Transform actionButtonPrefab;
    [SerializeField] Transform actionButtonContainerTransform; //Container dragged in from the scene, not as a prefab  






    void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChange += UnitActionSystem_OnSelectedUnitChange;
        
        CreateMechActionButtons();
    }






/* 
                                                AIN'T MUCH TO IT, THIS IS HERE TO BE PRETTY
==================================================================================================================================== 
*/
    void UnitActionSystem_OnSelectedUnitChange(object sender, System.EventArgs e)
    {
        CreateMechActionButtons();
    }

    void CreateMechActionButtons ()
    {
        //first, destroy all buttons. we look for a transform, then destroy the object attached to it
        foreach (Transform buttonTransform in actionButtonContainerTransform) {Destroy(buttonTransform.gameObject);}

        //for each action, instantiate a button under the parent object then set the text by setting the action for each button
        PCMech selectedMech = UnitActionSystem.Instance.GetSelectedMech();
        
        foreach (BaseAction baseAction in UnitActionSystem.Instance.GetAvailableActionsForSelectedUnit())
        {
            Transform actionButtonTransform = Instantiate (actionButtonPrefab, actionButtonContainerTransform);
            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponentInChildren<ActionButtonUI>();
            actionButtonUI.SetBaseAction(baseAction);
        }
    }
}
