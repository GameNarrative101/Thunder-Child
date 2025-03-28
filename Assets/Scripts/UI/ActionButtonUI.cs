using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class ActionButtonUI : MonoBehaviour
{
   //UGUI for UI text
   [SerializeField] TextMeshProUGUI textMeshProUGUI;
   [SerializeField] Button button;
        /*   
            //for selected button visuals. Not needed with current prefabs


            [SerializeField] GameObject selectedVisual;
            [SerializeField] GameObject light_Active;

            BaseAction baseAction; 
        */

    void Start()
    {
        UnitActionSystem.Instance.onBusyChanged += UnitActionSystem_onBusyChanged;
        // TurnSystemScript.Instance.OnTurnEnd += TurnSystemScript_OnTurnEnd;
    }

   /* 
        private void UnitActionSystem_onBusyChanged(object sender, bool isBusy)
        {
            if (button != null)  // Check if the button is still valid
            {
                button.interactable = !isBusy;
            }
            else
            {
                Debug.LogWarning("Button reference is missing. It may have been destroyed.");
            }
        } 
   */

    //subscribes to action system's onBusyChanged event and makes buttons unusable when busy
    private void UnitActionSystem_onBusyChanged(object sender, bool isBusy)
    {
        if (isBusy || !TurnSystemScript.Instance.IsPlayerTurn())
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
    }

    /*  Deactivate buttons on enemy turns
    
    // if button implementaion changes from instantiating them to putting on static ones, this could work to deactivate them when on enemy turn
    // as it stands, because the buttons are destroyed when the action is not possible, we can't do that.   

    private void TurnSystemScript_OnTurnEnd(object sender, EventArgs e)
    {
        if (!TurnSystemScript.Instance.IsPlayerTurn()) 
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
    } 
    
    */
    
   void OnDestroy()
{
    if (UnitActionSystem.Instance != null)
    {
        UnitActionSystem.Instance.onBusyChanged -= UnitActionSystem_onBusyChanged;
    }
}



    /*
        we call this from the MechActionSystemUI script, which is responsible for creating the buttons.
        that script will call this function and pass in the base action, which we then use to set the text and behaviour of the button 
    */
    public void SetBaseAction(BaseAction baseAction)
   {
            /* 
                //for selected button visuals. Not needed with current prefabs

                    this.baseAction = baseAction;
            */
       //get the action name from the base action script
       string actionName = baseAction.GetActionName();
       textMeshProUGUI.text = actionName;

       //when the button is clicked, we set the selected action in the UnitActionSystem to an action extending base action
       button.onClick.AddListener(() => 
         {
            UnitActionSystem.Instance.SetSelectedAction (baseAction);
         });
   }

   /*  
   //for selected button visuals. Not needed with current prefabs
   
   //updating the button visuals. interacts with MechActionSystemUI script
    public void UpdateSelectedVisual()
    { 
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        selectedVisual.SetActive(selectedAction == baseAction);
        light_Active.SetActive(selectedAction == baseAction);
        Debug.Log("UpdateSelectedVisual");
    } */

}

