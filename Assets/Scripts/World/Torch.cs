using UnityEngine;

public class Torch : MonoBehaviour
{
    bool isTorchLit = false;
    bool interact;
    GameObject torchFlame;
    Light torchLight;

    InteractableObject interactableObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        torchFlame = transform.Find("Torch Glow Particle effect").gameObject;
        torchLight = torchFlame.GetComponentInChildren<Light>();
        interactableObject = GetComponent<InteractableObject>();
        ExtinguishTorch();

        // Subscribe to the OnInteractedWith event once
        if (interactableObject != null)
        {
            interactableObject.OnInteractedWith += HandleInteraction;
        }
    }

    // Unsubscribe from the event when the object is destroyed to prevent memory leaks
    void OnDestroy()
    {
        if (interactableObject != null)
        {
            interactableObject.OnInteractedWith -= HandleInteraction;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HandleInteraction()
    {
        // Toggle the torch state when interacted with
        if (isTorchLit)
        {
            ExtinguishTorch();
        }
        else
        {
            LightTorch();
        }
    }

    void LightTorch()
    {
        isTorchLit = true;
        ParticleSystem flame = torchFlame.GetComponent<ParticleSystem>();
        flame.Play();
        if (torchLight != null) torchLight.enabled = true;
    }

    void ExtinguishTorch()
    {
        isTorchLit = false;
        ParticleSystem flame = torchFlame.GetComponent<ParticleSystem>();
        flame.Stop();
        if (torchLight != null) torchLight.enabled = false;
    }
}
