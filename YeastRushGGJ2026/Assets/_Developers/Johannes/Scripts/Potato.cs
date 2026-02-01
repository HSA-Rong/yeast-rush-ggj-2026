using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Potato : MonoBehaviour
{

    [SerializeField]
    private Vector3 startOffset = new Vector3(0, 1f, -0.5f);
    private Vector3 currentOffset; 
    [SerializeField]
    private Vector3 startRotation = new Vector3(0, 0, 0);

    [HideInInspector]
    public bool hasEatenSugar = false;

    private Transform camTransform;

    private Vector3 targetPosition;
    private float timer = 0f;
    private float wobbleDuration = 2f;

    private Vector3 startPotatoScale;
    private float startPotatoPulseTimer;
    PotatoParameters potatoParams;

    [Header("Fart")]
    public GameObject fartBubblePrefab;
    public AudioClip fartSfx;

    void Start()
    {
        camTransform = Camera.main.transform;
        potatoParams = this.GetComponent<PotatoParameters>();
        if (potatoParams != null) {
            startPotatoScale = potatoParams.shaderScale;
            startPotatoPulseTimer = potatoParams.pulseTimer;
        }
        currentOffset = startOffset;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasEatenSugar) // initiate wobbling
        {   if (timer < 0f) // initialization
            {
                timer = 0f;
            }
            else if (timer < wobbleDuration) // ongoing increase and wobbling
            {
                potatoParams.pulseTimer = 5f; // faster pulse
                timer += Time.deltaTime; // wobble for 2 seconds
                potatoParams.shaderScale = Vector3.Lerp(startPotatoScale, startPotatoScale*1.2f, timer / wobbleDuration); // bigger wobble
                potatoParams.shaderScale.x *= 1.2f; // emphasize Y axis
                currentOffset.y = Mathf.Lerp(startOffset.y, startOffset.y * 1.2f, timer / wobbleDuration); // move up a bit
            }
            else // increase finished, wobbling ended, reset
            {
                potatoParams.pulseTimer = startPotatoPulseTimer; // reset pulse
                startPotatoScale = potatoParams.shaderScale; // save scale for next time
                hasEatenSugar = false; // reset
                startOffset = currentOffset; // save current offset as start offset for next time
                timer = -1f;
                // fire a fart bubble + sound
                GameObject fartBubble = Instantiate(fartBubblePrefab, transform.position + transform.forward, Quaternion.identity);
                fartBubble.GetComponent<Rigidbody>().linearVelocity = Camera.main.transform.forward * 2f; // Adjust speed as needed
                if (fartSfx != null)
                {
                    AudioSource.PlayClipAtPoint(fartSfx, transform.position, 0.8f);
                }
             }
        }
        // I want that the object always is at 1 m below the camera, but Y-Axis facing upwards
        targetPosition = camTransform.position - currentOffset;
        transform.position = targetPosition;  
        transform.rotation = Quaternion.Euler(startRotation);        

    }
}
