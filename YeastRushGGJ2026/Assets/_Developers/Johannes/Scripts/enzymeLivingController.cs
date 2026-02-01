using UnityEngine;

public class enzymeLivingController : MonoBehaviour
{
    [HideInInspector]
    public GameObject gameObjectHit = null;

    [SerializeField]
    private float livingTime = 2f;

    [SerializeField]
    private bool reactToExternalStimuli = true; // used to control if this script reacts to collisions (enzyme) or not (fart b)

    float startTime = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - startTime > livingTime)
        {
            Destroy(this.gameObject);
        }

    }

    void OnTriggerEnter(Collider collision)
    {
        if (!reactToExternalStimuli) return;

        // collider might be on a child object, so also check parent
        bool isMultiSugar =
            collision.CompareTag("MultiSugar") ||
            (collision.transform.parent != null && collision.transform.parent.CompareTag("MultiSugar"));

        if (isMultiSugar)
        {
            gameObjectHit = collision.gameObject;
            Destroy(gameObject);
        }
    }

}
