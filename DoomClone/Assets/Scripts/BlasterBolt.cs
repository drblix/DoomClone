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

        // If not done, keeping moving bolt towards goal
        if (!_done)
            transform.position = Vector3.MoveTowards(transform.position, _goal, _speed * Time.deltaTime);
        
        // If reached destination and not done
        if ((_goal - transform.position).sqrMagnitude < .01f && !_done)
        {
            // Disable all children
            foreach (Transform child in transform)
                child.gameObject.SetActive(false);

            // enable explosion sprite; destroy game object after animation completes
            _explosion.SetActive(true);
            Destroy(gameObject, .7f);
            _done = true;
        }
    }

    public void BeginPath(Vector3 goal, float speed)
    {
        // Starting method
        this._goal = goal;
        this._speed = speed;
        _started = true;
    }
}
