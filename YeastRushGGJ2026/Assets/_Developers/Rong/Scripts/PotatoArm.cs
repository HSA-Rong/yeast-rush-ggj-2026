using UnityEngine;

public class PotatoArm : MonoBehaviour
{
    [SerializeField] private Transform controller;
    [SerializeField] private Vector3 offset = new Vector3(0, 0.1f, 0.2f);
    [SerializeField] private Vector3 rotationOffsetEuler = Vector3.zero;

    void LateUpdate()
    {
        if (!controller) return;

        transform.position = controller.position + controller.TransformVector(offset);
        transform.rotation = controller.rotation * Quaternion.Euler(rotationOffsetEuler);
    }
}
