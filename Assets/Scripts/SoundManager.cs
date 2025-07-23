using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField]
    private SoundLibrary sfxLibrary;
    [SerializeField]
    private AudioSource sfx2DSource;

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