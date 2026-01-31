using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class Potato : MonoBehaviour
{
    [SerializeField]
    private Vector3 offset = new Vector3(0, 1f, -0.5f);

    private Quaternion curRot;
    private Vector3 rotVec = new Vector3(0, 0, 0);

    // start is called before the first frame update
    void Start()
    {
        curRot = Quaternion.Euler(rotVec);
    }

    // Update is called once per frame
    void Update()
    {
                // I want that the object always is at 1 m below the camera, but Y-Axis facing upwards
        Transform camTransform = Camera.main.transform;
        Vector3 targetPosition = camTransform.position - offset;
        transform.position = targetPosition;  
        transform.rotation = Quaternion.Euler(rotVec);        
    }
}
