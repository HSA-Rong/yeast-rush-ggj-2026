using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class Potato : MonoBehaviour
{

    [SerializeField]
    private Vector3 startOffset = new Vector3(0, 1f, -0.5f);
    [SerializeField]
    private Vector3 startRotation = new Vector3(0, 0, 0);

    private Quaternion curRot;


    // Update is called once per frame
    void Update()
    {
                // I want that the object always is at 1 m below the camera, but Y-Axis facing upwards
        Transform camTransform = Camera.main.transform;
        Vector3 targetPosition = camTransform.position - startOffset;
        transform.position = targetPosition;  
        transform.rotation = Quaternion.Euler(startRotation);        
    }
}
