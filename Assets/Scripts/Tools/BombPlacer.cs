using UnityEngine;

/// <summary>
/// Attach this to your Player object.
/// Lets the player place a bomb prefab in front of them by pressing F.
/// Only one bomb can be placed at a time (until it explodes).
/// </summary>
public class BombPlacer : MonoBehaviour
{
    [Tooltip("Drag your Bomb prefab here.")]
    public GameObject bombPrefab;

    [Tooltip("Key used to place the bomb.")]
    public KeyCode placeKey = KeyCode.F;

    [Tooltip("How far in front of the player the bomb is placed.")]
    public float placeDistance = 1.5f;

    [Tooltip("How high above the ground the bomb spawns, so it drops and settles naturally.")]
    public float spawnHeight = 1f;

    // Keeps track of the currently placed bomb, so we don't spawn a second one
    private GameObject currentBomb;

    void Update()
    {
        // Only place a new bomb if the key is pressed AND there isn't one already active
        if (Input.GetKeyDown(placeKey) && currentBomb == null)
        {
            PlaceBomb();
        }
    }

    void PlaceBomb()
    {
        if (bombPrefab == null)
        {
            Debug.LogWarning("BombPlacer: No bomb prefab assigned in the Inspector!");
            return;
        }

        // Calculate a spot slightly in front of the player, raised up so it drops onto the ground
        Vector3 spawnPosition = transform.position + transform.forward * placeDistance + Vector3.up * spawnHeight;

        currentBomb = Instantiate(bombPrefab, spawnPosition, Quaternion.identity);

        // Tell the bomb script to notify us when it's destroyed, so we can place another one
        Bomb bombScript = currentBomb.GetComponent<Bomb>();
        if (bombScript != null)
        {
            bombScript.OnBombDestroyed += HandleBombDestroyed;
        }
    }

    void HandleBombDestroyed()
    {
        currentBomb = null;
    }
}