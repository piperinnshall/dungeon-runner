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

    private readonly HashSet<Altar> altars = new HashSet<Altar>();
    private readonly HashSet<Altar> litAltars = new HashSet<Altar>();
    private bool isOpen;

    /// <summary>Called by each Altar when the game starts, so the door knows how many altars it waits for.</summary>
    public void Register(Altar altar)
    {
        altars.Add(altar);
    }

    /// <summary>Called by an Altar when it gets lit. Opens the door once all registered altars are lit.</summary>
    public void AltarLit(Altar altar)
    {
        litAltars.Add(altar);
        Debug.Log("Altars lit: " + litAltars.Count + " / " + altars.Count);

        if (!isOpen && litAltars.Count >= altars.Count)
            Open();
    }

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;
        StartCoroutine(Slide());
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
