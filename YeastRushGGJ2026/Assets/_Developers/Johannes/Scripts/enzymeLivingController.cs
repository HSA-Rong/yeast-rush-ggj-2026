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
<<<<<<< HEAD
        // if (collision.gameObject.tag == "MultiSugar")
        if (collision.CompareTag("MultiSugar") || (collision.transform.parent != null && collision.transform.parent.CompareTag("MultiSugar")))
=======
        if (reactToExternalStimuli && collision.gameObject.tag == "MultiSugar")
>>>>>>> c672aabce8d0904b85c63fafcfa2bcbeaa2d0f5e
        {
            gameObjectHit = collision.gameObject;
            Destroy(this.gameObject);
            // Destroy(collision.gameObject);
            // Destroy(collision.transform.root.gameObject);
        }
    }
}
