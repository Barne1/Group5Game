//@Author: Teodor Tysklind / FutureGames / Teodor.Tysklind@FutureGames.nu

using System;
using System.Collections;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private string theme = "MainTheme";
    
    private AudioSource playingMusic;
    private Transform cameraTransform;

    public static MusicPlayer Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        cameraTransform = Camera.main.transform;
        playingMusic = SoundManager.Instance.PlayLoopingSoundAtCamera(theme).GetComponent<AudioSource>();
    }

    public IEnumerator PlayOneShot(String oneShot)
    {
        playingMusic.Stop();
        GameObject oneShotObject = SoundManager.Instance.PlaySound(oneShot, cameraTransform.position);
        AudioSource source = oneShotObject.GetComponent<AudioSource>();

        while (source.isPlaying)
        {
            yield return null;
        }

        playingMusic.Play();
    }
}
