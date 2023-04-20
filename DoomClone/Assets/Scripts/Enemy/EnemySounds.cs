using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] _deathClips, _spottedClips, _stepClips;
    [SerializeField] private AudioSource _enemySource, _stepSource;
    [SerializeField] private float _stepWait = .2f;
    private float _stepTimer = 0f;

    private EnemyAI _enemyAI;

    private void Awake() 
    {
        _enemyAI = GetComponent<EnemyAI>();
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
    }

    // enemy died
    private void DisableEnemy_Broadcast()
    {
        _enemySource.clip = _deathClips[Random.Range(0, _deathClips.Length)];
        _enemySource.Play();
    }
}
