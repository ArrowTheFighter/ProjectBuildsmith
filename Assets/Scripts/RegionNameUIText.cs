using UnityEngine;
using TMPro;
using DG.Tweening;

public class RegionNameUIText : MonoBehaviour
{
    public GameObject regionTextObject;
    private CanvasGroup canvasGroup;

    //Reference to the Canvas' region Text UI element that is displayed to the player
    public TextMeshProUGUI regionText;

    //The amount of time it takes for the text to fade in and out
    //The amount of time the text is displayed on screen after fading in and before fading out
    public float fadeTime;
    public float textDisplayedTime;

    //Strings for the player's tag
    private string PLAYER = "Player";

    //The string we set in editor for the region's name that will be displayed to the player
    public string currentRegionString;

    [SerializeField] string localized_string_key;

    //Bool to see if the text for that region has already been shown to the player
    private bool regionAlreadyDisplayed;

    //Bool to queue up the text if the player rapidly switches between zones
    private bool queueUpText;

    
    public void Awake()
    {
        canvasGroup = regionTextObject.GetComponent<CanvasGroup>();
    }

    //Happens when the player enters the collision's area
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PLAYER))
        {
            //Checks to see if the text for the region has already been shown to the player (this might not even be needed anymore?)
            if (!regionAlreadyDisplayed)
            {
                //Checks to see if there is a change in the region text to not repeat the text on screen for a region
                if (regionText.text != currentRegionString)
                {
                    queueUpText = true;
                } 
            }
        }
    }

    private void Update()
    {
        //Checks to see if the alpha of the text is 0 (using 0 as a value was not working for some reason, so I used a very small number)
        if (canvasGroup.alpha <= 0.01)
        {
            //If the player has rapidly changed regions, putting this in the update method will allow a queued up region text to be shown to the player, even after the trigger event has been called
            if (queueUpText)
            {
                //Resets the queue bool so the code only happens once
                //Fades in the text and sets the UI text to the string of the region the player entered
                //Invokes a method to fade the text back out after a certain amount of time (I do not know how to properly use Coroutines so I just use Invoke to make it easier for myself... I mean it works
                queueUpText = false;
                canvasGroup.DOFade(1, fadeTime).SetEase(Ease.OutSine);
                string localized_string = LocalizationManager.GetLocalizedString("Menu Lables",localized_string_key);
                regionText.text = localized_string;
                Invoke("FadeText", textDisplayedTime);
            }
        }
    }

    //When the player leaves a region, it resets the bool of it the region has already been shown to the player (this might not even be needed anymore?)
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(PLAYER))
        {
            regionAlreadyDisplayed = false;
        }
    }

    //Fades out the text when called in the Update method
    private void FadeText()
    {
        canvasGroup.DOFade(0, fadeTime).SetEase(Ease.InSine);
    }
}
