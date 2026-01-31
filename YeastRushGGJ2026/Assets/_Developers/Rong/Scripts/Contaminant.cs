using System;
using UnityEngine;

/// <summary>
/// Contaminant that wanders randomly and eat sugar when it reaches it.
/// </summary>

public class Contaminant : MonoBehaviour
{
    public event Action<Contaminant> OnReproduceRequested;

    [Header("Wander")]
    public float wanderRadius = 1.8f;
    public float moveSpeed = 0.6f;
    public float turnSpeed = 3.5f;
    public float targetChangeInterval = 1.2f;

    [Header("Float")]
    public float floatAmplitude = 0.05f;
    public float floatFrequency = 1.1f;

    [Header("Eat & Reproduce")]
    public int sugarsToReproduce = 3;

    private Vector3 _origin;
    private Vector3 _target;
    private float _nextChangeTime;
    private float _seed;
    private int _eaten;

    void Awake()
    {
        _seed = UnityEngine.Random.value * 10f;
    }

    void Start()
    {
        _origin = transform.position;
        PickNewTarget();
    }

    void Update()
    {
        // periodic target change
        if (Time.time >= _nextChangeTime)
            PickNewTarget();

        // move towards target
        Vector3 to = (_target - transform.position);
        Vector3 dir = to.sqrMagnitude > 0.001f ? to.normalized : transform.forward;

        // smooth rotate
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion desired = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, desired, turnSpeed * Time.deltaTime);
        }

        // forward move
        transform.position += transform.forward * (moveSpeed * Time.deltaTime);

        // float effect
        float y = Mathf.Sin((Time.time + _seed) * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, _origin.y + y, transform.position.z);
    }

    void PickNewTarget()
    {
        _nextChangeTime = Time.time + targetChangeInterval;

        Vector2 rnd = UnityEngine.Random.insideUnitCircle * wanderRadius;
        _target = _origin + new Vector3(rnd.x, 0f, rnd.y);
    }

    void OnTriggerEnter(Collider other)
    {
        // Enemy eats sugar by touching it
        if (other.CompareTag("Sugar") || other.GetComponent<SugarCube>() != null)
        {
            // If the sugar object has SugarCube, it will handle shrinking & Destroy itself.
            // We just count the "eat".
            _eaten++;

            if (_eaten >= sugarsToReproduce)
            {
                _eaten = 0;
                OnReproduceRequested?.Invoke(this);
            }
        }
    }
}

