using System.Collections;
using UnityEngine;

/// <summary>
/// A campfire the player can rest at. Walk up, press E: the fire lights, the screen fades to black,
/// the player's hearts are fully restored and bombs are refilled to the max, then the screen fades back.
/// The fire stays lit afterwards and the player can rest here again whenever they like.
///
/// Setup: put this on the campfire object with a BoxCollider ("Is Trigger" ticked, big enough to stand
/// in). Keep a separate solid collider on the campfire model if the player shouldn't walk through it.
/// Drag the fire particle effect (and a light, if you have one) into "Fire Objects".
/// </summary>
public class Campfire : MonoBehaviour
{
    [Header("Fire")]
    [Tooltip("Turned on when the player first rests here (fire particles, point light...). Off when the game starts.")]
    [SerializeField] private GameObject[] fireObjects;
    [Tooltip("Tick if the fire should already be burning when the level starts.")]
    [SerializeField] private bool startLit = false;

    [Header("Resting")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [Tooltip("Seconds to fade to black, and again to fade back in.")]
    [SerializeField] private float fadeDuration = 0.8f;
    [Tooltip("Seconds the screen stays black.")]
    [SerializeField] private float blackDuration = 1f;
    [Tooltip("Seconds between lighting the fire and the fade starting, so the player sees it catch.")]
    [SerializeField] private float delayBeforeFade = 0.6f;

    private bool playerInRange;
    private bool isResting;
    private GameObject player;
    private float alpha;

    private bool isLit;
    private FireSound fireSound;

    private void Awake()
    {
        fireSound = GetComponent<FireSound>();
        SetFire(startLit);
        isLit = startLit;
    }

    private void Start()
    {
        // A campfire that starts lit just crackles, with no "fire on" sound.
        if (startLit && fireSound != null) fireSound.StartLoop();
    }

    // NOTE: uses Unity's classic Input class. If Unity throws an error about it, set
    // Project Settings > Player > Active Input Handling to "Both".
    private void Update()
    {
        if (playerInRange && !isResting && Input.GetKeyDown(interactKey))
            StartCoroutine(Rest());
    }

    private IEnumerator Rest()
    {
        isResting = true;

        // Freeze the player while resting.
        Movement movement = player != null ? player.GetComponentInChildren<Movement>() : null;
        if (movement != null) movement.enabled = false;

        // Light the fire (and play the "fire on" sound) only the first time; after that it's already burning.
        if (!isLit)
        {
            isLit = true;
            SetFire(true);
            if (fireSound != null) fireSound.Ignite();
        }
        yield return new WaitForSecondsRealtime(delayBeforeFade);

        yield return Fade(0f, 1f);

        // ---- the screen is black: restore the player ----
        Health health = player != null ? player.GetComponentInChildren<Health>() : null;
        if (health != null && !health.isDead) health.health = health.maxHealth;
        PlayerItems.RefillBombs();
        Debug.Log("Rested at the campfire: health and bombs restored.");

        yield return new WaitForSecondsRealtime(blackDuration);
        yield return Fade(1f, 0f);

        if (movement != null) movement.enabled = true;
        isResting = false;
    }

    private void SetFire(bool lit)
    {
        foreach (GameObject obj in fireObjects)
            if (obj != null) obj.SetActive(lit);
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }
        alpha = to;
    }

    // Plain black rectangle over the whole screen, same as the scene-transition fade.
    private void OnGUI()
    {
        if (alpha <= 0f) return;
        GUI.depth = -1000;
        GUI.color = new Color(0f, 0f, 0f, alpha);
        GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        // The object with the player's scripts (PlayerMain), even if the collider is on a child.
        Health h = other.GetComponentInParent<Health>();
        player = h != null ? h.gameObject : other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }
}
