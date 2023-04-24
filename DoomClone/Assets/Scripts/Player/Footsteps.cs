using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public enum Surface
    {
        Concrete,
        Metal,
        Tile
    }

    private PlayerMovement _playerMovement;

    [SerializeField] private AudioClip[] _concreteSteps;
    [SerializeField] private AudioClip[] _metalSteps;
    [SerializeField] private AudioClip[] _tileSteps;
    [SerializeField] private AudioClip[] _currentClips;

    [SerializeField] private AudioSource _stepSource;
    
    [Tooltip("Time to wait between each step (in seconds)")]
    [SerializeField] private float _stepTimer = .2f;
    private float _stepClock = 0f;

    private void Awake() 
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update() 
    {
        if (_playerMovement.IsMoving())
        {
            if (_stepClock > _stepTimer)
            {
                _stepClock = 0f;

                int index = Random.Range(0, _currentClips.Length);
                
                _stepSource.clip = _currentClips[index];
                _stepSource.Play();
            }

            _stepClock += Time.deltaTime;
        }
        else
        {
            if (!_stepSource.isPlaying)
                _stepClock = _stepTimer;
        }

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit info, 2f))
        {
            if (info.collider)
            {
                if (info.collider.CompareTag("Concrete"))
                    _currentClips = _concreteSteps;
                else if (info.collider.CompareTag("Metal"))
                    _currentClips = _metalSteps;
                else if (info.collider.CompareTag("Tile"))
                    _currentClips = _tileSteps;
                else
                    _currentClips = _concreteSteps;
            }
        }
    }
}
