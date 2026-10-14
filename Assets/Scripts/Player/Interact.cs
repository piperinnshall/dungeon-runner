using UnityEngine;

public class Interact : MonoBehaviour
{
    float interactDistance = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
    }
}
