using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController _characterController;

    [SerializeField] private float _movementSpeed = 10f;

    private void Awake() 
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 movementVector = transform.TransformDirection(new Vector3(horizontal, 0f, vertical).normalized);
        movementVector *= _movementSpeed * Time.deltaTime;

        _characterController.Move(movementVector);
    }
}
