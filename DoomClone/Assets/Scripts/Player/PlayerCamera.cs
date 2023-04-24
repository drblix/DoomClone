using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerCamera : MonoBehaviour
{   
    public bool doHeadBob { get; set; } = true;

    private PlayerMovement _playerMovement;
    private CharacterController _characterController;

    [SerializeField] private Transform mainCam;

    [SerializeField] private float _mouseSensitivity = 50f;

    [SerializeField] private float _tiltSpeed = 5f;
    [SerializeField] private float _tiltIntensity = 1f;

    [Header("Head Bob Settings")]
    [SerializeField] private float _bobAmplitude = .1f;
    [SerializeField] private float _bobFrequency = .1f;
    [SerializeField] private float _bobSmoothing = 1f;

    private float bobTime = 0f;

    private Vector3 _startingPos;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerMovement = GetComponent<PlayerMovement>();
        Cursor.lockState = CursorLockMode.Locked;
        _startingPos = mainCam.localPosition;
    }

    private void Update() 
    {
        PlayerRotation();
        if (doHeadBob)
            HeadBob();
        HeadTilt();
    }
    
    private void PlayerRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
        transform.Rotate(0f, mouseX, 0f);
    }

    private void HeadBob()
    {   
        Vector3 goalPos;
        float headY = 0f;

        if (_playerMovement.IsMoving())
        {
            headY = Mathf.Sin(bobTime / _bobFrequency) * _bobAmplitude;
            goalPos = new Vector3(0f, headY, 0f);
            bobTime += Time.deltaTime;
        }
        else
        {
            bobTime = 0f;
            goalPos = _startingPos;
        }
        
        mainCam.localPosition = Vector3.Lerp(mainCam.localPosition, goalPos, _bobSmoothing);
    }

    private void HeadTilt()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float speed = Time.deltaTime * _tiltSpeed;

        if (horizontal != 0)
        {
            Quaternion to = Quaternion.Euler(Vector3.forward * _tiltIntensity * -horizontal);
            mainCam.localRotation = Quaternion.Lerp(mainCam.localRotation, to, speed);
        }
        else
            mainCam.localRotation = Quaternion.Lerp(mainCam.localRotation, Quaternion.Euler(Vector3.zero), speed);
    }
}
