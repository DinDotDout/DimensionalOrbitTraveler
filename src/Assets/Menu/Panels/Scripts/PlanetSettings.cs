using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetSettings : MonoBehaviour
{
    [SerializeField] Planeta planeta;

    [SerializeField] Slider radi;
    [SerializeField] Slider densitat;

    [SerializeField] Dropdown bioma;

    [SerializeField] InputField seed;

    [SerializeField] Transform grid;


    bool disable;

    float radiLast;
    float densitatLast;
    int biomaLast;
    string seedLast;

    void Start()
    {
        // Set last value
        radiLast = planeta.shapeSettings.planetRadius;
        densitatLast = Planeta.densitat_objectes;
        biomaLast = (int)Planeta.bioma;
        seedLast = Planeta.planet_seed.ToString();

        // Set values in Panel
        disable = true;
        radi.value = radiLast;
        densitat.value = densitatLast;
        bioma.value = biomaLast;
        seed.text = seedLast;
        disable = false;
    }
    public void setRadi(float r)
    {
        radiLast = r;
        if (disable) return;
        grid.localScale = Vector3.one * ((r * 2));
        planeta.shapeSettings.planetRadius = r;
        GeneratePlanet();
    }

    public void setDensity(float d)
    {
        densitatLast = d;
        if (disable) return;
        Planeta.densitat_objectes = d;
        planeta.GenerateItems();
    }

    public void setBiome(int i)
    {
        biomaLast = i;
        if (disable) return;
        Planeta.bioma = (BiomaEnum)i;
        planeta.UpdateBiome();
        planeta.GenerateItems();
        //GeneratePlanet();
    }

    public void setSeed(string s)
    {
        seedLast = s;
        if (disable) return;
        int.TryParse(s, out int r);
        Planeta.planet_seed = r;
        GeneratePlanet();
    }

    void GeneratePlanet()
    {
        disable = true;
        planeta.GeneratePlanetMenu();
        StartCoroutine(GeneratePlanetWait());
    }

    IEnumerator GeneratePlanetWait()
    {
        while (!planeta.Finished)
        {
            yield return null;
        }
        disable = false;

        bool change = false;
        if (radiLast != planeta.shapeSettings.planetRadius)
        {
            planeta.shapeSettings.planetRadius = radiLast;
            grid.localScale = Vector3.one * ((radiLast * 2));
            change = true;
        }
        if (densitatLast != Planeta.densitat_objectes)
        {
            Planeta.densitat_objectes = densitatLast;
            planeta.GenerateItems();
        }
        if (biomaLast != (int)Planeta.bioma)
        {
            Planeta.bioma = (BiomaEnum)biomaLast;
            planeta.UpdateBiome();
            planeta.GenerateItems();
        }
        int.TryParse(seedLast, out int seed);
        if (seed != Planeta.planet_seed)
        {
            Planeta.planet_seed = seed;
            change = true;
        }
        if (change) GeneratePlanet();
    }

    void OnEnable()
    {
        if (!planeta.Finished)
        {
            disable = true;
            StartCoroutine(GeneratePlanetWait());
        }
        else
            disable = false;
    }
}
