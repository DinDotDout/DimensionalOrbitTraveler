using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start_Button : MonoBehaviour
{
    public void Play()
    {
        LoadingManager.escena_seguent = 1;
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
    }
}
