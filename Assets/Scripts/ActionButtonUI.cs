using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour
{
   [SerializeField] TextMeshProUGUI textMeshProUGUI;
   [SerializeField] Button button;


public void SetBaseAction(BaseAction baseAction)
   {
    //    if (textMeshProUGUI == null)
    //    {
    //        Debug.LogError("TextMeshProUGUI component is not assigned.");
    //        return;
    //    }

    //    if (baseAction == null)
    //    {
    //        Debug.LogError("BaseAction is null.");
    //        return;
    //    }

       string actionName = baseAction.GetActionName();
    //    if (string.IsNullOrEmpty(actionName))
    //    {
    //        Debug.LogError("BaseAction.GetActionName() returned null or empty string.");
    //        return;
    //    }

    //    Debug.Log("Setting action name: " + actionName);
       textMeshProUGUI.text = actionName;
    //    Debug.Log("Action name set to: " + actionName);
   }
}


//    public void SetBaseAction(BaseAction baseAction)
//    {
//        textMeshProUGUI.text = baseAction.GetActionName();
// //        button.onClick.AddListener(() => baseAction.PerformAction());
//    }

