using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPrompt : MonoBehaviour
{
    public static TextPrompt Create(string text)
    {
        GameObject newPrompt = Resources.Load<GameObject>("TextPrompt");
        newPrompt = Instantiate(newPrompt, Vector3.zero, Quaternion.identity);

        TextPrompt prompt = newPrompt.GetComponent<TextPrompt>();
        prompt.Setup(text);
        return prompt;
    }

    private PlayerInteraction _playerInteraction;
    private TextMeshProUGUI _promptText;
    private string _textToDisplay;

    private void Awake() 
    {
        _playerInteraction = FindObjectOfType<PlayerInteraction>();
        _promptText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Setup(string text)
    {
        _textToDisplay = text;
        StartCoroutine(TextRoutine());
    }

    private IEnumerator TextRoutine()
    {
        string buildingStr = "";

        foreach (char c in _textToDisplay)
        {
            buildingStr += c;
            _promptText.SetText(buildingStr);
            yield return new WaitForSeconds(.05f);
        }

        _promptText.SetText(_textToDisplay);

        Destroy(gameObject, 2.5f);
    }
}
