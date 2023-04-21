using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController _characterController;

    [SerializeField] private float _movementSpeed = 10f;
    [SerializeField] private float _gravityStrength = 20f;
    [SerializeField] private float _rayDistance = 1f;

    // used if gone out of bounds
    private Vector3 _warpPosition = Vector3.one;

    public enum Facing
    {
        Front,
        Behind,
        Left,
        Right
    }

    private void Awake() 
    {
        // Get a reference to the CharacterController component attached to the game object
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Gravity();
        Movement();
    }

    private void LateUpdate() 
    {   
        // warps player back to center if out of map
        if (_warpPosition != Vector3.one)
            transform.position = _warpPosition;
    }

    private void Movement()
    {
        // Get the horizontal and vertical input from the player
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Create a movement vector based on the input
        Vector3 movementVector = transform.TransformDirection(new Vector3(horizontal, 0f, vertical).normalized);

        // Scale the movement vector by the movement speed and time elapsed since the last frame
        movementVector *= _movementSpeed * Time.deltaTime;

        // Move the character controller based on the movement vector
        _characterController.Move(movementVector);
    }

    private void Gravity()
    {
        if (!IsGrounded())
        {
            _characterController.Move(Vector3.down * _gravityStrength * Time.deltaTime);
            if (transform.position.y < -30f)
            {
                Debug.LogWarning("Out of bounds: resetting");
                _warpPosition = Vector3.up * 3;
            }
            else
                _warpPosition = Vector3.one;
        }
    }

    // Check if the player is currently moving
    public bool IsMoving() => _characterController.velocity.sqrMagnitude > 1f;
    public bool IsGrounded() => Physics.Raycast(transform.position, Vector3.down, _rayDistance);

    public static Facing GetFacing(Transform playerTrans, Transform otherTrans)
    {
        const float SEGMENT = .45f;
        
        Vector3 direction = otherTrans.InverseTransformPoint(playerTrans.position).normalized;
        
        // .45 to -.45
        if (direction.x > -SEGMENT && direction.x < SEGMENT)
        {
            if (direction.z > 0f)
                return Facing.Front;
            else
                return Facing.Behind;
        }
        else if (direction.x < SEGMENT)
            return Facing.Left;
        else
            return Facing.Right;
    }
}
