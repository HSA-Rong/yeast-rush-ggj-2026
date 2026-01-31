using UnityEngine;

public class HideEnvironmentOnce : MonoBehaviour
{
    [Header("Target to Hide")]
    public GameObject targetToHide;

    [Header("Trigger")]
    public string handTag = "Hand";
    public bool disableInsteadOfSetInactive = true;

    private bool _triggered;

    void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;
        
        if (other.CompareTag(handTag) || other.transform.root.CompareTag(handTag))
        {
            _triggered = true;

            if (targetToHide != null)
            {
                if (disableInsteadOfSetInactive)
                {
                    // only hide the render
                    var rens = targetToHide.GetComponentsInChildren<Renderer>(true);
                    foreach (var r in rens) r.enabled = false;
                }
                else
                {
                    // total hide the object
                    targetToHide.SetActive(false);
                }
            }
            
            gameObject.SetActive(false);
        }
    }
}