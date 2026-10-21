using UnityEngine;

/// <summary>
/// Footstep sounds for the player. Put this on PlayerMain (the object with the CharacterController).
/// Plays a step every "Step Distance" metres walked while on the ground, picking a random clip each time
/// (never the same one twice in a row) with a slight pitch change so it doesn't sound robotic.
/// No changes to Movement are needed: it just watches how far the player moves.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class Footsteps : MonoBehaviour
{
    [SerializeField] private AudioClip[] footstepClips;
    [Tooltip("Metres walked between steps. Lower = faster steps. Tune it to match the walk animation.")]
    [SerializeField] private float stepDistance = 1.6f;
    [Range(0f, 1f)] [SerializeField] private float volume = 0.5f;
    [Range(0f, 0.3f)] [SerializeField] private float pitchVariation = 0.1f;
    [Tooltip("Ignore tiny movements (e.g. shuffling while turning) below this speed, in metres per second.")]
    [SerializeField] private float minSpeed = 0.5f;

    private CharacterController controller;
    private AudioSource source;
    private float distanceSinceStep;
    private int lastClip = -1;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        // Own Audio Source so footsteps don't interfere with the combat sounds.
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f; // the player's own sound, so 2D
    }

    private void Update()
    {
        Vector3 velocity = controller.velocity;
        velocity.y = 0f; // only count walking, not falling or jumping
        float speed = velocity.magnitude;

        if (!controller.isGrounded || speed < minSpeed)
        {
            // Start the next walk with a step coming soon, rather than a long silent gap.
            distanceSinceStep = stepDistance * 0.5f;
            return;
        }

        distanceSinceStep += speed * Time.deltaTime;
        if (distanceSinceStep >= stepDistance)
        {
            distanceSinceStep = 0f;
            PlayStep();
        }
    }

    private void PlayStep()
    {
        if (footstepClips == null || footstepClips.Length == 0) return;

        int index;
        if (footstepClips.Length == 1 || lastClip < 0)
        {
            index = Random.Range(0, footstepClips.Length);
        }
        else
        {
            index = Random.Range(0, footstepClips.Length - 1);   // any clip except the last one
            if (index >= lastClip) index++;
        }
        lastClip = index;

        if (footstepClips[index] == null) return;
        source.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        source.PlayOneShot(footstepClips[index], volume);
    }
}
