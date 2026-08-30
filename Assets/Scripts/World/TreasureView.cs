using UnityEngine;

public class TreasureView : MonoBehaviour {
    private Animator animator;
    private ITreasure _treasure;

    [SerializeField] private string id;
    public string Id => id;

    public void Initialize(ITreasure treasure) => _treasure = treasure;

    // private void Awake() => animator = GetComponentInChildren<Animator>();
    private void Awake() {
      animator = GetComponentInChildren<Animator>();
      gameObject.AddComponent<BoxCollider>();
    }
    private void OnMouseDown() => Open();

    public void Open() {
      _treasure.Open();
      animator.SetTrigger("OpenChest");
    }
}
