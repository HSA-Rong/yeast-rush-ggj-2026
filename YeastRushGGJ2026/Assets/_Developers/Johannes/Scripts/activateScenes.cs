using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class activateScenes : MonoBehaviour 
{
    [Header("Scenes to activate at start in build, not in editor!")]
    [SerializeField] private int[] sceneBuildIndices = { 1, 2 }; // Deine 2 weiteren Szenen
    
    void Start() 
    {
#if !UNITY_EDITOR
        StartCoroutine(LoadAllScenesAdditive());
#endif
    }
    
    IEnumerator LoadAllScenesAdditive() 
    {
        // Alle Szenen additiv laden (Szene 0 bleibt aktiv)
        foreach (int buildIndex in sceneBuildIndices) 
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Additive);
            asyncLoad.allowSceneActivation = true;
            
            // Warten bis geladen
            while (!asyncLoad.isDone) 
            {
                yield return null;
            }
        }
        
        //Debug.Log("Alle Szenen geladen!");
    }
}
