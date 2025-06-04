using System;
using UnityEngine;

//subscribes to the action system's onBusyChanged event and changes the ui to show it's in use
public class ActionBusyUI : MonoBehaviour
{
    public event EventHandler <bool> onBusyActivated;



    void Start()
    {
        UnitActionSystem.Instance.onBusyChanged += UnitActionSystem_onBusyChanged;

        Hide();
    }



    void OnDestroy()
    {
        UnitActionSystem.Instance.onBusyChanged -= UnitActionSystem_onBusyChanged;
    }
    void Show()
    {
        gameObject.SetActive(true);

        onBusyActivated?.Invoke(this, true);
    }
    void Hide()
    {
        gameObject.SetActive(false);

        onBusyActivated?.Invoke(this, false);
    }
    //bool instead of eventargs because it was defined with <bool> on the UnitActionSystem
    void UnitActionSystem_onBusyChanged(object sender, bool isBusy)
    {
        if (isBusy)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }  
}
