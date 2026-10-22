using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HUD: one heart per point of the player's Max Health. Full hearts for remaining health, empty
/// hearts for health lost.
///
/// Setup: put this on the "Hearts" object (with a Horizontal Layout Group). Give it ONE child Image
/// (the heart) and drag that child into "Heart Template". Drag in the full and empty heart sprites.
/// </summary>
public class HeartDisplay : MonoBehaviour
{
    [SerializeField] private Image heartTemplate;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    private readonly List<Image> hearts = new List<Image>();
    private Health health;
    private int shownHealth = -1;
    private int shownMax = -1;

    private void Start()
    {
        if (heartTemplate != null) heartTemplate.gameObject.SetActive(false);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) health = player.GetComponentInChildren<Health>();

        if (health == null)
            Debug.LogWarning("HeartDisplay: couldn't find a Health script on the object tagged 'Player'.", this);
    }

    // Health.cs has no "changed" event, so check it each frame and only redraw when it changes.
    private void Update()
    {
        if (health == null || heartTemplate == null) return;

        int current = Mathf.Clamp(health.health, 0, health.maxHealth);
        if (current == shownHealth && health.maxHealth == shownMax) return;

        shownHealth = current;
        shownMax = health.maxHealth;
        Redraw();
    }

    private void Redraw()
    {
        while (hearts.Count < shownMax)
        {
            GameObject copy = Instantiate(heartTemplate.gameObject, heartTemplate.transform.parent);
            hearts.Add(copy.GetComponent<Image>());
        }

        for (int i = 0; i < hearts.Count; i++)
        {
            bool inUse = i < shownMax;
            hearts[i].gameObject.SetActive(inUse);
            if (inUse) hearts[i].sprite = i < shownHealth ? fullHeart : emptyHeart;
        }
    }
}
