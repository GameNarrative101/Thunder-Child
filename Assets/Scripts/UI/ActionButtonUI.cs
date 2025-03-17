using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class ActionButtonUI : MonoBehaviour
{
   [SerializeField] TextMeshProUGUI textMeshProUGUI;
   [SerializeField] Button button;

    void Start()
    {
      UnitActionSystem.Instance.onBusyChanged += UnitActionSystem_onBusyChanged;
    }

   // private void UnitActionSystem_onBusyChanged(object sender, bool isBusy)
   // {
   //    if (button != null)  // Check if the button is still valid
   //    {
   //       button.interactable = !isBusy;
   //    }
   //    else
   //    {
   //       Debug.LogWarning("Button reference is missing. It may have been destroyed.");
   //    }
   // }

    //subscribes to action system's onBusyChanged event and makes buttons unusable when busy
    private void UnitActionSystem_onBusyChanged(object sender, bool isBusy)
    {
        if (isBusy)
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
    }
    
   void OnDestroy()
{
    if (UnitActionSystem.Instance != null)
    {
        UnitActionSystem.Instance.onBusyChanged -= UnitActionSystem_onBusyChanged;
    }
}



    public void SetBaseAction(BaseAction baseAction)
   {
       string actionName = baseAction.GetActionName();
       textMeshProUGUI.text = actionName;

       button.onClick.AddListener(() => 
         {
            UnitActionSystem.Instance.SetSelectedAction (baseAction);
         });
   }

}

