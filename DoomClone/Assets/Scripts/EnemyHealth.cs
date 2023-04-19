using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private int _maxHealth = 3;
    public int _curHealth { get; private set; }

    private bool dead = false;

    private void Awake() 
    {
        _curHealth = _maxHealth;
    }

    public void Damage(int amnt)
    {
        _curHealth -= Mathf.Abs(amnt);

        if (_curHealth <= 0 && !dead)
            Death();
    }

    private void Death()
    {
        Debug.Log(transform.name + " is dead");
        dead = true;
        int d = Random.Range(1, 3);
        _animator.SetBool("Dead_" + d, true);

        GetComponentInChildren<BoxCollider>().enabled = false;
        BroadcastMessage("DisableEnemy");
    }
}
