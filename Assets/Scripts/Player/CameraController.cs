using TMPro;
using UnityEngine;

/*
    This file has a commented version with details about how each line works. 
    The commented version contains code that is easier and simpler to read. This file is minified.
*/

/// <summary>
/// Camera movement script for third person games.
/// This Script should not be applied to the camera! It is attached to an empty object and inside
/// it (as a child object) should be your game's MainCamera.
/// </summary>
public class CameraController : MonoBehaviour
{

    [Tooltip("Enable to move the camera by holding the right mouse button. Does not work with joysticks.")]
    public bool clickToMoveCamera = false;
    [Tooltip("Enable zoom in/out when scrolling the mouse wheel. Does not work with joysticks.")]
    public bool canZoom = false;
    [Space]
    [Tooltip("The higher it is, the faster the camera moves. It is recommended to increase this value for games that uses joystick.")]
    public float sensitivity = 2.5f;

    [Tooltip("Camera Y rotation limits. The X axis is the maximum it can go up and the Y axis is the maximum it can go down.")]
    public Vector2 cameraLimit = new Vector2(-60, 70);

    float mouseX;
    float mouseY;
    float offsetDistanceY;

    [SerializeField] float collisionOffset = 0.2f; // Offset to prevent camera from clipping into objects
    [SerializeField] LayerMask collisionMask; // Layer mask for objects that the camera can collide with
    [SerializeField] float collisionRadius = 0.3f; // Radius of the sphere cast for collision detection

    [SerializeField] float defaultFOV = 60f; // Default field of view for the camera
    [SerializeField] float topDownFOV = 70f; // Field of view when the camera is looking directly down
    [SerializeField] float zoomSpeed = 10f; // Speed at which the camera zooms in and out

    Camera playerCamera;
    Transform cameraTransform;
    Vector3 desiredLocalPosition;

    Transform player;

    void Start()
    {

        player = GameObject.FindWithTag("Player").transform;

        playerCamera = GetComponentInChildren<Camera>();
        cameraTransform = playerCamera.transform;

        // Store the initial desired position of the camera
        desiredLocalPosition = cameraTransform.localPosition;

        offsetDistanceY = transform.position.y - player.position.y;

        // Lock and hide cursor with option isn't checked
        if ( ! clickToMoveCamera )
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }

    }


    void Update()
    {

        // Follow player - camera offset
        transform.position = player.position + new Vector3(0, offsetDistanceY, 0);

        // Set camera zoom when mouse wheel is scrolled
        if( canZoom && Input.GetAxis("Mouse ScrollWheel") != 0 )
            Camera.main.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * sensitivity * 2;
        // You can use Mathf.Clamp to set limits on the field of view

        // Checker for right click to move camera
        if ( clickToMoveCamera )
            if (Input.GetAxisRaw("Fire2") == 0)
                return;
            
        // Calculate new position
        mouseX += Input.GetAxis("Mouse X") * sensitivity;
        mouseY += Input.GetAxis("Mouse Y") * sensitivity;
        // Apply camera limts
        mouseY = Mathf.Clamp(mouseY, cameraLimit.x, cameraLimit.y);

        transform.rotation = Quaternion.Euler(-mouseY, mouseX, 0);

        BirdEyeView();

    }

    void HandleCameraCollision()
    {
        Vector3 origin = transform.position;

        // Calculate the desired position of the camera based on its local position
        Vector3 desiredPosition = transform.TransformPoint(desiredLocalPosition);

        Vector3 direction = desiredPosition - origin;

        float distance = direction.magnitude;

        // Project the raycast between player and camera to check for collisions
        Vector3 targetPosition = desiredPosition;

        RaycastHit hit;
        if (Physics.SphereCast(origin, collisionRadius, direction.normalized, out hit, distance, collisionMask, QueryTriggerInteraction.Ignore))
        {
            float safeDistance = Mathf.Max(0.1f, hit.distance - collisionOffset);

            targetPosition = origin + direction.normalized * safeDistance;
        }

        // Keep the camera at the same height as the desired position
        targetPosition.y = desiredPosition.y;

        // Smoothly move the camera to the target position
        Vector3 targetLocalPosition = transform.InverseTransformPoint(targetPosition);

        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetLocalPosition, Time.deltaTime * 10f);
    }

    // Zoom out when camera is looking directly down for a bird's eye view
    void BirdEyeView()
    {
        // Calculate how much the camera is looking down
        float lookingDownAmount = Vector3.Dot(transform.forward, Vector3.down);
        bool isLookingDown = lookingDownAmount > 0.7f;

        // If the camera is looking down, set the field of view to the top-down FOV, otherwise set it to the default FOV
        // Changing variable in Unity works but doesn't work here sometimes
        float targetFOV = isLookingDown ? topDownFOV : defaultFOV;

        // Smoothly interpolate the camera's field of view to the target FOV
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
    }

    // LateUpdate is called AFTER all Update functions have been called.
    void LateUpdate()
    {
        HandleCameraCollision();
    }

}
