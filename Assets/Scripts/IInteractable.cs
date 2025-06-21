using UnityEngine;
using System;

public interface IInteractable
{
    void Interact(Action onInteractComplete);
}
