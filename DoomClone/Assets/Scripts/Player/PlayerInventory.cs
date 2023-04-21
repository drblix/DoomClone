using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<WeaponObject> playerWeapons;
    private PlayerAttack _playerAttack;
    private UIBob _gunBob;

    [SerializeField] private UnityEngine.UI.Image _gunSprite;
    [SerializeField] private Vector3 _swapDestination;
    [SerializeField] private Vector3 _defaultDestination;
    [SerializeField] private float _changeSpeed = 6f;
    private WeaponObject _currentWeapon;
    private int _currentIndex;

    private bool _swapping = false;

    private void Awake()
    {
        _gunBob = _gunSprite.GetComponent<UIBob>();
        _playerAttack = GetComponent<PlayerAttack>();
        _currentWeapon = playerWeapons[0];
        _currentIndex = 0;

        foreach (WeaponObject obj in playerWeapons)
            obj.ammo = obj.maxAmmo;
    }

    private void Update()
    {
        int num = -1;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            num = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            num = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            num = 2;

        if (Input.anyKeyDown && num != -1 && !_swapping && num != _currentIndex && num < playerWeapons.Count)
        {
            _swapping = true;
            _currentIndex = num;
            _currentWeapon = playerWeapons[num];
            _playerAttack.canShoot = false;
            StartCoroutine(SwapToWeapon(num));
        }
    }

    private IEnumerator SwapToWeapon(int index)
    {
        const float DIST_THRESHOLD = 25f;
        _gunBob.enabled = false;
        _gunSprite.GetComponent<AudioSource>().Play();

        float distance = (_swapDestination - _gunSprite.rectTransform.localPosition).sqrMagnitude;

        while (distance > DIST_THRESHOLD)
        {
            _gunSprite.rectTransform.localPosition = Vector3.MoveTowards(_gunSprite.rectTransform.localPosition, _swapDestination, Time.deltaTime * _changeSpeed);

            distance = (_swapDestination - _gunSprite.rectTransform.localPosition).sqrMagnitude;
            yield return new WaitForEndOfFrame();
        }

        // perform visual changes here
        _playerAttack.SelectWeapon(_currentWeapon);

        distance = (_defaultDestination - _gunSprite.rectTransform.localPosition).sqrMagnitude;

        while (distance > DIST_THRESHOLD)
        {
            _gunSprite.rectTransform.localPosition = Vector3.MoveTowards(_gunSprite.rectTransform.localPosition, _defaultDestination, Time.deltaTime * _changeSpeed);

            distance = (_defaultDestination - _gunSprite.rectTransform.localPosition).sqrMagnitude;
            yield return new WaitForEndOfFrame();
        }

        _swapping = false;
        _gunBob.enabled = true;
        _playerAttack.canShoot = true;
    }
}
