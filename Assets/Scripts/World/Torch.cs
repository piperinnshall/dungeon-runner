using UnityEngine;

public class Torch : MonoBehaviour
{
    bool isTorchLit = false;
    bool interact;
    GameObject torchFlame;

    InteractableObject interactableObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        torchFlame = transform.Find("Torch Glow Particle effect").gameObject;
        interactableObject = GetComponent<InteractableObject>();
        ExtinguishTorch();

        if(interactableObject != null)
        {
            interactableObject.OnInteractedWith += HandleInteraction;
        }
    }

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
        if (interactableObject != null && interactableObject.gameObject == this.gameObject)
        {
            if (isTorchLit)
            {
                ExtinguishTorch();
            }
            else
            {
                LightTorch();
            }
        }
    }

    void LightTorch()
    {
        isTorchLit = true;
        ParticleSystem flame = torchFlame.GetComponent<ParticleSystem>();
        flame.Play();
    }

    void ExtinguishTorch()
    {
        isTorchLit = false;
        ParticleSystem flame = torchFlame.GetComponent<ParticleSystem>();
        Debug.Log("Particle system name: " + flame.name);
        flame.Stop();
    }
}
