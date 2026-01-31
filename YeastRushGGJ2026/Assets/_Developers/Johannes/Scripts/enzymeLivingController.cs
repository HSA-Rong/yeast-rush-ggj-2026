using UnityEngine;

public class enzymeLivingController : MonoBehaviour
{
    public GameObject gameObjectHit = null;

    [SerializeField]
    private float livingTime = 2f;

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
        if (collision.gameObject.tag == "MultiSugar")
        {
            gameObjectHit = collision.gameObject;
            Destroy(this.gameObject);   
            Destroy(collision.gameObject);   
        }
    }
}
