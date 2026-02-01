using UnityEngine;

public class destroyMaskOnHead : MonoBehaviour
{
    public float destroyAfterSeconds = 20f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

}
