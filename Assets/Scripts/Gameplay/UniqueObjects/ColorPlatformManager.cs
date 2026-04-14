using System.Collections.Generic;
using UnityEngine;

public class ColorPlatformManager : MonoBehaviour
{
    public List<ColorPlatformCrystal> ColorCrystals = new List<ColorPlatformCrystal>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RandomizeCrystals();
    }

    public void RandomizeCrystals()
    {
        foreach(var crystal in ColorCrystals)
        {
            crystal.RandomizeCrystal();
        }
    }
}
