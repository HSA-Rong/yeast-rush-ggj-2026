using UnityEngine;

public class MultiSugarSplit : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject sugarPrefab;

    [Header("Split Settings")]
    public int spawnCount = 2;
    public float spreadDistance = 0.2f;
    public string bulletTag = "Bullet";

    [Header("Audio")]
    public AudioClip absorbSfx;
    public float sfxVolume = 0.8f;

    private bool _split;
    
    void OnTriggerEnter(Collider other)
    {
        if (_split) return;
        if (!other.CompareTag(bulletTag) && !other.transform.root.CompareTag(bulletTag)) return;

        Split();
    }

    private void Split()
    {
        _split = true;

        Vector3 center = transform.position;

        // generate two sugar
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 offset;

            if (spawnCount == 2)
            {
                offset = (i == 0) ? Vector3.left * (spreadDistance * 0.5f)
                                  : Vector3.right * (spreadDistance * 0.5f);
            }
            else
            {
                // if more than 2, random distance
                offset = Random.insideUnitSphere * (spreadDistance * 0.5f);
            }

            if (absorbSfx != null)
            {
                AudioSource.PlayClipAtPoint(absorbSfx, transform.position, sfxVolume);
            }

            // Instantiate(sugarPrefab, center + offset, Random.rotation);
            var go = Instantiate(sugarPrefab, center + offset, Random.rotation);

            var rb = go.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(offset.normalized * 0.2f, ForceMode.Impulse);
            }
        }

        
        Destroy(gameObject);
    }
}
