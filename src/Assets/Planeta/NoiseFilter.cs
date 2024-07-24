using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFilter
{
    Noise noise;
    NoiseSettings settings;
    readonly bool first;
    readonly int seed;

    public NoiseFilter(NoiseSettings settings, int seed, bool first)
    {
        noise = new Noise(seed);
        this.settings = settings;
        this.first = first;
        this.seed = seed;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;
        float frequency;
        Random.InitState(seed);

        frequency = Random.Range(1.0f, 2.0f);//settings.baseRoughness;
        float amplitude = 1;

        for (int i = 0; i < settings.numLayers; i++)
        {
            float v = noise.Evaluate(point * frequency);
            noiseValue += (v + 1) * .5f * amplitude;
            frequency *= Random.Range(0.5f, 1.2f);//settings.roughness;
            amplitude *= Random.Range(0.5f, 0.65f);//settings.persistence;
        }

        noiseValue = Mathf.Max(0, noiseValue - Random.Range(0.8f, 1.2f));
        if (first)
        {
            return noiseValue * Random.Range(0.02f, 0.07f);
        }
        else
        {
            return noiseValue * Random.Range(0.5f, 1.5f);
        }
    }
}
