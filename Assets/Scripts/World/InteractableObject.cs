using System;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    // Event that other scripts can subscribe to in order to be notified when the object is interacted with
    public event Action OnInteractedWith;

    public void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
        OnInteractedWith?.Invoke(); // Invoke the event to notify subscribers
    }
}
