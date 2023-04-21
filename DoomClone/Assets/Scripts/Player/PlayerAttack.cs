using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject _blasterBolt;

    private PlayerInventory _playerInventory;
    private WeaponObject _currentWeapon;
    
    [SerializeField] private Transform _boltSpawn;
    [SerializeField] private Transform _mainCam;
    [SerializeField] private AudioSource _gunSource;

    [SerializeField] private Animator _gunAnimator;
    [SerializeField] private UnityEngine.UI.Image _gunSprite;

    private float _fireTimer = 0f;
    public bool canShoot { get; set; } = true;

    private void Awake() 
    {
        _playerInventory = GetComponent<PlayerInventory>();
        _currentWeapon = _playerInventory.playerWeapons[0];
    }

    private void Update()
    {
        if (!canShoot)
            return;

        if (_currentWeapon.fullAuto)
        {
            if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0) && _fireTimer > _currentWeapon.fireRate)
                Shoot();
        }
        else
        {
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && _fireTimer > _currentWeapon.fireRate)
                Shoot();
        }

        _fireTimer = _fireTimer <= _currentWeapon.fireRate ? _fireTimer + Time.deltaTime : _fireTimer;
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

    public void SelectWeapon(WeaponObject weapon)
    {
        _currentWeapon = weapon;
        _gunAnimator.enabled = false;

        _gunAnimator.runtimeAnimatorController = weapon.animations;
        _gunSprite.sprite = weapon.idle;
        _fireTimer = weapon.fireRate;

        _gunAnimator.enabled = true;
    }
}
