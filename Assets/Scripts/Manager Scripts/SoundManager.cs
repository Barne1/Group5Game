//@Author: Teodor Tysklind / FutureGames / Teodor.Tysklind@FutureGames.nu

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[DefaultExecutionOrder(-20)]

public class SoundManager : MonoBehaviour
{
    private Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>();
    private Dictionary<string, float> volume = new Dictionary<string, float>();
    private Dictionary<string, AudioMixerGroup> audioMixerGroups = new Dictionary<string, AudioMixerGroup>();

    private GameObject soundObject;
    private Transform mainCameraTransform;

    [SerializeField] private KeyValue[] soundList;
    [SerializeField] private AudioMixer mixer;
    
    public static SoundManager Instance;

    [Serializable]
    public struct KeyValue
    {
        public string audioName;
        public AudioClip audioClip;
        [Range(0f, 1f)] public float volume;
        public AudioMixerGroup mixerGroup;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            soundObject = CreateSoundObject();
            Debug.Log(soundObject.GetComponent<AudioSource>().volume);
            PoolManager.Instance.CreatePool(soundObject);
        }

        for (int i = 0; i < soundList.Length; i++)
        {
            sounds.Add(soundList[i].audioName, soundList[i].audioClip);
            volume.Add(soundList[i].audioName, soundList[i].volume);
            audioMixerGroups.Add(soundList[i].audioName, soundList[i].mixerGroup);
        }

        mainCameraTransform = Camera.main.transform;
    }

    public GameObject PlaySound(string soundName, Vector3 position)
    {
        GameObject audioObject = PoolManager.Instance.ReuseObject(soundObject, position, Quaternion.identity);
        AudioSource audioSource = audioObject.GetComponent<AudioSource>();
        audioSource.clip = sounds[soundName];
        audioSource.volume = volume[soundName];
        audioSource.outputAudioMixerGroup = audioMixerGroups[soundName];

        audioSource.loop = false;
        audioSource.Play();
        
        return audioObject;
    }
    
    public GameObject PlayLoopingSoundAtCamera(string soundName, bool playInstantly = true)
    {
        GameObject loopingSoundObject = CreateSoundObject();

        loopingSoundObject.transform.position = mainCameraTransform.position;
        loopingSoundObject.transform.SetParent(mainCameraTransform);

        AudioSource audioSource = loopingSoundObject.GetComponent<AudioSource>();
        audioSource.clip = sounds[soundName];
        audioSource.volume = volume[soundName];
        audioSource.outputAudioMixerGroup = audioMixerGroups[soundName];
        
        audioSource.loop = true;
        audioSource.Play();

        return loopingSoundObject;
    }
    
    private GameObject CreateSoundObject()
    {
        GameObject newSoundObject = Instantiate(new GameObject());
        newSoundObject.AddComponent<AudioSource>();

        return newSoundObject;
    }

    public void AdjustGlobalSoundVolume(float volumeInPercent)
    {
        float minValue = -80;
        float maxValue = 20;

        float mixerVolume = Mathf.Lerp(minValue, maxValue, volumeInPercent);
        mixer.SetFloat("MasterVolume", mixerVolume);
    }

    public float GetCurrentMasterVolume()
    {
        float masterVolume;
        mixer.GetFloat("MasterVolume", out masterVolume);

        return masterVolume;
    }

    public float GetLerpedMasterVolume()
    {
        float minValue = -80;
        float maxValue = 20;
        
        float masterVolume = GetCurrentMasterVolume();
        return Mathf.InverseLerp(minValue, maxValue, masterVolume);
    }
}
