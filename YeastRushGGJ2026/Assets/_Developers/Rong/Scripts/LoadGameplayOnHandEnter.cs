using UnityEngine;

public class LoadGameplayOnHandEnter : MonoBehaviour
{
    public string handTag = "Hand";
    public bool triggerOnce = true;

    private bool _triggered;

    void OnTriggerEnter(Collider other)
    {
        if (triggerOnce && _triggered) return;

        if (other.CompareTag(handTag) || other.transform.root.CompareTag(handTag))
        {
            _triggered = true;
            SceneFlowManager.Instance.EnterGameplay();
        }
    }
}
