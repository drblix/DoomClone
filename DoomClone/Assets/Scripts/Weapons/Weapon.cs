using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public UnityEditor.Animations.AnimatorController animations;
    public Sprite idle;

    public float fireRate;
    public float ammo;
    public float maxAmmo;

    private void Awake() 
    {
        ammo = maxAmmo;
    }

    public void Shoot()
    {
        Debug.Log("BANG");
    }
}
