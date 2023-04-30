using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Transform _playerCam, _playerUI, _promptAnchor;
    [SerializeField] private float _interactDistance = 6f;

    private int _lastInstance = -1;

    private void Update()
    {
        Ray ray = new Ray(_playerCam.position, _playerCam.forward);

        if (Physics.Raycast(ray, out RaycastHit info, _interactDistance))
        {
            if (info.collider && info.collider.TryGetComponent<I_Interactable>(out I_Interactable interactable))
            {
                // check to make sure we aren't looking at the same object repeatedly
                if (info.collider.GetInstanceID() != _lastInstance && !AlreadyPrompt())
                {
                    _lastInstance = info.collider.GetInstanceID();
                    TextPrompt newPrompt = TextPrompt.Create(interactable.GetPrompt());
                    newPrompt.transform.SetParent(_playerUI);
                    newPrompt.transform.localScale = Vector3.one;
                    newPrompt.transform.localPosition = _promptAnchor.localPosition;
                }


                if (Input.GetKeyDown(KeyCode.F))
                {
                    Debug.Log("Found interactable!");
                    interactable.Interact();
                }
            }
            else
                _lastInstance = -1;
        }
    }

    public bool AlreadyPrompt() => _playerUI.Find("TextPrompt(Clone)");
}
