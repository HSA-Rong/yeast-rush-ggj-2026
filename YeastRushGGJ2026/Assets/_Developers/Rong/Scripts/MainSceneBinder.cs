using UnityEngine;

public class MainSceneBinder : MonoBehaviour
{
    public GameObject mainEnvironmentRoot;

    void Start()
    {
        if (SceneFlowManager.Instance != null)
        {
            SceneFlowManager.Instance.RegisterMainEnvironment(mainEnvironmentRoot);
        }
        else
        {
            Debug.LogWarning("[MainSceneBinder] SceneFlowManager not found.");
        }
    }
}
