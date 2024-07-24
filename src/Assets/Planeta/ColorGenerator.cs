using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGenerator
{
    ColorSettings settings;
    Texture2D texture;
    const int textureResolution = 1024;

    public void UpdateSettings(ColorSettings settings)
    {
        this.settings = settings;
        if(texture == null)
        {
            texture = new Texture2D(textureResolution, 1);
        }
    }

    public void UpdateElevation(MinMax elevationMinMax)
    {
        settings.planetMaterial.SetFloat("_minvalue", elevationMinMax.Min);
        settings.planetMaterial.SetFloat("_maxvalue", elevationMinMax.Max);
    }

    public void UpdateBioma(int bioma)
    {
        settings.planetMaterial.SetInt("_biome", bioma);
    }
}
