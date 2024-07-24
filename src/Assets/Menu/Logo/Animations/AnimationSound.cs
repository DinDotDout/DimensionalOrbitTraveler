using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSound : MonoBehaviour
{
    public AudioSource[] audios;
   
    private void Start()
    {
        audios = GetComponents<AudioSource>();
    }

    void Play_1()
    {
        audios[0].Play();
    }

    void Play_2()
    {
        audios[1].Play();
    }
}
