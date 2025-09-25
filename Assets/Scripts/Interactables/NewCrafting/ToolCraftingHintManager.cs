using UnityEngine.UI;
using UnityEngine;

public class ToolCraftingHintManager : MonoBehaviour
{
    public static ToolCraftingHintManager instance;

    public Image FirstSlot;
    public Image SecondSlot;
    public Image ThirdSlot;

    void Awake()
    {
        if (instance != this)
        {
            Destroy(instance);
        }
        instance = this;
    }
    public void SetHints(Sprite firstImage, Sprite secondImage, Sprite thirdImage)
    {
        FirstSlot.sprite = firstImage;
        SecondSlot.sprite = secondImage;
        ThirdSlot.sprite = thirdImage;
    }

    public void SetFirstSlot(Sprite firstImage)
    {
        if (firstImage != null)
        {
            FirstSlot.enabled = true;
            FirstSlot.sprite = firstImage;
        }
        else
        {
            FirstSlot.enabled = false;
        }
    }  

    public void SetSecondSlot(Sprite secondImage)
    {
        if (secondImage != null)
        {
            SecondSlot.enabled = true;
            SecondSlot.sprite = secondImage;
        }
        else
        {

            SecondSlot.enabled = false;
        }
    }

    public void SetThirdSlot(Sprite thirdImage)
    {
        if (thirdImage != null)
        {
            ThirdSlot.enabled = true;
            ThirdSlot.sprite = thirdImage;
        }
        else
        {

            ThirdSlot.enabled = false;
        }
    }

    public void HideHints()
    {
        FirstSlot.enabled = false;
        SecondSlot.enabled = false;
        ThirdSlot.enabled = false;
    }
}
