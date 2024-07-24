using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorFunctions : MonoBehaviour
{
	[SerializeField] MenuButtonController menuButtonController;
	private bool disabled = false;

	public bool Disabled { get => disabled; set => disabled = value; }

	void PlaySound(AudioClip whichSound){
		if(!Disabled){
			menuButtonController.audioSource.PlayOneShot (whichSound);
		}
	}
}	
