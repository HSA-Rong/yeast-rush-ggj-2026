using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for spawning and maintaining the sugar system in the scene.
/// </summary>

public class SugarSpawner : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Usually the XR Rig (or Camera Offset) transform.")]
    private Transform playerRoot;

    [Tooltip("Prefab with SugarCube script attached.")]
    public GameObject sugarPrefab;

    [Header("Spawn Settings")]
    public int targetCount = 12;

    // float minReachDistance = 0.35f;
    [Tooltip("Min distance from player.")]
    public float minRadius = 0.35f;

    [Tooltip("Max distance from player.")]
    public float maxRadius = 1.5f;

    [Tooltip("Spawn height offset relative to playerRoot.")]
    public Vector2 heightRange = new Vector2(0.3f, 1.5f);

    [Tooltip("If too close to the player, respawn.")]
    public float respawnIfCloserThan = 0.2f;

    [Header("Runtime")]
    public bool keepOnScreenRing = true;

    private readonly List<SugarCube> _alive = new();

    void Start()
    {
        playerRoot = PlayerLocator.Player;

        if (playerRoot == null)
        {
            Debug.LogError("[SugarSpawner] playerRoot not assigned.");
            enabled = false;
            return;
        }
        if (sugarPrefab == null)
        {
            Debug.LogError("[SugarSpawner] sugarPrefab not assigned.");
            enabled = false;
            return;
        }

        // at the begining to fill with sugars
        Refill();
    }

    void Update()
    {
        CleanupNulls();
        Refill();

        if (keepOnScreenRing)
        {
            for (int i = 0; i < _alive.Count; i++)
            {
                if (_alive[i] == null) continue;
                float d = Vector3.Distance(_alive[i].transform.position, playerRoot.position);
                if (d < respawnIfCloserThan && !_alive[i].IsAbsorbing)
                {
                    _alive[i].transform.position = GetSpawnPos();
                }
            }
        }
    }

    void CleanupNulls()
    {
        _alive.RemoveAll(x => x == null);
    }

    void Refill()
    {
        while (_alive.Count < targetCount)
        {
            var go = Instantiate(sugarPrefab, GetSpawnPos(), Random.rotation);
            var cube = go.GetComponent<SugarCube>();
            if (cube == null)
            {
                Debug.LogError("[SugarSpawner] sugarPrefab is missing SugarCube component.");
                Destroy(go);
                return;
            }

            cube.OnAbsorbed += HandleAbsorbed;
            _alive.Add(cube);
        }
    }

    void HandleAbsorbed(SugarCube cube)
    {
        if (cube != null)
        {
            cube.OnAbsorbed -= HandleAbsorbed;
            _alive.Remove(cube);
        }
    }

    Vector3 GetSpawnPos()
    {
        // random the direction and the range
        Vector2 dir2 = Random.insideUnitCircle.normalized;
        float r = Random.Range(minRadius, maxRadius);

        Vector3 basePos = playerRoot.position;
        float yOffset = Random.Range(heightRange.x, heightRange.y);

        Vector3 pos = basePos + new Vector3(dir2.x, 0f, dir2.y) * r;
        pos.y = basePos.y + yOffset;

        return pos;
    }
}
