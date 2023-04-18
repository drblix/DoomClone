using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerCamera : MonoBehaviour
{   
    private PlayerMovement _playerMovement;
    private CharacterController _characterController;

    [SerializeField] private Transform mainCam;

    [SerializeField] private float _mouseSensitivity = 50f;

    [SerializeField] private float _tiltSpeed = 5f;
    [SerializeField] private float _tiltIntensity = 1f;

    [Header("Head Bob Settings")]
    [SerializeField] private float _bobIntensity = .1f;
    [SerializeField] private float _bobFrequency = .1f;

    private float bobTime = 0f;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerMovement = GetComponent<PlayerMovement>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() 
    {
        PlayerRotation();
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
        float headY = 0f;

        if (_playerMovement.IsMoving())
        {
            bobTime += Time.deltaTime;
            headY = Mathf.Sin(bobTime / _bobFrequency) * _bobIntensity;
            mainCam.localPosition = new Vector3(0f, headY, 0f);
        }
        else
        {
            bobTime = 0f;
            mainCam.localPosition = Vector3.Lerp(mainCam.localPosition, Vector3.zero, Time.deltaTime * _bobIntensity);
        }       
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
