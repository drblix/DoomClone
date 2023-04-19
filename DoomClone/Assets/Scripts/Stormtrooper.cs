using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stormtrooper : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator animator;

    [SerializeField] private Sprite[] sprites;

    private bool moving = false;

    // Late update so animator doesn't override idle sprite changes
    private void LateUpdate()
    {
        enabled = !(animator.GetBool("Dead_1") || animator.GetBool("Dead_2"));
        moving = animator.GetBool("Moving");

        PlayerMovement.Facing face = PlayerMovement.GetFacing(_player, transform);

        if (!moving)
        {
            switch (face)
            {
                case PlayerMovement.Facing.Front:
                    _spriteRenderer.sprite = sprites[0];
                    break;
                case PlayerMovement.Facing.Behind:
                    _spriteRenderer.sprite = sprites[1];
                    break;
                case PlayerMovement.Facing.Left:
                    _spriteRenderer.sprite = sprites[2];
                    break;
                case PlayerMovement.Facing.Right:
                    _spriteRenderer.sprite = sprites[2];
                    break;
            }
        }

        SetAnimatorBools(face);
    }

    private void SetAnimatorBools(PlayerMovement.Facing face)
    {
        animator.SetBool("Front", face.Equals(PlayerMovement.Facing.Front));
        animator.SetBool("Behind", face.Equals(PlayerMovement.Facing.Behind));
        animator.SetBool("Side", face.Equals(PlayerMovement.Facing.Left) || face.Equals(PlayerMovement.Facing.Right));
        _spriteRenderer.flipX = face.Equals(PlayerMovement.Facing.Right);
    }

    private void DisableEnemy() => enabled = false;
}
