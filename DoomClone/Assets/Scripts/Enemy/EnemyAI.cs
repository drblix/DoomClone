using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// TODO:
// Have enemy's weapon be customizable (fire rate, damage, etc)
// Reference blaster bolt prefab
// 

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private GameObject _blasterBolt;
    [SerializeField] private Animator _animator;

    [Header("Behaviour Configuration")]
    [SerializeField] private float _closeRange;
    [SerializeField] private float _sightDistance;
    
    [Header("Patrol Configuration")]
    [SerializeField] private bool _shouldPatrol = false;
    [SerializeField] private float _patrolRadius;


    [Header("Weapon Configuration")]
    
    [Space(5f)]
    [Header("Settings")]
    [SerializeField] private float _fireRate;
    [SerializeField] private int _damage;
    
    [Space(5f)]
    [Header("Burst")]
    [SerializeField] private bool _fireInBursts;
    [SerializeField] private float _burstWait;

    private EnemyHealth _enemyHealth;
    private EnemySpriteRenderer _enemSpriteRenderer;
    private NavMeshAgent _navAgent;
    
    private bool _aiming = false;
    private bool _moving = false;
    private bool _playerDetected = false;

    private void Awake() 
    {
        _navAgent = GetComponent<NavMeshAgent>();
        _enemyHealth = GetComponent<EnemyHealth>();
        _enemSpriteRenderer = GetComponent<EnemySpriteRenderer>();
        // _navAgent.SetDestination(new Vector3(-1.4f, 3.25f, -3.85f));

        if (_shouldPatrol)
            StartCoroutine(PatrolRoutine());
    }

    private void Update() 
    {
        // PlayerDetected();
        _playerDetected = PlayerDetected();
        _moving = IsMoving();
        _animator.SetBool("Moving", _moving);
        _enemSpriteRenderer.aiming = _playerDetected;

        if (_playerDetected)
            AttackPlayer();

    }

    private void AttackPlayer()
    {

    }

    private bool GetRandomDestination(Vector3 center, float radius, out Vector3 goal)
    {
        for (int i = 0; i < 5; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * radius;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                goal = hit.position;
                return true;
            }
        }

        goal = Vector3.zero;
        return false;
    }

    private void DisableEnemy_Broadcast()
    {
        _navAgent.enabled = false;
        this.enabled = false;
    }

    public bool IsMoving() => !Mathf.Approximately(_navAgent.velocity.sqrMagnitude, 0f);
    public bool PlayerDetected()
    {
        Vector3 dir = _player.position - transform.position;

        // checking if player is close enough
        bool isClose = dir.sqrMagnitude <= _closeRange * _closeRange;

        // checking if enemy can see player
        Physics.Raycast(transform.position, dir, out RaycastHit info, _sightDistance);
        bool inSight = info.collider ? info.collider.CompareTag("Player") : false;

        // checking if enemy was damaged
        bool damaged = _enemyHealth.curHealth != _enemyHealth.maxHealth;

        // checking if player has shot nearby
        bool shotNear = (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && dir.sqrMagnitude <= 700f;

        return inSight || isClose || damaged || shotNear;
    }

    private IEnumerator PatrolRoutine()
    {
        while (!_playerDetected)
        {
            if (GetRandomDestination(transform.position, _patrolRadius, out Vector3 goal))
                _navAgent.SetDestination(goal);
    
            yield return new WaitForSeconds(Random.Range(8f, 16f));
        }
    }
}
