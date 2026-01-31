using System;
using UnityEngine;

/// <summary>
/// Contaminant that wanders randomly and eat sugar when it reaches it.
/// </summary>

public class Contaminant : MonoBehaviour
{
    public event Action<Contaminant> OnReproduceRequested;

    [Header("Wander")]
    private Transform playerRoot;
    public float wanderRadius = 1.5f;
    public float moveSpeed = 0.6f;
    public float turnSpeed = 3.0f;
    public float targetChangeInterval = 1.2f;

    [Header("3D Bounds")]
    public float verticalWanderRadius = 0.8f; // y

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

    [Header("Sugar Seek")]
    public float sugarSeekRadius = 2.0f;
    public float sugarScanInterval = 0.25f;
    public float sugarAttraction = 0.75f;         // 0-1, 1 = seek, 0 = random
    public float sugarApproachSpeedMultiplier = 0.8f; // speed like get closer to sugar
    public LayerMask sugarLayerMask = ~0;
    public string sugarTag = "Sugar";

    private Transform _sugarTarget;
    private float _nextSugarScanTime;

    void Awake()
    {
        _seed = UnityEngine.Random.value * 10f;
    }

    void Start()
    {
        playerRoot = PlayerLocator.Player;

        if (playerRoot == null)
        {
            Debug.LogError("[Contaminant] PlayerLocator.Player is null. Make sure PlayerLocator is on XR Rig and initialized.");
            enabled = false;
            return;
        }

        // _origin = transform.position;
        _origin = playerRoot.position;
        PickNewTarget();
    }

    void Update()
    {
        // keep wander center following the player
        _origin = playerRoot.position;

        // periodic target change
        if (Time.time >= _nextChangeTime)
            PickNewTarget();

        /*
        // move towards target
        Vector3 to = (_target - transform.position);
        Vector3 dir = to.sqrMagnitude > 0.001f ? to.normalized : transform.forward;
        */

        // periodic sugar scan
        if (Time.time >= _nextSugarScanTime)
        {
            _nextSugarScanTime = Time.time + sugarScanInterval;
            _sugarTarget = FindNearestSugar();
        }

        // --- Compute desired direction ---
        Vector3 wanderDir = (_target - transform.position);
        wanderDir = wanderDir.sqrMagnitude > 0.001f ? wanderDir.normalized : transform.forward;

        Vector3 dir = wanderDir;
        float speed = moveSpeed;

        // If a sugar is nearby, gently bias movement towards it
        if (_sugarTarget != null)
        {
            Vector3 toSugar = _sugarTarget.position - transform.position;
            float dist = toSugar.magnitude;

            // If sugar still within seek radius, blend direction
            if (dist <= sugarSeekRadius)
            {
                Vector3 sugarDir = toSugar.normalized;

                // Smoothly blend wander direction and sugar direction
                dir = Vector3.Slerp(wanderDir, sugarDir, sugarAttraction);

                // Slow approach feel (optional)
                speed = moveSpeed * sugarApproachSpeedMultiplier;
            }
            else
            {
                _sugarTarget = null; // out of range -> forget
            }
        }

        // smooth rotate
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion desired = Quaternion.LookRotation(dir.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, desired, turnSpeed * Time.deltaTime);
        }

        // move
        // transform.position += transform.forward * (moveSpeed * Time.deltaTime);
        // transform.position += transform.forward * (speed * Time.deltaTime);
        transform.position += dir.normalized * (speed * Time.deltaTime);

        // float effect
        float t = (Time.time + _seed) * floatFrequency;
        Vector3 floatOffset = new Vector3(
            Mathf.Sin(t),
            Mathf.Sin(t * 1.37f + 1.1f),
            Mathf.Sin(t * 1.73f + 2.2f)
        ) * floatAmplitude;

        transform.position += floatOffset * Time.deltaTime;
    }

    private Transform FindNearestSugar()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, sugarSeekRadius, sugarLayerMask, QueryTriggerInteraction.Collide);

        Transform best = null;
        float bestSqr = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            Collider c = hits[i];

            if (!c.CompareTag(sugarTag) && c.GetComponent<SugarCube>() == null)
                continue;

            float sqr = (c.transform.position - transform.position).sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                best = c.transform;
            }
        }

        return best;
    }

    void PickNewTarget()
    {
        _nextChangeTime = Time.time + targetChangeInterval;

        Vector3 rnd = UnityEngine.Random.insideUnitSphere * wanderRadius;
        rnd.y = UnityEngine.Random.Range(-verticalWanderRadius, verticalWanderRadius);

        _target = _origin + rnd;
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

