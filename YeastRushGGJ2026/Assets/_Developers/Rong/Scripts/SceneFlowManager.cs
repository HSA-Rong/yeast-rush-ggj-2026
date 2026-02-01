using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlowManager : MonoBehaviour
{
    public static SceneFlowManager Instance { get; private set; }

    [Header("Scene Names")]
    public string mainSceneName = "MainScene";
    public string gameplaySceneName = "GameplayScene";

    [Header("References")]
    [Tooltip("Root object in Main scene to hide/show (assign at runtime or via finder).")]
    public GameObject mainEnvironmentRoot;

    [Header("Persistent Visibility")]
    [Tooltip("Objects in Persistent scene that should be shown only during Gameplay.")]
    public GameObject[] showOnlyDuringGameplay;

    private bool _loading;
    private bool _gameplayLoaded;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SetGameplayOnlyObjectsVisible(false);
    }


    public void EnterGameplay()
    {
        if (_loading || _gameplayLoaded) return;
        StartCoroutine(EnterGameplayRoutine());
    }

    public void ExitGameplay()
    {
        if (_loading || !_gameplayLoaded) return;
        StartCoroutine(ExitGameplayRoutine());
    }

    IEnumerator EnterGameplayRoutine()
    {
        _loading = true;
        
        SetMainEnvironmentVisible(false);
        SetGameplayOnlyObjectsVisible(true);

        var op = SceneManager.LoadSceneAsync(gameplaySceneName, LoadSceneMode.Additive);
        yield return op;

        _gameplayLoaded = true;
        
        Scene gameplayScene = SceneManager.GetSceneByName(gameplaySceneName);
        if (gameplayScene.IsValid())
            SceneManager.SetActiveScene(gameplayScene);

        _loading = false;
    }

    IEnumerator ExitGameplayRoutine()
    {
        _loading = true;

        var op = SceneManager.UnloadSceneAsync(gameplaySceneName);
        yield return op;

        _gameplayLoaded = false;

        SetGameplayOnlyObjectsVisible(false);

        Scene mainScene = SceneManager.GetSceneByName(mainSceneName);
        if (mainScene.IsValid())
            SceneManager.SetActiveScene(mainScene);

        SetMainEnvironmentVisible(true);

        _loading = false;
    }

    public void SetMainEnvironmentVisible(bool visible)
    {
        if (mainEnvironmentRoot == null)
        {
            var go = GameObject.Find("MainEnvironmentRoot");
            if (go != null) mainEnvironmentRoot = go;
        }

        if (mainEnvironmentRoot != null)
            mainEnvironmentRoot.SetActive(visible);
        else
            Debug.LogWarning("[SceneFlowManager] MainEnvironmentRoot not assigned/found.");
    }

    public void RegisterMainEnvironment(GameObject root)
    {
        mainEnvironmentRoot = root;
    }

    public void SetGameplayOnlyObjectsVisible(bool visible)
    {
        if (showOnlyDuringGameplay == null) return;

        for (int i = 0; i < showOnlyDuringGameplay.Length; i++)
        {
            var go = showOnlyDuringGameplay[i];
            if (go != null) go.SetActive(visible);
        }
    }

}

