using UnityEngine;
using UnityEngine.InputSystem;
using VIVE.OpenXR;

public class PotatoParameters : MonoBehaviour
{

    [SerializeField]
    private float pulseTimer = 1.0f;
    [SerializeField] 
    private Vector3 shaderScale = new Vector3(0.5f, 0.5f, 0.5f);

    private float lastPulseTimerValue = 0.0f;
    private Vector3 lastShaderScaleValue = new Vector3();

    private Material mat;

    void Start()
    {
        mat = this.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()    
    {
        if (pulseTimer != lastPulseTimerValue)
        {
            Debug.Log("Change1");
            mat.SetFloat("_PotatoPulseTimer", pulseTimer);
            lastPulseTimerValue = pulseTimer;
        }

        if (shaderScale != lastShaderScaleValue)
        {
            Debug.Log("Change2");
            mat.SetVector("_PotatoShaderScale", shaderScale);
            lastShaderScaleValue = shaderScale;
        }        
    }
}
