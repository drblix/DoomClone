using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, ILockable
{
    [SerializeField] private AudioSource _doorSource;
    [SerializeField] private Transform _doorMesh;
    [Min(2.5f)] [SerializeField] private float _yIncrease;
    [SerializeField] private float _openTime, _speed;

    [Header("Locked Properties")]
    [SerializeField] private bool _locked = false;
    [SerializeField] private SpriteRenderer _glowRenderer;


    private Vector3 _closePos, _openPos;
    private bool _entityIn = false, _running = false;

    private void Awake() 
    {
        _closePos = _doorMesh.localPosition;
        _openPos = new Vector3(_doorMesh.localPosition.x, _doorMesh.localPosition.y + _yIncrease, _doorMesh.localPosition.z);

        if (_locked && _glowRenderer)
            _glowRenderer.color = Color.red;
    }

    private IEnumerator ToggleDoor()
    {
        _running = true;
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

        _running = false;
    }

    private void OnTriggerEnter(Collider other) 
    {
        if ((other.CompareTag("Player") || other.CompareTag("Enemy")) && !_locked)
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

    public void Unlock()
    {
        _locked = false;
        if (_glowRenderer)
            _glowRenderer.color = Color.green;
    }

    public bool SetLocked(bool s) => _locked = s;
}
