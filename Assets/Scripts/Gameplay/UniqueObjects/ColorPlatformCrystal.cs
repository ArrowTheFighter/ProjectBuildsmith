using System;
using System.Collections.Generic;
using UnityEngine;

public class ColorPlatformCrystal : MonoBehaviour
{
    public List<ColorPlatform> colorPlatforms = new List<ColorPlatform>();
    CrystalPlatformColors crystalColor;
    Material crystalMat;

    public void RandomizeCrystal()
    {
        Array values = Enum.GetValues(typeof(CrystalPlatformColors));
        System.Random random = new System.Random();
        int ColorValue = (int)values.GetValue(random.Next(values.Length));

        crystalColor = (CrystalPlatformColors)ColorValue;

        if(crystalMat == null)
        {
            crystalMat = GetComponent<Renderer>().material;
        } 
        
        crystalMat.SetFloat("_SelectedColor",ColorValue);

        foreach(var platform in colorPlatforms)
        {
            platform.SetPlatformCheckColor(crystalColor);
        }
    }
}

public enum CrystalPlatformColors
{
    Yellow,
    Green,
    Red,
    Blue
}