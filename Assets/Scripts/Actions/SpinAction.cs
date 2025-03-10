using UnityEngine;
using System;

public class SpinAction : BaseAction
{
    
    float totalSpinAmount;

    private void Update()
    {
        if (!isActive) { return; }

        SpinTheMech();
    }

    private void SpinTheMech()
    {
        float spinAddAmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAddAmount, 0);
        totalSpinAmount += spinAddAmount;

        if (totalSpinAmount >= 360f) 
        { 
            isActive = false;
            onActionComplete();
        }
    }

    //put the delegate (still empty container) in a function. This means when we call this function, we can also put whatever other function we like into the delegate box and it'll run.
    //note: no () after the function name because we're not calling the function itself, we are referencing it
    //see UnitActionSystem for how clearing isBusy works.
    public void Spin(Action onActionComplete)
    {
        this.onActionComplete = onActionComplete;
        isActive = true;
        totalSpinAmount = 0f;
    }

    public override string GetActionName()
    {
        return "Spin";
    }
}
