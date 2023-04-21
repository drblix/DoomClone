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
    [SerializeField] private SpriteRenderer _muzzleFlash;
    [SerializeField] private GameObject _blasterBolt;
    [SerializeField] private Animator _animator;

    [Header("Behaviour Configuration")]
    [Tooltip("Speed the enemy moves")]
    [SerializeField] private float _speed;

    [Tooltip("The distance at which the player is considered close to the enemy")]
    [SerializeField] private float _closeRange;
    
    [Tooltip("How far the enemy can see the player")]
    [SerializeField] private float _sightDistance;

    [Tooltip("The range at which an enemy can \"hear\" a player shooting")]
    [SerializeField] private float _hearDistance;

    [Tooltip("Does not attack player?")]
    public bool passive;

    [Tooltip("How long the AI will follow after initially detecting player")] 
    [SerializeField] private float _memoryLength;

    [Tooltip("Mask that will be used for shots that miss")]
    [SerializeField] private LayerMask _missingMask;

    [Header("Patrol Configuration")]

    [Tooltip("Should this enemy patrol random locations around it?")]
    [SerializeField] private bool _shouldPatrol = false;

    [Tooltip("Radius for which random locations are determined")]
    [SerializeField] private float _patrolRadius;


    [Header("Weapon Configuration")]

    [Space(5f)]
    [Header("Settings")]

    [Tooltip("Speed at which the enemy fires their weapon")]
    [SerializeField] private float _fireRate;

    [Tooltip("How much damage the enemy does to the player")]
    [SerializeField] private int _damage;

    [Tooltip("The chance of the enemy missing (25 = 25% chance of missing)")]
    [SerializeField][Range(0, 100)] private int _missChance;

    [Tooltip("How wide the bolts should fly should a shot miss")]
    [SerializeField][Range(0f, 1f)]  private float _missIntensity;

    [Space(5f)]
    [Header("Burst")]
    
    [Tooltip("Should the enemy fire their weapon in bursts?")]
    [SerializeField] private bool _fireInBursts;

    [Tooltip("How long between each shot being fired in a burst")]
    [SerializeField] private float _burstWait;

    [Tooltip("How many shots to fire in a burst")]
    [SerializeField] private int _burstAmount;

    private EnemyHealth _enemyHealth;
    private EnemySpriteRenderer _enemSpriteRenderer;
    private EnemySounds _enemySounds;
    private NavMeshAgent _navAgent;

    private bool _moving = false;
    [SerializeField] private bool _playerDetected = false;
    private bool _runningMemory = false;
    private bool _runningHealthMemory = false;

    private float _shootTimer = 0f;
    private int _healthBefore;

    private void Awake()
    {
        _player = Camera.main.transform.parent.parent;
        _navAgent = GetComponent<NavMeshAgent>();
        _enemyHealth = GetComponent<EnemyHealth>();
        _enemSpriteRenderer = GetComponent<EnemySpriteRenderer>();
        _enemySounds = GetComponent<EnemySounds>();
        _navAgent.speed = _speed;
        _healthBefore = _enemyHealth.curHealth;


        if (_shouldPatrol)
            StartCoroutine(PatrolRoutine());
    }

    private void Update()
    {
        // PlayerDetected();
        _playerDetected = PlayerDetected() && !passive;
        _moving = IsMoving();
        _animator.SetBool("Moving", _moving);
        _enemSpriteRenderer.aiming = _playerDetected;

        if (_playerDetected)
        {
            AttackPlayer();

            if (!_runningMemory)
            {
                _runningMemory = true;
                StartCoroutine(MemoryRoutine());
            }
        }
        else
            _shootTimer = 0f;
    }

    private void AttackPlayer()
    {
        if (IsClose(_closeRange) && !_moving && CanSeePlayer())
        {
            // in range and can shoot
            if (_shootTimer > _fireRate)
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
            if (!_moving)
            {
                _navAgent.SetDestination(_player.position);
            }
            else if (IsClose(_closeRange - 3f))
            {
                if (IsMoving())
                {
                    _navAgent.ResetPath();

                    _shootTimer = _fireRate;
                }
            }
        }
    }

    private void ShootAtPlayer()
    {
        FireShot(_player.position);
    }

    private void MissPlayer()
    {
        Vector3 randomDir = (_player.position - _boltSpawn.position).normalized + new Vector3(Random.Range(-_missIntensity, _missIntensity), Random.Range(-_missIntensity, _missIntensity), Random.Range(-_missIntensity, _missIntensity));
        bool result = Physics.Raycast(_boltSpawn.position, randomDir, out RaycastHit info, 100f, ~_missingMask);

        if (info.collider)
            FireShot(info.point);
    }

    private void FireShot(Vector3 goal)
    {
        if (_animator.GetBool("Dead")) 
            return;

        _enemySounds.Shoot();
        _animator.SetTrigger("Shoot");
        StartCoroutine(FlashRoutine());

        GameObject newBolt = Instantiate(_blasterBolt, _boltSpawn.position, Quaternion.identity);
        newBolt.transform.LookAt(goal, Vector3.up);
        newBolt.GetComponent<BlasterBolt>().BeginPath(goal, 100f);
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
        StopAllCoroutines();
        _muzzleFlash.enabled = false;
        _navAgent.enabled = false;
        this.enabled = false;
    }

    public bool IsMoving() => !Mathf.Approximately(_navAgent.velocity.sqrMagnitude, 0f);
    private bool IsClose(float reqDist) => (_player.position - transform.position).sqrMagnitude <= reqDist * reqDist;
    public bool CanSeePlayer()
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
        // bool isClose = IsClose(_closeRange);

        // checking if enemy can see player
        bool inSight = CanSeePlayer();

        // checking if enemy was damaged
        bool damaged = _enemyHealth.curHealth != _healthBefore;
        if (damaged && !_runningHealthMemory)
            StartCoroutine(HealthMemory());

        // checking if player has shot nearby
        bool shotNear = (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && dir.sqrMagnitude <= _hearDistance * _hearDistance;
        
        /* debug messages
        Debug.Log($"In side: {inSight}");
        Debug.Log($"Damaged: {damaged}");
        Debug.Log($"Shot near: {shotNear}");
        */

        return inSight || damaged || shotNear;
    }

    private IEnumerator PatrolRoutine()
    {
        while (true)
        {
            if (!_playerDetected && !_runningMemory)
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

    private IEnumerator MemoryRoutine()
    {
        float memoryTimer = 0f;
        while (true)
        {
            if (!_playerDetected)
            {
                if (memoryTimer > _memoryLength)
                {
                    _navAgent.ResetPath();
                    _runningMemory = false;
                    yield break;
                }
                else if (!IsClose(_closeRange - 3f))
                    _navAgent.SetDestination(_player.position);

                memoryTimer += Time.deltaTime;
            }
            else
                memoryTimer = 0f;
            
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator HealthMemory()
    {
        _runningHealthMemory = true;
        yield return new WaitForSeconds(_memoryLength);
        _healthBefore = _enemyHealth.curHealth;
        _runningHealthMemory = false;
    }

    private IEnumerator FlashRoutine()
    {
        _muzzleFlash.enabled = true;
        yield return new WaitForSeconds(.05f);
        _muzzleFlash.enabled = false;
    }
}
