using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{

    private int forced;
    public static int escena_seguent;

    void Start()
    {
        forced = 0;
    }

    private void Update()
    {
        switch (forced)
        {
            case 0:
                //if(unload.buildIndex == 0)
                //{
                //    load = 1;
                //} else
                //{
                //    load = 0;
                //}
                //SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(2));
                //SceneManager.UnloadSceneAsync(unload);
                break;
            case 30:

                SceneManager.LoadSceneAsync(escena_seguent, LoadSceneMode.Single);
                break;
        }

        forced++;
    }
}
