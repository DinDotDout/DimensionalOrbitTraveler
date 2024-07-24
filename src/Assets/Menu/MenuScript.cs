using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{

    public static int dificultad;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("dif", 1);
    }

    public void PlayGame ()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame ()
    {
        //Application.Quit();
    }

    public void EasyMode ()
    {
        dificultad = 1;
        PlayerPrefs.SetInt("dif",1);
    }

    public void NormalMode ()
    {
        dificultad = 2;
        PlayerPrefs.SetInt("dif", 2);
    }

    public void HardMode ()
    {
        dificultad = 3;
        PlayerPrefs.SetInt("dif", 3);
    }
}
