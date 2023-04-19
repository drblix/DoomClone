using System.Collections;
using UnityEngine;

public class BlasterBolt : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private GameObject _explosion;
    private Vector3 _goal;
    private bool _started = false;
    private bool _done = false;

    private void Update() 
    {
        if (!_started) { return; }

        if (!_done)
            transform.position = Vector3.MoveTowards(transform.position, _goal, _speed * Time.deltaTime);
        
        if ((_goal - transform.position).sqrMagnitude < .01f && !_done)
        {
            _explosion.SetActive(true);
            Destroy(gameObject, .7f);
            _done = true;
        }
    }

    public void BeginPath(Vector3 goal)
    {
        this._goal = goal;
        _started = true;
    }
}
