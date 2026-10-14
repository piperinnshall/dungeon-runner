using UnityEngine;

public class Interact : MonoBehaviour
{
    float interactDistance = 1f;
    Animator _animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // On pressing the E key, shoot a raycast in the forward direction to check for interactable objects
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
                if (interactable != null)
                {
                    _animator.SetTrigger("InteractTrigger");
                    interactable.Interact();
                }
            }
        }
    }
}
