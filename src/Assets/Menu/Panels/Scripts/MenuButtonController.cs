using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MenuButtonController : MonoBehaviour {
	public AudioSource audioSource;

	void Start () {
		audioSource = GetComponents<AudioSource>()[0]; // Efectes
	}	
}
