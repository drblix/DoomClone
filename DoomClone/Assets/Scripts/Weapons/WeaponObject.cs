using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapon", order = 0)]
public class WeaponObject : ScriptableObject
{
    public UnityEditor.Animations.AnimatorController animations;
    public Sprite idle;
    public AudioClip sound;

    public float fireRate;
    public int maxAmmo;
    public int ammo;
    public int damage;
}