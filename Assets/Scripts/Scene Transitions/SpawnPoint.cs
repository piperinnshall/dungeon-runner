using UnityEngine;

/// <summary>
/// Marks a place where the player can arrive after going through a SceneDoor.
/// When the scene loads and this spawn point's id matches the one the door asked for, the player
/// is moved here and turned to face the same direction as this object's blue (forward) arrow.
///
/// If you press Play directly in a scene (no door involved), nothing happens and the player
/// stays wherever you placed them.
/// </summary>
public class SpawnPoint : MonoBehaviour
{
    [Tooltip("Must match the 'Target Spawn Id' on the door that leads here.")]
    [SerializeField] private string spawnId = "Entrance";

    private void Start()
    {
        if (SceneTransition.PendingSpawnId != spawnId) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("SpawnPoint: no object tagged 'Player' in this scene.", this);
            return;
        }

        Teleport(player.transform);
        SceneTransition.ClearPendingSpawn();
    }

    private void Teleport(Transform player)
    {
        CharacterController controller = player.GetComponent<CharacterController>();
        Rigidbody body = player.GetComponent<Rigidbody>();

        // A CharacterController overrides direct position changes, so switch it off for the move.
        if (controller != null) controller.enabled = false;
        player.SetPositionAndRotation(transform.position, transform.rotation);
        if (controller != null) controller.enabled = true;

        // Stop any leftover momentum from carrying the player away from the spawn point.
        if (body != null && !body.isKinematic)
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }
    }

    // Shows roughly where the player will stand and which way they'll face.
    private void OnDrawGizmos()
    {
        Vector3 chest = transform.position + Vector3.up * 0.9f;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(chest, 0.3f);
        Gizmos.DrawRay(chest, transform.forward);
    }
}
