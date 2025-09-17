using System.Collections;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject MainMenuCam;
    public GameObject MainMenuCanvas;
    public GameObject MainMenuContents;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //GameplayUtils.instance.OpenMenu();
        StartCoroutine(freezePlayer());
    }

    IEnumerator freezePlayer()
    {
        yield return null;
        GameplayUtils.instance.OpenMenu();
    }

    public void SetGameToPlaying()
    {
        GameplayUtils.instance.CloseMenu();
        MainMenuCam.SetActive(false);
        MainMenuContents.GetComponent<CanvasGroup>().alpha = 0;
    }

    public void HideMainMenuCanvas()
    {
        MainMenuCanvas.SetActive(false);
    }
}
