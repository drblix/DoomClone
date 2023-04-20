using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapon", order = 0)]
public class WeaponObject : ScriptableObject
{
    [Header("Visual & Auditory")]
    public UnityEditor.Animations.AnimatorController animations;
    public Sprite idle;
    public AudioClip sound;

    [Header("Functionality")]
    public float fireRate;
    public bool fullAuto;
    public int maxAmmo;
    public int ammo;
    public int damage;
}