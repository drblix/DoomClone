using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{   
    private CharacterController _characterController;
    [SerializeField] private Transform mainCam;
    [SerializeField] private float _mouseSensitivity = 50f;
    [SerializeField] private float _bobIntensity = .1f;
    [SerializeField] private float _bobFrequency = .1f;

    private float bobTime = 0f;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() 
    {
        PlayerRotation();
        HeadBob();
    }
    
    private void PlayerRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
        transform.Rotate(0f, mouseX, 0f);
    }

    private void HeadBob()
    {
        float headY = 0f;

        if (IsMoving())
        {
            bobTime += Time.deltaTime;
            headY = Mathf.Sin(bobTime / _bobFrequency) * _bobIntensity;
        }
        else
            bobTime = 0f;
        
        mainCam.localPosition = new Vector3(0f, headY, 0f);
    }

    private bool IsMoving() => _characterController.velocity.sqrMagnitude > .01f;
}
