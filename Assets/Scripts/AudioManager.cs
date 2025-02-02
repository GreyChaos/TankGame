using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AudioManager : NetworkBehaviour
{
    public static AudioManager Instance;
    public AudioSource audioSource;
    public List<AudioClip> audioClips;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.M)){
            MuteSound();
        }
    }

    public void MuteSound(){
        if(audioSource.volume == 0f){
            audioSource.volume = 1f;
        }else{
            audioSource.volume = 0f;
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

