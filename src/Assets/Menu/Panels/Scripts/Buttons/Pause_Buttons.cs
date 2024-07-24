using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause_Buttons : MonoBehaviour
{
    [SerializeField] GameObject nextMenu;
    [SerializeField] GameObject endMenu;
    [SerializeField] GameObject ControlsGenerals;

    public void Resume()
    {
        ControlsGenerals.GetComponent<Controls_generals>().PauseGame();
        gameObject.SetActive(false);
    }

    public void Next()
    {
        if (Controls_generals.end_screen)
        {
            endMenu.SetActive(true);
        } else
        {
            nextMenu.SetActive(true);
        }
        gameObject.SetActive(false);
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        Controls_generals.game_paused = false;
        Controls_generals.end_screen = false;
        LoadingManager.escena_seguent = 0;
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
    }

    public void Skip()
    {
        ControlsGenerals.GetComponent<Controls_generals>().ChangeMode();
        ControlsGenerals.GetComponent<Controls_generals>().PauseGame();
        gameObject.SetActive(false);
    }
}
