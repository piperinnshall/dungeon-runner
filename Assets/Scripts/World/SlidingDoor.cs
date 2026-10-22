using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A door that slides open once EVERY altar linked to it has been lit.
/// Put this on the jail door (the object that moves). Then, on each Altar, drag this door into the
/// "Door" field. The altars register themselves, so there is nothing to count by hand:
/// one altar = opens when it's lit, three altars = opens when all three are lit.
/// </summary>
public class SlidingDoor : MonoBehaviour
{
    [Tooltip("How far the door slides, in WORLD directions (X = east/west, Z = north/south). (-2, 0, 0) moves it 2 metres along -X. If it slides the wrong way, flip the sign or use Z instead.")]
    [SerializeField] private Vector3 slideOffset = new Vector3(-2f, 0f, 0f);
    [SerializeField] private float slideDuration = 2f;

    [Header("Sound (optional)")]
    [Tooltip("Plays when the door starts sliding open, e.g. the gate sound.")]
    [SerializeField] private AudioClip openSound;
    [Range(0f, 1f)] [SerializeField] private float soundVolume = 1f;

    [Header("Release enemies (optional)")]
    [Tooltip("NavMesh Agents of enemies locked behind this door. Untick their NavMesh Agent component in the Inspector so they stay put; they are switched on when the door opens.")]
    [SerializeField] private UnityEngine.AI.NavMeshAgent[] releaseOnOpen;
    [Tooltip("Seconds after the door starts opening before the enemies can move.")]
    [SerializeField] private float releaseDelay = 1f;

    private readonly HashSet<Altar> altars = new HashSet<Altar>();
    private readonly HashSet<Altar> litAltars = new HashSet<Altar>();
    private bool isOpen;

    /// <summary>Called by each Altar when the game starts, so the door knows how many altars it waits for.</summary>
    public void Register(Altar altar)
    {
        altars.Add(altar);
    }

    /// <summary>
    /// Called by an Altar when it gets lit. Opens the door once all registered altars are lit.
    /// instant = true when restoring a level the player has been to before: no slide, no sound.
    /// </summary>
    public void AltarLit(Altar altar, bool instant = false)
    {
        litAltars.Add(altar);
        if (!instant) Debug.Log("Altars lit: " + litAltars.Count + " / " + altars.Count);

        if (!isOpen && litAltars.Count >= altars.Count)
        {
            if (instant) OpenInstantly();
            else Open();
        }
    }

    // Already-open state when coming back to a level: jump straight to the open position.
    private void OpenInstantly()
    {
        isOpen = true;
        transform.position += slideOffset;

        if (releaseOnOpen != null)
            foreach (UnityEngine.AI.NavMeshAgent agent in releaseOnOpen)
                if (agent != null) agent.enabled = true;
    }

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;

        if (openSound != null)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.spatialBlend = 1f; // 3D: comes from the door
            source.minDistance = 2f;
            source.maxDistance = 25f;
            source.PlayOneShot(openSound, soundVolume);
        }

        StartCoroutine(Slide());
        if (releaseOnOpen != null && releaseOnOpen.Length > 0) StartCoroutine(ReleaseEnemies());
    }

    private IEnumerator ReleaseEnemies()
    {
        yield return new WaitForSeconds(releaseDelay);
        foreach (UnityEngine.AI.NavMeshAgent agent in releaseOnOpen)
            if (agent != null) agent.enabled = true; // snaps onto the NavMesh and starts patrolling / chasing
    }

    private IEnumerator Slide()
    {
        Vector3 start = transform.position;
        Vector3 end = start + slideOffset;

        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / slideDuration);
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        transform.position = end; // snap exactly to the final position
    }

    // Select the door in the Scene view to see where it will end up (cyan line and sphere).
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + slideOffset);
        Gizmos.DrawWireSphere(transform.position + slideOffset, 0.2f);
    }
}
