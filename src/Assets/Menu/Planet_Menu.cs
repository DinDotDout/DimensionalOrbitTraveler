using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Planet_Menu : MonoBehaviour
{
    [SerializeField] Planeta planeta;
    [SerializeField] Transform grid;

    void Start()
    {
        System.Random rand = new System.Random();
        float planetRadius = rand.Next(21) + 15;
        grid.localScale = Vector3.one * ((planetRadius * 2) - 1);
        planeta.shapeSettings.planetRadius = planetRadius;
        Planeta.resolution = 100;
        Planeta.planet_seed = rand.Next(Int32.MinValue, Int32.MaxValue);
        Planeta.densitat_objectes = rand.Next(6)+2;
        Planeta.bioma = (BiomaEnum)rand.Next(3);
        planeta.GeneratePlanet();
        //planeta.GenerateItems();

        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(0));
            SceneManager.UnloadSceneAsync(2);
        }
    }
}
