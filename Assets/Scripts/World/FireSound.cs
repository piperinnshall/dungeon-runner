using System.Collections;
using UnityEngine;

/// <summary>
/// Fire audio for torches, altars and campfires. Ignite() plays the "fire on" sound, then starts the
/// looping fire crackle. Extinguish() stops it. Torch, Altar and Campfire call these automatically
/// when this component is on the same object as them.
///
/// Setup: add this to the torch / altar / campfire (the object with that script, NOT the fire particle
/// object). An Audio Source is added automatically; set its 3D sound settings there if you like.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class FireSound : MonoBehaviour
{
    [SerializeField] private AudioClip fireOn;
    [SerializeField] private AudioClip fireLoop;
    [Range(0f, 1f)] [SerializeField] private float fireOnVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float fireLoopVolume = 0.6f;
    [Tooltip("Start the loop this many seconds before the 'fire on' sound ends, so there's no silent gap.")]
    [SerializeField] private float loopOverlap = 0.2f;

    private AudioSource loopSource;
    private AudioSource oneShotSource;

    // Runs when the component is first added: make the sound come from the fire's position in 3D.
    private void Reset()
    {
        AudioSource source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 1f;
        source.minDistance = 2f;
        source.maxDistance = 20f;
    }

    private void Awake()
    {
        loopSource = GetComponent<AudioSource>();
        loopSource.playOnAwake = false;
        loopSource.loop = true;
        loopSource.Stop();

        // A second source for the "fire on" sound, with the same 3D settings, so it doesn't
        // change the loop's volume.
        oneShotSource = gameObject.AddComponent<AudioSource>();
        oneShotSource.playOnAwake = false;
        oneShotSource.spatialBlend = loopSource.spatialBlend;
        oneShotSource.minDistance = loopSource.minDistance;
        oneShotSource.maxDistance = loopSource.maxDistance;
        oneShotSource.rolloffMode = loopSource.rolloffMode;
        oneShotSource.outputAudioMixerGroup = loopSource.outputAudioMixerGroup;
    }

    /// <summary>Plays the "fire on" sound, then the fire loop.</summary>
    public void Ignite()
    {
        StopAllCoroutines();
        StartCoroutine(IgniteRoutine());
    }

    /// <summary>Starts the fire loop straight away, with no "fire on" sound (e.g. a fire that starts lit).</summary>
    public void StartLoop()
    {
        if (fireLoop == null) return;
        loopSource.clip = fireLoop;
        loopSource.volume = fireLoopVolume;
        if (!loopSource.isPlaying) loopSource.Play();
    }

    /// <summary>Stops all fire sound.</summary>
    public void Extinguish()
    {
        StopAllCoroutines();
        if (loopSource != null) loopSource.Stop();
    }

    private IEnumerator IgniteRoutine()
    {
        if (fireOn != null)
        {
            oneShotSource.PlayOneShot(fireOn, fireOnVolume);
            yield return new WaitForSeconds(Mathf.Max(0f, fireOn.length - loopOverlap));
        }
        StartLoop();
    }
}
