using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private SoundLibrary sfxLibrary;
    [SerializeField] private AudioSource sfx2DSource;
    [SerializeField] private AudioSource uiSource;
    [SerializeField] private AudioMixer audioMixer;

    private bool isSFXPaused;
    private float sfxVolume = 0f;
    private const string SFX_VOLUME = "SFXVolume";

    private Dictionary<string, AudioSource> playingLoops = new Dictionary<string, AudioSource>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }
    public void PlaySound2D(string soundName)
    {
        sfx2DSource.PlayOneShot(sfxLibrary.GetClipFromName(soundName));
    }

    public AudioSource PlaySound3D(string soundName, Vector3 pos, bool loop = false)
    {
        var clip = sfxLibrary.GetClipFromName(soundName);
        if (clip == null) return null;

        AudioSource source = AudioSourcePool.Instance.Get();
        source.transform.position = pos;
        source.clip = clip;
        source.spatialBlend = 1f;
        source.loop = loop;
        source.Play();

        if (loop)
        {
            // ƒодаЇмо до словника
            if (playingLoops.ContainsKey(soundName))
            {
                // якщо такий вже Ї Ч зупин€Їмо старий
                StopSound(soundName);
            }
            playingLoops[soundName] = source;
        }
        else
        {
            StartCoroutine(ReturnToPoolWhenFinished(source));
        }

        return source;
    }

    public void PlayUISound(string soundName)
    {
        AudioClip clip = sfxLibrary.GetClipFromName(soundName);

        if (clip == null)
        {
            Debug.LogWarning($"UI sound '{soundName}' not found!");
            return;
        }

        uiSource.PlayOneShot(clip);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;

        if (!isSFXPaused)
            audioMixer.SetFloat("SFXVolume", volume);
    }

    public void PauseSFX()
    {
        if (isSFXPaused)
            return;

        isSFXPaused = true;
        audioMixer.SetFloat("SFXVolume", -80f);
    }

    public void ResumeSFX()
    {
        if (!isSFXPaused)
            return;

        isSFXPaused = false;
        audioMixer.SetFloat("SFXVolume", sfxVolume);
    }

    public void StopAllSFX()
    {
        isSFXPaused = false;
        audioMixer.SetFloat("SFXVolume", sfxVolume);
        sfx2DSource.Stop();

        if (AudioSourcePool.Instance != null)
            AudioSourcePool.Instance.StopAll();

        playingLoops.Clear();
    }

    public void StopSound(string soundName)
    {
        if (playingLoops.TryGetValue(soundName, out AudioSource source))
        {
            source.Stop();
            AudioSourcePool.Instance.Return(source);
            playingLoops.Remove(soundName);
        }
    }

    private IEnumerator ReturnToPoolWhenFinished(AudioSource source)
    {
        yield return new WaitWhile(() => source.isPlaying);
        AudioSourcePool.Instance.Return(source);
    }

}