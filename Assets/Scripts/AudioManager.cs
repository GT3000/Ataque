using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [Range(0, 1)] [SerializeField] protected float bgmDefault;
    [Range(0, 1)] [SerializeField] protected float sfxDefault;
    protected float currentVolume;
    protected float defaultPitch;
    [SerializeField] protected List<AudioClip> bgmAudioClips;

    [SerializeField] protected AudioSource bgmAudioSource;
    [SerializeField] protected AudioSource sfxAudioSource;


    private void OnEnable()
    {
        GameEvents.PlaySfx += PlaySfx;
    }
    
    private void OnDisable()
    {
        GameEvents.PlaySfx -= PlaySfx;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (bgmAudioSource == null)
        {
            bgmAudioSource = GetComponent<AudioSource>();
        }

        currentVolume = bgmAudioSource.volume;
        defaultPitch = sfxAudioSource.pitch;
        
        PickRandomTrack();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PickRandomTrack()
    {
        int randomIndex = Random.Range(0, bgmAudioClips.Count);

        bgmAudioSource.clip = bgmAudioClips[randomIndex];
        bgmAudioSource.Play();
        bgmAudioSource.loop = true;
    }

    private void PlaySfx(AudioClip clip)
    {
        if (clip != null)
        {
            StartCoroutine(ExecuteSfx(clip));
        }
    }

    private IEnumerator ExecuteSfx(AudioClip clip)
    {
        float randomPitch = Random.Range(0.95f, 1.05f);
        sfxAudioSource.pitch = randomPitch;
        sfxAudioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);

        sfxAudioSource.pitch = defaultPitch;
    }
}
