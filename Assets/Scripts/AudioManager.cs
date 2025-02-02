using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class AudioManager : NetworkBehaviour
{
    public static AudioManager Instance;
    public AudioSource audioSource;
    public AudioSource musicSource;
    public List<AudioClip> audioClips;
    public List<AudioClip> musicClips;
    public float MusicVolumeCap = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        musicSource.loop = true;
        musicSource.clip = musicClips[0];
        musicSource.Play();
    }

    private void Start(){
        if(musicSource.isPlaying){
            musicSource.Stop();
            musicSource.clip = musicClips[0];
            musicSource.volume = PlayerPrefs.GetFloat("MusicVolume") * MusicVolumeCap;
            musicSource.Play();
        }
        audioSource.volume = PlayerPrefs.GetFloat("EffectVolume");
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.M)){
            MuteSound();
        }
    }

    public void MuteSound(){
        if(audioSource.volume == 0f){
            audioSource.volume = PlayerPrefs.GetFloat("EffectVolume");
        }else{
            audioSource.volume = 0f;
        }
        if(musicSource.volume == 0f){
            musicSource.volume = PlayerPrefs.GetFloat("MusicVolume") * MusicVolumeCap;
        }else{
            musicSource.volume = 0f;
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public AudioClip GetAudioClipByName(string name)
    {
        return audioClips.Find(clip => clip.name == name);
    }
}

