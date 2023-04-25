using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBob : MonoBehaviour
{
    private PlayerMovement _playerMovement;
    private RectTransform _rect;

    private float _bobTime = 0f;
    [SerializeField] private float _horizontalAmplitude = 1f;
    [SerializeField] private float _verticalAmplitude = 1f;
    [SerializeField] private float _bobFrequency = 1f;
    [SerializeField] private float _bobSmoothing = 4f;

    [SerializeField] private Vector3 _startingPos;

    private void Awake() 
    {
        _rect = GetComponent<RectTransform>();
        _playerMovement = FindObjectOfType<PlayerMovement>();
    }


    private void Update() 
    {
        Vector3 goalPos;

        if (_playerMovement.IsMoving())
        {
            float bobX = _startingPos.x;
            float bobY = _startingPos.y;

            bobX += Mathf.Cos(2 * _bobTime * _bobFrequency) * _horizontalAmplitude;
            bobY += Mathf.Sin(_bobTime * _bobFrequency) * _verticalAmplitude;

            goalPos = new Vector3(bobX, bobY, 0f);

            _bobTime += Time.deltaTime;
        }
        else
        {
            _bobTime = 0f;
            goalPos = _startingPos;
        }


        //Debug.Log(goalPos);
        //Debug.Log(_rect.localPosition);
        _rect.localPosition = Vector3.Lerp(_rect.localPosition, goalPos, _bobSmoothing);
    }
}
