using UnityEngine;

/// <summary>
/// A treasure chest in the level. The player stands in the trigger zone and presses E: the chest plays
/// its opening animation and its coins go to the player (TreasureManager.Shared).
///
/// Setup (on the chest's ROOT object):
///   - this script
///   - a BoxCollider with "Is Trigger" ticked, a bit bigger than the chest (the "press E" zone)
///   - the chest model as a child, with its own solid (non-trigger) BoxCollider so the player bumps into it
///   - the Animator on the model needs a Trigger parameter called "OpenChest"
/// </summary>
public class TreasureView : MonoBehaviour {
    public enum Value { Cheap, Moderate, Valuable, Sacred }

    [Tooltip("Unique name for this chest. Leave EMPTY to use '<scene name>/<object name>' automatically.")]
    [SerializeField] private string id;
    [Tooltip("How many coins this chest holds (Cheap 1-5, Moderate 7-13, Valuable 15-20, Sacred 22-26).")]
    [SerializeField] private Value value = Value.Cheap;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private string openTrigger = "OpenChest";

    [Header("Sounds (optional)")]
    [Tooltip("Plays the moment the chest starts opening.")]
    [SerializeField] private AudioClip openSound;
    [Tooltip("Played when the coins arrive, picked by this chest's Value.")]
    [SerializeField] private AudioClip cheapCoinSound;
    [SerializeField] private AudioClip moderateCoinSound;
    [SerializeField] private AudioClip valuableCoinSound;
    [SerializeField] private AudioClip sacredCoinSound;
    [Tooltip("Seconds between the chest opening and the coins arriving. Negative = wait until the open sound finishes.")]
    [SerializeField] private float coinDelay = -1f;
    [Range(0f, 1f)] [SerializeField] private float volume = 1f;

    public string Id => string.IsNullOrEmpty(id) ? $"{gameObject.scene.name}/{name}" : id;

    private Animator animator;
    private ITreasure _treasure;
    private bool playerInRange;
    private bool opening;
    private AudioSource audioSource;

    // Kept so other code can still hand a view its treasure manually, like the old World/Test setup did.
    public void Initialize(ITreasure treasure) => _treasure = treasure;

    private void Awake() {
      animator = GetComponentInChildren<Animator>();

      // Sound comes from the chest's position in 3D. Uses an Audio Source on the chest if there is one.
      audioSource = GetComponent<AudioSource>();
      if (audioSource == null) {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.minDistance = 2f;
        audioSource.maxDistance = 20f;
      }
      audioSource.playOnAwake = false;
    }

    private void Start() {
      // Register with the shared manager. If this chest was already opened earlier in the game
      // (e.g. the player left the level and came back), we get that same treasure back.
      if (_treasure == null) _treasure = TreasureManager.Shared.Register(CreateTreasure());

      if (_treasure.State.IsOpened) PlayOpenAnimation();
    }

    private ITreasure CreateTreasure() => value switch {
      Value.Moderate => new Treasure<ICoinPacket.Moderate>(Id),
      Value.Valuable => new Treasure<ICoinPacket.Valuable>(Id),
      Value.Sacred   => new Treasure<ICoinPacket.Sacred>(Id),
      _              => new Treasure<ICoinPacket.Cheap>(Id),
    };

    // NOTE: uses Unity's classic Input class. If Unity throws an error about it, set
    // Project Settings > Player > Active Input Handling to "Both".
    private void Update() {
      if (playerInRange && Input.GetKeyDown(interactKey)) Open();
    }

    public void Open() {
      if (_treasure == null || _treasure.State.IsOpened || opening) return;
      StartCoroutine(OpenRoutine());
    }

    // 1) lid opens + open sound   2) short wait   3) coins go to the player + coin sound for this chest's value
    private System.Collections.IEnumerator OpenRoutine() {
      opening = true;
      PlayOpenAnimation();
      if (openSound != null) audioSource.PlayOneShot(openSound, volume);

      float wait = coinDelay >= 0f ? coinDelay : (openSound != null ? openSound.length : 0f);
      if (wait > 0f) yield return new WaitForSeconds(wait);

      _treasure.Open();
      AudioClip coinSound = CoinSoundFor(value);
      if (coinSound != null) audioSource.PlayOneShot(coinSound, volume);
      opening = false;
    }

    private AudioClip CoinSoundFor(Value v) => v switch {
      Value.Moderate => moderateCoinSound,
      Value.Valuable => valuableCoinSound,
      Value.Sacred   => sacredCoinSound,
      _              => cheapCoinSound,
    };

    private void PlayOpenAnimation() {
      if (animator != null) animator.SetTrigger(openTrigger);
    }

    private void OnTriggerEnter(Collider other) {
      if (other.CompareTag("Player")) playerInRange = true;
    }

    private void OnTriggerExit(Collider other) {
      if (other.CompareTag("Player")) playerInRange = false;
    }
}
