using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAttack : MonoBehaviour
{
    [HideInInspector] public UnityEvent playerShoot;
    [SerializeField] private GameObject _blasterBolt;


    public List<WeaponObject> playerWeapons;
    private WeaponObject _currentWeapon;
    
    [SerializeField] private Transform _boltSpawn;
    [SerializeField] private Transform _mainCam;
    [SerializeField] private AudioSource _gunSource;

    [SerializeField] private Animator _gunAnimator;
    [SerializeField] private UnityEngine.UI.Image _gunSprite;

    private float _fireRate;
    private float _fireTimer = 0f;

    private void Awake() 
    {
        _currentWeapon = playerWeapons[0];
        foreach (WeaponObject obj in playerWeapons)
            obj.ammo = obj.maxAmmo;

        SelectWeapon(0);
    }

    private void Update()
    {
        if (_currentWeapon.fullAuto)
        {
            if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0) && _fireTimer > _fireRate)
                Shoot();
        }
        else
        {
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && _fireTimer > _fireRate)
                Shoot();
        }

        ChangeWeaponInput();
        _fireTimer = _fireTimer <= _fireRate ? _fireTimer + Time.deltaTime : _fireTimer;
    }

    private void Shoot()
    {
        if (_currentWeapon.ammo <= 0) { return; }
        _fireTimer = 0f;

        _gunSource.clip = _currentWeapon.sound;
        _gunSource.Play();
        _gunAnimator.SetTrigger("Shoot");

        if (Physics.Raycast(_mainCam.position, _mainCam.forward, out RaycastHit info, 100f))
        {
            if (info.collider)
            {
                if (info.collider.CompareTag("Enemy"))
                {
                    EnemyHealth health = info.collider.transform.parent.GetComponent<EnemyHealth>();
                    Debug.Log($"Hit {health.transform.name}");

                    health.Damage(_currentWeapon.damage);
                }

                GameObject bolt = Instantiate(_blasterBolt, _boltSpawn.position, Quaternion.Euler(Vector3.up * _mainCam.eulerAngles.y));
                bolt.GetComponent<BlasterBolt>().BeginPath(info.point, 80f);
            }
        }

    }

    private void ChangeWeaponInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            int index = 0;

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                index = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                index = 1;
            }

            if (index < playerWeapons.Count)
                SelectWeapon(index);
        }
    }

    private void SelectWeapon(int index)
    {
        // add swapping animation later
        WeaponObject weapon = playerWeapons[index];
        _currentWeapon = weapon;
        _gunAnimator.enabled = false;
        _gunAnimator.runtimeAnimatorController = weapon.animations;
        _gunSprite.sprite = weapon.idle;
        _fireRate = weapon.fireRate;
        _fireTimer = _fireRate;
        _gunAnimator.enabled = true;
    }
}
