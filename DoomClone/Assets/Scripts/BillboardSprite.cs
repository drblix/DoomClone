using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BillboardSprite : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private void Awake() 
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate() 
    {
        _spriteRenderer.transform.rotation = Quaternion.Euler(Vector3.up * Camera.main.transform.eulerAngles.y);
    }
}
