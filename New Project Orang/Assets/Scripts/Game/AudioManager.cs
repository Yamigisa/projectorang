using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public Sound[] musicSounds;
    public Sound[] sfxSounds;
    public AudioSource musicSource,sfxSource;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
            PlayMusic("Main Menu Theme");
    }
    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x=> x.name == name);

        if(s == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            musicSource.clip = s.clip;  
            musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if(s == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }
}
