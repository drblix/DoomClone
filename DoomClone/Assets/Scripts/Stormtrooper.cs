using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stormtrooper : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [SerializeField] private Sprite[] sprites;

    private void Update() 
    {
        PlayerMovement.Facing face = PlayerMovement.GetFacing(_player, transform);

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
                _spriteRenderer.flipX = true;
                break;
        }

        _spriteRenderer.flipX = face.Equals(PlayerMovement.Facing.Right);
    }
}
