using UnityEngine;
using TMPro;

public class TextFormater : MonoBehaviour
{
    TextMeshProUGUI textMeshProUGUI;
    string rawStoredText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        rawStoredText = textMeshProUGUI.text;
        ReformatText();
    }

    void OnEnable()
    {
        UIIconHandler.instance.InputDeviceChanged += ReformatText;
    }

    void OnDisable()
    {
        UIIconHandler.instance.InputDeviceChanged -= ReformatText;
    }

    public void SetText(string rawText)
    {
        rawStoredText = rawText;
        ReformatText();
    }

    public void ReformatText()
    {
        if(textMeshProUGUI == null) textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        string formatedText = UIIconHandler.instance.FormatText(rawStoredText);
        textMeshProUGUI.text = formatedText;
    }
}
