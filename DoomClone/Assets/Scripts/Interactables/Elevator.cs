using System.Collections;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [Header("Audio")]

    [SerializeField] private AudioClip[] _elevatorClips;
    [SerializeField] private AudioSource _elevatorSource;

    [Header("Properties")]

    [SerializeField] private float _waitTime;
    [SerializeField] private float _speed;
    [SerializeField] private bool _oneTimeUse, _stayAtDestination;
    [SerializeField] private Vector3 _destination;

    private Vector3 _startingPos;
    private Transform _playerTransform;
    private CharacterController _playerController;
    private bool _moving = false;
    private bool _playerOn = false;
    private float _timer = 0f;

    private void Awake()
    {
        _startingPos = transform.parent.position;
        _playerTransform = Camera.main.transform.parent.parent;
        _playerController = _playerTransform.GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (_playerOn && !_moving)
        {
            if (_timer > _waitTime)
            {
                StartCoroutine(MoveElevator());
                _timer = 0f;
            }
            else
                _timer += Time.deltaTime;
        }
        else
            _timer = 0f;
    }

    private IEnumerator MoveElevator()
    {
        _moving = true;
        Vector3 goal;

        if (transform.parent.position != _destination)
        {
            goal = _destination;
            StartCoroutine(MoveLoop(goal));
        }
        else
        {
            goal = _startingPos;
            StartCoroutine(MoveLoop(goal));
        }

        if (!_stayAtDestination)
        {
            yield return new WaitWhile(() => _playerOn);
            yield return new WaitUntil(() => transform.parent.position == goal);
            yield return new WaitForSeconds(.1f);

            if (transform.parent.position != _destination)
            {
                goal = _destination;
                StartCoroutine(MoveLoop(goal));
            }
            else
            {
                goal = _startingPos;
                StartCoroutine(MoveLoop(goal));
            }
        }

        yield return new WaitUntil(() => transform.parent.position == goal);
        if (!_oneTimeUse)
            _moving = false;
    }

    private IEnumerator MoveLoop(Vector3 goal)
    {
        _elevatorSource.clip = _elevatorClips[0];
        _elevatorSource.Play();

        while (transform.parent.position != goal)
        {
            if (!_elevatorSource.isPlaying)
            {
                _elevatorSource.clip = _elevatorClips[1];
                _elevatorSource.loop = true;
                _elevatorSource.Play();
            }

            Vector3 lastPos = transform.parent.position;
            transform.parent.position = Vector3.MoveTowards(transform.parent.position, goal, Time.deltaTime * _speed);

            // moving player with the elevator if still on
            if (_playerOn)
                _playerController.Move(transform.parent.position - lastPos);

            yield return new WaitForEndOfFrame();
        }

        transform.parent.position = goal;

        _elevatorSource.Stop();
        _elevatorSource.loop = false;
        _elevatorSource.clip = _elevatorClips[2];
        _elevatorSource.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        _playerOn = other.CompareTag("Player");
    }

    private void OnTriggerExit(Collider other)
    {
        _playerOn = !other.CompareTag("Player");
    }
}
