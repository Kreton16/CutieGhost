using System.Collections.Generic;
using UnityEngine;

public class AudioSourcePool : MonoBehaviour
{
    public static AudioSourcePool Instance;

    [Header("Settings")]
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private AudioClip defaultClip; // можна залишити пустим
    [SerializeField] private bool autoExpand = true;

    [Header("Prefab")]
    [SerializeField] private GameObject audioSourcePrefab; // або генеруємо динамічно

    private Queue<AudioSource> pool = new Queue<AudioSource>();
    private HashSet<AudioSource> activeSources = new HashSet<AudioSource>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreatePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CreatePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            pool.Enqueue(CreateNewSource());
        }
    }

    private AudioSource CreateNewSource()
    {
        GameObject go;

        if (audioSourcePrefab != null)
        {
            go = Instantiate(audioSourcePrefab, transform);
        }
        else
        {
            go = new GameObject("PooledAudioSource");
            go.transform.parent = transform;
            go.AddComponent<AudioSource>();
        }

        AudioSource source = go.GetComponent<AudioSource>();
        if (source == null) source = go.AddComponent<AudioSource>();
        go.SetActive(false);
        return source;
    }

    public AudioSource Get()
    {
        AudioSource source;

        if (pool.Count > 0)
        {
            source = pool.Dequeue();
        }
        else if (autoExpand)
        {
            source = CreateNewSource();
        }
        else
        {
            Debug.LogWarning("AudioSourcePool exhausted!");
            return null;
        }

        source.gameObject.SetActive(true);
        activeSources.Add(source);

        return source;
    }

    public void Return(AudioSource source)
    {
        if (source == null || !activeSources.Remove(source))
            return;

        source.Stop();
        source.clip = null;
        source.loop = false;
        source.gameObject.SetActive(false);

        pool.Enqueue(source);
    }

    public void StopAll()
    {
        foreach (AudioSource source in activeSources)
        {
            if (source == null)
                continue;

            source.Stop();
            source.clip = null;
            source.loop = false;
            source.gameObject.SetActive(false);

            pool.Enqueue(source);
        }

        activeSources.Clear();
    }
}
