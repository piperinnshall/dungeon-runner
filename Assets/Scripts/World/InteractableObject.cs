using System;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public event Action OnInteractedWith;

    public void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
        OnInteractedWith?.Invoke();
    }
}
