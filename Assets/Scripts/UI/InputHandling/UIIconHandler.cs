using TMPro;
using UnityEngine;

public class UIIconHandler : MonoBehaviour
{
    public TextMeshProUGUI[] TextBoxes;
    public TMP_SpriteAsset KeyboardIcons;
    public TMP_SpriteAsset PS4Icons;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIInputHandler.instance.OnSchemeChange += UpdateTextBoxesIcons;
    }

    void UpdateTextBoxesIcons(string scheme)
    {
        switch (scheme)
        {
            case "Keyboard&Mouse":
                SetTextBoxesToUseIconSet(KeyboardIcons);
                break;
            case "Gamepad":
                SetTextBoxesToUseIconSet(PS4Icons);
                break;
        }
    }

    void SetTextBoxesToUseIconSet(TMP_SpriteAsset set)
    {
        foreach (TextMeshProUGUI textMesh in TextBoxes)
        {
            textMesh.spriteAsset = set;
         }
     }
}
