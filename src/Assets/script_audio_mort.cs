using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class script_audio_mort : MonoBehaviour
{

    public void Play(AudioSource source)
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = source.clip;
        audio.volume = source.volume - (1 - audio.volume);
        audio.spatialBlend = source.spatialBlend;
        audio.Play();
        Invoke("Delete", 3);
    }

    private void Delete()
    {
        Destroy(gameObject);
    }
}
