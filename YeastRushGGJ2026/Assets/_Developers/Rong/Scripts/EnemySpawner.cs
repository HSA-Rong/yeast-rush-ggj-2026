using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Floating contaminant that applies a harmful effect on contact.
/// </summary>

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    private Transform playerRoot;
    public GameObject enemyPrefab;

    [Header("Spawn Settings")]
    public int startCount = 3;
    public int maxCount = 12;

    public float minRadius = 0.8f;
    public float maxRadius = 1.8f;
    public Vector2 heightRange = new Vector2(0.2f, 0.9f);

    private readonly List<Contaminant> _alive = new();

    void Start()
    {
        playerRoot = PlayerLocator.Player;

        if (playerRoot == null)
        {
            Debug.LogError("[EnemySpawner] playerRoot not assigned / not found.");
            enabled = false;
            return;
        }
        if (enemyPrefab == null)
        {
            Debug.LogError("[EnemySpawner] enemyPrefab not assigned.");
            enabled = false;
            return;
        }

        for (int i = 0; i < startCount; i++)
            SpawnEnemy();
    }

    void SpawnEnemy()
    {
        if (_alive.Count >= maxCount) return;

        var go = Instantiate(enemyPrefab, GetSpawnPos(), Random.rotation);
        var e = go.GetComponent<Contaminant>();
        if (e == null)
        {
            Debug.LogError("[EnemySpawner] enemyPrefab missing EnemyWanderer.");
            Destroy(go);
            return;
        }

        e.OnReproduceRequested += HandleReproduce;
        _alive.Add(e);
    }

    void HandleReproduce(Contaminant contaminant)
    {
        // spawn one new contaminant when one has eaten enough sugar
        SpawnEnemy();
    }

    Vector3 GetSpawnPos()
    {
        Vector2 dir2 = Random.insideUnitCircle.normalized;
        float r = Random.Range(minRadius, maxRadius);

        Vector3 basePos = playerRoot.position;
        float yOffset = Random.Range(heightRange.x, heightRange.y);

        Vector3 pos = basePos + new Vector3(dir2.x, 0f, dir2.y) * r;
        pos.y = basePos.y + yOffset;

        return pos;
    }
}
