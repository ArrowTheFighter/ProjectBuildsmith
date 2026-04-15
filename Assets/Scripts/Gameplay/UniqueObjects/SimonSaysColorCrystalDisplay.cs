using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SimonSaysColorCrystalDisplay : MonoBehaviour
{
    public float Delay;
    public List<ColorPlatformCrystal> colorCrystals = new List<ColorPlatformCrystal>();
    bool playingSequence;
    Material material;

    void Start()
    {
        SetMaterialColor(4);
    }

    [ContextMenu("Show Sequence")]
    public void ShowSequence()
    {
        StartCoroutine(ColorSequence());
    }

    IEnumerator ColorSequence()
    {
        if(playingSequence) yield break;
        playingSequence = true;

        for (int i = 0; i < colorCrystals.Count; i++)
        {
            SetMaterialColor((int)colorCrystals[i].crystalColor);
            yield return new WaitForSeconds(Delay * 0.5f);
            SetMaterialColor(4);
            yield return new WaitForSeconds(Delay * 0.5f);
        }

        playingSequence = false;
    }

    void SetMaterialColor(int color)
    {
        if(material == null) material = GetComponent<Renderer>().material;

        material.SetFloat("_SelectedColor",color);
    }



}
