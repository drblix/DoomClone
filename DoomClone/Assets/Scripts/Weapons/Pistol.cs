

public class Pistol : Weapon
{
    public Pistol(UnityEditor.Animations.AnimatorController animations, UnityEngine.Sprite idle, float fr, float am, float maxAm)
    {
        this.animations = animations;
        this.idle = idle;
        this.fireRate = fr;
        this.maxAmmo = maxAm;
        this.ammo = this.maxAmmo;
    }
}
