using UnityEngine;

/// <summary>
/// Stops the third-person camera going through walls. Put this on the CAMERA (the object with the Camera
/// component). Every frame, after the camera controller has placed the camera, it checks for walls
/// between the player's head and the camera; if there is one, the camera is pulled in to just in front of it.
/// Works alongside the existing camera controller, no changes to it needed.
/// </summary>
[DefaultExecutionOrder(1000)] // run after the camera controller has moved the camera
public class CameraCollision : MonoBehaviour
{
    [Tooltip("The player (PlayerMain). Found automatically by the 'Player' tag if left empty.")]
    [SerializeField] private Transform target;
    [Tooltip("Height above the player's feet that the camera looks from (roughly head height).")]
    [SerializeField] private float targetHeight = 1.5f;
    [Tooltip("How 'fat' the camera is when checking for walls. Bigger = keeps further from walls.")]
    [SerializeField] private float cameraRadius = 0.25f;
    [Tooltip("Extra gap kept between the camera and the wall.")]
    [SerializeField] private float wallPadding = 0.1f;
    [Tooltip("What the camera collides with. Untick the Player layer (and enemies, if you like) so they don't push the camera in.")]
    [SerializeField] private LayerMask collisionLayers = ~0;
    [Tooltip("Lets the camera slide back out after a wall when the camera controller only moves the PARENT object. " +
             "Safe to leave ticked: it only puts the offset back if nothing else has moved the camera since.")]
    [SerializeField] private bool restoreOffsetEachFrame = true;

    private Vector3 defaultLocalPosition;
    private bool pulledIn;
    private Vector3 lastPulledLocalPosition;

    private void Awake()
    {
        defaultLocalPosition = transform.localPosition;
    }

    private void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // If WE pulled the camera in last frame and nothing has moved it since, put it back at its normal
        // offset before checking again. If the camera controller moved the camera itself, leave its position alone
        // (overwriting it is what stopped the camera following the player).
        if (restoreOffsetEachFrame && pulledIn && transform.localPosition == lastPulledLocalPosition)
            transform.localPosition = defaultLocalPosition;
        pulledIn = false;

        Vector3 head = target.position + Vector3.up * targetHeight;
        Vector3 toCamera = transform.position - head;
        float distance = toCamera.magnitude;
        if (distance < 0.01f) return;

        Vector3 direction = toCamera / distance;

        // Triggers (door zones, chest zones...) are ignored, only solid colliders block the camera.
        if (Physics.SphereCast(head, cameraRadius, direction, out RaycastHit hit, distance,
                               collisionLayers, QueryTriggerInteraction.Ignore))
        {
            float safeDistance = Mathf.Max(hit.distance - wallPadding, 0.2f);
            transform.position = head + direction * safeDistance;
            pulledIn = true;
            lastPulledLocalPosition = transform.localPosition;
        }
    }
}
