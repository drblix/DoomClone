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
    [SerializeField] private Transform _player, _boltSpawn;
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
    [SerializeField][Range(0, 100)] private int _missChance;

    [Space(5f)]
    [Header("Burst")]
    [SerializeField] private bool _fireInBursts;
    [SerializeField] private float _burstWait;
    [SerializeField] private int _burstAmount;

    private EnemyHealth _enemyHealth;
    private EnemySpriteRenderer _enemSpriteRenderer;
    private EnemySounds _enemySounds;
    private NavMeshAgent _navAgent;

    private bool _aiming = false;
    private bool _moving = false;
    private bool _playerDetected = false;

    private float _shootTimer = 0f;

    private void Awake()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        _enemyHealth = GetComponent<EnemyHealth>();
        _enemSpriteRenderer = GetComponent<EnemySpriteRenderer>();
        _enemySounds = GetComponent<EnemySounds>();
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
        else
            _shootTimer = 0f;

        // Debug.DrawRay(_boltSpawn.position, (_player.position - _boltSpawn.position).normalized + new Vector3(Random.Range(-.5f, .5f), Random.Range(-.5f, .5f), Random.Range(-.5f, .5f)), Color.red, .1f);
    }

    private void AttackPlayer()
    {
        if (IsClose(_closeRange) && !IsMoving())
        {
            // in range and can shoot
            if (_shootTimer > _fireRate && CanSeePlayer())
            {
                if (_fireInBursts)
                    StartCoroutine(BurstRoutine());
                else
                {
                    if (Random.Range(0, 101) <= _missChance)
                        MissPlayer();
                    else
                        ShootAtPlayer();
                }

                _shootTimer = 0f;
            }

            _shootTimer += Time.deltaTime;
        }
        else
        {
            // move towards player until close enough
            if (!IsMoving())
                _navAgent.SetDestination(_player.position);
            else if (IsClose(_closeRange - 3f))
                _navAgent.ResetPath();
        }
    }

    private void ShootAtPlayer()
    {
        _enemySounds.Shoot();
        _animator.SetTrigger("Shoot");
        GameObject newBolt = Instantiate(_blasterBolt, _boltSpawn.position, Quaternion.identity);
        newBolt.transform.LookAt(Camera.main.transform.position, Vector3.up);
        newBolt.GetComponent<BlasterBolt>().BeginPath(_player.position, 100f);
    }

    private void MissPlayer()
    {
        string[] layers = { "Player", "Enemy" };
        const float MISS_OFFSET = .5f;

        Vector3 randomDir = (_player.position - _boltSpawn.position).normalized + new Vector3(Random.Range(-MISS_OFFSET, MISS_OFFSET), Random.Range(-MISS_OFFSET, MISS_OFFSET), Random.Range(-MISS_OFFSET, MISS_OFFSET));
        bool result = Physics.Raycast(_boltSpawn.position, randomDir, out RaycastHit info, 100f, ~LayerMask.GetMask(layers));

        if (info.collider)
        {
            _enemySounds.Shoot();
            _animator.SetTrigger("Shoot");
            GameObject newBolt = Instantiate(_blasterBolt, _boltSpawn.position, Quaternion.identity);
            newBolt.transform.LookAt(info.point, Vector3.up);
            newBolt.GetComponent<BlasterBolt>().BeginPath(info.point, 100f);
        }
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
    private bool IsClose(float reqDist) => (_player.position - transform.position).sqrMagnitude <= reqDist * reqDist;
    private bool CanSeePlayer()
    {
        Vector3 dir = _player.position - transform.position;
        Physics.Raycast(transform.position, dir, out RaycastHit info, _sightDistance, ~LayerMask.GetMask("Enemy"));

        if (info.collider)
            return info.collider.CompareTag("Player");

        return false;
    }

    public bool PlayerDetected()
    {
        Vector3 dir = _player.position - transform.position;

        // checking if player is close enough
        bool isClose = IsClose(_closeRange);

        // checking if enemy can see player
        bool inSight = CanSeePlayer();

        // checking if enemy was damaged
        bool damaged = _enemyHealth.curHealth != _enemyHealth.maxHealth;

        // checking if player has shot nearby
        bool shotNear = (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && dir.sqrMagnitude <= 700f;

        return inSight || isClose || damaged || shotNear;
    }

    private IEnumerator PatrolRoutine()
    {
        while (true)
        {
            if (!_playerDetected)
            {
                if (GetRandomDestination(transform.position, _patrolRadius, out Vector3 goal))
                    _navAgent.SetDestination(goal);
            }

            yield return new WaitForSeconds(Random.Range(8f, 16f));
        }
    }

    private IEnumerator BurstRoutine()
    {
        for (int i = 0; i < _burstAmount; i++)
        {
            if (Random.Range(0, 101) <= _missChance)
                MissPlayer();
            else
                ShootAtPlayer();
            yield return new WaitForSeconds(_burstWait);
        }
    }
}
