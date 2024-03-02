using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioConductor : BroSingleton<AudioConductor>
{ 
    
    [Tooltip("PreWarm")]
    public int poolingAmount = 10;
    [Tooltip("Enable this so, in the event more sources are requested than available, the pooler can take it upon itself to add more.")]
    public bool overflowProtection = true;

    //A queue of ready to use audio sources
    private Queue<AudioSource> audioSources;
    //A list of the active audio sources, this will be looped through to returned finished sources back to the queue
    private List<AudioSource> activeSources;

    
    private AudioSource cacheSource;
    private GameObject cacheObj;

    //How often, in seconds, should the AudioManager check for finished audio sources
    private float checkTime = 0.5f;

    public CustomList<string, AudioSequence> sfx;

    public CustomList<string, AudioSource> music;


    public List<AudioSequence> cheers;


    public Vector2 fillVol = new Vector2(1, 1);

    private void Start()
    {
        BuildPool();
    }

    private void Update()
    {
        //Only check every half of a second, no need to check every single frame.
        checkTime -= Time.deltaTime;
        if (checkTime <= 0)
        {
            CheckForFinished();
            checkTime = 0.5f;
        }
    }
    [ContextMenu("FillVol")]
    public void FillVol()
    {
        cheers.ForEach(c => c.volume = fillVol);
    }

    //Build the initial pool
    private void BuildPool()
    {
        //Create new queue and list objects
        audioSources = new Queue<AudioSource>();
        activeSources = new List<AudioSource>();

        //Add the number of entries specified by the pooling amount
        for (int i = 0; i < poolingAmount; i++)
        {
            AddEntry();
        }
    }

    //Adds an entry to audio sources queue
    private void AddEntry()
    {
        cacheObj = new GameObject();
        cacheObj.transform.parent = transform;
        cacheSource = cacheObj.AddComponent<AudioSource>();
        cacheSource.playOnAwake = false;
        cacheObj.SetActive(false);
        audioSources.Enqueue(cacheSource);
    }


    private void CheckForFinished()
    {
        //Iterate through all the active audio sources and check if they are done playing
        for (int i = 0; i < activeSources.Count; i++)
        {
            //If they are done playing deactivate them and add them back into the queue
            if (!activeSources[i].isPlaying)
            {
                audioSources.Enqueue(activeSources[i]);
                activeSources[i].gameObject.SetActive(false);
                activeSources.RemoveAt(i);
                i--;
            }
        }
    }

    //Grabs an Audio Source from the queue if we have one available
    private AudioSource GetAudioSource()
    {
        if (audioSources.Count < 1)
        {
            if (overflowProtection)
                AddEntry();
            else
            {
                Debug.LogError("Audio Source Queue Empty, consider increasing pool size or turn on overflow protection.");
                return null;
            }
        }

        return audioSources.Dequeue();
    }

    public static void PlayCheer()
    {
        Instance.InternalPlayCheer();
    }
    int tempCount;

    private  void InternalPlayCheer()
    {
        if(tempCount == 0)
        {
            cheers.Shuffle();
        }

        cheers[tempCount].PlaySound();

        tempCount++;

        if(tempCount >= cheers.Count)
        {
            tempCount= 0;
        }
    }

    public static void PlaySfx(string key)
    {
        try
        {
            Instance.sfx.Get(key)?.PlaySound();
        }
        catch
        {
            Debug.Log("UnableToPlay");
        }
    }
    public static void StopSfx(string key)
    {
        Instance.sfx.Get(key).Stop();
    }
    public static void PlayMusic(string key)
    {
        Debug.Log(key);
        if(Instance.music.Contains(key))
        {
            Instance.music.List.ForEach(a=>a.value.Stop());
            Instance.music.Get(key).Play() ;
        }
    }
    public static void StopMusic(string key)
    {
        if (Instance.music.Contains(key))
            Instance.music.Get(key).Stop();
    }
    /// <summary>
    /// Method for manually returning Audio Sources back into the pool
    /// </summary>
    /// <param name="source"></param>
    public static void ReturnToPool(AudioSource source)
    {
        source.gameObject.SetActive(false);
        Instance.audioSources.Enqueue(source);
    }

    #region 2D Sound Methods
    /// <summary>
    /// Plays an AudioClip without spatial blending with default settings
    /// </summary>
    /// <param name="clip"></param>
    public static AudioSource PlaySound2D(AudioClip clip)
    {
        Instance.cacheSource = Instance.GetAudioSource();
        Instance.cacheSource.clip = clip;
        Instance.cacheSource.volume = 1f;
        Instance.cacheSource.loop = false;
        Instance.cacheSource.spatialBlend = 0.0f;
        Instance.cacheSource.gameObject.SetActive(true);
        Instance.activeSources.Add(Instance.cacheSource);
        Instance.cacheSource.Play();
        return Instance.cacheSource;
    }
    /// <summary>
    /// Plays an AudioClip without spatial blending with adjustable volume
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    public static AudioSource PlaySound2D(AudioClip clip, float volume)
    {
        Instance.cacheSource = Instance.GetAudioSource();
        Instance.cacheSource.clip = clip;
        Instance.cacheSource.volume = volume;
        Instance.cacheSource.loop = false;
        Instance.cacheSource.spatialBlend = 0.0f;
        Instance.cacheSource.gameObject.SetActive(true);
        Instance.activeSources.Add(Instance.cacheSource);
        Instance.cacheSource.Play();
        return Instance.cacheSource;
    }
    /// <summary>
    /// Plays an AudioClip without spatial blending with adjustable volume and pitch
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    public static void PlaySound2D(AudioClip clip, float volume, float pitch)
    {
        Instance.cacheSource = Instance.GetAudioSource();
        Instance.cacheSource.clip = clip;
        Instance.cacheSource.volume = volume;
        Instance.cacheSource.pitch = pitch;
        Instance.cacheSource.loop = false;
        Instance.cacheSource.spatialBlend = 0.0f;
        Instance.cacheSource.gameObject.SetActive(true);
        Instance.activeSources.Add(Instance.cacheSource);
        Instance.cacheSource.Play();
    }
    /// <summary>
    /// Plays an AudioClip without spatial blending with adjustable volume and pitch that loops
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    /// <returns>AudioSource</returns>
    public static AudioSource PlaySound2DLooped(AudioClip clip, float volume, float pitch)
    {
        Instance.cacheSource = Instance.GetAudioSource();
        Instance.cacheSource.clip = clip;
        Instance.cacheSource.volume = volume;
        Instance.cacheSource.pitch = pitch;
        Instance.cacheSource.loop = true;
        Instance.cacheSource.spatialBlend = 0.0f;
        Instance.cacheSource.gameObject.SetActive(true);
        Instance.cacheSource.Play();
        return Instance.cacheSource;
    }

    /// <summary>
    /// Plays an AudioClip without spatial blending with adjustable volume and pitch that loops
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    /// <returns>AudioSource</returns>
    public static AudioSource PlaySound2DLooped(AudioClip clip, float volume)
    {
        Instance.cacheSource = Instance.GetAudioSource();
        Instance.cacheSource.clip = clip;
        Instance.cacheSource.volume = volume;
        Instance.cacheSource.loop = true;
        Instance.cacheSource.spatialBlend = 0.0f;
        Instance.cacheSource.gameObject.SetActive(true);
        Instance.cacheSource.Play();
        return Instance.cacheSource;
    }
    #endregion

    #region 3D Sound Methods
    /// <summary>
    /// Plays and AudioClip with spatial blending at a specific location
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="position"></param>
    public static void PlaySound3D(AudioClip clip, Vector3 position)
    {
        Instance.cacheSource = Instance.GetAudioSource();
        Instance.cacheSource.clip = clip;
        Instance.cacheSource.volume = 1f;
        Instance.cacheSource.loop = false;
        Instance.cacheSource.spatialBlend = 1.0f;
        Instance.cacheSource.gameObject.transform.position = position;
        Instance.cacheSource.gameObject.SetActive(true);
        Instance.activeSources.Add(Instance.cacheSource);
        Instance.cacheSource.Play();
    }

    /// <summary>
    /// Plays and AudioClip with spatial blending at a specific location with adjustable volume
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="position"></param>
    /// <param name="volume"></param>
    public static void PlaySound3D(AudioClip clip, Vector3 position, float volume)
    {
        Instance.cacheSource = Instance.GetAudioSource();
        Instance.cacheSource.clip = clip;
        Instance.cacheSource.volume = volume;
        Instance.cacheSource.loop = false;
        Instance.cacheSource.spatialBlend = 1.0f;
        Instance.cacheSource.gameObject.transform.position = position;
        Instance.cacheSource.gameObject.SetActive(true);
        Instance.activeSources.Add(Instance.cacheSource);
        Instance.cacheSource.Play();
    }
    /// <summary>
    /// Plays and AudioClip with spatial blending at a specific location with adjustable volume and pitch
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="position"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    public static void PlaySound3D(AudioClip clip, Vector3 position, float volume, float pitch)
    {
        Instance.cacheSource = Instance.GetAudioSource();
        Instance.cacheSource.clip = clip;
        Instance.cacheSource.volume = volume;
        Instance.cacheSource.pitch = pitch;
        Instance.cacheSource.loop = false;
        Instance.cacheSource.spatialBlend = 1.0f;
        Instance.cacheSource.gameObject.transform.position = position;
        Instance.cacheSource.gameObject.SetActive(true);
        Instance.activeSources.Add(Instance.cacheSource);
        Instance.cacheSource.Play();
    }
    /// <summary>
    /// Plays and AudioClip with spatial blending at a specific location with adjustable volume and pitch that loops
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="position"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    /// <returns></returns>
    public static AudioSource PlaySound3DLooped(AudioClip clip, Vector3 position, float volume, float pitch)
    {
        Instance.cacheSource = Instance.GetAudioSource();
        Instance.cacheSource.clip = clip;
        Instance.cacheSource.volume = volume;
        Instance.cacheSource.pitch = pitch;
        Instance.cacheSource.loop = false;
        Instance.cacheSource.spatialBlend = 1.0f;
        Instance.cacheSource.gameObject.transform.position = position;
        Instance.cacheSource.gameObject.SetActive(true);
        Instance.activeSources.Add(Instance.cacheSource);
        Instance.cacheSource.Play();
        return Instance.cacheSource;
    }
    #endregion
}

[Serializable]
public class AudioSequence
{
    public List<AudioClip> clips;
    public bool shuffle;
    public Vector2 volume = new Vector2(0.95f,1);
    public int currentClip = 0;
    public PlayStat waitIfBusy;
    public AudioSource audioSource;
    public bool IsBusy => audioSource.isPlaying;


    public void PlaySound()
    {
        AudioConductor.Instance.StartCoroutine(PlaySoundEnum());
    }
    public void Stop()
    {
        if(audioSource!=null)
            audioSource.Stop();
    }
    private IEnumerator PlaySoundEnum()
    {

        switch (waitIfBusy)
        {
            case PlayStat.Wait:
                if(audioSource != null)
                {
                    while (IsBusy)
                    {
                        yield return null;
                    }
                }
             
                break;
            case PlayStat.Ignore:
                yield break;
        }

        var vol = volume.GetRandom();
        audioSource = AudioConductor.PlaySound2D(clips[currentClip], vol);
        currentClip =  (currentClip +1 )% clips.Count;
    }

public enum PlayStat
    {
        Wait,
        DontWait,
        Ignore
        
    }
}



