using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] _deathClips, _spottedClips, _stepClips;
    [SerializeField] private AudioClip _shootClip;
    [SerializeField] private AudioSource _enemySource, _stepSource, _blasterSource;
    [SerializeField] private float _stepWait = .2f;
    [SerializeField] private float _voiceLineWait = 10f;
    private float _stepTimer = 0f;
    private float _lineTimer = 0f;

    private EnemyAI _enemyAI;

    private void Awake() 
    {
        _enemyAI = GetComponent<EnemyAI>();
        _blasterSource.clip = _shootClip;
        _lineTimer = _voiceLineWait;
    }

    private void Update() 
    {
        if (_enemyAI.IsMoving())
        {
            if (_stepTimer > _stepWait)
            {
                _stepSource.clip = _stepClips[Random.Range(0, _stepClips.Length)];
                _stepSource.Play();
                _stepTimer = 0f;
            }
            _stepTimer += Time.deltaTime;
        }

        if (_enemyAI.CanSeePlayer() && !_enemyAI.passive)
        {
            if (_lineTimer > _voiceLineWait)
            {
                _lineTimer = 0f;
                _enemySource.clip = _spottedClips[Random.Range(0, _spottedClips.Length)];
                _enemySource.Play();
            }

            _lineTimer += Time.deltaTime;
        }
    }

    // enemy died
    private void DisableEnemy_Broadcast()
    {
        _enemySource.Stop();
        _enemySource.clip = _deathClips[Random.Range(0, _deathClips.Length)];
        _enemySource.Play();
        enabled = false;
    }
    
    public void Shoot() => _blasterSource.Play();
}
