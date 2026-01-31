using UnityEngine;

public class MaskSpawner : MonoBehaviour
{
    [Header("References")]
    private Transform playerRoot;
    public GameObject maskPrefab;

    [Header("Spawn Settings")]
    public int targetCount = 1;
    public float minRadius = 0.8f;
    public float maxRadius = 1.8f;
    public Vector2 heightRange = new Vector2(-0.3f, 0.3f);

    [Header("Respawn")]
    public float respawnDelay = 30f;

    private bool _respawning;
    private int _alive;

    void Start()
    {
        playerRoot = PlayerLocator.Player;
        if (playerRoot == null || maskPrefab == null)
        {
            Debug.LogError("[MaskSpawner] Missing playerRoot or maskPrefab.");
            enabled = false;
            return;
        }

        SpawnUntilTarget();
    }

    void Update()
    {
        // Don't spawn immediately while waiting for respawn
        if (_respawning) return;

        if (_alive < targetCount)
            SpawnUntilTarget();
    }

    void SpawnUntilTarget()
    {
        while (_alive < targetCount)
        {
            var go = Instantiate(maskPrefab, GetSpawnPos(), Random.rotation);
            var mask = go.GetComponent<MaskPickup>();
            if (mask != null)
                mask.OnPicked += HandleMaskPicked;

            _alive++;
        }
    }

    Vector3 GetSpawnPos()
    {
        Vector2 dir2 = Random.insideUnitCircle.normalized;
        float r = Random.Range(minRadius, maxRadius);

        Vector3 basePos = playerRoot.position;
        
        float headY = (Camera.main != null) ? Camera.main.transform.position.y : basePos.y;
        float yOffset = Random.Range(heightRange.x, heightRange.y);

        Vector3 pos = basePos + new Vector3(dir2.x, 0f, dir2.y) * r;
        pos.y = headY + yOffset;
        return pos;
    }

    void HandleMaskPicked(MaskPickup mask)
    {
        _alive--;

        _respawning = true; // lock spawning immediately
        StartCoroutine(RespawnAfterDelay());
    }

    System.Collections.IEnumerator RespawnAfterDelay()
    {
        if (_respawning) yield break;
        _respawning = true;

        yield return new WaitForSeconds(respawnDelay);

        _respawning = false;
        SpawnUntilTarget();
    }

}
