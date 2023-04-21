using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private int _maxHealth = 3;
    public int maxHealth { get { return _maxHealth; } }
    public int curHealth { get; private set; }

    private bool _dead = false;

    private void Awake() 
    {
        curHealth = _maxHealth;
    }

    public void Damage(int amnt)
    {
        curHealth -= Mathf.Abs(amnt);

        if (curHealth <= 0 && !_dead)
            Death();
    }

    private void Death()
    {
        Debug.Log(transform.name + " is dead");

        // setting dead boolean and starting death animation
        // int d = Random.Range(1, 3);
        _dead = true;
        _animator.SetBool("Dead", true);

        // disabling collider, broadcasting death message, and starting coroutine
        GetComponentInChildren<BoxCollider>().enabled = false;
        BroadcastMessage("DisableEnemy_Broadcast");
        StartCoroutine(WaitForDeath());
    }

    private IEnumerator WaitForDeath()
    {
        // waiting for death animation to finish, then disabling animator
        yield return new WaitForSeconds(2f);
        _animator.enabled = false;
    }
}
