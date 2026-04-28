using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPlatformManager : MonoBehaviour
{
    public List<ColorPlatformCrystal> ColorCrystals = new List<ColorPlatformCrystal>();
    public SimonSaysColorCrystalDisplay colorCrystalDispaly;
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

    [ContextMenu("Spawn Platforms")]
    public void SpawnPlatforms()
    {
        StartCoroutine(spawnPlatformsCoroutine());
    }

    IEnumerator spawnPlatformsCoroutine()
    {
        foreach (var crystal in ColorCrystals)
        {
            crystal.SpawnInPlatforms();
            yield return new WaitForSeconds(0.25f);
        }
        yield return new WaitForSeconds(0.75f);
        ShowSequence();
    }

    public void ShowSequence()
    {
        colorCrystalDispaly.ShowSequence();
    }
}
