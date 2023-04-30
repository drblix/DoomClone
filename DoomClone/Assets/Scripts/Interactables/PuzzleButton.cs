using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleButton : MonoBehaviour, I_Interactable
{
    [SerializeField] private PuzzlePrimary _primaryConsole;

    [SerializeField] private AudioSource _interactSound;
    [SerializeField] private SpriteRenderer _iconDisplay;
    [SerializeField] private PuzzlePrimary.Shape _shape;

    private int _currentIndex = 0;

    private void Awake() 
    {
        _iconDisplay.color = _primaryConsole.consoleColor;
        _iconDisplay.sprite = _primaryConsole.GetSprite(_shape);
        _currentIndex = _primaryConsole.GetCodeAtIndex(_shape);
    }

    public void Interact()
    {
        if (!_primaryConsole.interactable)
            return;

        _interactSound.Play();

        _currentIndex++;
        if (_currentIndex >= 4)
            _currentIndex = 0;

        _iconDisplay.sprite = _primaryConsole.ConsoleInteract(_shape, _currentIndex);
    }

    public string GetPrompt()
    {
        return "Terminal\nF to interact";
    }
}
