using UnityEngine;

/// <summary>
/// The player's combat sounds. Put this on PlayerMain (the object with Movement and Health).
/// Movement and Health call it automatically:
///   - attack starts   -> sword swing + a random attack shout
///   - attack hits     -> attack land sound
///   - player hurt     -> a random hurt shout
///   - attack blocked  -> block sound (optional)
/// Shouts never play the same clip twice in a row, so with 2 clips they alternate.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class PlayerSounds : MonoBehaviour
{
    [Header("Attacking")]
    [SerializeField] private AudioClip swordSwing;
    [SerializeField] private AudioClip[] attackShouts;
    [SerializeField] private AudioClip attackLand;

    [Header("Getting hit")]
    [SerializeField] private AudioClip[] hurtShouts;
    [Tooltip("Optional: played when an attack is blocked by the shield.")]
    [SerializeField] private AudioClip block;

    [Header("Volume")]
    [Range(0f, 1f)] [SerializeField] private float shoutVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float effectVolume = 0.8f;
    [Tooltip("Small random pitch change on every sound so repeats don't sound identical. 0 = off.")]
    [Range(0f, 0.2f)] [SerializeField] private float pitchVariation = 0.05f;

    private AudioSource source;
    private int lastAttackShout = -1;
    private int lastHurtShout = -1;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
    }

    // Attack shout, when the attack starts.
    public void PlayAttack()
    {
        Play(PickRandom(attackShouts, ref lastAttackShout), shoutVolume);
    }

    // Sword swing, at the moment the hit is checked (same time as the land sound).
    public void PlaySwing() => Play(swordSwing, effectVolume);

    public void PlayHit() => Play(attackLand, effectVolume);

    public void PlayHurt() => Play(PickRandom(hurtShouts, ref lastHurtShout), shoutVolume);

    public void PlayBlock() => Play(block, effectVolume);

    private void Play(AudioClip clip, float volume)
    {
        if (clip == null) return;
        source.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        source.PlayOneShot(clip, volume);
    }

    // Random clip from the list, but never the same one as last time (if there's more than one).
    private static AudioClip PickRandom(AudioClip[] clips, ref int last)
    {
        if (clips == null || clips.Length == 0) return null;
        if (clips.Length == 1) return clips[0];

        int index;
        if (last < 0 || last >= clips.Length)
        {
            index = Random.Range(0, clips.Length);          // first time: any clip
        }
        else
        {
            index = Random.Range(0, clips.Length - 1);      // any clip except the last one
            if (index >= last) index++;
        }
        last = index;
        return clips[index];
    }
}
