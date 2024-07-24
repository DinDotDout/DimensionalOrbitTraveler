using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseButtons : MonoBehaviour
{
    private void Start()
    {
        CanvasGroup cg = gameObject.GetComponent<CanvasGroup>();
        cg.interactable = false;
        cg.alpha = 0;
        StartCoroutine(DoUnfade(cg, cg.alpha, 1));
    }

    private IEnumerator DoUnfade(CanvasGroup cg, float start, float end)//Runto complition beforex
    {
        float Duration = 1.5f;
        float counter = 0f;

        while (counter < Duration)
        {
            counter += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, counter / Duration);
            yield return null; //we don't need a return value.
        }
        cg.interactable = true;
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        LoadingManager.escena_seguent = 0;
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
    }

    public void Retry()
    {
        LoadingManager.escena_seguent = 1;
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
    }
    public void Quit()
    {
        Application.Quit();
    }

}
