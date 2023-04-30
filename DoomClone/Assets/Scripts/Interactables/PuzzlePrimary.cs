using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePrimary : MonoBehaviour
{

    public enum Shape
    {
        Corner,
        Circle,
        Triangle
    }

    public bool interactable { get; private set; } = true;

    [SerializeField] public Color consoleColor;
    [SerializeField] private SpriteRenderer _triRenderer, _circleRenderer, _cornerRenderer;
    [SerializeField] private SpriteRenderer[] _loadingDots;
    [SerializeField] private Sprite[] _corners, _circles, _tris;
    [Range(0, 3)] [SerializeField] private int[] _enteredCode = { 0, 0, 0 };
    [SerializeField] private int _correctCode = 132;
    [SerializeReference] private MonoBehaviour _unlockable;

    private AudioSource _correctSource;

    private void Awake() 
    {
        for (int i = 0; i < _enteredCode.Length; i++)
            _enteredCode[i] = Random.Range(0, 4);

        _triRenderer.color = _circleRenderer.color = _cornerRenderer.color = consoleColor;
        _cornerRenderer.sprite = _corners[_enteredCode[0]];
        _circleRenderer.sprite = _circles[_enteredCode[1]];
        _triRenderer.sprite = _tris[_enteredCode[2]];

        _correctSource = GetComponent<AudioSource>();
    }

    public Sprite ConsoleInteract(Shape shape, int index)
    {
        int spritesIndex = (int)shape;
        _enteredCode[spritesIndex] = index;
        CheckCode();

        return GetSprite(shape);
    }

    public Sprite GetSprite(PuzzlePrimary.Shape shape)
    {
        int spritesIndex = (int)shape;
        
        switch (spritesIndex)
        {
            case 0:
                _cornerRenderer.sprite = _corners[_enteredCode[spritesIndex]];
                return _corners[_enteredCode[spritesIndex]];
            case 1:
                _circleRenderer.sprite = _circles[_enteredCode[spritesIndex]];
                return _circles[_enteredCode[spritesIndex]];
            default:
                _triRenderer.sprite = _tris[_enteredCode[spritesIndex]];
                return _tris[_enteredCode[spritesIndex]];
        }
    }

    private void CheckCode()
    {
        int sum = 0, multiplier = 100;

        for (int i = 0; i < _enteredCode.Length; i++)
        {
            sum += _enteredCode[i] * multiplier;
            multiplier /= 10;
        }

        if (sum == _correctCode)
            StartCoroutine(CorrectSequence());
    }

    private IEnumerator CorrectSequence()
    {
        // Stopping terminal from being used any further
        interactable = false;
        _correctSource.Play();

        _triRenderer.enabled = _circleRenderer.enabled = _cornerRenderer.enabled = false;
        
        // Loading dots appearing slowly on screen
        float wait = _correctSource.clip.length / 4f;
        for (int i = 0; i < _loadingDots.Length + 1; i++)
        {
            yield return new WaitForSeconds(wait);
            if (i < _loadingDots.Length)
                _loadingDots[i].enabled = true;
        }

        // Hiding all loading dots
        for (int i = 0; i < _loadingDots.Length; i++)
            _loadingDots[i].enabled = false;

        // Displaying that the player was correct
        _triRenderer.enabled = _circleRenderer.enabled = _cornerRenderer.enabled = true;
        _triRenderer.color = _circleRenderer.color = _cornerRenderer.color = Color.green;

        // unlocking path
        _unlockable.GetComponent<ILockable>().Unlock();
    }

    public int GetCodeAtIndex(Shape shape) => _enteredCode[(int)shape];
}
