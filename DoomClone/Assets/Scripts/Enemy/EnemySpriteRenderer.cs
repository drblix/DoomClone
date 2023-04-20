using UnityEngine;

public class EnemySpriteRenderer : MonoBehaviour
{
    public bool aiming { get; set; } = false;

    [SerializeField] private Transform _player;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;
    [SerializeField] private Sprite[] _sprites;

    private bool _moving = false;

    // Late update so animator doesn't override idle sprite changes
    private void LateUpdate()
    {
        SetFaces();
    }

    // Gets the direction of where the player is viewing from and assigns sprite as needed
    private void SetFaces()
    {
        _moving = _animator.GetBool("Moving");

        PlayerMovement.Facing face = PlayerMovement.GetFacing(_player, transform);

        if (!_moving && !aiming)
        {
            switch (face)
            {
                case PlayerMovement.Facing.Front:
                    _spriteRenderer.sprite = _sprites[0];
                    break;
                case PlayerMovement.Facing.Behind:
                    _spriteRenderer.sprite = _sprites[1];
                    break;
                case PlayerMovement.Facing.Left:
                    _spriteRenderer.sprite = _sprites[2];
                    break;
                case PlayerMovement.Facing.Right:
                    _spriteRenderer.sprite = _sprites[2];
                    break;
            }
        }
        else if (!_moving && aiming)
        {
            _spriteRenderer.sprite = _sprites[3];
        }

        SetAnimatorBools(face);
    }

    // Setting animation bools depending on how player sees enemy
    private void SetAnimatorBools(PlayerMovement.Facing face)
    {
        _animator.SetBool("Front", face.Equals(PlayerMovement.Facing.Front));
        _animator.SetBool("Behind", face.Equals(PlayerMovement.Facing.Behind));
        _animator.SetBool("Side", face.Equals(PlayerMovement.Facing.Left) || face.Equals(PlayerMovement.Facing.Right));
        _spriteRenderer.flipX = face.Equals(PlayerMovement.Facing.Right) && !aiming;
    }

    // Broadcast receiver from "EnemyHealth"
    private void DisableEnemy_Broadcast() => enabled = false;
}
