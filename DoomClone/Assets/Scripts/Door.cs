using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private AudioSource _doorSource;
    [SerializeField] private Transform _doorMesh;
    [Min(2.5f)] [SerializeField] private float _yIncrease;
    [SerializeField] private float _openTime, _speed;


    private Vector3 _closePos, _openPos;
    private bool _entityIn = false, _doorOpen = false, _running = false;

    private void Awake() 
    {
        _closePos = _doorMesh.localPosition;
        _openPos = new Vector3(_doorMesh.localPosition.x, _doorMesh.localPosition.y + _yIncrease, _doorMesh.localPosition.z);
    }

    private IEnumerator ToggleDoor()
    {
        _running = true;
        _doorOpen = true;
        _doorSource.Play();

        while (_doorMesh.localPosition != _openPos)
        {
            _doorMesh.localPosition = Vector3.MoveTowards(_doorMesh.localPosition, _openPos, Time.deltaTime * _speed);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(_openTime);
        yield return new WaitUntil(() => !_entityIn);

        _doorSource.Play();
        while (_doorMesh.localPosition != _closePos)
        {
            _doorMesh.localPosition = Vector3.MoveTowards(_doorMesh.localPosition, _closePos, Time.deltaTime * _speed);
            yield return new WaitForEndOfFrame();
        }

        _doorOpen = false;
        _running = false;
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            _entityIn = true;
            if (!_running)
                StartCoroutine(ToggleDoor());
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
            _entityIn = false;
    }
}
